using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    

     //remove ra2 soldier's interaction
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "CanInteractNowWith", new Type[]
    {
        typeof(Pawn),
        typeof(InteractionDef)
    })]
    public static class HarmonyTest
    {
        public static void Postfix(Pawn_InteractionsTracker __instance,Pawn recipient, ref bool __result)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn value3 = traverse.Field("pawn").GetValue<Pawn>();

            if (recipient.kindDef.defName.StartsWith("ra2")||value3.kindDef.defName.StartsWith("ra2"))
            {
                
                //Log.Warning(recipient.kindDef.defName + "/" + value3.kindDef.defName + "/" + __result);
                __result = false;
            }


         
        }

    }


    //remove soldier death letter
    [HarmonyPatch(typeof(LetterStack), "ReceiveLetter", new Type[]
    {
        typeof(TaggedString),
        typeof(TaggedString),
        typeof(LetterDef),
        typeof(LookTargets),
        typeof(Faction),
        typeof(Quest),
        typeof(List<ThingDef>),
        typeof(string)
    })]
    public static class Harmony_SoldierNoLetter
    {
        public static bool Prefix(LetterDef textLetterDef, ref LookTargets lookTargets)
        {
            if(textLetterDef == LetterDefOf.Death)
            foreach (GlobalTargetInfo target in lookTargets.targets) {
                if (target.Thing is Pawn)
                {
                    Pawn pp = target.Thing as Pawn;
                    if (pp.Faction.IsPlayer&& pp.kindDef.defName.StartsWith("ra2_")) {
                        return false; }
                }
            }
            return true;
        }

    }

    //remove colonist thought of soldier death
  
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike", new Type[]
    {
        typeof(Pawn),
        typeof(DamageInfo),
        typeof(PawnDiedOrDownedThoughtsKind),
        typeof(List<IndividualThoughtToAdd>),
        typeof(List<ThoughtToAddToAll>)
    })]
    public static class Harmony_SoldierNoThought
    {
        public static bool Prefix(Pawn victim)
        {
            if (victim.kindDef.defName.StartsWith("ra2_"))
                return false;

            return true;
        }

    }
    

    //add group teleport to chrono
    [HarmonyPatch(typeof(Pawn), "GetGizmos", new Type[]
    {
    })]
    public static class Harmony_TeleportTogether
    {
        public static void Postfix(Pawn __instance,ref IEnumerable<Gizmo> __result)
        {

            if (__instance.IsColonistPlayerControlled)
            {
                List<Gizmo> nes = __result.ToList<Gizmo>();
                

                foreach (Gizmo attack in GetTeleGizmos(__instance))
                {
                    //yield return attack;
                   
                    nes.Add(attack);
                  //  Log.Warning(nes.Count+"/");
                }
                __result = nes.AsEnumerable<Gizmo>();
            }

        }

        private static IEnumerable<Gizmo> GetTeleGizmos(Pawn pawn)
        {
            if (ShouldUseSquadTeleGizmo())
            {
                yield return GetSquadTeleGizmo(pawn);
            }
            yield break;
        }


        private static bool ShouldUseSquadTeleGizmo()
        {
            List<object> selectedObjectsListForReading = Find.Selector.SelectedObjectsListForReading;
            if (selectedObjectsListForReading.Count < 2) return false;

            foreach (object pawn in selectedObjectsListForReading) {
                if (pawn is Pawn) {
                    if ((pawn as Pawn).kindDef.defName != "ra2_AlliedChrono"){
                        return false;
                    }
                }
            }

            return true;
        }

        private static Gizmo GetSquadTeleGizmo(Pawn pawn)
        {
           // Log.Warning("YES");
            Command_TargetPlus command_Target = new Command_TargetPlus();
            command_Target.defaultLabel = "ChronoTeleport".Translate();


            TargetingParameters tp = new TargetingParameters();
            tp.canTargetBuildings = false;
            tp.canTargetFires = false;
            tp.canTargetLocations = true;
            tp.canTargetPawns = false;
            tp.canTargetSelf = false;

            command_Target.disabled = pawn.stances.stunner.Stunned || !pawn.drafter.Drafted;
            command_Target.targetingParams = tp;
            command_Target.hotKey = KeyBindingDefOf.Misc4;
            command_Target.icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport", true);
            command_Target.aimIcon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport", true);
            command_Target.action = delegate (LocalTargetInfo target)
            {
                IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                {
                    Pawn pawn3 = x as Pawn;
                    return pawn3 != null && pawn3.IsColonistPlayerControlled && pawn3.Drafted;
                }).Cast<Pawn>();

                List<Pawn> tmpPawns = enumerable.ToList<Pawn>();
                List<Pawn> tmpPawns2 = new List<Pawn>();
                foreach (Pawn pawn2 in tmpPawns)
                {
                    
                    {
                        Pawn casterPawn = pawn2;
                        Map map = pawn2.Map;

                        IEnumerable<Thing> thi = map.thingGrid.ThingsAt(target.Cell);
                        foreach (Thing th in thi)
                        {
                            if (th is Building)
                            {
                                Messages.Message("ChronoNotToBuild".Translate(), MessageTypeDefOf.RejectInput);
                                return;
                            }
                        }

                        MoteMaker.ThrowExplosionCell(target.Cell, map, ThingDefOf.Mote_ExplosionFlash, new UnityEngine.Color(1, 1, 1));
                        // MoteMaker.ThrowExplosionCell(target.Cell, map, ThingDefOf.Mote_ExplosionFlash, new UnityEngine.Color(1,1,1));
                       // for (int asd = 0; asd < 60; asd++)
                            MoteMaker.ThrowExplosionCell(casterPawn.Position, map, ThingDefOf.Mote_ExplosionFlash, new UnityEngine.Color(1f,1,1));
                        double dist = Math.Sqrt(Math.Pow(casterPawn.Position.x - target.Cell.x, 2) + Math.Pow(casterPawn.Position.z - target.Cell.z, 2));
                        if (dist < 20) dist = 20;
                   
                        
                        pawn2.DeSpawn(DestroyMode.Vanish);
                        GenSpawn.Spawn(casterPawn, target.Cell, map, WipeMode.Vanish);
                        
                        casterPawn.drafter.Drafted = true;
                        tmpPawns2.Add(casterPawn);

                        SoundStarter.PlayOneShot(DefDatabase<SoundDef>.GetNamed("ra2_Chrono_move", true), casterPawn);
                        SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed("ra2_Chrono_movesay", true));
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Stun, (int)(dist * 0.1), -1, 1, null, null, casterPawn.equipment.Primary.def, DamageInfo.SourceCategory.ThingOrUnknown, casterPawn);
                        casterPawn.TakeDamage(dinfo);

                    }

             

                }
                foreach (Pawn pps in tmpPawns2)
                {
                    Find.Selector.SelectedObjects.Add(pps);

                }
            };
            return command_Target;
        }
        
    
    }
    

}
