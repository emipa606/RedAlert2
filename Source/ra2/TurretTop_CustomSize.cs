using UnityEngine;
using Verse;

namespace ra2 {
    public class TurretTop_CustomSize
    {

        private Building_CustomTurretGun parentTurret;
        private Building_AirDefense parentDefense;

        private float curRotationInt;

        private int ticksUntilIdleTurn;

        private int idleTurnTicksLeft;

        private bool idleTurnClockwise;

        private const float IdleTurnDegreesPerTick = 0.26f;

        private const int IdleTurnDuration = 140;

        private const int IdleTurnIntervalMin = 150;

        private const int IdleTurnIntervalMax = 350;

        private float CurRotation
        {
            get
            {
                return this.curRotationInt;
            }
            set
            {
                this.curRotationInt = value;
                if (this.curRotationInt > 360.0)
                {
                    this.curRotationInt -= 360f;
                }
                if (this.curRotationInt < 0.0)
                {
                    this.curRotationInt += 360f;
                }
            }
        }

        public TurretTop_CustomSize(Building_CustomTurretGun ParentTurret)
        {
            this.parentTurret = ParentTurret;
        }
        public TurretTop_CustomSize(Building_AirDefense ParentAD)
        {
            this.parentDefense = ParentAD;
        }

        public void TurretTopTick()
        {
            bool airmode = (parentDefense!=null &&parentTurret==null);

            LocalTargetInfo currentTarget = airmode?this.parentDefense.nowTarget:this.parentTurret.CurrentTarget;

            if (airmode?currentTarget!=null:currentTarget.IsValid)
            {
                IntVec3 cell = currentTarget.Cell;
                float num2 = this.CurRotation = (cell.ToVector3Shifted() - (airmode?this.parentDefense.DrawPos:this.parentTurret.DrawPos)).AngleFlat();
                this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
            }
            else if (this.ticksUntilIdleTurn > 0)
            {
                this.ticksUntilIdleTurn--;
                if (this.ticksUntilIdleTurn == 0)
                {
                    if (Rand.Value < 0.5)
                    {
                        this.idleTurnClockwise = true;
                    }
                    else
                    {
                        this.idleTurnClockwise = false;
                    }
                    this.idleTurnTicksLeft = 140;
                }
            }
            else
            {
                if (this.idleTurnClockwise)
                {
                    this.CurRotation += 0.26f;
                }
                else
                {
                    this.CurRotation -= 0.26f;
                }
                this.idleTurnTicksLeft--;
                if (this.idleTurnTicksLeft <= 0)
                {
                    this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
                }
            }
        }

        public void DrawTurret()
        {
            bool airmode = (parentDefense != null && parentTurret == null);
            
            Matrix4x4 matrix4x = default(Matrix4x4);
            matrix4x.SetTRS((airmode? this.parentDefense.DrawPos:this.parentTurret.DrawPos) + Altitudes.AltIncVect, this.CurRotation.ToQuat(), (airmode?this.parentDefense.TopSizeComp:this.parentTurret.TopSizeComp) == null ? Vector3.one : (airmode? this.parentDefense.TopSizeComp.Props.topSize:this.parentTurret.TopSizeComp.Props.topSize));
            Graphics.DrawMesh(MeshPool.plane20, matrix4x, (airmode? this.parentDefense.def.building.turretTopMat:this.parentTurret.def.building.turretTopMat), 0);
        }
    }
}
