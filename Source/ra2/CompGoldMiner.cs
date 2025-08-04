using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ra2;

public class CompGoldMiner : ThingComp
{
    public Building_GoldMineral goldMineral;

    private IntermittentGoldMiner steamSprayer;


    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        steamSprayer = new IntermittentGoldMiner(parent);
    }

    public override void CompTick()
    {
        base.CompTick();
        goldMineral ??= (Building_GoldMineral)parent.Map.thingGrid.ThingAt(
            parent.Position + new IntVec3(0, 0, -2), DefDatabase<ThingDef>.GetNamed("ra2_GoldMineral"));

        if (goldMineral == null)
        {
            return;
        }

        goldMineral.harvester = (Building)parent;

        var compPowerTrader = parent.TryGetComp<CompPowerTrader>();
        if (compPowerTrader is not { PowerOn: true })
        {
            return;
        }

        goldMineral.ticks++;
        steamSprayer.GoldMinerTick();
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
        base.PostDeSpawn(map, mode);
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