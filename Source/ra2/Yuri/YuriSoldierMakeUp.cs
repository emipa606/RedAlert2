using RimWorld;
using Verse;

namespace ra2.Yuri;

public static class YuriSoldierMakeUp
{
    public static void tryMakeUp(Pawn pawn)
    {
        makeUpYuriHair(pawn);
        makeUpBrute(pawn);
    }


    public static void makeUpYuriHair(Pawn pawn)
    {
        if (!pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
        {
            return;
        }

        if (pawn.story == null)
        {
            return;
        }

        pawn.gender = Gender.Male;
        pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved");
        pawn.story.bodyType = BodyTypeDefOf.Thin;
    }


    public static void makeUpBrute(Pawn pawn)
    {
        if (!pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuribrute"))
        {
            return;
        }

        if (pawn.story == null)
        {
            return;
        }

        pawn.story.bodyType = BodyTypeDefOf.Hulk;
        pawn.story.traits.GainTrait(new Trait(TraitDef.Named("Cannibal")));
    }


    public static void controlOtherFactionGoodWill(Pawn user, Pawn target)
    {
        if (user.Faction == null || target.Faction == null)
        {
            return;
        }

        if (target.Faction.RelationKindWith(user.Faction) == FactionRelationKind.Hostile)
        {
            return;
        }

        var faction = target.Faction;
        var faction2 = user.Faction;
        var goodwillImpact = -20;
        var reason =
            "GoodwillChangedReason_UsedItem".Translate(user.equipment.Primary.def.label, target.LabelShort);
        faction.TryAffectGoodwillWith(faction2, goodwillImpact, true, true, null, target);
        faction.Notify_GoodwillSituationsChanged(faction2, true, reason, target);
    }
}