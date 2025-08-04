using HarmonyLib;
using RimWorld;
using Verse;

namespace ra2.Yuri;

// 尤里光头以及狂兽人肤色预定
[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.HealthTick))]
public static class Pawn_HealthTracker_HealthTick
{
    public static void Postfix(Pawn ___pawn)
    {
        ModBaseRa2.Instance._controlstorage.checkBeControlerExist(___pawn);
        ModBaseRa2.Instance._controlstorage.checkControlerExist(___pawn);
        if (___pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
        {
            if (___pawn.story == null)
            {
                return;
            }

            ___pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved");
            ___pawn.story.bodyType = BodyTypeDefOf.Thin;
            ___pawn.gender = Gender.Male;
        }
        else if (___pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuribrute"))
        {
            if (___pawn.story == null)
            {
                return;
            }

            ___pawn.story.bodyType = BodyTypeDefOf.Hulk;
        }
    }
}

// 时刻检查控制
//绘画选择线条