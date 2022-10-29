namespace ra2;


//[HarmonyPatch(typeof(Projectile), "DeSpawn", new Type[] { typeof(DestroyMode) })]
//internal static class Harmony_Projectile_Die
//{
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