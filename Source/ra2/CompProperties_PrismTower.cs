namespace ra2
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class CompProperties_PrismTower : CompProperties
    {
        public Vector2 size;
        public int speed;
        
        public CompProperties_PrismTower()
        {
            
            base.compClass = typeof(CompPrismTower);
        }
    }
}

