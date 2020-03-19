using ra2.Yuri;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    public class AmazingBelt: Apparel
    {
        public int ticks;
        private bool tanyaHandWeapon() {
            Pawn_EquipmentTracker pe = base.Wearer.equipment;
            if (pe.Primary == null || pe.Primary.def.defName == "ra2_TanyaC4Bomb") {
                return false;
            }
            return true;
        }
        public List<IntVec3> CellsAround(IntVec3 pos, Map map, int range)
        {
            List<IntVec3> result = new List<IntVec3>();
            int num = GenRadial.NumCellsInRadius(range);
            for (int i = 0; i < num; i++)
            {
                result.Add(pos + GenRadial.RadialPattern[i]);
            }
            return result;
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.ticks = 0;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
           // Scribe_Deep.Look<int>(ref this.ticks,"ticks",0,false);
        }
        public override void Tick()
        {
            base.Tick();

            if (this.ticks > 0)
                this.ticks--;
            if (this.ticks < 0)
                this.ticks = 0;
        }


        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            
          //  base.GetWornGizmos();
            if (Find.Selector.SingleSelectedThing != base.Wearer)
            {
                yield break;
            }

            //tanya
            if (base.Wearer.kindDef.defName == "ra2_AlliedTanya")
                yield return new Command_Action
                {

                    icon = ContentFinder<Texture2D>.Get(!tanyaHandWeapon() ? DefDatabase<ThingDef>.GetNamed("ra2_Gun_Tanya", true).graphicData.texPath : DefDatabase<ThingDef>.GetNamed("ra2_TanyaC4Bomb", true).graphicData.texPath, true),
                    defaultLabel = "TanyaChangeWeapon".Translate(),
                    activateSound = DefDatabase<SoundDef>.GetNamed("ra2_tanya_select", true),
                    action = delegate
                    {
                        Pawn_EquipmentTracker pe = base.Wearer.equipment;
                        ThingDef tempDef = DefDatabase<ThingDef>.GetNamed(tanyaHandWeapon() ? "ra2_TanyaC4Bomb" : "ra2_Gun_Tanya", true);
                        pe.Remove(pe.Primary);
                        pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(tempDef));
                    }
                };
            
            //chrono
                TargetingParameters tp = new TargetingParameters();
                tp.canTargetBuildings = false;
                tp.canTargetFires = false;
                tp.canTargetLocations = true;
                tp.canTargetPawns = false;
                tp.canTargetSelf = false;
             

            if (base.Wearer.kindDef.defName == "ra2_AlliedChrono")
                yield return new Command_TargetPlus
                {

                    icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport", true),
                    defaultLabel = "ChronoTeleport".Translate(),
                    activateSound = DefDatabase<SoundDef>.GetNamed("ra2_Chrono_select", true),
                    targetingParams = tp,
                    hotKey = KeyBindingDefOf.Misc4,
                    disabled = !base.Wearer.drafter.Drafted || base.Wearer.stances.stunner.Stunned,
                    aimIcon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/ChTeleport", true),
                    action = delegate (LocalTargetInfo target)
                    {

                        Pawn casterPawn = base.Wearer;
                        Map map = base.Wearer.Map;

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
                        for (int asd = 0; asd < 60; asd++)
                            MoteMaker.ThrowExplosionCell(casterPawn.Position, map, ThingDefOf.Mote_ExplosionFlash, new UnityEngine.Color(1f - asd * 0.1f, asd * 0.1f, 1f - asd * 0.05f));


                        double dist = Math.Sqrt(Math.Pow(casterPawn.Position.x - target.Cell.x, 2) + Math.Pow(casterPawn.Position.z - target.Cell.z, 2));

                        if (dist < 20) dist = 20;


                        ThingSelectionUtility.SelectNextColonist();
                        base.Wearer.DeSpawn(DestroyMode.Vanish);
                        GenSpawn.Spawn(casterPawn, target.Cell, map, WipeMode.Vanish);
                        casterPawn.drafter.Drafted = true;
                        ThingSelectionUtility.SelectPreviousColonist();
                        SoundStarter.PlayOneShot(DefDatabase<SoundDef>.GetNamed("ra2_Chrono_move", true), casterPawn);
                        SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed("ra2_Chrono_movesay", true));

                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Stun, (int)(dist * 0.1), -1, 1, null, null, base.Wearer.equipment.Primary.def, DamageInfo.SourceCategory.ThingOrUnknown, casterPawn);
                        casterPawn.TakeDamage(dinfo);
                        // Log.Warning(target+"VV");
                    }

                };

            //yuri
                bool isYuri = false;
         
                if (this.Wearer != null)
                {
                    Pawn pawn = this.Wearer;
                    if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
                        isYuri = true;
                }

            if (isYuri)
            {
                bool canCast = this.ticks <= 0 && this.Wearer.drafter.Drafted && !this.Wearer.stances.stunner.Stunned;
                yield return new Command_YuriAction
                {
                    defaultLabel = "YuriExpTitle".Translate(),
                    icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/yuriExpIcon", true),
                    disabled = !canCast,
                    caster = this.Wearer,
                    hotKey = KeyBindingDefOf.Misc4,
                    action = delegate
                    {
                        Pawn pawn = this.Wearer;
                        DamageDef df= DefDatabase<DamageDef>.GetNamed("YuriExp", true);
                        foreach (IntVec3 cell in CellsAround(pawn.Position, pawn.Map, 7))
                        {
                           // if (!cell.IsValid) continue;
                            List<Thing> list = (pawn.Map.thingGrid.ThingsListAt(cell));
                            try
                            {
                                if (list.Count < 1) continue;
                                for (int i=0;i<list.Count;i++)
                                {
                                    Pawn p;
                                    if ((p = (list[i] as Pawn)) != null)
                                    {
                                        
                                        if (p.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
                                            continue;
                                        if (p.RaceProps.IsFlesh)
                                        {
                                            {
                                                p.Kill(new DamageInfo(df, 0, 0, -1, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null),null);
                                            }

                                            if(!(p.Faction==Faction.OfPlayer&& p.kindDef.defName.StartsWith("ra2_")))
                                              p.Corpse.Kill(null);

                                        }
                                    }
                                }
                            }
                            catch (Exception e) { Log.Warning(e + "233"); }
                            
                                  
                        }

                        pawn.stances.stunner.StunFor(120, null);
                        this.ticks = 300;
                        GenExplosion.DoExplosion(pawn.Position, pawn.Map, 7f, df, pawn, 0, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                    }

                };
            }






                yield break;
        }
     
    }
}
