using UnityEngine;
using Verse;

namespace ra2;

public class CompProperties_TeslaCoil : CompProperties
{
    public Vector2 size;
    public int speed;

    public CompProperties_TeslaCoil()
    {
        compClass = typeof(CompTeslaCoil);
    }
}