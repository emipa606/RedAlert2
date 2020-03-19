using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    public class Projectile_Tanya : Projectile
    {


        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
 

           
            if (hitThing != null) {

                Pawn p = hitThing as Pawn;
                if (p != null)
                {
                   

                    int ptype = canBeHarm(p);
                    int damageAmountBase = 60;
                    if (ptype==2)
                    { damageAmountBase = 50; }






                    BattleLogEntry_RangedImpact entry = new BattleLogEntry_RangedImpact(base.launcher, hitThing, this.intendedTarget.Thing, base.equipmentDef, base.def, base.targetCoverDef);
                    Find.BattleLog.Add(entry);

                    DamageDef damageDef = base.def.projectile.damageDef;
                    float y = this.ExactRotation.eulerAngles.y;
                    Thing launcher = base.launcher;
                    ThingDef equipmentDef = base.equipmentDef;
                   // DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                   // DamageInfo dinfo = new DamageInfo(damageDef,damageAmountBase,0,y,null,null,equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                    DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, base.ArmorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);


                    BodyPartRecord bodypart = null;
                    foreach (BodyPartRecord bpr in p.health.hediffSet.GetNotMissingParts()) {
                        if (bpr.def.defName.EqualsIgnoreCase("head"))
                        {
                            bodypart = bpr;
                            break;
                        }
                    }//.GetBrain();
                    if (bodypart != null&&ptype==1) {
                        dinfo.SetHitPart(bodypart);
                    }

                    hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                    //DamageInfo dinfo = new DamageInfo(DamageDefOfRA2.TanyaGun, 9999, -1, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                    //hitThing.TakeDamage(dinfo).InsertIntoLog(entry);
                }
                else {
                    BattleLogEntry_RangedImpact entry = new BattleLogEntry_RangedImpact(base.launcher, hitThing, this.intendedTarget.Thing, base.equipmentDef, base.def, base.targetCoverDef);
                    Find.BattleLog.Add(entry);


                    DamageDef damageDef = base.def.projectile.damageDef;
                    float y = this.ExactRotation.eulerAngles.y;
                    Thing launcher = base.launcher;
                    ThingDef equipmentDef = base.equipmentDef;
                    int damageAmountBase = base.DamageAmount;
                    DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, base.ArmorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);



                    hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                }
            }
           
        }


        private int canBeHarm(Pawn p)
        {
            if (p.kindDef.RaceProps.IsMechanoid) { return 2; }

            if (p.kindDef.RaceProps.IsFlesh) { return 1; }



            return 0;
        }

    }
}
