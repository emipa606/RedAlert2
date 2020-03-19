using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ra2
{
    // Token: 0x02000318 RID: 792
    public class AlliedStormCondition : ThingWithComps//GameCondition
    {


        // Token: 0x06000D6C RID: 3436 RVA: 0x00065634 File Offset: 0x00063A34
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<IntVec2>(ref this.centerLocation, "centerLocation", default(IntVec2), false);
            Scribe_Values.Look<int>(ref this.areaRadius, "areaRadius", 0, false);
            Scribe_Values.Look<int>(ref this.hitTime, "hitTime", 0, false);
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<int>(ref this.nextLightningTicks, "nextLightningTicks", 0, false);
        }

        // Token: 0x06000D6D RID: 3437 RVA: 0x00065688 File Offset: 0x00063A88
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            //this.areaRadius = AlliedStormCondition.AreaRadiusRange.RandomInRange;
            //this.FindGoodCenterLocation();
            CellsAround(centerLocation.ToIntVec3,map);
           
        }
        /*
        public override void Init()
        {
            base.Init();
            this.areaRadius = AlliedStormCondition.AreaRadiusRange.RandomInRange;
            this.FindGoodCenterLocation();
        }
        */
        // Token: 0x06000D6E RID: 3438 RVA: 0x000656B4 File Offset: 0x00063AB4
        public override void Tick()
        {
            base.Tick();
            if (this.ticks < 120) {
                this.ticks++;
                return;
            }



            if (Find.TickManager.TicksGame > this.nextLightningTicks)
            {
              //  Vector2 vector = Rand.UnitVector2 * Rand.Range(0f, (float)this.areaRadius);
                IntVec3 intVec = 
                    canATKCells.RandomElement<IntVec3>();
               // new IntVec3((int)Math.Round((double)vector.x) + this.centerLocation.x, 0, (int)Math.Round((double)vector.y) + this.centerLocation.z);
                if (this.IsGoodLocationForStrike(intVec))
                {
                    SingleMap.weatherManager.eventHandler.AddEvent(new WeatherObject_Thunder(SingleMap, intVec));
                    hitTime++;
                    this.nextLightningTicks = Find.TickManager.TicksGame + AlliedStormCondition.TicksBetweenStrikes.RandomInRange;
                }
            }
            if (hitTime > 16) End();
        }
        public void CellsAround(IntVec3 pos, Map map)
        {
            this.canATKCells.Clear();

            int num = GenRadial.NumCellsInRadius(30);
            for (int i = 0; i < num; i++)
            {
                canATKCells.Add((pos) +(GenRadial.RadialPattern[i]));
            }


           // return this.airCells;

        }
        /*
        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame > this.nextLightningTicks)
            {
                Vector2 vector = Rand.UnitVector2 * Rand.Range(0f, (float)this.areaRadius);
                IntVec3 intVec = new IntVec3((int)Math.Round((double)vector.x) + this.centerLocation.x, 0, (int)Math.Round((double)vector.y) + this.centerLocation.z);
                if (this.IsGoodLocationForStrike(intVec))
                {
                    base.SingleMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(base.SingleMap, intVec));
                    this.nextLightningTicks = Find.TickManager.TicksGame + AlliedStormCondition.TicksBetweenStrikes.RandomInRange;
                }
            }
            if (hitTime > 12) End();
        }
        */
        // Token: 0x06000D6F RID: 3439 RVA: 0x00065779 File Offset: 0x00063B79
        public void End()
        {
            this.Destroy(DestroyMode.Vanish);
            //base.SingleMap.weatherDecider.DisableRainFor(30000);
            //base.End();
        }

        // Token: 0x06000D70 RID: 3440 RVA: 0x00065798 File Offset: 0x00063B98
        private void FindGoodCenterLocation()
        {
            if (SingleMap.Size.x <= 16 || SingleMap.Size.z <= 16)
            {
                throw new Exception("Map too small for flashstorm.");
            }
            for (int i = 0; i < 10; i++)
            {
                this.centerLocation = new IntVec2(Rand.Range(8, SingleMap.Size.x - 8), Rand.Range(8, SingleMap.Size.z - 8));
                if (this.IsGoodCenterLocation(this.centerLocation))
                {
                    break;
                }
            }
        }

        // Token: 0x06000D71 RID: 3441 RVA: 0x0006584F File Offset: 0x00063C4F
        private bool IsGoodLocationForStrike(IntVec3 loc)
        {
            return loc.InBounds(SingleMap);// && !loc.Roofed(SingleMap) && loc.Standable(SingleMap);
        }

        // Token: 0x06000D72 RID: 3442 RVA: 0x00065884 File Offset: 0x00063C84
        private bool IsGoodCenterLocation(IntVec2 loc)
        {
            int num = 0;
            int num2 = (int)(3.14159274f * (float)this.areaRadius * (float)this.areaRadius / 2f);
            foreach (IntVec3 loc2 in this.GetPotentiallyAffectedCells(loc))
            {
                if (this.IsGoodLocationForStrike(loc2))
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

        // Token: 0x06000D73 RID: 3443 RVA: 0x0006591C File Offset: 0x00063D1C
        private IEnumerable<IntVec3> GetPotentiallyAffectedCells(IntVec2 center)
        {
            for (int x = center.x - this.areaRadius; x <= center.x + this.areaRadius; x++)
            {
                for (int z = center.z - this.areaRadius; z <= center.z + this.areaRadius; z++)
                {
                    if ((center.x - x) * (center.x - x) + (center.z - z) * (center.z - z) <= this.areaRadius * this.areaRadius)
                    {
                        yield return new IntVec3(x, 0, z);
                    }
                }
            }
            yield break;
        }

        // Token: 0x040008BE RID: 2238
       // private static readonly IntRange AreaRadiusRange = new IntRange(10,35);

        // Token: 0x040008BF RID: 2239
        private static readonly IntRange TicksBetweenStrikes = new IntRange(20,60);

        // Token: 0x040008C0 RID: 2240
        private const int RainDisableTicksAfterConditionEnds = 30000;

        // Token: 0x040008C1 RID: 2241
        public IntVec2 centerLocation;

        // Token: 0x040008C2 RID: 2242
        private int areaRadius;
        private int hitTime=0;
        // Token: 0x040008C3 RID: 2243
        private int nextLightningTicks;
        private List<IntVec3> canATKCells=new List<IntVec3>();
        public Map SingleMap;
        private int ticks = 0;
   
    }
}
