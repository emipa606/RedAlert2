using HarmonyLib;
using UnityEngine;
using Verse;

namespace ra2
{
    // Token: 0x02000006 RID: 6
    [HarmonyPatch(typeof(Projectile), "Launch", typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo),
        typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef))]
    public static class Harmony_Projectile_Add
    {
        // Token: 0x0600000D RID: 13 RVA: 0x000022B4 File Offset: 0x000004B4
        public static void Postfix(Projectile __instance)
        {
            if (__instance.def.projectile == null || !__instance.def.projectile.flyOverhead)
            {
                return;
            }

            //MapComponent_Rimatomics component = __instance.Map.GetComponent<MapComponent_Rimatomics>();
            var traverse = Traverse.Create(__instance);
            //Vector3 value = traverse.Field("destination").GetValue<Vector3>();
            //Vector3 value2 = traverse.Field("origin").GetValue<Vector3>();
            var value3 = traverse.Field("launcher").GetValue<Thing>();
            //component.RegisterProjectile(new ProjectileEnd(__instance, value3));
            var PE = new ProjectileEnd(__instance, value3);
            if (!BulletStore.theBullet.Contains(PE))
            {
                BulletStore.theBullet.Add(PE);
            }
        }
    }
}