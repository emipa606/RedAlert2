namespace ra2
{
    using System;
    using UnityEngine;
    using Verse;

    public class CompProperties_TurretTopSize : CompProperties
    {
        public Vector3 topSize = Vector3.one;
        public String soundShoot = null;


        public CompProperties_TurretTopSize()
        {
            base.compClass = typeof(CompTurretTopSize);
        }
    }
}
