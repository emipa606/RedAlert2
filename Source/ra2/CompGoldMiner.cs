using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ra2
{
    // Token: 0x0200042C RID: 1068
    public class CompGoldMiner : ThingComp
    {
        // Token: 0x04000B62 RID: 2914
        public Building_GoldMineral goldMineral;

        // Token: 0x04000B61 RID: 2913
        private IntermittentGoldMiner steamSprayer;


        // Token: 0x06001290 RID: 4752 RVA: 0x0008E4E3 File Offset: 0x0008C8E3
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            steamSprayer = new IntermittentGoldMiner(parent);
        }

        // Token: 0x06001291 RID: 4753 RVA: 0x0008E500 File Offset: 0x0008C900
        public override void CompTick()
        {
            base.CompTick();
            if (goldMineral == null)
            {
                goldMineral = (Building_GoldMineral) parent.Map.thingGrid.ThingAt(
                    parent.Position + new IntVec3(0, 0, -2), DefDatabase<ThingDef>.GetNamed("ra2_GoldMineral"));
            }

            if (goldMineral == null)
            {
                return;
            }

            goldMineral.harvester = (Building) parent;

            var compPowerTrader = parent.TryGetComp<CompPowerTrader>();
            if (compPowerTrader == null || !compPowerTrader.PowerOn)
            {
                return;
            }

            goldMineral.ticks++;
            steamSprayer.GoldMinerTick();
        }

        // Token: 0x06001292 RID: 4754 RVA: 0x0008E57A File Offset: 0x0008C97A
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (goldMineral != null)
            {
                goldMineral.harvester = null;
            }
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!DebugSettings.godMode)
            {
                yield break;
            }

            yield return new Command_Action
            {
                defaultLabel = "DEBUG: Fill",
                action = delegate
                {
                    if (goldMineral != null)
                    {
                        goldMineral.GoldStore = 3000;
                    }
                }
            };
            yield return new Command_Action
            {
                defaultLabel = "DEBUG: AlmostEmpty",
                action = delegate
                {
                    if (goldMineral != null)
                    {
                        goldMineral.GoldStore = 50;
                    }
                }
            };
        }


        public override void PostDraw()
        {
            base.PostDraw();
            if (goldMineral == null)
            {
                parent.Map.overlayDrawer.DrawOverlay(parent, OverlayTypes.OutOfFuel);


                // if (this.Props.drawFuelGaugeInMap)
                //  {
            }
        }
    }
}