using RimWorld;
using Verse;

namespace ra2;

public class CompUseEffect_TanyaNote : CompUseEffect
{
    public override void DoEffect(Pawn usedBy)
    {
        base.DoEffect(usedBy);
        var named = DefDatabase<ResearchProjectDef>.GetNamed("Ra2AlliedTanya");
        if (named is { IsFinished: false })
        {
            FinishInstantly(named);
        }
    }

    public override bool CanBeUsedBy(Pawn p, out string failReason)
    {
        failReason = null;
        return true;
    }

    private void FinishInstantly(ResearchProjectDef proj)
    {
        Find.ResearchManager.FinishProject(proj);
        Messages.Message("MessageResearchProjectFinishedByItem".Translate(proj.label),
            MessageTypeDefOf.PositiveEvent);
    }
}