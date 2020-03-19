using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace ra2
{
    public  static class IncidentWorker_Ra2RaidEnemy
    {




        public static bool TryExecuteWorker(IncidentParms parms)
        {

            StringBuilder builder = new StringBuilder();
            if (parms.faction.def.defName.EqualsIgnoreCase("ra2_soviet"))
            {
                Find.MusicManagerPlay.ForceStartSong(DefDatabase<SongDef>.GetNamed("ra2_soviet_music", true), false);

               // Log.Warning("your points is :"+ parms.points);
                if (parms.points >= 600) {
                    spawnSpecialPawn(parms, "ra2_SovietEngineer");
                }

                if (parms.points >= 3600)
                {
                    Pawn apoTank= spawnSpecialPawn(parms, "ra2_ApoTank");
                    SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed("ra2_apo_enter"), (Map)parms.target);
                    String text = Translator.Translate("DangerousunitMsg", new object[] { parms.faction.Name,apoTank.def.label});
                    builder.AppendLine(text);

                    Find.LetterStack.ReceiveLetter(Translator.Translate("Dangerousunit"), builder.ToString(), LetterDefOf.Death,apoTank);
                    
                  //  Messages.Message(text+apoTank,MessageTypeDefOf.PawnDeath);
                }


                return true;
            }else 
            if (parms.faction.def.defName.EqualsIgnoreCase("ra2_allied"))
            {
              
                Find.MusicManagerPlay.ForceStartSong(DefDatabase<SongDef>.GetNamed("ra2_allied_music", true), false);

                if (parms.points >= 600)
                {
                    spawnSpecialPawn(parms, "ra2_AlliedEngineer");
                }

                if (parms.points >= 3600)
                {
                    Pawn item = getTanya(parms);
                    Pawn_EquipmentTracker et = item.equipment;
                    Random nr = new Random((int)parms.points);
                    int result = nr.Next(100);
                    et.AddEquipment((ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(result > 33 ? "ra2_Gun_Tanya" : "ra2_TanyaC4Bomb", true)));


                    IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, (Map)parms.target, 8, null);
                    Pawn pp = (Pawn)(GenSpawn.Spawn(item, loc, (Map)parms.target, parms.spawnRotation, WipeMode.Vanish,false));
                    //Lord ld = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, false, false, true, false, true), (Map)parms.target);
                    //ld.AddPawn(pp);
                    List<Pawn> pl = new List<Pawn>();
                    pl.Add(pp);
                    parms.raidStrategy.Worker.MakeLords(parms, pl);

                    SoundStarter.PlayOneShotOnCamera(DefDatabase<SoundDef>.GetNamed("ra2_tanya_enter"), (Map)parms.target);

                    String text = Translator.Translate("DangerousunitMsg", new object[] { parms.faction.Name, item.Name });
                    builder.AppendLine(text);
                    builder.AppendLine("TanyaWeaponUse".Translate()+":"+ DefDatabase<ThingDef>.GetNamed(result > 33 ? "ra2_Gun_Tanya" : "ra2_TanyaC4Bomb", true).label);

                    Find.LetterStack.ReceiveLetter(Translator.Translate("Dangerousunit"), builder.ToString(), LetterDefOf.Death, pp);
                }


                return true;
            }else
                if (parms.faction.def.defName.EqualsIgnoreCase("ra2_yuri"))
            {
               
                Find.MusicManagerPlay.ForceStartSong(DefDatabase<SongDef>.GetNamed("ra2_yuri_music", true), false);

                // Log.Warning("your points is :"+ parms.points);
                if (parms.points >= 500)
                {
                    spawnSpecialPawn(parms, "ra2_YuriEngineer");
                }
                return true;
            }



            return false;
        }



        private static Pawn spawnSpecialPawn(IncidentParms parms,String pawndef) {
                PawnGenerationRequest request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed(pawndef, true), parms.faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, true, 1f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null, null);
                Pawn item = PawnGenerator.GeneratePawn(request);
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, (Map)parms.target, 8, null);
                Pawn pp = (Pawn)(GenSpawn.Spawn(item, loc, (Map)parms.target, parms.spawnRotation, WipeMode.Vanish, false));
                List<Pawn> pl = new List<Pawn>();
                pl.Add(pp);
                parms.raidStrategy.Worker.MakeLords(parms,pl);
               // Lord ld = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, false, false, true, false, true), (Map)parms.target);
                //ld.AddPawn(pp);

                return pp;
               
                
                
        }



        private static Pawn getTanya(IncidentParms parms) {
            PawnGenerationRequest request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed("ra2_AlliedTanya", true), parms.faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, true, 1f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null, null, null, Gender.Female, null, null);
            Pawn item = PawnGenerator.GeneratePawn(request);
            
            Pawn_StoryTracker st = item.story;
            HairDef hair = DefDatabase<HairDef>.GetNamed("Curly");
            Dictionary<String,Backstory> bsdb =  BackstoryDatabase.allBackstories;
            Backstory child = bsdb.TryGetValue("YouthSoldier99");
            Backstory old = bsdb.TryGetValue("VeteranSoldier2");
            st.adulthood = old;
            st.childhood = child;

            NameTriple triple = NameTriple.FromString(item.kindDef.label);
            triple.ResolveMissingPieces("Adams".Translate());
            item.Name = triple;

            Pawn_SkillTracker skt = item.skills;
            foreach (SkillRecord sr in skt.skills) {
                sr.Level = 20;
            }

              
            st.bodyType = BodyTypeDefOf.Female;
            st.hairDef = hair;
            st.hairColor = new UnityEngine.Color(1,0.8f,0);

            //st.SkinColor = new UnityEngine.Color(0.98f,0.76f,0.71f);
            st.melanin = 0f;





            //item.story = st;
            return item;
        }


    }
}
