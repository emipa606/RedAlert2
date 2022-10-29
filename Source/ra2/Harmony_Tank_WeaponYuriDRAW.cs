using HarmonyLib;
using Verse;

namespace ra2;

[HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
public static class Harmony_Tank_WeaponYuriDRAW
{
    public static bool Prefix(PawnRenderer __instance)
    {
        var traverse = Traverse.Create(__instance);
        var pp = traverse.Field("pawn").GetValue<Pawn>();
        return !pp.kindDef.defName.StartsWith("ra2_Yuri");
    }
}