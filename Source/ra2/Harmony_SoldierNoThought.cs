using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ra2
{
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike", typeof(Pawn),
        typeof(DamageInfo), typeof(PawnDiedOrDownedThoughtsKind), typeof(List<IndividualThoughtToAdd>),
        typeof(List<ThoughtToAddToAll>))]
    public static class Harmony_SoldierNoThought
    {
        public static bool Prefix(Pawn victim)
        {
            if (victim.kindDef.defName.StartsWith("ra2_"))
            {
                return false;
            }

            return true;
        }
    }
}