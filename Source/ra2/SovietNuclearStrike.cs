using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

public class SovietNuclearStrike : ThingWithComps
{
    private static List<Thing> thingsToAffect = [];

    private static readonly List<IntVec3> openCells = [];

    private static readonly List<IntVec3> adjWallCells = [];

    private static readonly ThingDef burnedTree = ThingDef.Named("BurnedTree");

    public readonly int BlastRadius = 20;

    private readonly List<IntVec3> cellsToAffect = [];

    private readonly List<Thing> damagedThings = [];

    private readonly int duration = 6000;

    private readonly int range = 56;

    public readonly int shockwave = 20;

    public readonly float Yield = 250f;


    public IntVec3 landPos;

    private int startTick;

    protected int TicksPassed => Find.TickManager.TicksGame - startTick;

    protected int TicksLeft => duration - TicksPassed;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref startTick, "startTick");
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        Comps_PostDraw();
        WorldRendererUtility.DrawQuadTangentialToPlanet(drawLoc, MRC(0f, BlastRadius, 5f, 0f, shockwave), 0.008f,
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

    public virtual void StartStrike()
    {
        if (!Spawned)
        {
            Log.Error("Called StartStrike() on unspawned thing.");
            return;
        }

        startTick = Find.TickManager.TicksGame;
        Find.TickManager.slower.SignalForceNormalSpeed();


        //  this.StripRoofs();


        LongEventHandler.QueueLongEvent(delegate
        {
            if (Find.CurrentMap == Map)
            {
                SoundDef.Named("ra2_NuclearExpBefore").PlayOneShotOnCamera();
            }
        }, "Emmmm", false, null);

        LongEventHandler.QueueLongEvent(StripRoofs, "Detonating", false, null);
        LongEventHandler.QueueLongEvent(Flash, "Detonating", false, null);
        LongEventHandler.QueueLongEvent(GetCells, "Detonating", false, null);

        LongEventHandler.QueueLongEvent(BlastWaveLeaves, "Detonating", false, null);

        LongEventHandler.QueueLongEvent(BlastWavePawn, "Detonating", false, null);

        LongEventHandler.QueueLongEvent(BlastWaveThing, "Detonating", false, null);

        LongEventHandler.QueueLongEvent(BurnSurface, "Detonating", false, null);

        LongEventHandler.QueueLongEvent(LeaveAsh, "Detonating", false, null);

        GenExplosion.DoExplosion(landPos, Map, range, DefDatabase<DamageDef>.GetNamed("Nuclear"), this, 1, -1, null,
            null, null, null, null, 0, 1, null, true);
        if (Find.CurrentMap == Map)
        {
            SoundDef.Named("ra2_NuclearExplode").PlayOneShotOnCamera();
        }
    }

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
                Log.Warning($"Flash could not affect cell {intVec}: {ex}");
            }
        }
    }

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
                    if (thing1.def.category != ThingCategory.Building &&
                        thing1.def.category != ThingCategory.Item || thing1 == this || thing1.Destroyed)
                    {
                        continue;
                    }

                    var dinfo = new DamageInfo(DamageDefOf.Bomb, num);
                    thing1.TakeDamage(dinfo);
                    if (thing1.Destroyed)
                    {
                        continue;
                    }

                    var compRottable = thing1.TryGetComp<CompRottable>();
                    if (compRottable is { Stage: < RotStage.Dessicated })
                    {
                        compRottable.RotProgress += 3000f;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"Explosion could not affect cell {intVec}: {ex}");
                }
            }
        }
    }

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
                Log.Warning($"BlastWavePawn could not affect cell {intVec}: {ex}");
            }
        }
    }

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
                Log.Warning($"BlastWaveLeaves could not affect cell {intVec}: {ex}");
            }
        }
    }

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
                                plant.def != burnedTree)
                            {
                                ((DeadPlant)GenSpawn.Spawn(burnedTree, plant.Position, Map))
                                    .Growth = plant.Growth;
                            }
                        }
                        else if (Rand.Chance(0.25f))
                        {
                            var fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
                            fire.fireSize = 1f;
                            GenSpawn.Spawn(fire, plant.Position, Map, Rot4.North);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"BlastWavePlants could not affect cell {intVec}: {ex}");
            }
        }
    }

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
                Log.Warning($"BurnSurface could not affect cell {intVec}: {ex}");
            }
        }
    }

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

            _ = Rand.Value;

            return false;
        });
    }
}