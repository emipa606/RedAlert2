using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    public class Building_GoldMiner: Building
    {

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(base.GetInspectString());
            CompGoldMiner cgm = ThingCompUtility.TryGetComp<CompGoldMiner>(this);
            if (cgm != null) {
                int tickss = cgm.goldMineral != null ? cgm.goldMineral.ticks : 0;
                sb.Append("\n"+"GoldMinerProgress".Translate()+":"+ ((float)(tickss/36.0)).ToString("#0.00") + "%");

            }




            // String sb = base.GetInspectString() + "\n" + Translator.Translate("restoreBullet") + ":" + this.restoreBullet + "\n" + Translator.Translate("restoreProgress") + ":" + (this.ticks);
            return sb.ToString();

        }

    }
}
