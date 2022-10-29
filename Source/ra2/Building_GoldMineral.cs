using System.Text;
using RimWorld;
using Verse;

namespace ra2;

public class Building_GoldMineral : Building
{
    // private Sustainer spraySustainer;
    private int goldStore = 3000;


    //private IntermittentSteamSprayer steamSprayer;

    public Building harvester;
    public int ticks;

    public int GoldStore
    {
        get => goldStore;
        set => goldStore = value;
    }

    public override string GetInspectString()
    {
        var sb = new StringBuilder();

        // sb.Append(base.GetInspectString());
        sb.Append($"{"GoldMineralStore".Translate() + ":"}{goldStore}/3000");

        return sb.ToString();
    }


    public override void Tick()
    {
        if (ticks < 3600)
        {
            return;
        }

        var gold = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_GoldBar"));
        gold.stackCount = 100;
        GenPlace.TryPlaceThing(gold, harvester.Position, Map, ThingPlaceMode.Near);
        ticks = 0;
        if (goldStore <= 100)
        {
            Messages.Message("MessageGoldMineralLifeOut".Translate().CapitalizeFirst(), harvester,
                MessageTypeDefOf.NeutralEvent);
            Destroy();
            var cg = harvester.TryGetComp<CompGoldMiner>();
            if (cg?.goldMineral != null)
            {
                cg.goldMineral = null;
            }
        }
        else
        {
            goldStore -= 100;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticks, "ticks");
        Scribe_Values.Look(ref goldStore, "goldStore", 3000);
    }

    //private int spraySustainerStartTick = -999;
}