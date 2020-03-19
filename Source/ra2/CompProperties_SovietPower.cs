namespace ra2
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class CompProperties_SovietPower : CompProperties
    {
        public Vector2 size;
        public int speed;

        public CompProperties_SovietPower()
        {

            base.compClass = typeof(CompSovietPower);
        }
    }
}

