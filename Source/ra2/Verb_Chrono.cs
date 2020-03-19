using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ra2
{
    // Token: 0x02001013 RID: 4115
    public class Verb_Chrono : Verb
    {

    

        // Token: 0x0600640B RID: 25611 RVA: 0x001B46FC File Offset: 0x001B2AFC
        public override void WarmupComplete()
        {
            base.WarmupComplete();
           // Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster, (!this.currentTarget.HasThing) ? null : this.currentTarget.Thing, (base.EquipmentSource == null) ? null : base.EquipmentSource.def, null, this.ShotsPerBurst > 1));
        }

        private void hitThing() {
           
 


                foreach (Apparel ap in this.CasterPawn.apparel.WornApparel)
                {
                    if (ap.def.defName == "ra2_Hat_Chrono")
                    {
                        Comp_RemovingGun crg = ap.TryGetComp<Comp_RemovingGun>();
                        crg.resolveHitBuilding(this.currentTarget.Thing);
                        break;
                    }
                }
     
       


        }
        
        // Token: 0x0600640C RID: 25612 RVA: 0x001B4770 File Offset: 0x001B2B70
        protected override bool TryCastShot()
        {
            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
            {
                return false;
            }

            ShootLine shootLine;
            bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
            if (this.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            if (base.EquipmentSource != null)
            {
                CompChangeableProjectile comp = base.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
            }
            Thing launcher = this.caster;
            Thing equipment = base.EquipmentSource;
            CompMannable compMannable = this.caster.TryGetComp<CompMannable>();
            if (compMannable != null && compMannable.ManningPawn != null)
            {
                launcher = compMannable.ManningPawn;
                equipment = this.caster;
            }
            Vector3 drawPos = this.caster.DrawPos;


            SpawnBeam();
            hitThing();




            ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = (randomCoverToMissInto == null) ? null : randomCoverToMissInto.def;
            if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);

                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
               
                return true;
            }

            if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
            {
     
                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
                
                return true;
            }


            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }

            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }

            return true;
        }



        private void SpawnBeam()
        {
       
            ChronoBeam cb = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ChronoBeam",true), null) as ChronoBeam;
            bool flag = cb == null;
            if (!flag)
            {
                
                cb.Setup(this.caster,this.currentTarget);
                GenSpawn.Spawn(cb, IntVec3Utility.ToIntVec3(this.caster.Position.ToVector3()), this.caster.Map, 0);
            }
        }


        // Token: 0x06006410 RID: 25616 RVA: 0x001B4C60 File Offset: 0x001B3060
        public override bool Available()
        {
            if (!base.Available())
            {
                return false;
            }
            if (base.CasterIsPawn)
            {
                Pawn casterPawn = base.CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }
            return true;
        }
       
    }
    
}
