using RimWorld;
using Verse;

namespace ra2.Yuri
{
    public class InitiateBullet : Projectile
    {
        // Token: 0x060026E7 RID: 9959 RVA: 0x00127F60 File Offset: 0x00126360
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            //else
            //{
                Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
                fire.fireSize = 0.1f;
                if (map.thingGrid.ThingAt(this.ExactPosition.ToIntVec3(), ThingDefOf.Fire) != null) {
                    ((Fire)(map.thingGrid.ThingAt(this.ExactPosition.ToIntVec3(), ThingDefOf.Fire))).fireSize += 0.1f;
                }
                else
                GenSpawn.Spawn(fire, this.ExactPosition.ToIntVec3(), map, Rot4.North, WipeMode.Vanish, false);


           // }
        }
    }
}
