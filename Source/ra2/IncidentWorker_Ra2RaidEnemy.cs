using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Random = System.Random;

namespace ra2;

public static class IncidentWorker_Ra2RaidEnemy
{
    public static void TryExecuteWorker(IncidentParms parms)
    {
        var builder = new StringBuilder();
        if (parms.faction.def.defName.EqualsIgnoreCase("ra2_soviet"))
        {
            Find.MusicManagerPlay.ForcePlaySong(DefDatabase<SongDef>.GetNamed("ra2_soviet_music"), false);

            // Log.Warning("your points is :"+ parms.points);
            if (parms.points >= 600)
            {
                spawnSpecialPawn(parms, "ra2_SovietEngineer");
            }

            if (!(parms.points >= 3600))
            {
                return;
            }

            var apoTank = spawnSpecialPawn(parms, "ra2_ApoTank");
            DefDatabase<SoundDef>.GetNamed("ra2_apo_enter").PlayOneShotOnCamera((Map)parms.target);
            var text = "DangerousunitMsg".Translate(parms.faction.Name, apoTank.def.label);
            builder.AppendLine(text);

            Find.LetterStack.ReceiveLetter("Dangerousunit".Translate(), builder.ToString(), LetterDefOf.Death,
                apoTank);

            //  Messages.Message(text+apoTank,MessageTypeDefOf.PawnDeath);


            return;
        }

        if (parms.faction.def.defName.EqualsIgnoreCase("ra2_allied"))
        {
            Find.MusicManagerPlay.ForcePlaySong(DefDatabase<SongDef>.GetNamed("ra2_allied_music"), false);

            if (parms.points >= 600)
            {
                spawnSpecialPawn(parms, "ra2_AlliedEngineer");
            }

            if (!(parms.points >= 3600))
            {
                return;
            }

            var item = getTanya(parms);
            var et = item.equipment;
            var nr = new Random((int)parms.points);
            var result = nr.Next(100);
            et.AddEquipment((ThingWithComps)ThingMaker.MakeThing(
                DefDatabase<ThingDef>.GetNamed(result > 33 ? "ra2_Gun_Tanya" : "ra2_TanyaC4Bomb")));


            var loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, (Map)parms.target, 8);
            var pp = (Pawn)GenSpawn.Spawn(item, loc, (Map)parms.target, parms.spawnRotation);
            //Lord ld = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, false, false, true, false, true), (Map)parms.target);
            //ld.AddPawn(pp);
            var pl = new List<Pawn> { pp };
            parms.raidStrategy.Worker.MakeLords(parms, pl);

            DefDatabase<SoundDef>.GetNamed("ra2_tanya_enter").PlayOneShotOnCamera((Map)parms.target);

            var text = "DangerousunitMsg".Translate(parms.faction.Name, item.Name.ToString());
            builder.AppendLine(text);
            builder.AppendLine("TanyaWeaponUse".Translate() + ":" + DefDatabase<ThingDef>
                .GetNamed(result > 33 ? "ra2_Gun_Tanya" : "ra2_TanyaC4Bomb").label);

            Find.LetterStack.ReceiveLetter("Dangerousunit".Translate(), builder.ToString(), LetterDefOf.Death,
                pp);


            return;
        }

        if (!parms.faction.def.defName.EqualsIgnoreCase("ra2_yuri"))
        {
            return;
        }

        Find.MusicManagerPlay.ForcePlaySong(DefDatabase<SongDef>.GetNamed("ra2_yuri_music"), false);

        // Log.Warning("your points is :"+ parms.points);
        if (parms.points >= 500)
        {
            spawnSpecialPawn(parms, "ra2_YuriEngineer");
        }
    }


    private static Pawn spawnSpecialPawn(IncidentParms parms, string pawndef)
    {
        var request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed(pawndef), parms.faction,
            PawnGenerationContext.NonPlayer, -1, false, false, false, true, true, 1f, false, true, true,
            false);
        var item = PawnGenerator.GeneratePawn(request);
        var loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, (Map)parms.target, 8);
        var pp = (Pawn)GenSpawn.Spawn(item, loc, (Map)parms.target, parms.spawnRotation);
        var pl = new List<Pawn> { pp };
        parms.raidStrategy.Worker.MakeLords(parms, pl);
        // Lord ld = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, false, false, true, false, true), (Map)parms.target);
        //ld.AddPawn(pp);

        return pp;
    }


    private static Pawn getTanya(IncidentParms parms)
    {
        var request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed("ra2_AlliedTanya"), parms.faction,
            PawnGenerationContext.NonPlayer, -1, false, false, false, true, true, 1f, false, true, false, true,
            false, false, false, false, false, 0, 0, null, 1, null, null, null, null, null, null, null,
            Gender.Female);
        var item = PawnGenerator.GeneratePawn(request);

        var st = item.story;
        var hair = DefDatabase<HairDef>.GetNamed("Curly");
        var child = DefDatabase<BackstoryDef>.GetNamedSilentFail("YouthSoldier99");
        var old = DefDatabase<BackstoryDef>.GetNamedSilentFail("VeteranSoldier2");
        st.Adulthood = old;
        st.Childhood = child;

        var triple = NameTriple.FromString(item.kindDef.label);
        triple.ResolveMissingPieces("Adams".Translate());
        item.Name = triple;

        var skt = item.skills;
        foreach (var sr in skt.skills)
        {
            sr.Level = 20;
        }


        st.bodyType = BodyTypeDefOf.Female;
        st.hairDef = hair;
        st.HairColor = new Color(1, 0.8f, 0);

        //st.SkinColor = new UnityEngine.Color(0.98f,0.76f,0.71f);
        st.skinColorOverride = new Color(0.98f, 0.76f, 0.71f);


        //item.story = st;
        return item;
    }
}