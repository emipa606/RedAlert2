using HarmonyLib;
using RimWorld;
using Verse;

namespace ra2;

//remove ra2 soldier's interaction
[HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.CanInteractNowWith), typeof(Pawn),
    typeof(InteractionDef))]
public static class Pawn_InteractionsTracker_CanInteractNowWith
{
    public static void Postfix(Pawn recipient, ref bool __result, Pawn ___pawn)
    {
        if (recipient.kindDef.defName.StartsWith("ra2") || ___pawn.kindDef.defName.StartsWith("ra2"))
        {
            //Log.Warning(recipient.kindDef.defName + "/" + value3.kindDef.defName + "/" + __result);
            __result = false;
        }
    }
}