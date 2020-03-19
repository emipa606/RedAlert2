using ra2.Yuri;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    // Token: 0x0200042C RID: 1068
    public class CompBarracks : ThingComp
    {

        public CompProperties_Barracks Props
        {
            get
            {
                return (CompProperties_Barracks)this.props;
            }
        }

        

        public override string CompInspectStringExtra()
        {
            string str;
            
            if (this.trainPawns.Count > 0)
            {
                PawnKindDef pkd = DefDatabase<PawnKindDef>.GetNamed(this.trainPawns[0],true);
                str = "BarracksTraining".Translate() +pkd.label+ ":" + ((this.ticks*1f)/(0.01*trainTimeCalculate(this.trainPawns[0]))).ToString("f2") + "%"+"\n"+"BarracksLeft".Translate()+"("+(this.trainPawns.Count-1)+")";
            }
            else {
            
                str = "BarracksEmpty".Translate();
            }
            return str;//+ base.CompInspectStringExtra();
        }


        // Token: 0x06001290 RID: 4752 RVA: 0x0008E4E3 File Offset: 0x0008C8E3
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
           
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.ticks = 0;
            this.trainPawns = new List<string>();
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            //Scribe_Values.Look<List<String>>(ref this.trainPawns, "trainPawns", new List<string>(), false);
            Scribe_Collections.Look<String>(ref this.trainPawns, true, "trainPawns",LookMode.Undefined, new object[0]);
        }

        // Token: 0x06001291 RID: 4753 RVA: 0x0008E500 File Offset: 0x0008C900
        public override void CompTick()
        {
            base.CompTick();
            
            CompPowerTrader compPowerTrader = ThingCompUtility.TryGetComp<CompPowerTrader>(this.parent);

                if (compPowerTrader != null && compPowerTrader.PowerOn)
                {
              //  Log.Warning("1");
                 if (this.trainPawns.Count > 0)
                 {
                  //  Log.Warning("2");
                    this.ticks++;

                    if (getPawnCost(this.trainPawns[0]) > 0f && this.ticks >= trainTimeCalculate(this.trainPawns[0]))
                    {
                        trainPawnDone(this.trainPawns[0]);
                    }

                 }
                 
               
                }
         
               //String firstPawn = trainPawns[0];

            

        }

        private float trainTimeCalculate(String pawn) {
            if (!Settings.useGameSpeed)
            {
                return 6 * getPawnCost(pawn);
            }
            else {
                switch (pawn)
                {
                    case "ra2_SovietConscript":
                        return 60f;
                    case "ra2_SovietTeslaTrooper":
                        return 120f;
                    case "ra2_SovietDesolator":
                        return 300f;
                    case "ra2_AlliedGI":
                        return 60f;
                    case "ra2_AlliedSiegeCadre":
                        return 120f;
                    case "ra2_AlliedSniper":
                        return 300f;
                    case "ra2_AlliedChrono":
                        return 600f;
                    case "ra2_AlliedTanya":
                        return 900f;
                    case "ra2_YuriInitiate":
                        return 60f;
                    case "ra2_YuriBrute":
                        return 240f;
                    case "ra2_YuriYuri":
                        return 540f;

                }
            }




            return 0f;
        }

        // Token: 0x04000B61 RID: 2913
        List<String> trainPawns = new List<string>();
        private int ticks=0;
        // Token: 0x04000B62 RID: 2914
        //  public Building Barracks;




        private void canTrain(CompRefuelable cr,String def,float amount) {

            if (cr != null && cr.Fuel >= amount)
            {
                SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed(this.parent.def.defName + "_Training", true));
                this.trainPawns.Add(def);
                cr.ConsumeFuel(amount);
            }
            else {
                SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed("ra2_Click", true));
            }
        }


        private void trainPawnDone(String def) {

            PawnGenerationRequest request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed(def, true), Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, true, 1f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null, null);
            Pawn item = PawnGenerator.GeneratePawn(request);
            Pawn_StoryTracker ps = item.story;
            ps.childhood = null;
            ps.adulthood = null;
            ps.traits.allTraits = new List<Trait>();
            ps.traits.GainTrait(new Trait(DefDatabase<TraitDef>.GetNamed("ra2_MakeSoldier", true)));
            ps.traits.GainTrait(new Trait(TraitDefOf.Psychopath));

            
            Pawn_WorkSettings pws = item.workSettings;
            pws.DisableAll();
            Pawn_PlayerSettings pps = item.playerSettings;
            pps.hostilityResponse = HostilityResponseMode.Attack;
           
            NameTriple triple = NameTriple.FromString(item.kindDef.label.Replace(" ", ""));
            item.Name = triple;
            item.inventory.DestroyAll();


            YuriSoldierMakeUp.tryMakeUp(item);



            SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed(this.parent.def.defName + "_UnitReady", true));


            IntVec3 loc = CellFinder.RandomClosewalkCellNear(this.parent.Position, this.parent.Map, 3, null);

            Pawn pp;

            if (this.trainPawns[0] != "ra2_AlliedTanya")
                pp = (Pawn)(GenSpawn.Spawn(item, loc, this.parent.Map, WipeMode.Vanish));
            else {
               // bool flag = true;
                if (getAllTanya().Count > 0) {
                    foreach (Pawn tanya in getAllTanya()) {
                        tanya.Destroy(DestroyMode.Vanish);
                    }

                }
                  pp = (Pawn)(GenSpawn.Spawn(getTanya(), loc, this.parent.Map, WipeMode.Vanish));
            }
        

            this.trainPawns.Remove(this.trainPawns[0]);
            this.ticks = 0;
        }

    

        private float getPawnCost(String def) {
            switch (def) {
                case "ra2_SovietConscript":
                    return 50f;
                case "ra2_SovietTeslaTrooper":
                    return 400f;
                case "ra2_SovietDesolator":
                    return 850f;
                case "ra2_AlliedGI":
                    return 100f;
                case "ra2_AlliedSiegeCadre":
                    return 500f;
                case "ra2_AlliedSniper":
                    return 700f;
                case "ra2_AlliedChrono":
                    return 1400f;
                case "ra2_AlliedTanya":
                    return 1500f;
                case "ra2_YuriInitiate":
                    return 150f;
                case "ra2_YuriBrute":
                    return 500f;
                case "ra2_YuriYuri":
                    return 1200f;

            }
            return 0f;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            String baDef = this.parent.def.defName;
            CompRefuelable cr = this.parent.TryGetComp<CompRefuelable>();

            if (baDef.Equals("ra2_SovietBarracks"))
            {
                yield return CAction(cr, "Soviet", "Conscript");
                yield return CAction(cr, "Soviet", "TeslaTrooper");
                yield return CAction(cr, "Soviet", "Desolator");

            } else if (baDef.Equals("ra2_YuriBarracks")) {
                yield return CAction(cr, "Yuri", "Initiate");
                yield return CAction(cr, "Yuri", "Brute");
                if (DefDatabase<ResearchProjectDef>.GetNamed("Ra2YuriPsychicTech", true).IsFinished)
                    yield return CAction(cr, "Yuri", "Yuri");
            }
            else
                 if (baDef.Equals("ra2_AlliedBarracks")) {
                yield return CAction(cr, "Allied", "GI");
                yield return CAction(cr, "Allied", "SiegeCadre");
                yield return CAction(cr, "Allied", "Sniper");
                yield return CAction(cr, "Allied", "Chrono");


                if (DefDatabase<ResearchProjectDef>.GetNamed("Ra2AlliedTanya", true).IsFinished)
                    yield return CAction(cr, "Allied", "Tanya");


            


            }
                yield break;
        }



        private Command_Action CAction(CompRefuelable cr, String faction, String def) {
            bool canClick = cr.Fuel>=getPawnCost("ra2_"+faction+def)?true:false;
            return new Command_Action
            {
                
                icon = ContentFinder<Texture2D>.Get("ra2/Things/Misc/Icon/ra2_" + faction +  def, true),
                disabled = !canClick,
                defaultDesc = getPawnCost("ra2_"+faction+def)+"$",
                defaultLabel = "ra2_Train".Translate() + DefDatabase<PawnKindDef>.GetNamed("ra2_"+faction+def, true).label,
                action = delegate
                {
                    canTrain(cr, "ra2_"+faction+def, getPawnCost("ra2_"+faction+def));
                }

            };
            
            }


        private Pawn getTanya()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed("ra2_AlliedTanya", true), Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, true, 1f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null, null, null, Gender.Female, null, null);
            Pawn item = PawnGenerator.GeneratePawn(request);

            Pawn_StoryTracker ps = item.story;
            HairDef hair = DefDatabase<HairDef>.GetNamed("Curly");
            ps.childhood = null;
            ps.adulthood = null;
            ps.traits.allTraits = new List<Trait>();
            ps.traits.GainTrait(new Trait(DefDatabase<TraitDef>.GetNamed("ra2_MakeSoldier", true)));
            ps.traits.GainTrait(new Trait(TraitDefOf.Psychopath));
            Pawn_WorkSettings pws = item.workSettings;
            pws.DisableAll();

            NameTriple triple = NameTriple.FromString(item.kindDef.label);
            triple.ResolveMissingPieces("Adams".Translate());
            item.Name = triple;

            Pawn_SkillTracker skt = item.skills;
            foreach (SkillRecord sr in skt.skills)
            {
                sr.Level = 20;
            }

            item.inventory.DestroyAll();
            ps.bodyType = BodyTypeDefOf.Female;
            ps.hairDef = hair;
            ps.hairColor = new UnityEngine.Color(1, 0.8f, 0);

            //st.SkinColor = new UnityEngine.Color(0.98f,0.76f,0.71f);
            ps.melanin = 0f;


            Pawn_EquipmentTracker pe = item.equipment;
            pe.Remove(pe.Primary);
            pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Gun_Tanya", true)));


            //item.story = st;
            return item;
        }

        private List<Pawn> getAllTanya() {
            List<Pawn> result = new List<Pawn>();
            
            foreach (Pawn ppod in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists) {
                if (ppod.kindDef.defName == "ra2_AlliedTanya") {
                    result.Add(ppod);
                }
            }
           
        

            return result;
        }
    

    }
}
