namespace ra2
{
    using RimWorld;
    using System;
    using System.Collections.Generic;
    using Verse;
    using Verse.AI;


    public class Building_CustomTurretGun : Building_TurretGun
    {



        protected new TurretTop_CustomSize top;
        protected CompTurretTopSize topSizeComp;
   

        public bool IsStun 
        {
            get{
                return base.stunner.Stunned;
            }

        }

        public Building_CustomTurretGun()
        {
            this.top = new TurretTop_CustomSize(this);
            

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

     

        public override void Tick()
        {
            base.Tick();
      

  
            if ((((base.powerComp == null) || base.powerComp.PowerOn) && ((base.mannableComp == null) || base.mannableComp.MannedNow)) && base.Spawned)
            {
                base.GunCompEq.verbTracker.VerbsTick();
                if (!base.stunner.Stunned && (base.GunCompEq.PrimaryVerb.state != VerbState.Bursting))
                {
                
                    this.top.TurretTopTick();
                }
            }
 
        }







       
        public CompTurretTopSize TopSizeComp =>
            this.topSizeComp;

 
    }
}
