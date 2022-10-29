using HarmonyLib;
using RimWorld;

namespace ra2;

[HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker", typeof(IncidentParms))]
public static class Harmony_Raidenemy_Ra2
{
    public static void Postfix(IncidentParms parms)
    {
        //Traverse traverse = Traverse.Create(__parms);
        //Thing value3 = traverse.Field("launcher").GetValue<Thing>();
        IncidentWorker_Ra2RaidEnemy.TryExecuteWorker(parms);
    }
}