using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    public class CompProperties_GoldMiner : CompProperties
    {
        public CompProperties_GoldMiner()
        {
            base.compClass = typeof(CompGoldMiner);
        }
    }
}
