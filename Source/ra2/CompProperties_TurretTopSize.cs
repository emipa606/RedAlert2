using UnityEngine;
using Verse;

namespace ra2
{
    public class CompProperties_TurretTopSize : CompProperties
    {
        public string soundShoot = null;
        public Vector3 topSize = Vector3.one;


        public CompProperties_TurretTopSize()
        {
            compClass = typeof(CompTurretTopSize);
        }
    }
}