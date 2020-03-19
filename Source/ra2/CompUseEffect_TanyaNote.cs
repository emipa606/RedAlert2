using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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
                ResearchProjectDef named = DefDatabase<ResearchProjectDef>.GetNamed("Ra2AlliedTanya", true);
                if (named != null && !named.IsFinished)
                {
                    this.FinishInstantly(named);
                    return;
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
            Find.ResearchManager.FinishProject(proj, false, null);
            Messages.Message(Translator.Translate("MessageResearchProjectFinishedByItem", new object[]
            {
                proj.label
            }), MessageTypeDefOf.PositiveEvent, true);
        }
    }
}
