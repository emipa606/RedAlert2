using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    public class CompProperties_Barracks : CompProperties
    {
        public CompProperties_Barracks()
        {
            base.compClass = typeof(CompBarracks);
        }
    }
}
