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
    public class SovietNuclearLeaving:ThingWithComps
    {
        //public IntVec3 targetCell;
        public Vector3 ExactPosition = Vector3.zero;
        private float speed = 0.001f;
        //  public SovietNuclearLeaving(IntVec3 target) {
        //  this.targetCell = target;
        //this.ExactPosition = new Vector3(target.x,10,this.Map.Size.z);
        // }
        
        public override Vector3 DrawPos
        {
            get
            {
                return this.ExactPosition;
            }
        }





        public override void Tick()
        {
            base.Tick();
            if (this.ExactPosition == Vector3.zero) {
                SoundStarter.PlayOneShotOnCamera(SoundDef.Named("ra2_NuclearAlert"));
                SoundStarter.PlayOneShotOnCamera(SoundDef.Named("ra2_NuclearLaunch"));
                this.ExactPosition = this.TrueCenter() + new Vector3(0.5f, 10, 5); }

            if(speed<1f)
            speed += 0.002f;
            
            this.ExactPosition += new Vector3(0,0,speed);
            MoteMaker.ThrowExplosionCell((this.ExactPosition+new Vector3(0,0,-5)).ToIntVec3(),this.Map,ThingDefOf.Mote_ExplosionFlash,new Color(1,0,0));
            if (this.ExactPosition.z >= this.Map.Size.z)
            {
                Booom();
                this.Destroy(DestroyMode.Vanish);
            }
        }

        private MapParent mapParent;
        public int destinationTile = -1;
        public IntVec3 targetCell = IntVec3.Zero;


        // Token: 0x04001627 RID: 5671

        public void Booom()
        {

            if (!Find.WorldObjects.AnyMapParentAt(this.destinationTile))
            {
                Messages.Message("SWeaponDestoryBase".Translate(), MessageTypeDefOf.NeutralEvent, true);
                // DubDef.hugeExplosionDistantDef.PlayOneShotOnCamera(null);
                WorldObject worldObject = Find.WorldObjects.WorldObjectAt<WorldObject>(this.destinationTile);
                if (worldObject != null && worldObject.Faction != null && !worldObject.Faction.IsPlayer)
                {
                    worldObject.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999, true, true, null, null);
                }
                PeaceTalks peaceTalks = Find.WorldObjects.WorldObjectAt<PeaceTalks>(this.destinationTile);
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



            this.mapParent = Find.WorldObjects.MapParentAt(this.destinationTile);
            if (this.mapParent.Faction != null && !this.mapParent.Faction.IsPlayer)
            {
                this.mapParent.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999, true, true, null, null);
            }
            /*
            if (this.mapParent is DestroyedSettlement)
            {
                Messages.Message("critBombDet".Translate(), MessageTypeDefOf.NeutralEvent, true);
                // DubDef.hugeExplosionDistantDef.PlayOneShotOnCamera(null);
                return;
            }
            */
            if (this.mapParent.HasMap)
            {

                //IntVec3 destinationCell = CellFinderLoose.TryFindCentralCell(this.mapParent.Map, 25, 1000, null);

                // SkyfallerMaker.SpawnSkyfaller(ThingDef.Named("NuclearIncoming"), ThingDef.Named("SovietNuclearStrike"), destinationCell, this.mapParent.Map);
                SovietNuclearFallen bomb = (SovietNuclearFallen)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearFallen"));
                bomb.targetCell = this.targetCell;
                bomb.ExactPosition = new Vector3(this.targetCell.x, 10, this.mapParent.Map.Size.z - 10);
                GenSpawn.Spawn(bomb, bomb.ExactPosition.ToIntVec3(), this.mapParent.Map);
                return;
            }

            Messages.Message("SWeaponDestoryBase".Translate(), this.mapParent, MessageTypeDefOf.NegativeEvent, true);
            SoundDef.Named("ra2_NuclearExplode").PlayOneShotOnCamera(null);
            Settlement settlementBase;
            if ((settlementBase = (this.mapParent as Settlement)) != null)
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
                    stringBuilder.Append("LetterFactionBaseDefeated".Translate(new object[] { settlementBase.Label, TimedForcedExit.GetForceExitAndRemoveMapCountdownTimeLeftString(60000)}));

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
            Find.WorldObjects.Remove(this.mapParent);
        }


    }
}
