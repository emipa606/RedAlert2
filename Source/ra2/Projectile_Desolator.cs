using RimWorld;
using Verse;

namespace ra2
{
    public class Projectile_Desolator : Projectile_Line
    {
        protected override void Impact(Thing hitThing)
        {
            if (hitThing is Pawn p)
            {
                var harmType = canBeHarm(p);


                if (harmType == 0)
                {
                    var entry = new BattleLogEntry_RangedImpact(launcher, p, intendedTarget.Thing,
                        equipmentDef, def, targetCoverDef);
                    Find.BattleLog.Add(entry);
                    var damageAmountBase = 60;
                    var damageDef = def.projectile.damageDef;
                    var y = ExactRotation.eulerAngles.y;
                    var instigator = launcher;
                    var thingDef = equipmentDef;
                    var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                        thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


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
                    p.TakeDamage(dinfo).AssociateWithLog(entry);

                    // ((Pawn)hitThing).health.hediffSet.AddDirect(HediffMaker.MakeHediff(HediffDefOf.ToxicBuildup,(Pawn)hitThing));

                    // hitThing.Kill(dinfo);
                    if (p.Dead)
                    {
                        if (p.Spawned)
                        {
                            p.DeSpawn();
                        }

                        var news = p;

                        var cp = (Corpse) ThingMaker.MakeThing(p.RaceProps.corpseDef);
                        // hitThing.Destroy(DestroyMode.Vanish);
                        p.holdingOwner = null;
                        cp.InnerPawn = news;
                        Corpse.PostCorpseDestroy(news);
                        //cp.InnerPawn.health.hediffSet.part
                        var comp = cp.GetComp<CompRottable>();
                        //comp.RotImmediately();
                        // Log.Warning(comp.RotProgress+" int");
                        comp.RotProgress += 6000000000f;
                    }
                }
                else if (harmType == 2)
                {
                    var entry = new BattleLogEntry_RangedImpact(launcher, p, intendedTarget.Thing,
                        equipmentDef, def, targetCoverDef);
                    Find.BattleLog.Add(entry);
                    var damageAmountBase = 100;
                    var damageDef = def.projectile.damageDef;
                    var y = ExactRotation.eulerAngles.y;
                    var instigator = launcher;
                    var thingDef = equipmentDef;
                    var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                        thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


                    p.TakeDamage(dinfo).AssociateWithLog(entry);
                }
            }

            base.Impact(hitThing);
        }

        private int canBeHarm(Pawn p)
        {
            if (p.kindDef.RaceProps.IsMechanoid)
            {
                return 2;
            }

            if (p.AnimalOrWildMan())
            {
                return 0;
            }

            if (p.apparel.WornApparelCount <= 0)
            {
                return 0;
            }

            var hat = false;
            var upc = false;
            var belowc = false;
            foreach (var ap in p.apparel.WornApparel)
            {
                if (ap.def.defName.EqualsIgnoreCase("ra2_hat_desolator"))
                {
                    hat = true;
                }

                if (ap.def.defName.EqualsIgnoreCase("ra2_parka_desolator"))
                {
                    upc = true;
                }

                if (ap.def.defName.EqualsIgnoreCase("ra2_pants_desolator"))
                {
                    belowc = true;
                }
            }

            if (hat && upc && belowc)
            {
                return 1;
            }


            return 0;
        }
    }
}