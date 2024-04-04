using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ra2;

public class IntermittentGoldMiner(Thing parent)
{
    private const int MinTicksBetweenSprays = 500;

    private const int MaxTicksBetweenSprays = 2000;

    private const int MinSprayDuration = 200;

    private const int MaxSprayDuration = 500;

    private const float SprayThickness = 0.6f;

    public Action endSprayCallback;

    private int sprayTicksLeft;

    public Action startSprayCallback;

    private int ticksUntilSpray = 500;

    public void GoldMinerTick()
    {
        if (sprayTicksLeft > 0)
        {
            sprayTicksLeft--;
            if (Rand.Value < SprayThickness)
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

            ticksUntilSpray = Rand.RangeInclusive(MinTicksBetweenSprays, MaxTicksBetweenSprays);
        }
        else
        {
            ticksUntilSpray--;
            if (ticksUntilSpray > 0)
            {
                return;
            }

            startSprayCallback?.Invoke();

            sprayTicksLeft = Rand.RangeInclusive(MinSprayDuration, MaxSprayDuration);
        }
    }
}