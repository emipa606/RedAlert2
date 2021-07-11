using RimWorld;
using Verse;

namespace ra2.Yuri
{
    // Token: 0x02001013 RID: 4115
    public class Verb_Yuri : Verb
    {
        // Token: 0x0600640B RID: 25611 RVA: 0x001B46FC File Offset: 0x001B2AFC

        private bool hitThing(Pawn becontroler)
        {
            return ModBaseRa2.Instance._controlstorage.ControlSomeone(CasterPawn, becontroler);
        }

        // Token: 0x0600640C RID: 25612 RVA: 0x001B4770 File Offset: 0x001B2B70
        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            if (!(currentTarget.Thing is Pawn))
            {
                return false;
            }

            if (!ModBaseRa2.Instance._controlstorage.canBeControled(currentTarget.Thing as Pawn))
            {
                return false;
            }

            if (verbProps.stopBurstWithoutLos && !TryFindShootLineFromTo(caster.Position, currentTarget, out _))
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
            var unused = caster;
            Thing unused1 = EquipmentSource;


            //SpawnBeam();
            var succ = hitThing(currentTarget.Thing as Pawn);

            if (!succ)
            {
                return false;
            }

            return true;
        }


        // Token: 0x06006410 RID: 25616 RVA: 0x001B4C60 File Offset: 0x001B3060
        public override bool Available()
        {
            if (!base.Available())
            {
                return false;
            }

            if (!base.CasterIsPawn)
            {
                return true;
            }

            var casterPawn = base.CasterPawn;
            if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat &&
                casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
            {
                return false;
            }

            return true;
        }
    }
}