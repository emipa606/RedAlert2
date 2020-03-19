using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2
{
    public class Projectile_SiegeCadre : Projectile_Line
    {


        protected override void Impact(Thing hitThing)
        {



            if (hitThing != null)
            {
                Pawn p = hitThing as Pawn;
                if (p != null)
                {

                    int harmType = canBeHarm(p);


                    if (harmType == 1)
                    {
                        BattleLogEntry_RangedImpact entry = new BattleLogEntry_RangedImpact(base.launcher, hitThing, this.intendedTarget.Thing, base.equipmentDef, base.def, base.targetCoverDef);
                        Find.BattleLog.Add(entry);

                        int damageAmountBase = base.DamageAmount / 3;
                        DamageDef damageDef = base.def.projectile.damageDef;
                        float y = this.ExactRotation.eulerAngles.y;
                        Thing launcher = base.launcher;
                        ThingDef equipmentDef = base.equipmentDef;
                        DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, base.ArmorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);

                      


                        hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                        return;
                    }
                    else {
                        base.Impact(hitThing);
                    }


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
