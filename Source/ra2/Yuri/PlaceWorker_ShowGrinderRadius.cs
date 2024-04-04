using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ra2.Yuri;

public class PlaceWorker_ShowGrinderRadius : PlaceWorker
{
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
    {
        _ = Find.CurrentMap;
        // drawit(center.ToVector3()+new Vector3(0.5f,0,1),7f, MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.red));
        GenDraw.DrawFieldEdges(CellsAround(center, Find.CurrentMap), Color.magenta);
    }

    public List<IntVec3> CellsAround(IntVec3 pos, Map map)
    {
        var result = new List<IntVec3>();
        if (!pos.InBounds(map))
        {
            return result;
        }

        var region = pos.GetRegion(map, RegionType.Set_All);
        if (region == null)
        {
            return result;
        }

        RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
        {
            foreach (var item in r.Cells)
            {
                if (item.InHorDistOf(pos, 6f))
                {
                    result.Add(item);
                }
            }

            return false;
        }, 13, RegionType.Set_All);
        return result;
    }
}