using HarmonyLib;
using Verse;

namespace ra2;

[HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.CarryWeaponOpenly))]
public static class Harmony_Tank_WeaponAim
{
    public static bool Prefix(Pawn pawn, ref bool __result)
    {
        if (!pawn.kindDef.defName.EqualsIgnoreCase("ra2_rhinotank") &&
            !pawn.kindDef.defName.EqualsIgnoreCase("ra2_grizzlytank") &&
            !pawn.kindDef.defName.EqualsIgnoreCase("ra2_apotank"))
        {
            return true;
        }

        __result = true;
        return false;
    }
}