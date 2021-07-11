using System;
using HarmonyLib;
using Verse;

namespace ra2
{
    [HarmonyPatch(typeof(PawnRenderer), "CarryWeaponOpenly", new Type[]
    {
    })]
    public static class Harmony_Tank_WeaponAim
    {
        public static bool Prefix(PawnRenderer __instance, ref bool __result)
        {
            var traverse = Traverse.Create(__instance);
            var pp = traverse.Field("pawn").GetValue<Pawn>();
            if (!pp.kindDef.defName.EqualsIgnoreCase("ra2_rhinotank") &&
                !pp.kindDef.defName.EqualsIgnoreCase("ra2_grizzlytank") &&
                !pp.kindDef.defName.EqualsIgnoreCase("ra2_apotank"))
            {
                return true;
            }

            __result = true;
            return false;

            //return (pp.carryTracker == null || pp.carryTracker.CarriedThing == null) && (pp.Drafted || (pp.CurJob != null && pp.CurJob.def.alwaysShowWeapon) || (pp.mindState.duty != null && pp.mindState.duty.def.alwaysShowWeapon));
        }
    }
}