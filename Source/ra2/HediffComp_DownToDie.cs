namespace ra2
{
    using RimWorld;
    using System;
    using Verse;
    using Verse.Sound;

    public class HediffComp_DownToDie : HediffComp
    {
   

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);



           
             {

                Pawn_EquipmentTracker pe = base.Pawn.equipment;
                Pawn_ApparelTracker pa = base.Pawn.apparel;
                if (base.Pawn.Faction == Faction.OfPlayer)
                {
                  
                    if (base.Pawn.kindDef.defName == "ra2_AlliedTanya")
                    {
                       
                    }
                   else if (pe.Primary==null|| pe.Primary.def != getDefaultGun(base.Pawn.kindDef.defName)) {

                        
                      



                       
                            pe.Remove(pe.Primary);
                            pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(getDefaultGun(base.Pawn.kindDef.defName)));



                    }

                    if (base.Pawn.kindDef.defName == "ra2_AlliedChrono")
                    {
                        bool hasHat = false;
                        foreach (Apparel ap in base.Pawn.apparel.WornApparel)
                        {
                            if (ap.def.defName == "ra2_Hat_Chrono")
                            {
                                hasHat = true;
                                break;
                            }
                        }
                        if (!hasHat)
                        {
                            base.Pawn.apparel.Wear((Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Hat_Chrono", true)));
                        }
                    }

                    if (pa.WornApparel == null) {
                        pa.Wear((Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Belt",true)));
                    }

                        bool hasBelt = false;
                        foreach (Apparel ap in pa.WornApparel) {
                            if (ap.def.defName.Equals("ra2_Belt")) {
                                hasBelt = true;
                                break;
                            }
                        }
                        if (!hasBelt) {
                        pa.Wear((Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Belt", true)));
                    }
                    
                }

            }

            if (base.Pawn.apparel.WornApparel.Find(x => x.TryGetComp<CompDownToDie>() != null) == null)
            {
                base.Pawn.health.RemoveHediff(base.parent);
            }

        
            if ((base.Pawn.Downed && !base.Pawn.Dead)||base.Pawn.IsPrisoner) {
                //Apparel ap = base.Pawn.apparel.WornApparel.Find(x => x.TryGetComp<CompDownToDie>() != null);
                //ap.Destroy(DestroyMode.Vanish);
                //base.Pawn.apparel.Remove(ap);
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Crush, 100, 0,0, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown);
                HediffSet bodypart = base.Pawn.health.hediffSet;
                foreach (BodyPartRecord record in bodypart.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined))
                {
                    if (record.def.tags.Contains(BodyPartTagDefOf.BloodPumpingSource))
                    {

                        dinfo.SetHitPart(record);
                        break;
                    }
                }

                    base.Pawn.TakeDamage(dinfo);
                
            }  
          
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            string pk = base.Pawn.kindDef.defName;
           // Log.Warning(pk);
           // if (pk.EqualsIgnoreCase("ra2_sovietdesolator")||pk.EqualsIgnoreCase("ra2_sovietteslatrooper")|| pk.EqualsIgnoreCase("ra2_alliedsiegecadre")|| pk.EqualsIgnoreCase("ra2_alliedsniper")||pk.EqualsIgnoreCase("ra2_AlliedChrono")|| pk.EqualsIgnoreCase("ra2_yuriyuri")|| pk.EqualsIgnoreCase("ra2_yuribrute")) {
          //      base.Pawn.apparel.DestroyAll();
          //  }
            SoundDef death = DefDatabase<SoundDef>.GetNamed(pk+"_death",false);
            if (pk.EndsWith("Engineer")) {
                death = DefDatabase<SoundDef>.GetNamed("ra2_Engineer_death", false);
            }else if (pk.EndsWith("Chrono"))
            {
                death = DefDatabase<SoundDef>.GetNamed("ra2_Chrono_death", false);

            }

            if (death != null && this.Pawn.MapHeld!=null)
            {
                SoundStarter.PlayOneShot(death, new TargetInfo(base.Pawn.Position, this.Pawn.MapHeld, true));
                
            }
            if (base.Pawn.Faction == Faction.OfPlayer)
            {
                try {
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

                } catch (Exception e) { }
                base.Pawn.Corpse.Destroy(DestroyMode.Vanish);

            }
           
         
  
        }


        private ThingDef getDefaultGun(string def) {
            string gundef= "ra2_Gun_Conscript";
            switch (def) {
                case "ra2_SovietConscript" :
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
                


            return DefDatabase<ThingDef>.GetNamed(gundef,true);
        }



        public HediffCompProperties_DownToDie Props =>
            ((HediffCompProperties_DownToDie)base.props);

      
}

}
