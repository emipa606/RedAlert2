using Verse;

namespace ra2;

public class Projectile_SiegeCadre : Projectile_Line
{
    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        if (hitThing is not Pawn p)
        {
            return;
        }

        var harmType = canBeHarm(p);


        if (harmType == 1)
        {
            var entry = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
                equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(entry);

            var damageAmountBase = DamageAmount / 3;
            var damageDef = def.projectile.damageDef;
            var y = ExactRotation.eulerAngles.y;
            var instigator = launcher;
            var thingDef = equipmentDef;
            var dinfo = new DamageInfo(damageDef, damageAmountBase, ArmorPenetration, y, instigator, null,
                thingDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);


            hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
        }
        else
        {
            base.Impact(hitThing, blockedByShield);
        }
    }

    private static int canBeHarm(Pawn p)
    {
        if (p.kindDef.RaceProps.IsMechanoid)
        {
            return 2;
        }

        return p.kindDef.RaceProps.IsFlesh ? 1 : 0;
    }
}