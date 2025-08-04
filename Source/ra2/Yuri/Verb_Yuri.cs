using RimWorld;
using Verse;

namespace ra2.Yuri;

public class Verb_Yuri : Verb
{
    private bool hitThing(Pawn becontroler)
    {
        return ModBaseRa2.Instance._controlstorage.ControlSomeone(CasterPawn, becontroler);
    }

    protected override bool TryCastShot()
    {
        if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
        {
            return false;
        }

        if (currentTarget.Thing is not Pawn pawn)
        {
            return false;
        }

        if (!Controlstorage.canBeControled(pawn))
        {
            return false;
        }

        if (verbProps.stopBurstWithoutLos && !TryFindShootLineFromTo(caster.Position, currentTarget, out _))
        {
            return false;
        }

        _ = caster;
        Thing unused1 = EquipmentSource;


        //SpawnBeam();

        return hitThing(currentTarget.Thing as Pawn);
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