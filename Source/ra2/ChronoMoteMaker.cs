using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ra2
{
    public static class ChronoMoteMaker
    {

        public static Mote ThrowCell(IntVec3 cell, Map map, ThingDef moteDef, Color color,IntVec2 size)
        {
            if (!cell.ShouldSpawnMotesAt(map))
            {
                return null;
            }
            Mote mote = (Mote)ThingMaker.MakeThing(moteDef, null);
            mote.Scale = Rand.Range(size.x,size.z);
            mote.exactRotation = (float)(90 * Rand.RangeInclusive(0, 3));
            mote.exactPosition = cell.ToVector3Shifted();
            mote.instanceColor = color;
            GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);
            return mote;
        }

    }
}
