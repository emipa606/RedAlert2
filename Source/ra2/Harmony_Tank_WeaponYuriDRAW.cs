using HarmonyLib;
using Verse;

namespace ra2
{
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public static class Harmony_Tank_WeaponYuriDRAW
    {
        public static bool Prefix(PawnRenderer __instance)
        {
            var traverse = Traverse.Create(__instance);
            var pp = traverse.Field("pawn").GetValue<Pawn>();
            if (pp.kindDef.defName.StartsWith("ra2_Yuri"))
            {
                return false;
            }

            //return (pp.carryTracker == null || pp.carryTracker.CarriedThing == null) && (pp.Drafted || (pp.CurJob != null && pp.CurJob.def.alwaysShowWeapon) || (pp.mindState.duty != null && pp.mindState.duty.def.alwaysShowWeapon));
            return true;
        }
    }
}