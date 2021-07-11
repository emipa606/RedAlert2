using RimWorld;
using Verse;

namespace ra2
{
    // Token: 0x02001013 RID: 4115
    public class Verb_Chrono : Verb
    {
        // Token: 0x0600640B RID: 25611 RVA: 0x001B46FC File Offset: 0x001B2AFC

        private void hitThing()
        {
            foreach (var ap in CasterPawn.apparel.WornApparel)
            {
                if (ap.def.defName != "ra2_Hat_Chrono")
                {
                    continue;
                }

                var crg = ap.TryGetComp<Comp_RemovingGun>();
                crg.resolveHitBuilding(currentTarget.Thing);
                break;
            }
        }

        // Token: 0x0600640C RID: 25612 RVA: 0x001B4770 File Offset: 0x001B2B70
        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            var shootLine = new ShootLine();
            if (verbProps.stopBurstWithoutLos && !TryFindShootLineFromTo(caster.Position, currentTarget, out shootLine))
            {
                return false;
            }

            var comp = EquipmentSource?.GetComp<CompChangeableProjectile>();
            comp?.Notify_ProjectileLaunched();

            var compMannable = caster.TryGetComp<CompMannable>();
            if (compMannable?.ManningPawn != null)
            {
            }


            SpawnBeam();
            hitThing();


            var shotReport = ShotReport.HitReportFor(caster, this, currentTarget);
            var randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            var unused = randomCoverToMissInto?.def;
            if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);

                var nonTargetWorld = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                {
                    nonTargetWorld |= ProjectileHitFlags.NonTargetPawns;
                }

                return true;
            }

            if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn &&
                !Rand.Chance(shotReport.PassCoverChance))
            {
                var projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }

                return true;
            }


            var projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }

            if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }

            return true;
        }


        private void SpawnBeam()
        {
            var cb = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ChronoBeam")) as ChronoBeam;
            if (cb == null)
            {
                return;
            }

            cb.Setup(caster, currentTarget);
            GenSpawn.Spawn(cb, caster.Position.ToVector3().ToIntVec3(), caster.Map);
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