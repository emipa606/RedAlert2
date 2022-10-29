using RimWorld;
using Verse;

namespace ra2;

public class CompDownToDie : ThingComp
{
    public CompProperties_DownToDie Props =>
        (CompProperties_DownToDie)props;


    public override void CompTick()
    {
        base.CompTick();
        Apparel apparel;
        if ((apparel = parent as Apparel) == null || apparel.Wearer == null ||
            apparel.Wearer.health.hediffSet.hediffs.Find(x => x.TryGetComp<HediffComp_DownToDie>() != null) != null)
        {
            return;
        }

        var hediff =
            HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("Hediffs_DownToDie"), apparel.Wearer);
        apparel.Wearer.health.AddHediff(hediff);
    }
}