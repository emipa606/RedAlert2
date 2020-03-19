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
        // Token: 0x17000019 RID: 25
        // (get) Token: 0x060000D6 RID: 214 RVA: 0x00006502 File Offset: 0x00004702
        protected int TicksPassed
        {
            get
            {
                return Find.TickManager.TicksGame - this.startTick;
            }
        }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x060000D7 RID: 215 RVA: 0x00006515 File Offset: 0x00004715
        protected int TicksLeft
        {
            get
            {
                return this.duration - this.TicksPassed;
            }
        }

        // Token: 0x060000D8 RID: 216 RVA: 0x00006524 File Offset: 0x00004724
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.startTick, "startTick", 0, false);
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00006540 File Offset: 0x00004740
        public override void Draw()
        {
            base.Comps_PostDraw();
            WorldRendererUtility.DrawQuadTangentialToPlanet(this.DrawPos, MRC(0f, (float)this.BlastRadius, 5f, 0f, (float)this.shockwave), 0.008f, GraphicDatabase.Get<Graphic_Single>("ra2/Things/Misc/Nuclear/flash", ShaderDatabase.MoteGlow, new Vector2(20f, 20f), Color.white).MatSingle, false, false, null);
        }
        public static float MRC(float InputRangeA, float InputRangeB, float OutputRangeA, float OutputRangeB, float Value)
        {
            float pct = Mathf.Clamp(GetRangePct(InputRangeA, InputRangeB, Value), 0f, 1f);
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
            if (this.startTick == 0 && base.Spawned)
            {
              
               
                this.StartStrike();
            }
            if (this.startTick != 0 && this.TicksPassed >= this.duration)
            {
                this.Destroy(DestroyMode.Vanish);
            }
        }

        // Token: 0x060000DB RID: 219 RVA: 0x000065C8 File Offset: 0x000047C8
        public virtual void StartStrike()
        {
            if (!base.Spawned)
            {
                Log.Error("Called StartStrike() on unspawned thing.", false);
                return;
            }
            this.startTick = Find.TickManager.TicksGame;
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
                if (Find.CurrentMap == this.Map)
                {
                    SoundStarter.PlayOneShotOnCamera(SoundDef.Named("ra2_NuclearExpBefore"));
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
            
            LongEventHandler.QueueLongEvent(delegate
            {
                this.StripRoofs();
            }, "Detonating", false, null);
            LongEventHandler.QueueLongEvent(delegate
            {
                this.Flash();
            }, "Detonating", false, null);
            LongEventHandler.QueueLongEvent(delegate
            {
                this.GetCells();
            }, "Detonating", false, null);

            LongEventHandler.QueueLongEvent(delegate
            {
                this.BlastWaveLeaves();
            }, "Detonating", false, null);
            
            LongEventHandler.QueueLongEvent(delegate
            {
                this.BlastWavePawn();
            }, "Detonating", false, null);
            
            LongEventHandler.QueueLongEvent(delegate
            {
                this.BlastWaveThing();
            }, "Detonating", false, null);
            
            LongEventHandler.QueueLongEvent(delegate
            {
                this.BurnSurface();
            }, "Detonating", false, null);
            
            LongEventHandler.QueueLongEvent(delegate
            {
                this.LeaveAsh();
            }, "Detonating", false, null);
            
            GenExplosion.DoExplosion(this.landPos,this.Map,range,DefDatabase<DamageDef>.GetNamed("Nuclear", true),this,1,-1,null,null,null,null,null,0,1,true,null,0,1,0,false);
            if (Find.CurrentMap == this.Map)
            {
                SoundStarter.PlayOneShotOnCamera(SoundDef.Named("ra2_NuclearExplode"));
            }
          
        }

        // Token: 0x060000DC RID: 220 RVA: 0x00006888 File Offset: 0x00004A88
        public float DamageAt(IntVec3 c)
        {
            float num = base.Position.DistanceTo(c);
            if (num < 1f)
            {
                return 500f;
            }
            // return this.Yield * -Mathf.Log(num / this.Yield);
            return 500f - 1f*num;
        }

        // Token: 0x060000DD RID: 221 RVA: 0x000068C8 File Offset: 0x00004AC8
        public void StripRoofs()
        {
            foreach (IntVec3 c in CellsAround(landPos, this.Map, range))
            {
                if (!c.IsValid)
                    continue;

                if (c.GetRoof(base.Map) == RoofDefOf.RoofConstructed)
                {
                    base.Map.roofGrid.SetRoof(c, null);
                }
            }
        }
        public List<IntVec3> CellsAround(IntVec3 pos, Map map,int range)
        {
            List<IntVec3> result = new List<IntVec3>();

            int num = GenRadial.NumCellsInRadius(range);
            for (int i = 0; i < num; i++)
            {
               result.Add(pos + GenRadial.RadialPattern[i]);
               
            }


            return result;

        }

        // Token: 0x060000DE RID: 222 RVA: 0x00006938 File Offset: 0x00004B38
        public void GetCells()
        {
            this.cellsToAffect.Clear();
            this.damagedThings.Clear();
            SovietNuclearStrike.openCells.Clear();
            SovietNuclearStrike.adjWallCells.Clear();
            Map map = base.Map;
            List<IntVec3> cells = CellsAround(this.landPos,this.Map,range);


            foreach (IntVec3 intVec in cells)
            {
                if (!intVec.IsValid)
                    continue;
                if (intVec.InBounds(map) && !intVec.Roofed(map))
                {
                    SovietNuclearStrike.openCells.Add(intVec);
                }
            }
            for (int i = 0; i < SovietNuclearStrike.openCells.Count; i++)
            {
                IntVec3 intVec2 = SovietNuclearStrike.openCells[i];
                if (intVec2.Walkable(map))
                {
                    for (int j = 0; j < 4; j++)
                    {
                        IntVec3 intVec3 = intVec2 + GenAdj.CardinalDirections[j];
                        if (intVec3.InBounds(map) && !intVec3.Standable(map) && intVec3.GetEdifice(map) != null && !SovietNuclearStrike.openCells.Contains(intVec3) && SovietNuclearStrike.adjWallCells.Contains(intVec3))
                        {
                            if (!intVec3.IsValid)
                                continue;
                            SovietNuclearStrike.adjWallCells.Add(intVec3);
                        }
                    }
                }
            }
            this.cellsToAffect.AddRange(SovietNuclearStrike.openCells.Concat(SovietNuclearStrike.adjWallCells));
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00006A84 File Offset: 0x00004C84
        public void Flash()
        {
            foreach (IntVec3 intVec in GenRadial.RadialCellsAround(base.Position, this.Yield / 10f, true))
            {
                if (!intVec.IsValid)
                    continue;
                try
                {
                    this.DamageAt(intVec);
                    base.Map.roofGrid.SetRoof(intVec, null);
                    base.Map.terrainGrid.RemoveTopLayer(intVec, false);
                    TerrainDef driesTo = base.Map.terrainGrid.TerrainAt(intVec).driesTo;
                    if (driesTo != null)
                    {
                        base.Map.terrainGrid.SetTerrain(intVec, driesTo);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "Flash could not affect cell ",
                        intVec,
                        ": ",
                        ex
                    }), false);
                }
            }
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x00006B7C File Offset: 0x00004D7C
        public void BlastWaveThing()
        {
            foreach (IntVec3 intVec in this.cellsToAffect)
            {
                if (!intVec.IsValid)
                    continue;
                float num = this.DamageAt(intVec);
                if (num > 0f)
                {
                    List<Thing> list = base.Map.thingGrid.ThingsListAt(intVec);
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            Thing thing = list[i];
                            if ((thing.def.category == ThingCategory.Building || thing.def.category == ThingCategory.Item) && thing != this && !thing.Destroyed)
                            {
                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Bomb, num, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                thing.TakeDamage(dinfo);
                                if (!thing.Destroyed)
                                {
                                    CompRottable compRottable = thing.TryGetComp<CompRottable>();
                                    if (compRottable != null && compRottable.Stage < RotStage.Dessicated)
                                    {
                                        compRottable.RotProgress += 3000f;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(string.Concat(new object[]
                            {
                                "Explosion could not affect cell ",
                                intVec,
                                ": ",
                                ex
                            }), false);
                        }
                    }
                }
            }
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x00006CF8 File Offset: 0x00004EF8
        public void BlastWavePawn()
        {
            foreach (IntVec3 intVec in this.cellsToAffect)
            {
                if (!intVec.IsValid)
                    continue;
                try
                {
                    float num = this.DamageAt(intVec);
                    if (num > 0f)
                    {
                        List<Thing> list = base.Map.thingGrid.ThingsListAt(intVec);
                        for (int i = 0; i < list.Count; i++)
                        {
                            Pawn pawn;
                            if ((pawn = (list[i] as Pawn)) != null)
                            {
                                
                                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Burn, num / 10f, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                    dinfo.SetBodyRegion(BodyPartHeight.Top, BodyPartDepth.Outside);
                                    pawn.TakeDamage(dinfo);
                                    DamageInfo dinfo2 = new DamageInfo(DamageDefOf.Bomb, num, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                    pawn.TakeDamage(dinfo2);
                                
                                if (pawn.Dead)
                                {
                                    CompRottable compRottable = pawn.Corpse.TryGetComp<CompRottable>();
                                    if (compRottable != null)
                                    {
                                        compRottable.RotProgress = 1E+10f;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "BlastWavePawn could not affect cell ",
                        intVec,
                        ": ",
                        ex
                    }), false);
                }
            }
        }

        // Token: 0x060000E2 RID: 226 RVA: 0x00006E84 File Offset: 0x00005084
        public void BlastWaveLeaves()
        {

            foreach (IntVec3 intVec in this.cellsToAffect)
            {
                if (!intVec.IsValid)
                    continue;
                try
                {
                    if (this.DamageAt(intVec) > 0f)
                    {
                        List<Thing> list = base.Map.thingGrid.ThingsListAt(intVec);
                        for (int i = 0; i < list.Count; i++)
                        {
                            Plant plant;
                            if ((plant = (list[i] as Plant)) != null)
                            {
                                plant.MakeLeafless(Plant.LeaflessCause.Poison);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "BlastWaveLeaves could not affect cell ",
                        intVec,
                        ": ",
                        ex
                    }), false);
                }
            }
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x00006F64 File Offset: 0x00005164
        public void BlastWavePlants()
        {

            foreach (IntVec3 intVec in this.cellsToAffect)
            {
                if (!intVec.IsValid)
                    continue;
                try
                {
                    float num = this.DamageAt(intVec);
                    if (num > 0f)
                    {
                        List<Thing> list = base.Map.thingGrid.ThingsListAt(intVec);
                        for (int i = 0; i < list.Count; i++)
                        {
                            Plant plant;
                            if (list[i].def.category == ThingCategory.Plant && (plant = (list[i] as Plant)) != null)
                            {
                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Bomb, num, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                plant.TakeDamage(dinfo);
                                if (!plant.Destroyed && Rand.Value * 300f > num)
                                {
                                    plant.Destroy(DestroyMode.Vanish);
                                    if (plant.def.plant.IsTree && plant.LifeStage != PlantLifeStage.Sowing && plant.def != ThingDefOf.BurnedTree)
                                    {
                                        ((DeadPlant)GenSpawn.Spawn(ThingDefOf.BurnedTree, plant.Position, base.Map, WipeMode.Vanish)).Growth = plant.Growth;
                                    }
                                }
                                else if (Rand.Chance(0.25f))
                                {
                                    Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
                                    fire.fireSize = 1f;
                                    GenSpawn.Spawn(fire, plant.Position, base.Map, Rot4.North, WipeMode.Vanish, false);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "BlastWavePlants could not affect cell ",
                        intVec,
                        ": ",
                        ex
                    }), false);
                }
            }
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x0000715C File Offset: 0x0000535C
        public void BurnSurface()
        {
            foreach (IntVec3 intVec in this.cellsToAffect)
            {
                if (!intVec.IsValid)
                    continue;
                try
                {
                    TerrainGrid terrainGrid = base.Map.terrainGrid;
                    TerrainDef terrain = intVec.GetTerrain(base.Map);
                    if (terrain.burnedDef != null && intVec.TerrainFlammableNow(base.Map))
                    {
                        terrainGrid.RemoveTopLayer(intVec, false);
                        terrainGrid.SetTerrain(intVec, terrain.burnedDef);
                    }
                    if (base.Map.snowGrid.GetDepth(intVec) > 0f)
                    {
                        base.Map.snowGrid.SetDepth(intVec, 0f);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "BurnSurface could not affect cell ",
                        intVec,
                        ": ",
                        ex
                    }), false);
                }
            }
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x00007264 File Offset: 0x00005464
        public void LeaveAsh()
        {
            foreach (IntVec3 intVec in this.cellsToAffect)
            {
                if (!intVec.IsValid)
                    continue;
                float num = base.Position.DistanceTo(intVec);
                if (Rand.Value * 100f > num)
                {
                    FilthMaker.TryMakeFilth(intVec, base.Map, ThingDefOf.Filth_Ash, 1);
                }
            }
        }

        // Token: 0x060000E6 RID: 230 RVA: 0x000072E0 File Offset: 0x000054E0
        public void GenShockwave(int center, int radius)
        {
            Find.WorldFloodFiller.FloodFill(center, (int tile) => true, delegate (int tile, int dist)
            {
                if (dist > radius + 1)
                {
                    return true;
                }
                if (dist == radius + 1)
                {
                    Find.WorldGrid.tiles[tile].biome = BiomeDefOf.Tundra;
                    Settlement settlementBase = Find.WorldObjects.SettlementBases.FirstOrDefault((Settlement x) => x.Tile == tile);
                    if (settlementBase != null)
                    {
                        Find.WorldObjects.Remove(settlementBase);
                    }
                    float value = Rand.Value;
                }
                return false;
            }, int.MaxValue, null);
        }

        // Token: 0x060000E7 RID: 231 RVA: 0x00007338 File Offset: 0x00005538

        public IntVec3 landPos;
        // Token: 0x04000084 RID: 132
        private int duration = 6000;

        // Token: 0x04000085 RID: 133
        private int startTick;

        // Token: 0x04000086 RID: 134
        public int shockwave = 20;

        // Token: 0x04000087 RID: 135
        public int BlastRadius = 20;

        // Token: 0x04000088 RID: 136
        private static List<Thing> thingsToAffect = new List<Thing>();

        // Token: 0x04000089 RID: 137
        private static List<IntVec3> openCells = new List<IntVec3>();

        // Token: 0x0400008A RID: 138
        private static List<IntVec3> adjWallCells = new List<IntVec3>();

        // Token: 0x0400008B RID: 139
        private List<IntVec3> cellsToAffect = new List<IntVec3>();

        // Token: 0x0400008C RID: 140
        private List<Thing> damagedThings = new List<Thing>();

        // Token: 0x0400008D RID: 141
        public float Yield = 250f;

        private int range = 56;
    }
}
