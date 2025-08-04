using Verse;

namespace ra2;

public class Projectile_Sniper : Projectile_Line
{
    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        if (hitThing is Pawn p)
        {
            var harmType = canBeHarm(p);


            switch (harmType)
            {
                case 1:
                {
                    var entry = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
                        equipmentDef, def, targetCoverDef);
                    Find.BattleLog.Add(entry);

                    var damageAmountBase = 100;
                    var damageDef = def.projectile.damageDef;
                    var y = ExactRotation.eulerAngles.y;
                    var instigator = launcher;
                    var thingDef = equipmentDef;
                    var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                        thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


                    hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                    break;
                }
                case 2:
                {
                    var entry = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
                        equipmentDef, def, targetCoverDef);
                    Find.BattleLog.Add(entry);

                    var damageAmountBase = 30;
                    var damageDef = def.projectile.damageDef;
                    var y = ExactRotation.eulerAngles.y;
                    var instigator = launcher;
                    var thingDef = equipmentDef;
                    var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                        thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


                    hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
                    break;
                }
            }
        }

        base.Impact(hitThing, blockedByShield);
    }

    private static int canBeHarm(Pawn p)
    {
        return p.kindDef.RaceProps.IsMechanoid ? 2 : 1;
    }
}