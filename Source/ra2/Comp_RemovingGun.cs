using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    public class Comp_RemovingGun : ThingComp
    {
        // public Thing nowHitBuild;
        public int hitTime;
        public Thing lastHitBuild;
        public int ticks;


        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            hitTime = 0;
            ticks = 0;
            lastHitBuild = null;
            //    this.nowHitBuild = null;
        }

        public void resolveHitBuilding(Thing nowbuilding)
        {
            if (nowbuilding == lastHitBuild)
            {
                hitTime++;
            }
            else
            {
                lastHitBuild = nowbuilding;
                hitTime = 1;
            }

            ticks = 300;
            var dinfo = new DamageInfo(DamageDefOf.Stun, 10, -1, 1, null, null, parent.def,
                DamageInfo.SourceCategory.ThingOrUnknown, lastHitBuild);
            lastHitBuild.TakeDamage(dinfo);
        }


        public override void PostExposeData()
        {
            Scribe_Values.Look(ref hitTime, "hitTime");
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_References.Look(ref lastHitBuild, "lastHitBuild");
            // Scribe_References.Look<Thing>(ref this.nowHitBuild, "nowHitBuild", false);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (ticks > 0)
            {
                ticks--;
            }

            if (ticks < 0)
            {
                ticks = 0;
            }

            if (ticks < 1)
            {
                lastHitBuild = null;
                hitTime = 0;
            }


            if (ticks > 0 && ticks % 5 == 0 && lastHitBuild != null)
            {
                ChronoMoteMaker.ThrowCell(lastHitBuild.TrueCenter().ToIntVec3(), lastHitBuild.Map,
                    DefDatabase<ThingDef>.GetNamed("Mote_BlastEMP"), new Color(1, 1, 1),
                    lastHitBuild is Pawn pawn
                        ? new IntVec2((int) pawn.RaceProps.baseBodySize,
                            (int) pawn.RaceProps.baseBodySize)
                        : lastHitBuild.def.size);
            }


            if (lastHitBuild == null)
            {
                return;
            }

            if (lastHitBuild.Destroyed)
            {
                lastHitBuild = null;
                hitTime = 0;
                return;
            }

            var HP = 1000;
            if (lastHitBuild.def.useHitPoints)
            {
                HP = lastHitBuild.MaxHitPoints;
            }

            if (lastHitBuild is Pawn)
            {
                HP = 400;
            }

            if (hitTime <= HP / (20 * hitTime))
            {
                return;
            }

            DefDatabase<SoundDef>.GetNamed("ra2_Chrono_kill").PlayOneShot(lastHitBuild);
            FleckMaker.ThrowExplosionCell(lastHitBuild.Position, lastHitBuild.Map, FleckDefOf.ExplosionFlash,
                new Color(1, 1, 1));


            if (lastHitBuild.def == ThingDefOf.SteamGeyser)
            {
                lastHitBuild.DeSpawn();
            }
            else
            {
                lastHitBuild.Destroy();
            }


            lastHitBuild = null;
            hitTime = 0;
            ticks = 0;
        }

        // public CompProperties_RemovingGun Props =>
        //  ((CompProperties_RemovingGun)base.props);
    }
}