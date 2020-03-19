namespace ra2
{
    using RimWorld;

    using Verse;

    public class CompDownToDie : ThingComp
    {


        public override void CompTick()
        {
                base.CompTick();
                Apparel apparel;
                if ((((apparel = base.parent as Apparel) != null) && (apparel.Wearer != null)) && (apparel.Wearer.health.hediffSet.hediffs.Find(x => x.TryGetComp<HediffComp_DownToDie>() != null) == null))
                {
                    Hediff hediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("Hediffs_DownToDie", true), apparel.Wearer, null);
                    apparel.Wearer.health.AddHediff(hediff, null, null);
                return;
                }


 

        }

        public CompProperties_DownToDie Props =>
            ((CompProperties_DownToDie)base.props);

}
}
