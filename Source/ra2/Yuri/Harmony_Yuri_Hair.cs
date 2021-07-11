using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ra2.Yuri
{
    // 尤里光头以及狂兽人肤色预定
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[] { })]
    public static class Harmony_Yuri_Hair
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            var traverse = Traverse.Create(__instance);

            var pawn = traverse.Field("pawn").GetValue<Pawn>();

            if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
            {
                if (pawn.story == null)
                {
                    return;
                }

                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved");
                pawn.story.bodyType = BodyTypeDefOf.Thin;
                pawn.gender = Gender.Male;
                pawn.story.melanin = 0f;
            }
            else if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuribrute"))
            {
                if (pawn.story == null)
                {
                    return;
                }

                pawn.story.melanin = 10f;
                pawn.story.bodyType = BodyTypeDefOf.Hulk;
            }
        }
    }

    // 时刻检查控制
    //绘画选择线条
}