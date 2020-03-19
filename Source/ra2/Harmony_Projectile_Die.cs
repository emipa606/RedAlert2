using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    // Token: 0x02000007 RID: 7
    //[HarmonyPatch(typeof(Projectile), "DeSpawn", new Type[] { typeof(DestroyMode) })]
    //internal static class Harmony_Projectile_Die
    //{
    //    // Token: 0x0600000E RID: 14 RVA: 0x00002332 File Offset: 0x00000532
    //    public static void Prefix(Projectile __instance)
    //    {
    //        if (__instance.def.projectile != null && __instance.def.projectile.flyOverhead)
    //        {
    //            foreach (ProjectileEnd pe in BulletStore.theBullet)
    //            {
    //                if (pe.shell == __instance)
    //                {
    //                    BulletStore.theBullet.Remove(pe);
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}
}
