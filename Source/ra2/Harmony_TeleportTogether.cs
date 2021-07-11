using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos", new Type[]
    {
    })]
    public static class Harmony_TeleportTogether
    {
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (!__instance.IsColonistPlayerControlled)
            {
                return;
            }

            var nes = __result.ToList();


            foreach (var attack in GetTeleGizmos(__instance))
            {
                //yield return attack;

                nes.Add(attack);
                //  Log.Warning(nes.Count+"/");
            }

            __result = nes.AsEnumerable();
        }

        private static IEnumerable<Gizmo> GetTeleGizmos(Pawn pawn)
        {
            if (ShouldUseSquadTeleGizmo())
            {
                yield return GetSquadTeleGizmo(pawn);
            }
        }


        private static bool ShouldUseSquadTeleGizmo()
        {
            var selectedObjectsListForReading = Find.Selector.SelectedObjectsListForReading;
            if (selectedObjectsListForReading.Count < 2)
            {
                return false;
            }

            foreach (var pawn in selectedObjectsListForReading)
            {
                if (pawn is not Pawn)
                {
                    continue;
                }

                if ((pawn as Pawn).kindDef.defName != "ra2_AlliedChrono")
                {
                    return false;
                }
            }

            return true;
        }

        private static Gizmo GetSquadTeleGizmo(Pawn pawn)
        {
            // Log.Warning("YES");
            var command_Target = new Command_TargetPlus {defaultLabel = "ChronoTeleport".Translate()};


            var tp = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetFires = false,
                canTargetLocations = true,
                canTargetPawns = false,
                canTargetSelf = false
            };

            command_Target.disabled = pawn.stances.stunner.Stunned || !pawn.drafter.Drafted;
            command_Target.targetingParams = tp;
            command_Target.hotKey = KeyBindingDefOf.Misc4;
            command_Target.icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport");
            command_Target.aimIcon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport");
            command_Target.action = delegate(LocalTargetInfo target)
            {
                var enumerable = Find.Selector.SelectedObjects.Where(x =>
                    x is Pawn {IsColonistPlayerControlled: true, Drafted: true}).Cast<Pawn>();

                var tmpPawns = enumerable.ToList();
                var tmpPawns2 = new List<Pawn>();
                foreach (var pawn2 in tmpPawns)
                {
                    {
                        var casterPawn = pawn2;
                        var map = pawn2.Map;

                        var thi = map.thingGrid.ThingsAt(target.Cell);
                        foreach (var th in thi)
                        {
                            if (th is not Building)
                            {
                                continue;
                            }

                            Messages.Message("ChronoNotToBuild".Translate(), MessageTypeDefOf.RejectInput);
                            return;
                        }

                        FleckMaker.ThrowExplosionCell(target.Cell, map, FleckDefOf.ExplosionFlash, new Color(1, 1, 1));
                        // MoteMaker.ThrowExplosionCell(target.Cell, map, ThingDefOf.Mote_ExplosionFlash, new UnityEngine.Color(1,1,1));
                        // for (int asd = 0; asd < 60; asd++)
                        FleckMaker.ThrowExplosionCell(casterPawn.Position, map, FleckDefOf.ExplosionFlash,
                            new Color(1f, 1, 1));
                        var dist = Math.Sqrt(Math.Pow(casterPawn.Position.x - target.Cell.x, 2) +
                                             Math.Pow(casterPawn.Position.z - target.Cell.z, 2));
                        if (dist < 20)
                        {
                            dist = 20;
                        }


                        pawn2.DeSpawn();
                        GenSpawn.Spawn(casterPawn, target.Cell, map);

                        casterPawn.drafter.Drafted = true;
                        tmpPawns2.Add(casterPawn);

                        DefDatabase<SoundDef>.GetNamed("ra2_Chrono_move").PlayOneShot(casterPawn);
                        DefDatabase<SoundDef>.GetNamed("ra2_Chrono_movesay").PlayOneShotOnCamera();
                        var dinfo = new DamageInfo(DamageDefOf.Stun, (int) (dist * 0.1), -1, 1, null, null,
                            casterPawn.equipment.Primary.def, DamageInfo.SourceCategory.ThingOrUnknown, casterPawn);
                        casterPawn.TakeDamage(dinfo);
                    }
                }

                foreach (var pps in tmpPawns2)
                {
                    Find.Selector.SelectedObjects.Add(pps);
                }
            };
            return command_Target;
        }
    }
}