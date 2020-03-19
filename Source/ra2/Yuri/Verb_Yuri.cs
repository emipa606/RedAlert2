using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ra2.Yuri
{
    // Token: 0x02001013 RID: 4115
    public class Verb_Yuri : Verb
    {



        // Token: 0x0600640B RID: 25611 RVA: 0x001B46FC File Offset: 0x001B2AFC
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            // Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster, (!this.currentTarget.HasThing) ? null : this.currentTarget.Thing, (base.EquipmentSource == null) ? null : base.EquipmentSource.def, null, this.ShotsPerBurst > 1));
        }

        private bool hitThing(Pawn becontroler)
        {
            return ModBaseRa2.Instance._controlstorage.ControlSomeone(this.CasterPawn,becontroler);
        }

        // Token: 0x0600640C RID: 25612 RVA: 0x001B4770 File Offset: 0x001B2B70
        protected override bool TryCastShot()
        {
            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
            {
                return false;
            }
            if (!(this.currentTarget.Thing is Pawn)) { return false; }

            if (!ModBaseRa2.Instance._controlstorage.canBeControled(this.currentTarget.Thing as Pawn)) {
                return false;
            }
            ShootLine shootLine;
            bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
            if (this.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            /*
            if (base.EquipmentSource != null)
            {
                CompChangeableProjectile comp = base.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
            }
            */
            Thing launcher = this.caster;
            Thing equipment = base.EquipmentSource;
            Vector3 drawPos = this.caster.DrawPos;


            //SpawnBeam();
           bool succ= hitThing(this.currentTarget.Thing as Pawn);

            if (!succ) return false;

            return true;
        }






        // Token: 0x06006410 RID: 25616 RVA: 0x001B4C60 File Offset: 0x001B3060
        public override bool Available()
        {
            if (!base.Available())
            {
                return false;
            }
            if (base.CasterIsPawn)
            {
                Pawn casterPawn = base.CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
