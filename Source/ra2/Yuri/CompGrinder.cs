using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2.Yuri;

public class CompGrinder : ThingComp
{
    private int ticks;

    public override string CompInspectStringExtra()
    {
        string str = "GrinderHelpInfo".Translate();

        return str;
    }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        ticks = 0;
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref ticks, "ticks");
    }

    public override void CompTick()
    {
        base.CompTick();
        if (ticks > 0)
        {
            ticks--;
        }
    }

    private bool hasPower()
    {
        var cp = parent.TryGetComp<CompPowerTrader>();
        return cp is { PowerOn: true };
    }

    private void spawnGold(Pawn p)
    {
        int result;
        switch (p.kindDef.defName)
        {
            case "ra2_SovietConscript":
                result = 25;
                break;
            case "ra2_SovietTeslaTrooper":
                result = 200;
                break;
            case "ra2_SovietDesolator":
                result = 425;
                break;
            case "ra2_AlliedGI":
                result = 50;
                break;
            case "ra2_AlliedSiegeCadre":
                result = 250;
                break;
            case "ra2_AlliedSniper":
                result = 350;
                break;
            case "ra2_AlliedChrono":
                result = 700;
                break;
            case "ra2_AlliedTanya":
                result = 750;
                break;
            default:
                result = 100;
                break;
        }

        var gold = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_GoldBar"));
        gold.stackCount = result;
        GenPlace.TryPlaceThing(gold, parent.Position, parent.Map, ThingPlaceMode.Near);
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        yield return new Command_Action
        {
            defaultLabel = "GrinderAction".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/TradeMode"),
            Disabled = ticks > 0 || !hasPower(),
            action = delegate
            {
                var pp = (parent as Building)?.def.PlaceWorkers[0] as PlaceWorker_ShowGrinderRadius;
                var js = pp?.CellsAround(parent.DrawPos.ToIntVec3(), parent.Map);
                var count = 0;
                if (js != null)
                {
                    foreach (var cell in js)
                    {
                        // if (!cell.IsValid) continue;
                        var list = parent.Map.thingGrid.ThingsListAt(cell);

                        if (list.Count < 1)
                        {
                            continue;
                        }

                        foreach (var thing in list)
                        {
                            Pawn p;
                            if ((p = thing as Pawn) == null)
                            {
                                continue;
                            }

                            if (p.Faction != Faction.OfPlayer)
                            {
                                continue;
                            }

                            spawnGold(p);
                            count++;
                            p.Kill(new DamageInfo(DamageDefOf.Crush, 0));

                            if (!p.kindDef.defName.StartsWith("ra2_"))
                            {
                                p.Corpse.Kill();
                            }
                        }
                    }
                }

                if (count <= 0)
                {
                    return;
                }

                ticks = 300;
                SoundDef.Named("ra2_grinder_action").PlayOneShot(parent);
                ThrowSparks(parent.Position.ToVector3(), parent.Map);
            }
        };
    }

    private void ThrowSparks(Vector3 loc, Map map)
    {
        if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
        {
            return;
        }

        for (var i = 0; i < 3; i++)
        {
            var fleckMakerData = FleckMaker.GetDataThrowMetaIcon(loc.ToIntVec3(), map, FleckDefOf.MicroSparks, 2f);
            fleckMakerData.rotation = Rand.Range(-12f, 12f);
            fleckMakerData.scale = Rand.Range(2.5f, 3f);
            fleckMakerData.velocityAngle = Rand.Range(7 * (i + 1), 10 * (i + 1));
            map.flecks.CreateFleck(fleckMakerData);
        }

        FleckMaker.ThrowLightningGlow(parent.Position.ToVector3(), parent.Map, 6f);
    }
}