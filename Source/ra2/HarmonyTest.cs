using HarmonyLib;
using RimWorld;
using Verse;

namespace ra2;

//remove ra2 soldier's interaction
[HarmonyPatch(typeof(Pawn_InteractionsTracker), "CanInteractNowWith", typeof(Pawn), typeof(InteractionDef))]
public static class HarmonyTest
{
    public static void Postfix(Pawn_InteractionsTracker __instance, Pawn recipient, ref bool __result)
    {
        var traverse = Traverse.Create(__instance);
        var value3 = traverse.Field("pawn").GetValue<Pawn>();

        if (recipient.kindDef.defName.StartsWith("ra2") || value3.kindDef.defName.StartsWith("ra2"))
        {
            //Log.Warning(recipient.kindDef.defName + "/" + value3.kindDef.defName + "/" + __result);
            __result = false;
        }
    }
}