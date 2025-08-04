using System.Reflection;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace ra2;

public static class SuperWeaponAction
{
    public static void Soviet(GlobalTargetInfo target, IntVec3 pos, Map map)
    {
        var leavingMissle = (SovietNuclearLeaving)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearLeaving"));


        leavingMissle.targetCell = target.Cell;
        leavingMissle.destinationTile = target.Tile;

        GenSpawn.Spawn(leavingMissle, pos, map);
    }

    public static void Allied(GlobalTargetInfo target, IntVec3 pos, Map map, PlanetTile tile)
    {
        // map.GameConditionManager.ConditionIsActive(GameConditionDef.Named("ra2_AlliedStorm"));
        AlliedBooom(tile, target.Cell);
    }

    private static void AlliedBooom(int destinationTile, IntVec3 targetCell)
    {
        if (!Find.WorldObjects.AnyMapParentAt(destinationTile))
        {
            Messages.Message("SWeaponDestoryBase".Translate(), MessageTypeDefOf.NeutralEvent);

            SoundDef.Named("ra2_StormAppear").PlayOneShotOnCamera();


            var worldObject = Find.WorldObjects.WorldObjectAt<WorldObject>(destinationTile);
            if (worldObject?.Faction is { IsPlayer: false })
            {
                worldObject.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999);
            }

            var peaceTalks = Find.WorldObjects.WorldObjectAt<PeaceTalks>(destinationTile);
            if (peaceTalks == null)
            {
                return;
            }

            try
            {
                peaceTalks.GetType()
                    .GetMethod("Outcome_Disaster", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(peaceTalks, null);
            }
            catch
            {
                // ignored
            }

            Find.WorldObjects.Remove(peaceTalks);

            return;
        }


        var mapParent = Find.WorldObjects.MapParentAt(destinationTile);
        if (mapParent.Faction is { IsPlayer: false })
        {
            mapParent.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999);
        }

        if (mapParent.HasMap)
        {
            var asc = (AlliedStormCondition)ThingMaker.MakeThing(ThingDef.Named("AlliedStorm"));
            asc.centerLocation = targetCell.ToIntVec2;

            asc.SingleMap = mapParent.Map;
            _ = SoundInfo.InMap(new TargetInfo(asc.centerLocation.ToIntVec3, asc.SingleMap));
            SoundDef.Named("ra2_StormAppear").PlayOneShotOnCamera(mapParent.Map); //.PlayOneShot(info);


            // SovietNuclearFallen bomb = (SovietNuclearFallen)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearFallen"));
            // bomb.targetCell = targetCell;
            // bomb.ExactPosition = new Vector3(targetCell.x, 10, mapParent.Map.Size.z - 10);
            GenSpawn.Spawn(asc, asc.centerLocation.ToIntVec3, mapParent.Map);
            return;
        }

        Messages.Message("SWeaponDestoryBase".Translate(), mapParent, MessageTypeDefOf.NegativeEvent);
        SoundDef.Named("ra2_StormAppear").PlayOneShotOnCamera();
        Settlement settlementBase;
        if ((settlementBase = mapParent as Settlement) != null)
        {
            if (settlementBase.Faction == Faction.OfPlayer)
            {
                return;
            }

            foreach (var faction in Find.FactionManager.AllFactionsListForReading)
            {
                if (faction != Faction.OfPlayer)
                {
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, -10);
                }
            }

            try
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("LetterFactionBaseDefeated".Translate(settlementBase.Label,
                    TimedForcedExit.GetForceExitAndRemoveMapCountdownTimeLeftString(60000)));

                Find.LetterStack.ReceiveLetter("LetterLabelFactionBaseDefeated".Translate(),
                    stringBuilder.ToString(), LetterDefOf.PositiveEvent, new GlobalTargetInfo(settlementBase.Tile));
                var destroyedSettlement =
                    (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
                destroyedSettlement.Tile = settlementBase.Tile;
                Find.WorldObjects.Add(destroyedSettlement);
            }
            catch
            {
                Log.Warning("Failed to end settlement");
            }
        }

        Find.WorldObjects.Remove(mapParent);
    }
}