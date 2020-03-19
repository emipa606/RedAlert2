using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    public static class SuperWeaponAction
    {

        public static void Soviet(GlobalTargetInfo target,IntVec3 pos,Map map)
        {

            SovietNuclearLeaving leavingMissle = (SovietNuclearLeaving)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearLeaving"), null);


            leavingMissle.targetCell = target.Cell;
            leavingMissle.destinationTile = target.Tile;

            GenSpawn.Spawn(leavingMissle, pos, map, WipeMode.Vanish);

        }

        public static void Allied(GlobalTargetInfo target, IntVec3 pos, Map map,int tile) {

            // map.GameConditionManager.ConditionIsActive(GameConditionDef.Named("ra2_AlliedStorm"));
            AlliedBooom(tile,target.Cell);
      
        }

        public static void AlliedBooom(int destinationTile,IntVec3 targetCell)
        {

            if (!Find.WorldObjects.AnyMapParentAt(destinationTile))
            {
                Messages.Message("SWeaponDestoryBase".Translate(), MessageTypeDefOf.NeutralEvent, true);
             
                    SoundDef.Named("ra2_StormAppear").PlayOneShotOnCamera(null);
         

                WorldObject worldObject = Find.WorldObjects.WorldObjectAt<WorldObject>(destinationTile);
                if (worldObject != null && worldObject.Faction != null && !worldObject.Faction.IsPlayer)
                {
                    worldObject.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999, true, true, null, null);
                }
                PeaceTalks peaceTalks = Find.WorldObjects.WorldObjectAt<PeaceTalks>(destinationTile);
                if (peaceTalks != null)
                {
                    try
                    {
                        peaceTalks.GetType().GetMethod("Outcome_Disaster", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(peaceTalks, null);
                    }
                    catch (Exception)
                    {
                    }
                    Find.WorldObjects.Remove(peaceTalks);
                }
                return;
            }



            MapParent mapParent = Find.WorldObjects.MapParentAt(destinationTile);
            if (mapParent.Faction != null && !mapParent.Faction.IsPlayer)
            {
                mapParent.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999, true, true, null, null);
            }
   
            if (mapParent.HasMap)
            {
               
                AlliedStormCondition asc = (AlliedStormCondition)ThingMaker.MakeThing(ThingDef.Named("AlliedStorm"), null);
                asc.centerLocation = targetCell.ToIntVec2;

                asc.SingleMap = mapParent.Map;
                SoundInfo info = SoundInfo.InMap(new TargetInfo(asc.centerLocation.ToIntVec3, asc.SingleMap, false), MaintenanceType.None);
                SoundDef.Named("ra2_StormAppear").PlayOneShotOnCamera(mapParent.Map);//.PlayOneShot(info);

               
                // SovietNuclearFallen bomb = (SovietNuclearFallen)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearFallen"));
                // bomb.targetCell = targetCell;
                // bomb.ExactPosition = new Vector3(targetCell.x, 10, mapParent.Map.Size.z - 10);
                GenSpawn.Spawn(asc, asc.centerLocation.ToIntVec3, mapParent.Map);
                return;
            }

            Messages.Message("SWeaponDestoryBase".Translate(), mapParent, MessageTypeDefOf.NegativeEvent, true);
            SoundDef.Named("ra2_StormAppear").PlayOneShotOnCamera(null);
            Settlement settlementBase;
            if ((settlementBase = (mapParent as Settlement)) != null)
            {
                if (settlementBase.Faction == Faction.OfPlayer)
                {
                    return;
                }
                foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
                {
                    if (faction != Faction.OfPlayer)
                    {
                        faction.TryAffectGoodwillWith(Faction.OfPlayer, -10, true, true, null, null);
                    }
                }
                try
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("LetterFactionBaseDefeated".Translate(new object[]
                    {
                        settlementBase.Label,
                        TimedForcedExit.GetForceExitAndRemoveMapCountdownTimeLeftString(60000)
                    }));

                    Find.LetterStack.ReceiveLetter("LetterLabelFactionBaseDefeated".Translate(), stringBuilder.ToString(), LetterDefOf.PositiveEvent, new GlobalTargetInfo(settlementBase.Tile), null, null);
                    DestroyedSettlement destroyedSettlement = (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
                    destroyedSettlement.Tile = settlementBase.Tile;
                    Find.WorldObjects.Add(destroyedSettlement);
                }
                catch (Exception)
                {
                    Log.Warning("Failed to end settlement", false);
                }
            }
            Find.WorldObjects.Remove(mapParent);
        }
    }
}
