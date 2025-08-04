using System;
using System.Collections.Generic;
using Verse;

namespace ra2;

public class AlliedStormCondition : ThingWithComps //GameCondition
{
    private const int RainDisableTicksAfterConditionEnds = 30000;

    // private static readonly IntRange AreaRadiusRange = new IntRange(10,35);

    private static readonly IntRange TicksBetweenStrikes = new(20, 60);
    private readonly List<IntVec3> canATKCells = [];

    private int areaRadius;

    public IntVec2 centerLocation;

    private int hitTime;

    private int nextLightningTicks;
    public Map SingleMap;
    private int ticks;


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref centerLocation, "centerLocation");
        Scribe_Values.Look(ref areaRadius, "areaRadius");
        Scribe_Values.Look(ref hitTime, "hitTime");
        Scribe_Values.Look(ref ticks, "ticks");
        Scribe_Values.Look(ref nextLightningTicks, "nextLightningTicks");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        //this.areaRadius = AlliedStormCondition.AreaRadiusRange.RandomInRange;
        //this.FindGoodCenterLocation();
        CellsAround(centerLocation.ToIntVec3);
    }

    protected override void Tick()
    {
        base.Tick();
        if (ticks < 120)
        {
            ticks++;
            return;
        }


        if (Find.TickManager.TicksGame > nextLightningTicks)
        {
            //  Vector2 vector = Rand.UnitVector2 * Rand.Range(0f, (float)this.areaRadius);
            var intVec =
                canATKCells.RandomElement();
            // new IntVec3((int)Math.Round((double)vector.x) + this.centerLocation.x, 0, (int)Math.Round((double)vector.y) + this.centerLocation.z);
            if (IsGoodLocationForStrike(intVec))
            {
                SingleMap.weatherManager.eventHandler.AddEvent(new WeatherObject_Thunder(SingleMap, intVec));
                hitTime++;
                nextLightningTicks = Find.TickManager.TicksGame + TicksBetweenStrikes.RandomInRange;
            }
        }

        if (hitTime > 16)
        {
            End();
        }
    }

    private void CellsAround(IntVec3 pos)
    {
        canATKCells.Clear();

        var num = GenRadial.NumCellsInRadius(30);
        for (var i = 0; i < num; i++)
        {
            canATKCells.Add(pos + GenRadial.RadialPattern[i]);
        }


        // return this.airCells;
    }

    private void End()
    {
        Destroy();
    }

    private void FindGoodCenterLocation()
    {
        if (SingleMap.Size.x <= 16 || SingleMap.Size.z <= 16)
        {
            throw new Exception("Map too small for flashstorm.");
        }

        for (var i = 0; i < 10; i++)
        {
            centerLocation = new IntVec2(Rand.Range(8, SingleMap.Size.x - 8), Rand.Range(8, SingleMap.Size.z - 8));
            if (IsGoodCenterLocation(centerLocation))
            {
                break;
            }
        }
    }

    private bool IsGoodLocationForStrike(IntVec3 loc)
    {
        return loc.InBounds(SingleMap); // && !loc.Roofed(SingleMap) && loc.Standable(SingleMap);
    }

    private bool IsGoodCenterLocation(IntVec2 loc)
    {
        var num = 0;
        var num2 = (int)(3.14159274f * areaRadius * areaRadius / 2f);
        foreach (var loc2 in GetPotentiallyAffectedCells(loc))
        {
            if (IsGoodLocationForStrike(loc2))
            {
                num++;
            }

            if (num >= num2)
            {
                break;
            }
        }

        return num >= num2;
    }

    private IEnumerable<IntVec3> GetPotentiallyAffectedCells(IntVec2 center)
    {
        for (var x = center.x - areaRadius; x <= center.x + areaRadius; x++)
        {
            for (var z = center.z - areaRadius; z <= center.z + areaRadius; z++)
            {
                if (((center.x - x) * (center.x - x)) + ((center.z - z) * (center.z - z)) <=
                    areaRadius * areaRadius)
                {
                    yield return new IntVec3(x, 0, z);
                }
            }
        }
    }
}