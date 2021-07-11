using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    // Token: 0x02000EA1 RID: 3745
    public class Command_TargetPlus : Command
    {
        // Token: 0x04003B2D RID: 15149
        public Action<LocalTargetInfo> action;
        public Texture2D aimIcon;

        // Token: 0x04003B2E RID: 15150
        public TargetingParameters targetingParams;

        // Token: 0x06005899 RID: 22681 RVA: 0x00288809 File Offset: 0x00286C09
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            Find.Targeter.BeginTargeting(targetingParams, delegate(LocalTargetInfo target) { action(target); }, null,
                null, aimIcon);
        }

        // Token: 0x0600589A RID: 22682 RVA: 0x0028883C File Offset: 0x00286C3C
        public override bool InheritInteractionsFrom(Gizmo other)
        {
            return false;
        }
    }
}