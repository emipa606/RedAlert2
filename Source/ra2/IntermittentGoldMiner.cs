using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ra2
{
    // Token: 0x020006EC RID: 1772
    public class IntermittentGoldMiner
    {
        // Token: 0x040015C4 RID: 5572
        private const int MinTicksBetweenSprays = 500;

        // Token: 0x040015C5 RID: 5573
        private const int MaxTicksBetweenSprays = 2000;

        // Token: 0x040015C6 RID: 5574
        private const int MinSprayDuration = 200;

        // Token: 0x040015C7 RID: 5575
        private const int MaxSprayDuration = 500;

        // Token: 0x040015C8 RID: 5576
        private const float SprayThickness = 0.6f;

        // Token: 0x040015BF RID: 5567
        private readonly Thing parent;

        // Token: 0x040015C3 RID: 5571
        public Action endSprayCallback;

        // Token: 0x040015C1 RID: 5569
        private int sprayTicksLeft;

        // Token: 0x040015C2 RID: 5570
        public Action startSprayCallback;

        // Token: 0x040015C0 RID: 5568
        private int ticksUntilSpray = 500;

        // Token: 0x06002661 RID: 9825 RVA: 0x00122D46 File Offset: 0x00121146
        public IntermittentGoldMiner(Thing parent)
        {
            this.parent = parent;
        }

        // Token: 0x06002662 RID: 9826 RVA: 0x00122D60 File Offset: 0x00121160
        public void GoldMinerTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < 0.6f)
                {
                    FleckMaker.ThrowAirPuffUp(parent.TrueCenter() + new Vector3(-0.5f, 0, 3.8f), parent.Map);
                    FleckMaker.ThrowAirPuffUp(parent.TrueCenter() + new Vector3(-1f, 0, 3.8f), parent.Map);
                }

                if (Find.TickManager.TicksGame % 20 == 0)
                {
                    GenTemperature.PushHeat(parent, 40f);
                }

                if (sprayTicksLeft > 0)
                {
                    return;
                }

                endSprayCallback?.Invoke();

                ticksUntilSpray = Rand.RangeInclusive(500, 2000);
            }
            else
            {
                ticksUntilSpray--;
                if (ticksUntilSpray > 0)
                {
                    return;
                }

                startSprayCallback?.Invoke();

                sprayTicksLeft = Rand.RangeInclusive(200, 500);
            }
        }
    }
}