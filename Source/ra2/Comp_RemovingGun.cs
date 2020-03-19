using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.Sound;

namespace ra2
{
    public class Comp_RemovingGun:ThingComp
    {
        public Thing lastHitBuild;
       // public Thing nowHitBuild;
        public int hitTime;
        public int ticks;
      

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.hitTime = 0;
            this.ticks = 0;
            this.lastHitBuild = null;
        //    this.nowHitBuild = null;

        }

        public void resolveHitBuilding(Thing nowbuilding) {
            if (nowbuilding == lastHitBuild)
            {
                this.hitTime++;
            }
            else {
                lastHitBuild = nowbuilding;
                this.hitTime = 1;
            }
            this.ticks = 300;
            DamageInfo dinfo = new DamageInfo(DamageDefOf.Stun, 10, -1,1, null, null, this.parent.def, DamageInfo.SourceCategory.ThingOrUnknown, lastHitBuild);
            lastHitBuild.TakeDamage(dinfo);
          

        }


        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.hitTime, "hitTime", 0, false);
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_References.Look<Thing>(ref this.lastHitBuild, "lastHitBuild", false);
           // Scribe_References.Look<Thing>(ref this.nowHitBuild, "nowHitBuild", false);
        }

        public override void CompTick()
        {
            base.CompTick();
            
            if (this.ticks > 0) this.ticks --;
            if (this.ticks < 0) this.ticks = 0;
            if (this.ticks < 1) {
                lastHitBuild = null;
                hitTime = 0; }


            if (this.ticks>0&& this.ticks % 5 == 0 && lastHitBuild != null )
               ChronoMoteMaker.ThrowCell(lastHitBuild.TrueCenter().ToIntVec3(), lastHitBuild.Map, DefDatabase<ThingDef>.GetNamed("Mote_BlastEMP", true), new UnityEngine.Color(1, 1, 1),(lastHitBuild is Pawn)?new IntVec2((int)((Pawn)lastHitBuild).RaceProps.baseBodySize, (int)((Pawn)lastHitBuild).RaceProps.baseBodySize) :lastHitBuild.def.size);
            

            if (lastHitBuild != null)
            {
                if (lastHitBuild.Destroyed)
                {
                    lastHitBuild = null;
                    hitTime = 0;
                    return;
                }

                int HP=1000;
                if(lastHitBuild.def.useHitPoints)
                 HP=this.lastHitBuild.MaxHitPoints;
                if (lastHitBuild is Pawn)
                    HP = 400;
           
                if (this.hitTime > (int)(HP / (20*this.hitTime)))
                {
                   
                        SoundStarter.PlayOneShot(DefDatabase<SoundDef>.GetNamed("ra2_Chrono_kill", true),lastHitBuild);
                    MoteMaker.ThrowExplosionCell(lastHitBuild.Position,lastHitBuild.Map,ThingDefOf.Mote_ExplosionFlash,new UnityEngine.Color(1,1,1));
                    

                    if (lastHitBuild.def == ThingDefOf.SteamGeyser)
                        lastHitBuild.DeSpawn(DestroyMode.Vanish);
                    else
                    lastHitBuild.Destroy(DestroyMode.Vanish);


                    lastHitBuild = null;
                    hitTime = 0;
                    ticks = 0;
                }
            }else
            {
               // Log.Warning("null");

            }
        }

       // public CompProperties_RemovingGun Props =>
     //  ((CompProperties_RemovingGun)base.props);

    

    }
}
