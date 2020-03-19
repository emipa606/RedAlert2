using RimWorld;
using System.Text;
using Verse;


namespace ra2
{
    // Token: 0x020006EB RID: 1771
    public class Building_GoldMineral : Building
    {

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();

           // sb.Append(base.GetInspectString());
            sb.Append("GoldMineralStore".Translate()+":"+this.goldStore+"/3000");

            return sb.ToString();

        }

        // Token: 0x0600265D RID: 9821 RVA: 0x00122C08 File Offset: 0x00121008
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

  
        public override void Tick()
        {
 
    

            if (this.ticks >= 3600) {
                Thing gold = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_GoldBar",true));
                gold.stackCount = 100;
                GenPlace.TryPlaceThing(gold,this.harvester.Position,this.Map,ThingPlaceMode.Near);
                this.ticks = 0;
                if (this.goldStore <= 100)
                {

                       Messages.Message("MessageGoldMineralLifeOut".Translate().CapitalizeFirst(), this.harvester,MessageTypeDefOf.NeutralEvent, true);
                       this.Destroy(DestroyMode.Vanish);
                    CompGoldMiner cg = this.harvester.TryGetComp<CompGoldMiner>();
                    if (cg != null && cg.goldMineral != null) {
                        cg.goldMineral = null;
                    }
                }
                else
                this.goldStore -= 100;
                
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<int>(ref this.goldStore, "goldStore", 3000, false);
        }


        // Token: 0x040015BB RID: 5563
        //private IntermittentSteamSprayer steamSprayer;

        // Token: 0x040015BC RID: 5564
        public Building harvester;

        // Token: 0x040015BD RID: 5565
       // private Sustainer spraySustainer;
        private int goldStore = 3000;
        public int GoldStore {
          get {
                return this.goldStore;
            }
            set{
                this.goldStore = value;
            }
        }
        public int ticks;
        
        // Token: 0x040015BE RID: 5566
        //private int spraySustainerStartTick = -999;
    }
}
