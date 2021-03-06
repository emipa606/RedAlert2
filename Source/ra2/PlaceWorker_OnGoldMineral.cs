﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ra2
{
    public class PlaceWorker_OnGoldMineral : PlaceWorker
    {

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            IntVec3 newloc = loc + new IntVec3(0,0,-2);
            Thing localThing = map.thingGrid.ThingAt(newloc, DefDatabase<ThingDef>.GetNamed("ra2_GoldMineral", true));
            if (localThing == null || localThing.Position != newloc)
            {
                return "MustPlaceOnGoldMineral".Translate();
            }
            return true;
        }

        // Token: 0x06004746 RID: 18246 RVA: 0x00215755 File Offset: 0x00213B55
        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == DefDatabase<ThingDef>.GetNamed("ra2_GoldMineral", true);
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            List<IntVec3> result = new List<IntVec3>();
            result.Add(center + new IntVec3(0, 0, -2));
            GenDraw.DrawFieldEdges(result);
        }


    }
}
