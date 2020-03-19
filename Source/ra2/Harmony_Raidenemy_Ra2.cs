using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;

namespace ra2
{
    
    // Token: 0x02000006 RID: 6
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker", new Type[]
    {
        typeof(IncidentParms)
    })]
    public static class Harmony_Raidenemy_Ra2
    {
        public static void Postfix(IncidentParms parms)
        {
            //Traverse traverse = Traverse.Create(__parms);
            //Thing value3 = traverse.Field("launcher").GetValue<Thing>();
            IncidentWorker_Ra2RaidEnemy.TryExecuteWorker(parms);
          
        }

        }
        
}
