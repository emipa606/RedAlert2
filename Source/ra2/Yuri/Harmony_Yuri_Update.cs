using HarmonyLib;
using Verse;

namespace ra2.Yuri;

[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.HealthTick))]
public static class Harmony_Yuri_Update
{
    public static void Postfix(Pawn_HealthTracker __instance, Pawn ___pawn)
    {
        ModBaseRa2.Instance._controlstorage.checkBeControlerExist(___pawn);
        ModBaseRa2.Instance._controlstorage.checkControlerExist(___pawn);
    }
}