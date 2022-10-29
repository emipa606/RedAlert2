using RimWorld;
using Verse;

namespace ra2;

public class Projectile_Desolator : Projectile_Line
{
    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        if (hitThing is Pawn p)
        {
            var harmType = canBeHarm(p);


            switch (harmType)
            {
                case 0:
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

                    p.TakeDamage(dinfo).AssociateWithLog(entry);

                    if (p.Dead)
                    {
                        if (p.Spawned)
                        {
                            p.DeSpawn();
                        }

                        var cp = (Corpse)ThingMaker.MakeThing(p.RaceProps.corpseDef);
                        p.holdingOwner = null;
                        cp.InnerPawn = p;
                        Corpse.PostCorpseDestroy(p);
                        var comp = cp.GetComp<CompRottable>();
                        comp.RotProgress += 6000000000f;
                    }

                    break;
                }
                case 2:
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
                    break;
                }
            }
        }

        base.Impact(hitThing, blockedByShield);
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