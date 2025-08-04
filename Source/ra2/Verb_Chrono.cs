using RimWorld;
using Verse;

namespace ra2;

public class Verb_Chrono : Verb
{
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
        _ = randomCoverToMissInto?.def;
        if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
        {
            shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget, false, caster.Map);

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
            var nonTargetWorld = ProjectileHitFlags.NonTargetWorld;
            if (canHitNonTargetPawnsNow)
            {
                nonTargetWorld |= ProjectileHitFlags.NonTargetPawns;
            }

            return true;
        }


        var intendedTarget = ProjectileHitFlags.IntendedTarget;
        if (canHitNonTargetPawnsNow)
        {
            intendedTarget |= ProjectileHitFlags.NonTargetPawns;
        }

        if (currentTarget.Thing != null &&
            (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full))
        {
            intendedTarget |= ProjectileHitFlags.NonTargetWorld;
        }

        return true;
    }


    private void SpawnBeam()
    {
        if (ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ChronoBeam")) is not ChronoBeam cb)
        {
            return;
        }

        cb.Setup(caster, currentTarget);
        GenSpawn.Spawn(cb, caster.Position.ToVector3().ToIntVec3(), caster.Map);
    }


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
        return casterPawn.Faction == Faction.OfPlayer || !casterPawn.mindState.MeleeThreatStillThreat ||
               !casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position);
    }
}