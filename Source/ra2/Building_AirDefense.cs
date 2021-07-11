﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Random = System.Random;

namespace ra2
{
    // Token: 0x02000006 RID: 6
    public class Building_AirDefense : Building
    {
        // Token: 0x04000006 RID: 6
        private readonly int ticksAirInterval = 120;

        // Token: 0x04000003 RID: 3
        public List<IntVec3> airCells = new List<IntVec3>();

        // Token: 0x04000005 RID: 5
        private List<Thing> airThings = new List<Thing>();

        public LocalTargetInfo nowTarget;

        protected StunHandler stunner;

        // Token: 0x04000007 RID: 7
        private int ticks;


        protected TurretTop_CustomSize top;
        protected CompTurretTopSize topSizeComp;

        public Building_AirDefense()
        {
            top = new TurretTop_CustomSize(this);
            stunner = new StunHandler(this);
            nowTarget = null;
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000A RID: 10 RVA: 0x00002324 File Offset: 0x00000524
        private IEnumerable<IntVec3> AirCells => AirCellsAround(Position, Map);

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000B RID: 11 RVA: 0x00002338 File Offset: 0x00000538
        private bool ShouldAttackNow
        {
            get
            {
                if (!Spawned)
                {
                    return false;
                }

                if (!FlickUtility.WantsToBeOn(this))
                {
                    return false;
                }

                if (underRoof)
                {
                    return false;
                }

                // if (this.restoreBullet < 1) return false;
                var compPowerTrader = this.TryGetComp<CompPowerTrader>();
                if (compPowerTrader != null && !compPowerTrader.PowerOn)
                {
                    return false;
                }

                if (stunner.Stunned)
                {
                    return false;
                }

                var compRefuelable = this.TryGetComp<CompRefuelable>();
                return compRefuelable == null || compRefuelable.HasFuel;
            }
        }

        private int range => 20;
        private bool underRoof => Position.Roofed(Map);


        public CompTurretTopSize TopSizeComp =>
            topSizeComp;

        public override string GetInspectString()
        {
            var sb = new StringBuilder();

            sb.Append(base.GetInspectString());

            if (underRoof)
            {
                sb.Append("\n" + "ADAUnderRoof".Translate());
            }


            // String sb = base.GetInspectString() + "\n" + Translator.Translate("restoreBullet") + ":" + this.restoreBullet + "\n" + Translator.Translate("restoreProgress") + ":" + (this.ticks);
            return sb.ToString();
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002384 File Offset: 0x00000584
        public override void Tick()
        {
            base.Tick();
            doTurretTick();
            //  if (this.restoreBullet < this.TopSizeComp.Props.maxStoreBullet) {
            var compPowerTrader = this.TryGetComp<CompPowerTrader>();
            if (compPowerTrader != null && compPowerTrader.PowerOn)
            {
                ticks++;
            }
            //    }


            if (ticks < ticksAirInterval)
            {
                return;
            }

            //this.restoreBullet++;
            if (ShouldAttackNow)
            {
                airThings = AirThings(AirCells);
                if (airThings.Count > 0)
                {
                    // Log.Warning("c > 0");
                    var t = airThings.RandomElement();


                    turnAndAttack(t);
                }

                //  Log.Warning("air thing is "+this.airThings.Count);
            }

            ticks = 0;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002428 File Offset: 0x00000628
        private List<Thing> AirThings(IEnumerable<IntVec3> intVecs)
        {
            var list = new List<Thing>();
            list.Clear();
            foreach (var intVec in intVecs)
            {
                foreach (var thing in Map.thingGrid.ThingsListAt(intVec)
                    .Where(x => x != null))
                {
                    if (thing is Building || thing.def.altitudeLayer == AltitudeLayer.Item || thing is Filth ||
                        thing is Mote || thing is Pawn || thing is Plant)
                    {
                        continue;
                    }
                    //if(thing is Skyfaller)
                    // Log.Warning("thing in "+intVec+" is "+thing);

                    if (IsAirTarget(thing))
                    {
                        list.Add(thing);
                        //Log.Warning("thing in " + intVec + " is " + thing + " isair");
                    }
                }
            }

            return list;
        }


        public List<IntVec3> AirCellsAround(IntVec3 pos, Map map)
        {
            airCells.Clear();

            var num = GenRadial.NumCellsInRadius(range);
            for (var i = 0; i < num; i++)
            {
                airCells.Add(pos + GenRadial.RadialPattern[i]);
            }


            return airCells;
        }


        private void turnAndAttack(Thing t)
        {
            if (!IsAirTarget(t))
            {
                return;
            }

            nowTarget = t;
            doTurretTick();
            var sd = DefDatabase<SoundDef>.GetNamed(TopSizeComp.Props.soundShoot);
            sd.PlayOneShot(new TargetInfo(Position, Map, true));
            FleckMaker.ThrowExplosionCell(Position, Map, FleckDefOf.ExplosionFlash, new Color(1, 1, 1));

            MoteMaker.ThrowText(DrawPos, Map, "AirDefenseHit".Translate(), new Color(0.66f, 0.66f, 0.12f));
            if (t is Projectile)
            {
                FleckMaker.ThrowSmoke(t.DrawPos, Map, 3f);
            }
            //  GenExplosion.DoExplosion(t.Position, t.Map, 2f, DamageDefOf.Bomb, this, 3, -1, SoundDefOf.Artillery_ShellLoaded, null, null, t, null, 0, 1, false, null, 0, 1, 0, false);

            if (isPlayerInside(t) != null)
            {
                var builder = new StringBuilder();
                var text = "BeshotdownMsg".Translate(Faction.def.fixedName);

                // builder.AppendLine("Oh,no!One or more of our colonists were shot down by " + this.Faction.def.fixedName + "'s Antiaircraft gun:");
                builder.AppendLine(text);

                var pl = isPlayerInside(t);
                if (pl.Count > 0)
                {
                    foreach (var p in pl)
                    {
                        builder.AppendLine("    -" + p.Name);
                    }

                    Find.LetterStack.ReceiveLetter("Beshotdown".Translate(), builder.ToString(), LetterDefOf.Death,
                        this);
                }
            }

            destoryAir(t);
            nowTarget = null;

            //  Log.Warning(t + " is air target");
        }

        public void destoryAir(Thing t)
        {
            if (t is DropPodIncoming dp)
            {
                var leftthing = new List<Thing>();
                for (var i = dp.Contents.innerContainer.Count - 1; i >= 0; i--)
                {
                    var thing = dp.Contents.innerContainer[i];
                    if (thing is Pawn p)
                    {
                        p.Kill(null);
                        //Corpse cp = ((Corpse)ThingMaker.MakeThing(p.RaceProps.corpseDef,null));
                        //cp.InnerPawn = p;
                        //cp.GetComp<CompRottable>().RotImmediately();

                        //leftthing.Add(cp);
                        leftthing.Add(p);
                    }
                    else
                    {
                        leftthing.Add(thing);
                    }
                }


                var rr = new Random();
                foreach (var tt in leftthing)
                {
                    if (!(tt is Pawn))
                    {
                        var v = rr.Next(100);
                        // Log.Warning(v+"/"+Settings.droppodCargoDropPercentage+"%");
                        if (v >= Settings.droppodCargoDropPercentage)
                        {
                            continue;
                        }
                    }


                    GenPlace.TryPlaceThing(tt, dp.Position, Map, ThingPlaceMode.Near);


                    SoundDefOf.DropPod_Open.PlayOneShot(new TargetInfo(tt));
                }

                for (var i = 0; i < 3; i++)
                {
                    var sl = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel);
                    GenPlace.TryPlaceThing(sl, dp.Position, Map, ThingPlaceMode.Near);
                }
            }

            t.Destroy();
        }


        // Token: 0x06000010 RID: 16 RVA: 0x00002604 File Offset: 0x00000804
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticks, "ticks");
        }


