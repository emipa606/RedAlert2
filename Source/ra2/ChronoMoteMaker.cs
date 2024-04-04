using UnityEngine;
using Verse;

namespace ra2;

public static class ChronoMoteMaker
{
    public static void ThrowCell(IntVec3 cell, Map map, ThingDef moteDef, Color color, IntVec2 size)
    {
        if (!cell.ShouldSpawnMotesAt(map))
        {
            return;
        }

        var mote = (Mote)ThingMaker.MakeThing(moteDef);
        mote.Scale = Rand.Range(size.x, size.z);
        mote.exactRotation = 90 * Rand.RangeInclusive(0, 3);
        mote.exactPosition = cell.ToVector3Shifted();
        mote.instanceColor = color;
        GenSpawn.Spawn(mote, cell, map);
    }
}