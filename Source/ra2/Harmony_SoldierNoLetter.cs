using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ra2
{
    [HarmonyPatch(typeof(LetterStack), "ReceiveLetter", typeof(TaggedString), typeof(TaggedString), typeof(LetterDef),
        typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string))]
    public static class Harmony_SoldierNoLetter
    {
        public static bool Prefix(LetterDef textLetterDef, ref LookTargets lookTargets)
        {
            if (textLetterDef != LetterDefOf.Death)
            {
                return true;
            }

            foreach (var target in lookTargets.targets)
            {
                if (target.Thing is not Pawn)
                {
                    continue;
                }

                if (target.Thing is Pawn pp && pp.Faction.IsPlayer && pp.kindDef.defName.StartsWith("ra2_"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}