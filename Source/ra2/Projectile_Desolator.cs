using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    public class Projectile_Desolator : Projectile_Line
    {


        protected override void Impact(Thing hitThing)
        {
           


            if (hitThing != null) {
                Pawn p = hitThing as Pawn;
                if (p != null)
                {

                    int harmType = canBeHarm(p);


                    if (harmType == 0)
                    {
                        BattleLogEntry_RangedImpact entry = new BattleLogEntry_RangedImpact(base.launcher, hitThing, this.intendedTarget.Thing, base.equipmentDef, base.def, base.targetCoverDef);
                        Find.BattleLog.Add(entry);
                        int damageAmountBase = 60;
                        DamageDef damageDef = base.def.projectile.damageDef;
                        float y = this.ExactRotation.eulerAngles.y;
                        Thing launcher = base.launcher;
                        ThingDef equipmentDef = base.equipmentDef;
                        DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, base.ArmorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);



                        /*
                        HediffSet bodypart = p.health.hediffSet;
                        IEnumerable<BodyPartRecord> blist = bodypart.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);
                        if(blist!=null)
                        foreach (BodyPartRecord record in blist)
                        {
                            if (record.def.tags.Contains(BodyPartTagDefOf.BloodPumpingSource))
                            {
                                

                                dinfo.SetHitPart(record);
                                break;
                            }
                        }
                        
                         */
                        hitThing.TakeDamage(dinfo).AssociateWithLog(entry);

                        // ((Pawn)hitThing).health.hediffSet.AddDirect(HediffMaker.MakeHediff(HediffDefOf.ToxicBuildup,(Pawn)hitThing));

                        // hitThing.Kill(dinfo);
                        if ((hitThing as Pawn).Dead) {
                            if (((Pawn)hitThing).Spawned)
                            {
                                hitThing.DeSpawn(DestroyMode.Vanish);
                            }

                            Pawn news = hitThing as Pawn;

                            Corpse cp = (Corpse)ThingMaker.MakeThing(((Pawn)hitThing).RaceProps.corpseDef, null);
                            // hitThing.Destroy(DestroyMode.Vanish);
                            hitThing.holdingOwner = null;
                            cp.InnerPawn = news;
                            Corpse.PostCorpseDestroy(news);
                            //cp.InnerPawn.health.hediffSet.part
                            CompRottable comp = cp.GetComp<CompRottable>();
                            //comp.RotImmediately();
                            // Log.Warning(comp.RotProgress+" int");
                            comp.RotProgress += 6000000000f;

                        }


                    }
                    else if (harmType == 2)
                    {
                        BattleLogEntry_RangedImpact entry = new BattleLogEntry_RangedImpact(base.launcher, hitThing, this.intendedTarget.Thing, base.equipmentDef, base.def, base.targetCoverDef);
                        Find.BattleLog.Add(entry);
                        int damageAmountBase = 100;
                        DamageDef damageDef = base.def.projectile.damageDef;
                        float y = this.ExactRotation.eulerAngles.y;
                        Thing launcher = base.launcher;
                        ThingDef equipmentDef = base.equipmentDef;
                        DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, base.ArmorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);



                        hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                    }
                    else { }

                    
                }
             
            }
            base.Impact(hitThing);

        }

        private int canBeHarm(Pawn p) {
         
            if (p.kindDef.RaceProps.IsMechanoid) { return 2; }
            if (p.AnimalOrWildMan()) return 0;
            if (p.apparel.WornApparelCount > 0) {
                bool hat = false;
                bool upc = false;
                bool belowc = false;
                foreach (Apparel ap in p.apparel.WornApparel) {
                    if (ap.def.defName.EqualsIgnoreCase("ra2_hat_desolator")) { hat = true; }
                    if (ap.def.defName.EqualsIgnoreCase("ra2_parka_desolator")) { upc = true; }
                    if (ap.def.defName.EqualsIgnoreCase("ra2_pants_desolator")) { belowc = true; }
                }

                if (hat && upc && belowc) { return 1; }
            }



                return 0;
        }
    }
}
