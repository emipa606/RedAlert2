using HarmonyLib;
using UnityEngine;
using Verse;

namespace ra2;

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Launch), typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo),
    typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef))]
public static class Projectile_Launch
{
    public static void Postfix(Projectile __instance, Thing ___launcher)
    {
        if (__instance.def.projectile is not { flyOverhead: true })
        {
            return;
        }

        //MapComponent_Rimatomics component = __instance.Map.GetComponent<MapComponent_Rimatomics>();
        //Vector3 value = traverse.Field("destination").GetValue<Vector3>();
        //Vector3 value2 = traverse.Field("origin").GetValue<Vector3>();
        //component.RegisterProjectile(new ProjectileEnd(__instance, value3));
        var PE = new ProjectileEnd(__instance, ___launcher);
        if (!BulletStore.theBullet.Contains(PE))
        {
            BulletStore.theBullet.Add(PE);
        }
    }
}