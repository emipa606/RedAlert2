using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

public class Command_TargetPlus : Command
{
    public Action<LocalTargetInfo> action;
    public Texture2D aimIcon;

    public TargetingParameters targetingParams;

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
        Find.Targeter.BeginTargeting(targetingParams, delegate(LocalTargetInfo target) { action(target); }, null,
            null, aimIcon);
    }

    public override bool InheritInteractionsFrom(Gizmo other)
    {
        return false;
    }
}