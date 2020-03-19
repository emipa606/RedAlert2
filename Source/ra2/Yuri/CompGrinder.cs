using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2.Yuri
{
    public class CompGrinder:ThingComp
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
            this.ticks = 0;
        }
        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
        }
        public override void CompTick()
        {
            base.CompTick();
            if (this.ticks > 0) this.ticks--;
        }
        private bool hasPower() {
            CompPowerTrader cp = this.parent.TryGetComp<CompPowerTrader>();
            if (cp != null && cp.PowerOn) return true;

            return false;
        }
        private void spawnGold(Pawn p) {
            int result = 100;
            switch (p.kindDef.defName)
            {
                case "ra2_SovietConscript":
                    result = 25;break;
                case "ra2_SovietTeslaTrooper":
                    result = 200; break;
                case "ra2_SovietDesolator":
                    result = 425; break;
                case "ra2_AlliedGI":
                    result = 50; break;
                case "ra2_AlliedSiegeCadre":
                    result = 250; break;
                case "ra2_AlliedSniper":
                    result = 350; break;
                case "ra2_AlliedChrono":
                    result = 700; break;
                case "ra2_AlliedTanya":
                    result = 750; break;
                default:result = 100;break;
            }

            Thing gold = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_GoldBar", true));
            gold.stackCount =result;
            GenPlace.TryPlaceThing(gold, this.parent.Position, this.parent.Map, ThingPlaceMode.Near);
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action {
                defaultLabel = "GrinderAction".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/TradeMode", true),
                disabled = this.ticks > 0 || !hasPower(),
                action = delegate {
                    PlaceWorker_ShowGrinderRadius pp = (this.parent as Building).def.PlaceWorkers[0] as PlaceWorker_ShowGrinderRadius;
                    List<IntVec3> js = pp.CellsAround(this.parent.DrawPos.ToIntVec3(), this.parent.Map);
                    int count = 0;
                    foreach (IntVec3 cell in js)
                    {
                        // if (!cell.IsValid) continue;
                        List<Thing> list = (this.parent.Map.thingGrid.ThingsListAt(cell));

                        if (list.Count < 1) continue;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Pawn p;
                            if ((p = (list[i] as Pawn)) != null)
                            {

                                if (p.Faction != Faction.OfPlayer)
                                    continue;

                                spawnGold(p);
                                count++;
                                p.Kill(new DamageInfo(DamageDefOf.Crush, 0, 0, -1, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null), null);

                                if (!(p.kindDef.defName.StartsWith("ra2_")))
                                    p.Corpse.Kill(null);



                            }
                        }
                    }
                    if (count > 0) {
                        this.ticks = 300;
                        SoundDef.Named("ra2_grinder_action").PlayOneShot(this.parent);
                        ThrowSparks(this.parent.Position.ToVector3(),this.parent.Map);

                    }
                
            
                    
                  }
                };



            yield break;

        }

        public void ThrowSparks(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            
            for (int i=0;i<3;i++) {
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_MicroSparks, null);
                moteThrown.Scale = Rand.Range(2.5f, 3f);
                moteThrown.rotationRate = Rand.Range(-12f, 12f);
                moteThrown.exactPosition = loc;
                moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
                moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
                moteThrown.SetVelocity((float)Rand.Range(7*(i+1),10*(i+1)), 2f);
                GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            }
            MoteMaker.ThrowLightningGlow(this.parent.Position.ToVector3(),this.parent.Map,6f);
        }

    }
}
