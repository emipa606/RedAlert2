using Verse;

namespace ra2
{
    public class Projectile_Tanya : Projectile
    {
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);


            if (hitThing == null)
            {
                return;
            }

            if (hitThing is Pawn p)
            {
                var ptype = canBeHarm(p);
                var damageAmountBase = 60;
                if (ptype == 2)
                {
                    damageAmountBase = 50;
                }


                var entry = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
                    equipmentDef, def, targetCoverDef);
                Find.BattleLog.Add(entry);

                var damageDef = def.projectile.damageDef;
                var y = ExactRotation.eulerAngles.y;
                var instigator = launcher;
                var thingDef = equipmentDef;
                // DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                // DamageInfo dinfo = new DamageInfo(damageDef,damageAmountBase,0,y,null,null,equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                    thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


                BodyPartRecord bodypart = null;
                foreach (var bpr in p.health.hediffSet.GetNotMissingParts())
                {
                    if (!bpr.def.defName.EqualsIgnoreCase("head"))
                    {
                        continue;
                    }

                    bodypart = bpr;
                    break;
                } //.GetBrain();

                if (bodypart != null && ptype == 1)
                {
                    dinfo.SetHitPart(bodypart);
                }

                hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                //DamageInfo dinfo = new DamageInfo(DamageDefOfRA2.TanyaGun, 9999, -1, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                //hitThing.TakeDamage(dinfo).InsertIntoLog(entry);
            }
            else
            {
                var entry = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
                    equipmentDef, def, targetCoverDef);
                Find.BattleLog.Add(entry);


                var damageDef = def.projectile.damageDef;
                var y = ExactRotation.eulerAngles.y;
                var instigator = launcher;
                var thingDef = equipmentDef;
                var damageAmountBase = DamageAmount;
                var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                    thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


                hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
            }
        }


        private int canBeHarm(Pawn p)
        {
            if (p.kindDef.RaceProps.IsMechanoid)
            {
                return 2;
            }

            if (p.kindDef.RaceProps.IsFlesh)
            {
                return 1;
            }


            return 0;
        }
    }
}