        private bool IsAirTarget(Thing t)
        {
            if (t is Projectile p1)
            {
                var airbullet = p1.def.projectile.flyOverhead;

                if (!airbullet)
                {
                    return false;
                }

                // Log.Warning(BulletStore.theBullet.Count+" now");
                foreach (var pe in BulletStore.theBullet)
                {
                    if (pe.shell != p1)
                    {
                        continue;
                    }

                    // Log.Message("equal!");
                    if (pe.launcher == null)
                    {
                        continue;
                    }

                    //   Log.Message("not null");
                    if (pe.launcher.Faction != Faction && pe.launcher.Faction.HostileTo(Faction))
                    {
                        //  Log.Message("equal faction!");
                        // BulletStore.theBullet.Remove(pe);
                        return true;
                    }
                }
            }
            else if (t is DropPodIncoming dp)
            {
                for (var i = dp.Contents.innerContainer.Count - 1; i >= 0; i--)
                {
                    var thing = dp.Contents.innerContainer[i];
                    if (thing is not Pawn)
                    {
                        continue;
                    }

                    var p = thing as Pawn;
                    if (p.Faction != Faction && p.Faction.HostileTo(Faction))
                    {
                        return true;
                    }
                }

                return false;
            }


            return false;
        }


        public List<Pawn> isPlayerInside(Thing t)
        {
            var result = new List<Pawn>();
            if (t is not DropPodIncoming)
            {
                return result;
            }

            var dp = (DropPodIncoming) t;
            for (var i = dp.Contents.innerContainer.Count - 1; i >= 0; i--)
            {
                var thing = dp.Contents.innerContainer[i];
                if (thing is not Pawn)
                {
                    continue;
                }

                var p = thing as Pawn;
                if (p.Faction != Faction && p.Faction.HostileTo(Faction) && p.Faction == Faction.OfPlayer)
                {
                    result.Add(p);
                }
            }

            return result;
        }


        private void doTurretTick()
        {
            var powerComp = this.TryGetComp<CompPowerTrader>();
            if ((powerComp == null || powerComp.PowerOn) && Spawned)
            {
                //  if ((this.ShouldAttackNow))
                // {

                top.TurretTopTick();
                // }
            }
        }


        public override void Draw()
        {
            top.DrawTurret();
            Comps_PostDraw();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            topSizeComp = GetComp<CompTurretTopSize>();
        }
    }
}