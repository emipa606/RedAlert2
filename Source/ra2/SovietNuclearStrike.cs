using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    // Token: 0x0200002C RID: 44
    public class SovietNuclearStrike : ThingWithComps
    {
        // Token: 0x04000088 RID: 136
        private static List<Thing> thingsToAffect = new List<Thing>();

        // Token: 0x04000089 RID: 137
        private static readonly List<IntVec3> openCells = new List<IntVec3>();

        // Token: 0x0400008A RID: 138
        private static readonly List<IntVec3> adjWallCells = new List<IntVec3>();

        // Token: 0x0400008B RID: 139
        private readonly List<IntVec3> cellsToAffect = new List<IntVec3>();

        // Token: 0x0400008C RID: 140
        private readonly List<Thing> damagedThings = new List<Thing>();

        // Token: 0x04000084 RID: 132
        private readonly int duration = 6000;

        private readonly int range = 56;

        // Token: 0x04000087 RID: 135
        public int BlastRadius = 20;

        // Token: 0x060000E7 RID: 231 RVA: 0x00007338 File Offset: 0x00005538

        public IntVec3 landPos;

        // Token: 0x04000086 RID: 134
        public int shockwave = 20;

        // Token: 0x04000085 RID: 133
        private int startTick;

        // Token: 0x0400008D RID: 141
        public float Yield = 250f;

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x060000D6 RID: 214 RVA: 0x00006502 File Offset: 0x00004702
        protected int TicksPassed => Find.TickManager.TicksGame - startTick;

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x060000D7 RID: 215 RVA: 0x00006515 File Offset: 0x00004715
        protected int TicksLeft => duration - TicksPassed;

        // Token: 0x060000D8 RID: 216 RVA: 0x00006524 File Offset: 0x00004724
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref startTick, "startTick");
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00006540 File Offset: 0x00004740
        public override void Draw()
        {
            Comps_PostDraw();
            WorldRendererUtility.DrawQuadTangentialToPlanet(DrawPos, MRC(0f, BlastRadius, 5f, 0f, shockwave), 0.008f,
                GraphicDatabase.Get<Graphic_Single>("ra2/Things/Misc/Nuclear/flash", ShaderDatabase.MoteGlow,
                    new Vector2(20f, 20f), Color.white).MatSingle);
        }

        public static float MRC(float InputRangeA, float InputRangeB, float OutputRangeA, float OutputRangeB,
            float Value)
        {
            var pct = Mathf.Clamp(GetRangePct(InputRangeA, InputRangeB, Value), 0f, 1f);
            return GetRangeValue(OutputRangeA, OutputRangeB, pct);
        }

        public static float GetRangePct(float RangeA, float RangeB, float Value)
        {
            if (RangeA == RangeB)
            {
                return RangeA;
            }

            return (Value - RangeA) / (RangeB - RangeA);
        }

        public static float GetRangeValue(float RangeA, float RangeB, float Pct)
        {
            return Mathf.Lerp(RangeA, RangeB, Pct);
        }

        // Token: 0x060000DA RID: 218 RVA: 0x0000658D File Offset: 0x0000478D
        public override void Tick()
        {
            base.Tick();
            if (startTick == 0 && Spawned)
            {
                StartStrike();
            }

            if (startTick != 0 && TicksPassed >= duration)
            {
                Destroy();
            }
        }

        // Token: 0x060000DB RID: 219 RVA: 0x000065C8 File Offset: 0x000047C8
        public virtual void StartStrike()
        {
            if (!Spawned)
            {
                Log.Error("Called StartStrike() on unspawned thing.");
                return;
            }

            startTick = Find.TickManager.TicksGame;
            // base.GetComp<CompAffectsSky>().StartFadeInHoldFadeOut(30, this.duration - 30 - 15, 15, 1f);

            //  MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("Mote_NukeFlash"), null);
            //  moteThrown.Scale = 400f;
            //    moteThrown.exactPosition = base.Position.ToVector3();
            //   GenSpawn.Spawn(moteThrown, base.Position, base.Map, WipeMode.Vanish);
            //  MoteThrown moteThrown2 = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("Mote_NukeCloud"), null);
            //   moteThrown2.Scale = 150f;
            //   moteThrown2.rotationRate = 1f;
            //    moteThrown2.exactPosition = base.Position.ToVector3();
            //   GenSpawn.Spawn(moteThrown2, base.Position, base.Map, WipeMode.Vanish);
            //  MoteThrown moteThrown3 = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("Mote_NukeCloud"), null);
            //   moteThrown3.rotationRate = -1f;
            //     GenSpawn.Spawn(moteThrown3, base.Position, base.Map, WipeMode.Vanish);

            /*
            if (Find.CurrentMap == this.Map)
            {
                SoundStarter.PlayOneShotOnCamera(SoundDef.Named("ra2_NuclearExpBefore"));
            }
            else
            {
             //   DubDef.hugeExplosionDistantDef.PlayOneShotOnCamera(null);
            }
            */
            Find.TickManager.slower.SignalForceNormalSpeed();


            //  this.StripRoofs();


            LongEventHandler.QueueLongEvent(delegate
            {
                if (Find.CurrentMap == Map)
                {
                    SoundDef.Named("ra2_NuclearExpBefore").PlayOneShotOnCamera();
                }
            }, "Emmmm", false, null);
            /*
            this.Flash();
            this.GetCells();
            this.BlastWaveLeaves();
            this.BlastWavePawn();
            this.BlastWaveThing();
            this.BurnSurface();
            this.LeaveAsh();
            */

            LongEventHandler.QueueLongEvent(StripRoofs, "Detonating", false, null);
            LongEventHandler.QueueLongEvent(Flash, "Detonating", false, null);
            LongEventHandler.QueueLongEvent(GetCells, "Detonating", false, null);

            LongEventHandler.QueueLongEvent(BlastWaveLeaves, "Detonating", false, null);

            LongEventHandler.QueueLongEvent(BlastWavePawn, "Detonating", false, null);

            LongEventHandler.QueueLongEvent(BlastWaveThing, "Detonating", false, null);

            LongEventHandler.QueueLongEvent(BurnSurface, "Detonating", false, null);

            LongEventHandler.QueueLongEvent(LeaveAsh, "Detonating", false, null);

            GenExplosion.DoExplosion(landPos, Map, range, DefDatabase<DamageDef>.GetNamed("Nuclear"), this, 1, -1, null,
                null, null, null, null, 0, 1, true);
            if (Find.CurrentMap == Map)
            {
                SoundDef.Named("ra2_NuclearExplode").PlayOneShotOnCamera();
            }
        }

        // Token: 0x060000DC RID: 220 RVA: 0x00006888 File Offset: 0x00004A88
        public float DamageAt(IntVec3 c)
        {
            var num = Position.DistanceTo(c);
            if (num < 1f)
            {
                return 500f;
            }

            // return this.Yield * -Mathf.Log(num / this.Yield);
            return 500f - (1f * num);
        }

        // Token: 0x060000DD RID: 221 RVA: 0x000068C8 File Offset: 0x00004AC8
        public void StripRoofs()
        {
            foreach (var c in CellsAround(landPos, Map, range))
            {
                if (!c.IsValid)
                {
                    continue;
                }

                if (c.GetRoof(Map) == RoofDefOf.RoofConstructed)
                {
                    Map.roofGrid.SetRoof(c, null);
                }
            }
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

        // Token: 0x060000DE RID: 222 RVA: 0x00006938 File Offset: 0x00004B38
        public void GetCells()
        {
            cellsToAffect.Clear();
            damagedThings.Clear();
            openCells.Clear();
            adjWallCells.Clear();
            var map = Map;
            var cells = CellsAround(landPos, Map, range);


            foreach (var intVec in cells)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                if (intVec.InBounds(map) && !intVec.Roofed(map))
                {
                    openCells.Add(intVec);
                }
            }

            foreach (var intVec2 in openCells)
            {
                if (!intVec2.Walkable(map))
                {
                    continue;
                }

                for (var j = 0; j < 4; j++)
                {
                    var intVec3 = intVec2 + GenAdj.CardinalDirections[j];
                    if (!intVec3.InBounds(map) || intVec3.Standable(map) || intVec3.GetEdifice(map) == null ||
                        openCells.Contains(intVec3) || !adjWallCells.Contains(intVec3))
                    {
                        continue;
                    }

                    if (!intVec3.IsValid)
                    {
                        continue;
                    }

                    adjWallCells.Add(intVec3);
                }
            }

            cellsToAffect.AddRange(openCells.Concat(adjWallCells));
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00006A84 File Offset: 0x00004C84
        public void Flash()
        {
            foreach (var intVec in GenRadial.RadialCellsAround(Position, Yield / 10f, true))
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                try
                {
                    DamageAt(intVec);
                    Map.roofGrid.SetRoof(intVec, null);
                    Map.terrainGrid.RemoveTopLayer(intVec, false);
                    var driesTo = Map.terrainGrid.TerrainAt(intVec).driesTo;
                    if (driesTo != null)
                    {
                        Map.terrainGrid.SetTerrain(intVec, driesTo);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat("Flash could not affect cell ", intVec, ": ", ex));
                }
            }
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x00006B7C File Offset: 0x00004D7C
        public void BlastWaveThing()
        {
            foreach (var intVec in cellsToAffect)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                var num = DamageAt(intVec);
                if (!(num > 0f))
                {
                    continue;
                }

                var list = Map.thingGrid.ThingsListAt(intVec);
                foreach (var thing1 in list)
                {
                    try
                    {
                        var thing = thing1;
                        if (thing.def.category != ThingCategory.Building &&
                            thing.def.category != ThingCategory.Item || thing == this || thing.Destroyed)
                        {
                            continue;
                        }

                        var dinfo = new DamageInfo(DamageDefOf.Bomb, num);
                        thing.TakeDamage(dinfo);
                        if (thing.Destroyed)
                        {
                            continue;
                        }

                        var compRottable = thing.TryGetComp<CompRottable>();
                        if (compRottable != null && compRottable.Stage < RotStage.Dessicated)
                        {
                            compRottable.RotProgress += 3000f;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(string.Concat("Explosion could not affect cell ", intVec, ": ", ex));
                    }
                }
            }
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x00006CF8 File Offset: 0x00004EF8
        public void BlastWavePawn()
        {
            foreach (var intVec in cellsToAffect)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                try
                {
                    var num = DamageAt(intVec);
                    if (num > 0f)
                    {
                        var list = Map.thingGrid.ThingsListAt(intVec);
                        foreach (var thing in list)
                        {
                            Pawn pawn;
                            if ((pawn = thing as Pawn) == null)
                            {
                                continue;
                            }

                            var dinfo = new DamageInfo(DamageDefOf.Burn, num / 10f);
                            dinfo.SetBodyRegion(BodyPartHeight.Top, BodyPartDepth.Outside);
                            pawn.TakeDamage(dinfo);
                            var dinfo2 = new DamageInfo(DamageDefOf.Bomb, num);
                            pawn.TakeDamage(dinfo2);

                            if (!pawn.Dead)
                            {
                                continue;
                            }

                            var compRottable = pawn.Corpse.TryGetComp<CompRottable>();
                            if (compRottable != null)
                            {
                                compRottable.RotProgress = 1E+10f;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat("BlastWavePawn could not affect cell ", intVec, ": ", ex));
                }
            }
        }

        // Token: 0x060000E2 RID: 226 RVA: 0x00006E84 File Offset: 0x00005084
        public void BlastWaveLeaves()
        {
            foreach (var intVec in cellsToAffect)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                try
                {
                    if (DamageAt(intVec) > 0f)
                    {
                        var list = Map.thingGrid.ThingsListAt(intVec);
                        foreach (var thing in list)
                        {
                            Plant plant;
                            if ((plant = thing as Plant) != null)
                            {
                                plant.MakeLeafless(Plant.LeaflessCause.Poison);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat("BlastWaveLeaves could not affect cell ", intVec, ": ", ex));
                }
            }
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x00006F64 File Offset: 0x00005164
        public void BlastWavePlants()
        {
            foreach (var intVec in cellsToAffect)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                try
                {
                    var num = DamageAt(intVec);
                    if (num > 0f)
                    {
                        var list = Map.thingGrid.ThingsListAt(intVec);
                        foreach (var thing in list)
                        {
                            Plant plant;
                            if (thing.def.category != ThingCategory.Plant || (plant = thing as Plant) == null)
                            {
                                continue;
                            }

                            var dinfo = new DamageInfo(DamageDefOf.Bomb, num);
                            plant.TakeDamage(dinfo);
                            if (!plant.Destroyed && Rand.Value * 300f > num)
                            {
                                plant.Destroy();
                                if (plant.def.plant.IsTree && plant.LifeStage != PlantLifeStage.Sowing &&
                                    plant.def != ThingDefOf.BurnedTree)
                                {
                                    ((DeadPlant) GenSpawn.Spawn(ThingDefOf.BurnedTree, plant.Position, Map))
                                        .Growth = plant.Growth;
                                }
                            }
                            else if (Rand.Chance(0.25f))
                            {
                                var fire = (Fire) ThingMaker.MakeThing(ThingDefOf.Fire);
                                fire.fireSize = 1f;
                                GenSpawn.Spawn(fire, plant.Position, Map, Rot4.North);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat("BlastWavePlants could not affect cell ", intVec, ": ", ex));
                }
            }
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x0000715C File Offset: 0x0000535C
        public void BurnSurface()
        {
            foreach (var intVec in cellsToAffect)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                try
                {
                    var terrainGrid = Map.terrainGrid;
                    var terrain = intVec.GetTerrain(Map);
                    if (terrain.burnedDef != null && intVec.TerrainFlammableNow(Map))
                    {
                        terrainGrid.RemoveTopLayer(intVec, false);
                        terrainGrid.SetTerrain(intVec, terrain.burnedDef);
                    }

                    if (Map.snowGrid.GetDepth(intVec) > 0f)
                    {
                        Map.snowGrid.SetDepth(intVec, 0f);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat("BurnSurface could not affect cell ", intVec, ": ", ex));
                }
            }
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x00007264 File Offset: 0x00005464
        public void LeaveAsh()
        {
            foreach (var intVec in cellsToAffect)
            {
                if (!intVec.IsValid)
                {
                    continue;
                }

                var num = Position.DistanceTo(intVec);
                if (Rand.Value * 100f > num)
                {
                    FilthMaker.TryMakeFilth(intVec, Map, ThingDefOf.Filth_Ash);
                }
            }
        }

        // Token: 0x060000E6 RID: 230 RVA: 0x000072E0 File Offset: 0x000054E0
        public void GenShockwave(int center, int radius)
        {
            Find.WorldFloodFiller.FloodFill(center, _ => true, delegate(int tile, int dist)
            {
                if (dist > radius + 1)
                {
                    return true;
                }

                if (dist != radius + 1)
                {
                    return false;
                }

                Find.WorldGrid.tiles[tile].biome = BiomeDefOf.Tundra;
                var settlementBase = Find.WorldObjects.SettlementBases.FirstOrDefault(x => x.Tile == tile);
                if (settlementBase != null)
                {
                    Find.WorldObjects.Remove(settlementBase);
                }

                var unused = Rand.Value;

                return false;
            });
        }
    }
}