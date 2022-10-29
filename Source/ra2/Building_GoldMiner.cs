using System.Text;
using Verse;

namespace ra2;

public class Building_GoldMiner : Building
{
    public override string GetInspectString()
    {
        var sb = new StringBuilder();

        sb.Append(base.GetInspectString());
        var cgm = this.TryGetComp<CompGoldMiner>();
        if (cgm == null)
        {
            return sb.ToString();
        }

        var tickss = cgm.goldMineral?.ticks ?? 0;
        sb.Append("\n" + "GoldMinerProgress".Translate() + ":" + ((float)(tickss / 36.0)).ToString("#0.00") +
                  "%");


        // String sb = base.GetInspectString() + "\n" + Translator.Translate("restoreBullet") + ":" + this.restoreBullet + "\n" + Translator.Translate("restoreProgress") + ":" + (this.ticks);
        return sb.ToString();
    }
}