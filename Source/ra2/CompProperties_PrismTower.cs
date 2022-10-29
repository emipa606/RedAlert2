using UnityEngine;
using Verse;

namespace ra2;

public class CompProperties_PrismTower : CompProperties
{
    public Vector2 size;
    public int speed;

    public CompProperties_PrismTower()
    {
        compClass = typeof(CompPrismTower);
    }
}