using System.Collections.Generic;
using ra2.Yuri;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

public class CompBarracks : ThingComp
{
    private int ticks;

    private List<string> trainPawns = new List<string>();

    public CompProperties_Barracks Props => (CompProperties_Barracks)props;


    public override string CompInspectStringExtra()
    {
        string str;

        if (trainPawns.Count > 0)
        {
            var pkd = DefDatabase<PawnKindDef>.GetNamed(trainPawns[0]);
            str =
                $"{"BarracksTraining".Translate() + pkd.label + ":" + (ticks * 1f / (0.01 * trainTimeCalculate(trainPawns[0]))).ToString("f2") + "%" + "\n" + "BarracksLeft".Translate() + "("}{trainPawns.Count - 1})";
        }
        else
        {
            str = "BarracksEmpty".Translate();
        }

        return str; //+ base.CompInspectStringExtra();
    }


    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        ticks = 0;
        trainPawns = new List<string>();
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref ticks, "ticks");
        //Scribe_Values.Look<List<String>>(ref this.trainPawns, "trainPawns", new List<string>(), false);
        Scribe_Collections.Look(ref trainPawns, true, "trainPawns");
    }

    public override void CompTick()
    {
        base.CompTick();

        var compPowerTrader = parent.TryGetComp<CompPowerTrader>();

        if (compPowerTrader is not { PowerOn: true })
        {
            return;
        }

        //  Log.Warning("1");
        if (trainPawns.Count <= 0)
        {
            return;
        }

        //  Log.Warning("2");
        ticks++;

        if (getPawnCost(trainPawns[0]) > 0f && ticks >= trainTimeCalculate(trainPawns[0]))
        {
            trainPawnDone(trainPawns[0]);
        }

        //String firstPawn = trainPawns[0];
    }

    private float trainTimeCalculate(string pawn)
    {
        if (!Settings.useGameSpeed)
        {
            return 6 * getPawnCost(pawn);
        }

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


        return 0f;
    }
    //  public Building Barracks;


    private void canTrain(CompRefuelable cr, string def, float amount)
    {
        if (cr != null && cr.Fuel >= amount)
        {
            DefDatabase<SoundDef>.GetNamed($"{parent.def.defName}_Training").PlayOneShotOnCamera();
            trainPawns.Add(def);
            cr.ConsumeFuel(amount);
        }
        else
        {
            DefDatabase<SoundDef>.GetNamed("ra2_Click").PlayOneShotOnCamera();
        }
    }


    private void trainPawnDone(string def)
    {
        var request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed(def), Faction.OfPlayer,
            PawnGenerationContext.NonPlayer, -1, false, false, false, true, true, 1f, false, true, allowFood: true,
            allowAddictions: false);
        var item = PawnGenerator.GeneratePawn(request);
        var ps = item.story;
        ps.Childhood = null;
        ps.Adulthood = null;
        ps.traits.allTraits = new List<Trait>();
        ps.traits.GainTrait(new Trait(DefDatabase<TraitDef>.GetNamed("ra2_MakeSoldier")));
        ps.traits.GainTrait(new Trait(TraitDefOf.Psychopath));


        var pws = item.workSettings;
        pws.DisableAll();
        var pps = item.playerSettings;
        pps.hostilityResponse = HostilityResponseMode.Attack;

        var triple = NameTriple.FromString(item.kindDef.label.Replace(" ", ""));
        item.Name = triple;
        item.inventory.DestroyAll();


        YuriSoldierMakeUp.tryMakeUp(item);


        DefDatabase<SoundDef>.GetNamed($"{parent.def.defName}_UnitReady").PlayOneShotOnCamera();


        var loc = CellFinder.RandomClosewalkCellNear(parent.Position, parent.Map, 3);

        Pawn unused;

        if (trainPawns[0] != "ra2_AlliedTanya")
        {
            unused = (Pawn)GenSpawn.Spawn(item, loc, parent.Map);
        }
        else
        {
            // bool flag = true;
            if (getAllTanya().Count > 0)
            {
                foreach (var tanya in getAllTanya())
                {
                    tanya.Destroy();
                }
            }

            unused = (Pawn)GenSpawn.Spawn(getTanya(), loc, parent.Map);
        }


        trainPawns.Remove(trainPawns[0]);
        ticks = 0;
    }


    private float getPawnCost(string def)
    {
        switch (def)
        {
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
        var baDef = parent.def.defName;
        var cr = parent.TryGetComp<CompRefuelable>();

        switch (baDef)
        {
            case "ra2_SovietBarracks":
                yield return CAction(cr, "Soviet", "Conscript");
                yield return CAction(cr, "Soviet", "TeslaTrooper");
                yield return CAction(cr, "Soviet", "Desolator");
                break;
            case "ra2_YuriBarracks":
            {
                yield return CAction(cr, "Yuri", "Initiate");
                yield return CAction(cr, "Yuri", "Brute");
                if (DefDatabase<ResearchProjectDef>.GetNamed("Ra2YuriPsychicTech").IsFinished)
                {
                    yield return CAction(cr, "Yuri", "Yuri");
                }

                break;
            }
            case "ra2_AlliedBarracks":
            {
                yield return CAction(cr, "Allied", "GI");
                yield return CAction(cr, "Allied", "SiegeCadre");
                yield return CAction(cr, "Allied", "Sniper");
                yield return CAction(cr, "Allied", "Chrono");


                if (DefDatabase<ResearchProjectDef>.GetNamed("Ra2AlliedTanya").IsFinished)
                {
                    yield return CAction(cr, "Allied", "Tanya");
                }

                break;
            }
        }
    }


    private Command_Action CAction(CompRefuelable cr, string faction, string def)
    {
        var canClick = cr.Fuel >= getPawnCost($"ra2_{faction}{def}");
        return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get($"ra2/Things/Misc/Icon/ra2_{faction}{def}"),
            disabled = !canClick,
            defaultDesc = $"{getPawnCost($"ra2_{faction}{def}")}$",
            defaultLabel =
                "ra2_Train".Translate() + DefDatabase<PawnKindDef>.GetNamed($"ra2_{faction}{def}").label,
            action = delegate { canTrain(cr, $"ra2_{faction}{def}", getPawnCost($"ra2_{faction}{def}")); }
        };
    }


    private Pawn getTanya()
    {
        var request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed("ra2_AlliedTanya"),
            Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, true, true, 1f,
            false, true, allowFood: true, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false,
            forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0,
            biocodeApparelChance: 0, extraPawnForExtraRelationChance: null, relationWithExtraPawnChanceFactor: 1,
            validatorPreGear: null, validatorPostGear: null, forcedTraits: null, prohibitedTraits: null,
            minChanceToRedressWorldPawn: null, fixedBiologicalAge: null,
            fixedChronologicalAge: null, fixedGender: Gender.Female);
        var item = PawnGenerator.GeneratePawn(request);

        var ps = item.story;
        var hair = DefDatabase<HairDef>.GetNamed("Curly");
        ps.Childhood = null;
        ps.Adulthood = null;
        ps.traits.allTraits = new List<Trait>();
        ps.traits.GainTrait(new Trait(DefDatabase<TraitDef>.GetNamed("ra2_MakeSoldier")));
        ps.traits.GainTrait(new Trait(TraitDefOf.Psychopath));
        var pws = item.workSettings;
        pws.DisableAll();

        var triple = NameTriple.FromString(item.kindDef.label);
        triple.ResolveMissingPieces("Adams".Translate());
        item.Name = triple;

        var skt = item.skills;
        foreach (var sr in skt.skills)
        {
            sr.Level = 20;
        }

        item.inventory.DestroyAll();
        ps.bodyType = BodyTypeDefOf.Female;
        ps.hairDef = hair;
        ps.HairColor = new Color(1, 0.8f, 0);

        //st.SkinColor = new UnityEngine.Color(0.98f,0.76f,0.71f);
        ps.skinColorOverride = new Color(0.98f, 0.76f, 0.71f);


        var pe = item.equipment;
        pe.Remove(pe.Primary);
        pe.AddEquipment((ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("ra2_Gun_Tanya")));


        //item.story = st;
        return item;
    }

    private List<Pawn> getAllTanya()
    {
        var result = new List<Pawn>();

        foreach (var ppod in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
        {
            if (ppod.kindDef.defName == "ra2_AlliedTanya")
            {
                result.Add(ppod);
            }
        }


        return result;
    }
}