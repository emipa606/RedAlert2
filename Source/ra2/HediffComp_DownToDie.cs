using RimWorld;
using Verse;
using Verse.Sound;

namespace ra2;

public class HediffComp_DownToDie : HediffComp
{
    public HediffCompProperties_DownToDie Props =>
        (HediffCompProperties_DownToDie)props;


    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);


        {
            var pe = Pawn.equipment;
            var pa = Pawn.apparel;
            if (Pawn.Faction == Faction.OfPlayer)
            {
                if (Pawn.kindDef.defName == "ra2_AlliedTanya")
                {
                }
                else if (pe.Primary == null || pe.Primary.def != getDefaultGun(Pawn.kindDef.defName))
                {
                    pe.Remove(pe.Primary);
                    pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(getDefaultGun(Pawn.kindDef.defName)));
                }

                if (Pawn.kindDef.defName == "ra2_AlliedChrono")
                {
                    var hasHat = false;
                    foreach (var ap in Pawn.apparel.WornApparel)
                    {
                        if (ap.def.defName != "ra2_Hat_Chrono")
                        {
                            continue;
                        }

                        hasHat = true;
                        break;
                    }

                    if (!hasHat)
                    {
                        Pawn.apparel.Wear(
                            (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Hat_Chrono")));
                    }
                }

                if (pa.WornApparel == null)
                {
                    pa.Wear((Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Belt")));
                }

                var hasBelt = false;
                if (pa.WornApparel != null)
                {
                    foreach (var ap in pa.WornApparel)
                    {
                        if (!ap.def.defName.Equals("ra2_Belt"))
                        {
                            continue;
                        }

                        hasBelt = true;
                        break;
                    }
                }

                if (!hasBelt)
                {
                    pa.Wear((Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Belt")));
                }
            }
        }

        if (Pawn.apparel.WornApparel.Find(x => x.TryGetComp<CompDownToDie>() != null) == null)
        {
            Pawn.health.RemoveHediff(parent);
        }


        if ((!Pawn.Downed || Pawn.Dead) && !Pawn.IsPrisoner)
        {
            return;
        }

        //Apparel ap = base.Pawn.apparel.WornApparel.Find(x => x.TryGetComp<CompDownToDie>() != null);
        //ap.Destroy(DestroyMode.Vanish);
        //base.Pawn.apparel.Remove(ap);
        var dinfo = new DamageInfo(DamageDefOf.Crush, 100, 0, 0);
        var bodypart = Pawn.health.hediffSet;
        foreach (var record in bodypart.GetNotMissingParts())
        {
            if (!record.def.tags.Contains(BodyPartTagDefOf.BloodPumpingSource))
            {
                continue;
            }

            dinfo.SetHitPart(record);
            break;
        }

        Pawn.TakeDamage(dinfo);
    }

    public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
    {
        base.Notify_PawnDied(dinfo, culprit);
        var pk = Pawn.kindDef.defName;
        // Log.Warning(pk);
        // if (pk.EqualsIgnoreCase("ra2_sovietdesolator")||pk.EqualsIgnoreCase("ra2_sovietteslatrooper")|| pk.EqualsIgnoreCase("ra2_alliedsiegecadre")|| pk.EqualsIgnoreCase("ra2_alliedsniper")||pk.EqualsIgnoreCase("ra2_AlliedChrono")|| pk.EqualsIgnoreCase("ra2_yuriyuri")|| pk.EqualsIgnoreCase("ra2_yuribrute")) {
        //      base.Pawn.apparel.DestroyAll();
        //  }
        var death = DefDatabase<SoundDef>.GetNamed($"{pk}_death", false);
        if (pk.EndsWith("Engineer"))
        {
            death = DefDatabase<SoundDef>.GetNamed("ra2_Engineer_death", false);
        }
        else if (pk.EndsWith("Chrono"))
        {
            death = DefDatabase<SoundDef>.GetNamed("ra2_Chrono_death", false);
        }

        if (death != null && Pawn.MapHeld != null)
        {
            death.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.MapHeld, true));
        }

        if (Pawn.Faction != Faction.OfPlayer)
        {
            return;
        }

        try
        {
            if (Pawn.kindDef.defName.StartsWith("ra2_Soviet"))
            {
                SoundDef.Named("ra2_SovietBarracks_UnitLost").PlayOneShotOnCamera();
            }
            else if (Pawn.kindDef.defName.StartsWith("ra2_Allied"))
            {
                SoundDef.Named("ra2_AlliedBarracks_UnitLost").PlayOneShotOnCamera();
            }
            else if (Pawn.kindDef.defName.StartsWith("ra2_Yuri"))
            {
                SoundDef.Named("ra2_YuriBarracks_UnitLost").PlayOneShotOnCamera();
            }
        }
        catch
        {
            // ignored
        }

        Pawn.Corpse.Destroy();
    }


    private ThingDef getDefaultGun(string def)
    {
        var gundef = "ra2_Gun_Conscript";
        switch (def)
        {
            case "ra2_SovietConscript":
                gundef = "ra2_Gun_Conscript";
                break;
            case "ra2_SovietTeslaTrooper":
                gundef = "ra2_Gun_TeslaTrooper";
                break;
            case "ra2_SovietDesolator":
                gundef = "ra2_Gun_Desolator";
                break;
            case "ra2_AlliedGI":
                gundef = "ra2_Gun_GI";
                break;
            case "ra2_AlliedSiegeCadre":
                gundef = "ra2_Gun_SiegeCadre";
                break;
            case "ra2_AlliedSniper":
                gundef = "ra2_Gun_Sniper";
                break;
            case "ra2_AlliedChrono":
                gundef = "ra2_Gun_Chrono";
                break;
            case "ra2_YuriInitiate":
                gundef = "ra2_Gun_Initiate";
                break;
            case "ra2_YuriBrute":
                gundef = "ra2_Gun_Brute";
                break;
            case "ra2_YuriYuri":
                gundef = "ra2_Gun_Yuri";
                break;
        }


        return DefDatabase<ThingDef>.GetNamed(gundef);
    }
}