using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2.Yuri
{
    public static class YuriSoldierMakeUp
    {

        public static void tryMakeUp(Pawn pawn) {
            makeUpYuriHair(pawn);
            makeUpBrute(pawn);
        }





        public static void makeUpYuriHair(Pawn pawn) {
            if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
            {
                if (pawn.story != null)
                {
                    pawn.gender = Gender.Male;
                    pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved", true);
                    pawn.story.bodyType = BodyTypeDefOf.Thin;

                    pawn.story.melanin = 0f;
                }
            }
        }



        public static void makeUpBrute(Pawn pawn)
        {
            if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuribrute"))
            {
                if (pawn.story != null)
                {
                    pawn.story.melanin = 10f;
                    pawn.story.bodyType = BodyTypeDefOf.Hulk;
                    pawn.story.traits.GainTrait(new Trait(TraitDefOf.Cannibal));
                }
            }
        }


        public static void controlOtherFactionGoodWill(Pawn user,Pawn target) {
            if (user.Faction != null && target.Faction != null)
            {
                if (target.Faction.RelationKindWith(user.Faction) == FactionRelationKind.Hostile) { return; }
                Faction faction = target.Faction;
                Faction faction2 = user.Faction;
                int goodwillImpact = -20;
                string reason = "GoodwillChangedReason_UsedItem".Translate(new object[]
                {
                    user.equipment.Primary.def.label,
                    target.LabelShort
                });
                GlobalTargetInfo? lookTarget = new GlobalTargetInfo?(target);
                faction.TryAffectGoodwillWith(faction2, goodwillImpact, true, true, reason, lookTarget);
            }
        }



    }
}
