using UnityEngine;
using Verse;

namespace ra2;

public class CompProperties_SovietPower : CompProperties
{
    public Vector2 size;
    public int speed;

    public CompProperties_SovietPower()
    {
        compClass = typeof(CompSovietPower);
    }
}