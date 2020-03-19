namespace ra2
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class CompProperties_TeslaCoil : CompProperties
    {
        public Vector2 size;
        public int speed;

        public CompProperties_TeslaCoil()
        {

            base.compClass = typeof(CompTeslaCoil);
        }
    }
}

