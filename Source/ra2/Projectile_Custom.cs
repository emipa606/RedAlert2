using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

public class Projectile_Custom : Projectile
{
    // usually for armor-piercing projectile


    private int ticksToDetonation;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksToDetonation, "ticksToDetonation");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
    }

    public override void Tick()
    {
        base.Tick();
        if (ticksToDetonation <= 0)
        {
            return;
        }

        ticksToDetonation--;
        if (ticksToDetonation <= 0)
        {
            Explode();
        }
    }

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        var map = Map;
        if (map == null)
        {
            Log.Error("map is null!");
        }

        if (def.projectile.explosionRadius > 0)
        {
            ImpactExplode(hitThing);
        }
        else
        {
            ImpactDirectly(hitThing, map);
        }
    }


    protected virtual void ImpactDirectly(Thing hitThing, Map map)
    {
        base.Impact(hitThing);

        var entry = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
            equipmentDef, def, targetCoverDef);
        Find.BattleLog.Add(entry);
        if (hitThing != null)
        {
            var damageAmount = DamageAmount;
            var damageDef = def.projectile.damageDef;
            var y = ExactRotation.eulerAngles.y;
            var armorPenetration = ArmorPenetration;
            var instigator = launcher;
            var thingDef = equipmentDef;
            var dinfo = new DamageInfo(damageDef, damageAmount, armorPenetration, y, instigator, null, thingDef,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            hitThing.TakeDamage(dinfo).AssociateWithLog(entry);
        }
        else
        {
            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
            FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
            if (Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(ExactPosition, map, (float)(Mathf.Sqrt(DamageAmount) * 1.0), 4f);
            }
        }
    }

    protected virtual void ImpactExplode(Thing hitThing)
    {
        if (def.projectile.explosionDelay == 0)
        {
            Explode();
        }
        else
        {
            landed = true;
            ticksToDetonation = def.projectile.explosionDelay;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, def.projectile.damageDef, launcher.Faction);
        }
    }

    protected virtual void Explode()
    {
        var map = Map;
        Destroy();
        if (def.projectile.explosionEffect != null)
        {
            var effecter = def.projectile.explosionEffect.Spawn();
            effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
            effecter.Cleanup();
        }

        var position = Position;
        var explosionRadius = def.projectile.explosionRadius;
        var damageDef = def.projectile.damageDef;
        var instigator = launcher;
        var damageAmountBase = DamageAmount;
        var soundExplode = def.projectile.soundExplode;
        var thingDef = equipmentDef;
        var projectile = def;
        var postExplosionSpawnThingDef = def.projectile.postExplosionSpawnThingDef;
        var postExplosionSpawnChance = def.projectile.postExplosionSpawnChance;
        var postExplosionSpawnThingCount = def.projectile.postExplosionSpawnThingCount;
        var preExplosionSpawnThingDef = def.projectile.preExplosionSpawnThingDef;
        GenExplosion.DoExplosion(position, map, explosionRadius, damageDef, instigator, damageAmountBase,
            ArmorPenetration, soundExplode, thingDef, projectile, intendedTarget.Thing, postExplosionSpawnThingDef,
            postExplosionSpawnChance, postExplosionSpawnThingCount,
            def.projectile.postExplosionGasType,
            def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef,
            def.projectile.preExplosionSpawnChance, def.projectile.preExplosionSpawnThingCount,
            def.projectile.explosionChanceToStartFire, def.projectile.explosionDamageFalloff);
    }
}