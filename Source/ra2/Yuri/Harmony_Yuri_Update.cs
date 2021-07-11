using System;
using HarmonyLib;
using Verse;

namespace ra2.Yuri
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[] { })]
    public static class Harmony_Yuri_Update
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            var traverse = Traverse.Create(__instance);

            var pawn = traverse.Field("pawn").GetValue<Pawn>();

            ModBaseRa2.Instance._controlstorage.checkBeControlerExist(pawn);
            ModBaseRa2.Instance._controlstorage.checkControlerExist(pawn);
        }
    }
}