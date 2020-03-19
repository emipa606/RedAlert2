using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ra2
{
    // Token: 0x0200042C RID: 1068
    public class CompGoldMiner : ThingComp
    {



        // Token: 0x06001290 RID: 4752 RVA: 0x0008E4E3 File Offset: 0x0008C8E3
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.steamSprayer = new IntermittentGoldMiner(this.parent);
        }

        // Token: 0x06001291 RID: 4753 RVA: 0x0008E500 File Offset: 0x0008C900
        public override void CompTick()
        {
            base.CompTick();
            if (this.goldMineral == null)
            {
                this.goldMineral = (Building_GoldMineral)this.parent.Map.thingGrid.ThingAt(this.parent.Position+new IntVec3(0,0,-2), DefDatabase<ThingDef>.GetNamed("ra2_GoldMineral",true));
            }
            if (this.goldMineral != null)
            {
                this.goldMineral.harvester = (Building)this.parent;
                
                CompPowerTrader compPowerTrader = ThingCompUtility.TryGetComp<CompPowerTrader>(this.parent);
                if (compPowerTrader != null && compPowerTrader.PowerOn)
                {
                    this.goldMineral.ticks++;
                    this.steamSprayer.GoldMinerTick();
                }

            }

        }

        // Token: 0x06001292 RID: 4754 RVA: 0x0008E57A File Offset: 0x0008C97A
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (this.goldMineral != null)
            {
                this.goldMineral.harvester = null;
            }
        }

        // Token: 0x04000B61 RID: 2913
        private IntermittentGoldMiner steamSprayer;

        // Token: 0x04000B62 RID: 2914
        public Building_GoldMineral goldMineral;









        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            
            if (DebugSettings.godMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Fill",
                    action = delegate
                    {
                        if (this.goldMineral != null)
                            this.goldMineral.GoldStore = 3000;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: AlmostEmpty",
                    action = delegate
                    {
                        if(this.goldMineral!=null)
                        this.goldMineral.GoldStore = 50;
                    }
                };

            }
            yield break;
        }


        public override void PostDraw()
        {
            base.PostDraw();
            if (this.goldMineral==null)
            {
                this.parent.Map.overlayDrawer.DrawOverlay(this.parent, OverlayTypes.OutOfFuel);
            

           // if (this.Props.drawFuelGaugeInMap)
          //  {

            }
        }

 

    }
}
