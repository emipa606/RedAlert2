using RimWorld;
using Verse;

namespace ra2
{
    // Token: 0x02000002 RID: 2
    public class CompUseEffect_TanyaNote : CompUseEffect
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            var named = DefDatabase<ResearchProjectDef>.GetNamed("Ra2AlliedTanya");
            if (named != null && !named.IsFinished)
            {
                FinishInstantly(named);
            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000021E0 File Offset: 0x000003E0
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = null;
            return true;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000021E6 File Offset: 0x000003E6
        private void FinishInstantly(ResearchProjectDef proj)
        {
            Find.ResearchManager.FinishProject(proj);
            Messages.Message("MessageResearchProjectFinishedByItem".Translate(proj.label),
                MessageTypeDefOf.PositiveEvent);
        }
    }
}