using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;
using System.Text;

namespace ra2
{
    // Token: 0x02000006 RID: 6
    public class Building_AirDefense : Building
    {



        protected  TurretTop_CustomSize top;
        protected CompTurretTopSize topSizeComp;
        protected StunHandler stunner;

        public Building_AirDefense()
        {
            this.top = new TurretTop_CustomSize(this);
            this.stunner = new StunHandler(this);
            this.nowTarget = null;
        }

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(base.GetInspectString());
            
            if(underRoof )
            sb.Append("\n"+"ADAUnderRoof".Translate());
            
        
           
            
           // String sb = base.GetInspectString() + "\n" + Translator.Translate("restoreBullet") + ":" + this.restoreBullet + "\n" + Translator.Translate("restoreProgress") + ":" + (this.ticks);
            return sb.ToString();

        }
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000A RID: 10 RVA: 0x00002324 File Offset: 0x00000524
        private IEnumerable<IntVec3> AirCells
        {
            get
            {
                return this.AirCellsAround(base.Position, base.Map);
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000B RID: 11 RVA: 0x00002338 File Offset: 0x00000538
        private bool ShouldAttackNow
        {
            get
            {
                if (!base.Spawned)
                {
                    return false;
                }
                if (!FlickUtility.WantsToBeOn(this))
                {
                    return false;
                }
                if (underRoof) {
                    return false;
                }
              
               // if (this.restoreBullet < 1) return false;
                CompPowerTrader compPowerTrader = ThingCompUtility.TryGetComp<CompPowerTrader>(this);
                if (compPowerTrader != null && !compPowerTrader.PowerOn)
                {
                    return false;
                }
                if (this.stunner.Stunned) {
                    return false;
                }
                CompRefuelable compRefuelable = ThingCompUtility.TryGetComp<CompRefuelable>(this);
                return compRefuelable == null || compRefuelable.HasFuel;
            }
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002384 File Offset: 0x00000584
        public override void Tick()
        {
            base.Tick();
            doTurretTick();
          //  if (this.restoreBullet < this.TopSizeComp.Props.maxStoreBullet) {
                CompPowerTrader compPowerTrader = ThingCompUtility.TryGetComp<CompPowerTrader>(this);
                if (compPowerTrader != null && compPowerTrader.PowerOn)
                {
                    this.ticks++;
                }
             //    }

     

            if(this.ticks >= ticksAirInterval)
            {
                //this.restoreBullet++;
                if (this.ShouldAttackNow)
                {
                   
                    this.airThings = this.AirThings(this.AirCells);
                    if (this.airThings.Count() > 0)
                    {
                        // Log.Warning("c > 0");
                        Thing t = this.airThings.RandomElement();
                       
                           
                            this.turnAndAttack(t);
                        
                    }
                    else {
                     //   Log.Warning("air c = 0");
                    }
                  //  Log.Warning("air thing is "+this.airThings.Count);
           
                }
                this.ticks = 0;
            }
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002428 File Offset: 0x00000628
        private List<Thing> AirThings(IEnumerable<IntVec3> intVecs)
        {
            List<Thing> list = new List<Thing>();
            list.Clear();
            foreach (IntVec3 intVec in intVecs)
            {
               
                foreach (Thing thing in Enumerable.Where<Thing>(base.Map.thingGrid.ThingsListAt(intVec), delegate (Thing x) {
                    return x is Thing;
                }))
                {


                    if ((thing is Building || thing.def.altitudeLayer == AltitudeLayer.Item || thing is Filth|| thing is Mote||thing is Pawn||thing is Plant))
                        continue;
                    //if(thing is Skyfaller)
                    // Log.Warning("thing in "+intVec+" is "+thing);

                    if (IsAirTarget(thing))
                    {
                        list.Add(thing);
                        //Log.Warning("thing in " + intVec + " is " + thing + " isair");
                    }
                    else {
                      //  Log.Warning("thing in " + intVec + " is " + thing +" is not air");
                    }
              
                }



            }
            return list;
        }





        public List<IntVec3> AirCellsAround(IntVec3 pos, Map map)
        {
            this.airCells.Clear();
            
            int num = GenRadial.NumCellsInRadius(range);
            for (int i = 0; i < num; i++)
            {
               airCells.Add(pos + GenRadial.RadialPattern[i]);
            }


            return this.airCells;

        }



        private void turnAndAttack(Thing t)
        {

            if (IsAirTarget(t))
            {
                this.nowTarget = t;
                doTurretTick();
                SoundDef sd = DefDatabase<SoundDef>.GetNamed(this.TopSizeComp.Props.soundShoot,true); 
                SoundStarter.PlayOneShot(sd, new TargetInfo(base.Position, base.Map, true));
                MoteMaker.ThrowExplosionCell(this.Position, this.Map, ThingDefOf.Mote_ExplosionFlash, new UnityEngine.Color(1, 1, 1));
                
                MoteMaker.ThrowText(this.DrawPos,this.Map,"AirDefenseHit".Translate(),new UnityEngine.Color(0.66f,0.66f,0.12f));
                if (t is Projectile) {
                    MoteMaker.ThrowSmoke((t.DrawPos), this.Map, 3f);
                }
                //  GenExplosion.DoExplosion(t.Position, t.Map, 2f, DamageDefOf.Bomb, this, 3, -1, SoundDefOf.Artillery_ShellLoaded, null, null, t, null, 0, 1, false, null, 0, 1, 0, false);

                if (isPlayerInside(t)!=null) {
                    StringBuilder builder = new StringBuilder();
                    String text = Translator.Translate("BeshotdownMsg", new object[]{this.Faction.def.fixedName});
                  
                    // builder.AppendLine("Oh,no!One or more of our colonists were shot down by " + this.Faction.def.fixedName + "'s Antiaircraft gun:");
                    builder.AppendLine(text);

                    List < Pawn > pl = isPlayerInside(t);
                    if (pl.Count > 0) {
                        foreach (Pawn p in pl) {
                            builder.AppendLine("    -"+p.Name);
                        }
                        Find.LetterStack.ReceiveLetter(Translator.Translate("Beshotdown"), builder.ToString(), LetterDefOf.Death, this);
                    }
                }
                destoryAir(t);
                this.nowTarget = null;

              //  Log.Warning(t + " is air target");
            }
            else {
              //  Log.Warning(t+" is not air target");
            }
        }

        public void destoryAir(Thing t) {
            if (t is DropPodIncoming) {
                List<Thing> leftthing = new List<Thing>();
                DropPodIncoming dp = t as DropPodIncoming;
                for (int i = dp.Contents.innerContainer.Count - 1; i >= 0; i--)
                {

                    Thing thing = dp.Contents.innerContainer[i];
                    if (thing is Pawn)
                    {
                        Pawn p = thing as Pawn;
                        p.Kill(null, null);
                                    //Corpse cp = ((Corpse)ThingMaker.MakeThing(p.RaceProps.corpseDef,null));
                                    //cp.InnerPawn = p;
                                    //cp.GetComp<CompRottable>().RotImmediately();

                                    //leftthing.Add(cp);
                        leftthing.Add(p);
                   
                    }else
                    leftthing.Add(thing);
                }

                




                Random rr = new Random();
                foreach (Thing tt in leftthing) {
                    if (!(tt is Pawn)) {
                        
                        int v = rr.Next(100);
                       // Log.Warning(v+"/"+Settings.droppodCargoDropPercentage+"%");
                        if (v >= Settings.droppodCargoDropPercentage ) { continue; }
                    }

                  
                        GenPlace.TryPlaceThing(tt, t.Position, this.Map, ThingPlaceMode.Near);
                    

                    SoundStarter.PlayOneShot(SoundDefOf.DropPod_Open,new TargetInfo(tt));
                }
                for (int i = 0; i < 3; i++)
                {
                    Thing sl = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel);
                    GenPlace.TryPlaceThing(sl, t.Position, this.Map, ThingPlaceMode.Near);
                }

            }

            t.Destroy();

        }





















        // Token: 0x06000010 RID: 16 RVA: 0x00002604 File Offset: 0x00000804
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
        }





        private bool IsAirTarget(Thing t)
        {
            if (t is Projectile)
            {
                Projectile p = t as Projectile;
                bool airbullet = p.def.projectile.flyOverhead;

                if(airbullet)
                {
                    // Log.Warning(BulletStore.theBullet.Count+" now");
                    foreach (ProjectileEnd pe in BulletStore.theBullet)
                    {
                        if (pe.shell == p)
                        {
                            // Log.Message("equal!");
                            if (pe.launcher != null)
                            {
                                //   Log.Message("not null");
                                if (pe.launcher.Faction != this.Faction && pe.launcher.Faction.HostileTo(this.Faction))
                                {
                                      //  Log.Message("equal faction!");
                                    // BulletStore.theBullet.Remove(pe);
                                    return true;
                                }
                            }
                        }
                    }

                }
            }else
            if (t is DropPodIncoming)
            {
                DropPodIncoming dp = t as DropPodIncoming;
                for (int i = dp.Contents.innerContainer.Count - 1; i >= 0; i--)
                {
                   
                    Thing thing = dp.Contents.innerContainer[i];
                    if (thing is Pawn) {
                        Pawn p = thing as Pawn;
                        if (p.Faction != this.Faction && p.Faction.HostileTo(this.Faction)) {
                            return true;
                        }
                    }
                }
                    return false;
    
            }

         

            return false;
        }


        public List<Pawn> isPlayerInside(Thing t) {
            List<Pawn> result = new List<Pawn>();
            if (t is DropPodIncoming)
            {
                DropPodIncoming dp = t as DropPodIncoming;
                for (int i = dp.Contents.innerContainer.Count - 1; i >= 0; i--)
                {

                    Thing thing = dp.Contents.innerContainer[i];
                    if (thing is Pawn)
                    {
                        Pawn p = thing as Pawn;
                        if (p.Faction != this.Faction && p.Faction.HostileTo(this.Faction)&&p.Faction==Faction.OfPlayer)
                        {
                            result.Add(p);
                        }
                    }
                }
                
            }
            return result;
        }


        private void doTurretTick() {
            CompPowerTrader powerComp = ThingCompUtility.TryGetComp<CompPowerTrader>(this);
            if ((((powerComp == null) || powerComp.PowerOn)  && base.Spawned))
            {
               
              //  if ((this.ShouldAttackNow))
               // {

                    this.top.TurretTopTick();
               // }
            }
        }




        public override void Draw()
        {
            this.top.DrawTurret();
            base.Comps_PostDraw();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.topSizeComp = base.GetComp<CompTurretTopSize>();
     
        }

        public LocalTargetInfo nowTarget;


        // Token: 0x04000003 RID: 3
        public List<IntVec3> airCells = new List<IntVec3>();

        private int range => 20;

        // Token: 0x04000005 RID: 5
        private List<Thing> airThings = new List<Thing>();

        // Token: 0x04000006 RID: 6
        private int ticksAirInterval =120;
        private bool underRoof => GridsUtility.Roofed(this.Position, this.Map);
        // Token: 0x04000007 RID: 7
        private int ticks;

    
        public CompTurretTopSize TopSizeComp =>
    this.topSizeComp;
    }
}
