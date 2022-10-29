using UnityEngine;
using Verse;

namespace ra2;

public class TurretTop_CustomSize
{
    private const float IdleTurnDegreesPerTick = 0.26f;

    private const int IdleTurnDuration = 140;

    private const int IdleTurnIntervalMin = 150;

    private const int IdleTurnIntervalMax = 350;
    private readonly Building_AirDefense parentDefense;

    private readonly Building_CustomTurretGun parentTurret;

    private float curRotationInt;

    private bool idleTurnClockwise;

    private int idleTurnTicksLeft;

    private int ticksUntilIdleTurn;

    public TurretTop_CustomSize(Building_CustomTurretGun ParentTurret)
    {
        parentTurret = ParentTurret;
    }

    public TurretTop_CustomSize(Building_AirDefense ParentAD)
    {
        parentDefense = ParentAD;
    }

    private float CurRotation
    {
        get => curRotationInt;
        set
        {
            curRotationInt = value;
            if (curRotationInt > 360.0)
            {
                curRotationInt -= 360f;
            }

            if (curRotationInt < 0.0)
            {
                curRotationInt += 360f;
            }
        }
    }

    public void TurretTopTick()
    {
        var airmode = parentDefense != null && parentTurret == null;

        var currentTarget = airmode ? parentDefense.nowTarget : parentTurret.CurrentTarget;

        if (airmode ? currentTarget != null : currentTarget.IsValid)
        {
            var cell = currentTarget.Cell;
            var unused = CurRotation =
                (cell.ToVector3Shifted() - (airmode ? parentDefense.DrawPos : parentTurret.DrawPos)).AngleFlat();
            ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
        }
        else if (ticksUntilIdleTurn > 0)
        {
            ticksUntilIdleTurn--;
            if (ticksUntilIdleTurn != 0)
            {
                return;
            }

            idleTurnClockwise = Rand.Value < 0.5;

            idleTurnTicksLeft = 140;
        }
        else
        {
            if (idleTurnClockwise)
            {
                CurRotation += 0.26f;
            }
            else
            {
                CurRotation -= 0.26f;
            }

            idleTurnTicksLeft--;
            if (idleTurnTicksLeft <= 0)
            {
                ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
            }
        }
    }

    public void DrawTurret()
    {
        var airmode = parentDefense != null && parentTurret == null;

        var matrix4x = default(Matrix4x4);
        matrix4x.SetTRS((airmode ? parentDefense.DrawPos : parentTurret.DrawPos) + Altitudes.AltIncVect,
            CurRotation.ToQuat(),
            (airmode ? parentDefense.TopSizeComp : parentTurret.TopSizeComp) == null ? Vector3.one :
            airmode ? parentDefense.TopSizeComp.Props.topSize : parentTurret.TopSizeComp.Props.topSize);
        Graphics.DrawMesh(MeshPool.plane20, matrix4x,
            airmode ? parentDefense.def.building.turretTopMat : parentTurret.def.building.turretTopMat, 0);
    }
}