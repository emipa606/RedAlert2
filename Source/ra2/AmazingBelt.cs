using System;
using System.Collections.Generic;
using ra2.Yuri;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    public class AmazingBelt : Apparel
    {
        public int ticks;

        private bool tanyaHandWeapon()
        {
            var pe = Wearer.equipment;
            if (pe.Primary == null || pe.Primary.def.defName == "ra2_TanyaC4Bomb")
            {
                return false;
            }

            return true;
        }

        public List<IntVec3> CellsAround(IntVec3 pos, Map map, int range)
        {
            var result = new List<IntVec3>();
            var num = GenRadial.NumCellsInRadius(range);
            for (var i = 0; i < num; i++)
            {
                result.Add(pos + GenRadial.RadialPattern[i]);
            }

            return result;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            ticks = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticks, "ticks");
            // Scribe_Deep.Look<int>(ref this.ticks,"ticks",0,false);
        }

        public override void Tick()
        {
            base.Tick();

            if (ticks > 0)
            {
                ticks--;
            }

            if (ticks < 0)
            {
                ticks = 0;
            }
        }


        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            //  base.GetWornGizmos();
            if (Find.Selector.SingleSelectedThing != Wearer)
            {
                yield break;
            }

            //tanya
            if (Wearer.kindDef.defName == "ra2_AlliedTanya")
            {
                yield return new Command_Action
                {
                    icon = ContentFinder<Texture2D>.Get(!tanyaHandWeapon()
                        ? DefDatabase<ThingDef>.GetNamed("ra2_Gun_Tanya").graphicData.texPath
                        : DefDatabase<ThingDef>.GetNamed("ra2_TanyaC4Bomb").graphicData.texPath),
                    defaultLabel = "TanyaChangeWeapon".Translate(),
                    activateSound = DefDatabase<SoundDef>.GetNamed("ra2_tanya_select"),
                    action = delegate
                    {
                        var pe = Wearer.equipment;
                        var tempDef =
                            DefDatabase<ThingDef>.GetNamed(tanyaHandWeapon() ? "ra2_TanyaC4Bomb" : "ra2_Gun_Tanya");
                        pe.Remove(pe.Primary);
                        pe.AddEquipment((ThingWithComps) ThingMaker.MakeThing(tempDef));
                    }
                };
            }

            //chrono
            var tp = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetFires = false,
                canTargetLocations = true,
                canTargetPawns = false,
                canTargetSelf = false
            };


            if (Wearer.kindDef.defName == "ra2_AlliedChrono")
            {
                yield return new Command_TargetPlus
                {
                    icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport"),
                    defaultLabel = "ChronoTeleport".Translate(),
                    activateSound = DefDatabase<SoundDef>.GetNamed("ra2_Chrono_select"),
                    targetingParams = tp,
                    hotKey = KeyBindingDefOf.Misc4,
                    disabled = !Wearer.drafter.Drafted || Wearer.stances.stunner.Stunned,
                    aimIcon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport"),
                    action = delegate(LocalTargetInfo target)
                    {
                        var casterPawn = Wearer;
                        var map = Wearer.Map;

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
                        for (var asd = 0; asd < 60; asd++)
                        {
                            FleckMaker.ThrowExplosionCell(casterPawn.Position, map, FleckDefOf.ExplosionFlash,
                                new Color(1f - (asd * 0.1f), asd * 0.1f, 1f - (asd * 0.05f)));
                        }


                        var dist = Math.Sqrt(Math.Pow(casterPawn.Position.x - target.Cell.x, 2) +
                                             Math.Pow(casterPawn.Position.z - target.Cell.z, 2));

                        if (dist < 20)
                        {
                            dist = 20;
                        }


                        ThingSelectionUtility.SelectNextColonist();
                        Wearer.DeSpawn();
                        GenSpawn.Spawn(casterPawn, target.Cell, map);
                        casterPawn.drafter.Drafted = true;
                        ThingSelectionUtility.SelectPreviousColonist();
                        DefDatabase<SoundDef>.GetNamed("ra2_Chrono_move").PlayOneShot(casterPawn);
                        DefDatabase<SoundDef>.GetNamed("ra2_Chrono_movesay").PlayOneShotOnCamera();

                        var dinfo = new DamageInfo(DamageDefOf.Stun, (int) (dist * 0.1), -1, 1, null, null,
                            Wearer.equipment.Primary.def, DamageInfo.SourceCategory.ThingOrUnknown, casterPawn);
                        casterPawn.TakeDamage(dinfo);
                        // Log.Warning(target+"VV");
                    }
                };
            }

            //yuri
            var isYuri = false;

            if (Wearer != null)
            {
                var pawn = Wearer;
                if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
                {
                    isYuri = true;
                }
            }

            if (!isYuri)
            {
                yield break;
            }

            {
                var canCast = ticks <= 0 && Wearer.drafter.Drafted && !Wearer.stances.stunner.Stunned;
                yield return new Command_YuriAction
                {
                    defaultLabel = "YuriExpTitle".Translate(),
                    icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/yuriExpIcon"),
                    disabled = !canCast,
                    caster = Wearer,
                    hotKey = KeyBindingDefOf.Misc4,
                    action = delegate
                    {
                        var pawn = Wearer;
                        var df = DefDatabase<DamageDef>.GetNamed("YuriExp");
                        foreach (var cell in CellsAround(pawn.Position, pawn.Map, 7))
                        {
                            // if (!cell.IsValid) continue;
                            var list = pawn.Map.thingGrid.ThingsListAt(cell);
                            try
                            {
                                if (list.Count < 1)
                                {
                                    continue;
                                }

                                foreach (var thing in list)
                                {
                                    Pawn p;
                                    if ((p = thing as Pawn) == null)
                                    {
                                        continue;
                                    }

                                    if (p.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
                                    {
                                        continue;
                                    }

                                    if (!p.RaceProps.IsFlesh)
                                    {
                                        continue;
                                    }

                                    {
                                        p.Kill(new DamageInfo(df, 0));
                                    }

                                    if (!(p.Faction == Faction.OfPlayer &&
                                          p.kindDef.defName.StartsWith("ra2_")))
                                    {
                                        p.Corpse.Kill();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Warning(e + "233");
                            }
                        }

                        pawn.stances.stunner.StunFor(120, null);
                        ticks = 300;
                        GenExplosion.DoExplosion(pawn.Position, pawn.Map, 7f, df, pawn, 0);
                    }
                };
            }
        }
    }
}