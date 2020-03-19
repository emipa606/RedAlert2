namespace ra2
{
    using System;
    using UnityEngine;
    using Verse;

    public class ProjectileEnd
    {
        public Thing launcher;
        public Projectile shell;

        public ProjectileEnd(Projectile s, Thing l)
        {
            this.shell = s;

            this.launcher = l;

        }
    }
}