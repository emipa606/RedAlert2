using RimWorld;
using Verse;

namespace ra2.Yuri;

public class InitiateBullet : Projectile
{
    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        var map = Map;
        base.Impact(hitThing, blockedByShield);
        var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing,
            intendedTarget.Thing, equipmentDef, def, targetCoverDef);
        Find.BattleLog.Add(battleLogEntry_RangedImpact);
        if (hitThing != null)
        {
            var damageDef = def.projectile.damageDef;
            float amount = DamageAmount;
            var armorPenetration = ArmorPenetration;
            var y = ExactRotation.eulerAngles.y;
            var instigator = launcher;
            var thingDef = equipmentDef;
            var dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, instigator, null, thingDef,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
            if (hitThing is Pawn { stances: { } } pawn && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
            {
                pawn.stances.stagger.StaggerFor(95);
            }
        }

        //else
        //{
        var fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
        fire.fireSize = 0.1f;
        if (map.thingGrid.ThingAt(ExactPosition.ToIntVec3(), ThingDefOf.Fire) != null)
        {
            ((Fire)map.thingGrid.ThingAt(ExactPosition.ToIntVec3(), ThingDefOf.Fire)).fireSize += 0.1f;
        }
        else
        {
            GenSpawn.Spawn(fire, ExactPosition.ToIntVec3(), map, Rot4.North);
        }


        // }
    }
}