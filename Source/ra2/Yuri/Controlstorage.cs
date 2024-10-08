﻿using System.Collections.Generic;
using HugsLib.Utils;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace ra2.Yuri;

public class Controlstorage : UtilityWorldObject, IExposable

{
    public Dictionary<Pawn, Faction> BeControlerAndBeforeFaction = new Dictionary<Pawn, Faction>();
    public Dictionary<Pawn, Pawn> BeControlerAndControler = new Dictionary<Pawn, Pawn>();

    public Dictionary<Pawn, Pawn> ControlerAndBeControler = new Dictionary<Pawn, Pawn>();

    public override void ExposeData()
    {
        base.ExposeData();
        //Scribe_Deep.Look<Pawn, Pawn>(ref this.ControlerAndBeControler, "ControlerAndBeControler", LookMode.Value, LookMode.Deep);
        //Scribe_Collections.Look(ref _store, "store",LookMode.Value, LookMode.Deep, ref _idWorkingList, ref _extendedPawnDataWorkingList);
        Scribe_Collections.Look(ref ControlerAndBeControler, "ControlerAndBeControler", LookMode.Reference,
            LookMode.Reference);
        Scribe_Collections.Look(ref BeControlerAndControler, "BeControlerAndControler", LookMode.Reference,
            LookMode.Reference);
        Scribe_Collections.Look(ref BeControlerAndBeforeFaction, "BeControlerAndBeforeFaction", LookMode.Reference,
            LookMode.Reference);
    }

    //清除二人关系
    public void remove2pawnRelation(Pawn controler, Pawn becontroler)
    {
        var check1 = false;
        var check2 = false;
        if (ControlerAndBeControler.ContainsKey(controler) &&
            ControlerAndBeControler.TryGetValue(controler) == becontroler)
        {
            check1 = true;
        }

        if (BeControlerAndControler.ContainsKey(becontroler) &&
            BeControlerAndControler.TryGetValue(becontroler) == controler)
        {
            check2 = true;
        }

        if (!check1 || !check2)
        {
            return;
        }

        ControlerAndBeControler.Remove(controler);
        BeControlerAndControler.Remove(becontroler);
        becontroler.SetFaction(BeControlerAndBeforeFaction.TryGetValue(becontroler));
        BeControlerAndBeforeFaction.Remove(becontroler);
        SoundDef.Named("ra2_yuri_nocontrol").PlayOneShot(becontroler);
        if (becontroler.Dead)
        {
            return;
        }


        if (becontroler.AnimalOrWildMan() || becontroler.Faction == Faction.OfPlayer)
        {
            return;
        }

        var becontrolers = new List<Pawn> { becontroler };
        if (becontroler.GetLord() != null && becontroler.Faction == Faction.OfPlayer)
        {
            becontroler.GetLord().Cleanup();
        }
        else if (!becontroler.AnimalOrWildMan())
        {
            _ = LordMaker.MakeNewLord(controler.Faction,
                new LordJob_AssaultColony(controler.Faction), becontroler.Map, becontrolers);
        }
    }

    //添加二人关系
    public void add2pawnRelation(Pawn controler, Pawn becontroler)
    {
        ControlerAndBeControler.Add(controler, becontroler);
        BeControlerAndControler.Add(becontroler, controler);
        BeControlerAndBeforeFaction.Add(becontroler, becontroler.Faction);
    }

    //检查是否能被控制
    public bool canBeControled(Pawn becontroler)
    {
        if (becontroler.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
        {
            return false;
        }

        var result = becontroler.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 1.0f);
        if (result == 0)
        {
            return false;
        }

        return true;
    }

    //检查控制者是否存在,并看看被控制者是否存在
    public void checkControlerExist(Pawn controler)
    {
        if (!ControlerAndBeControler.ContainsKey(controler))
        {
            return;
        }

        var becontroler = ControlerAndBeControler.TryGetValue(controler);
        if (!controler.Spawned || controler.Dead)
        {
            remove2pawnRelation(controler, becontroler);
        }

        if (becontroler is not { Spawned: true } || becontroler.Dead)
        {
            remove2pawnRelation(controler, becontroler);
        }
    }

    //检查被控制者是否存在
    public void checkBeControlerExist(Pawn becontroler)
    {
        if (!BeControlerAndControler.ContainsKey(becontroler))
        {
            return;
        }

        becontroler.Map.overlayDrawer.DrawOverlay(becontroler, OverlayTypes.QuestionMark);
        var controler = BeControlerAndControler.TryGetValue(becontroler);
        if (!becontroler.Spawned || becontroler.Dead)
        {
            remove2pawnRelation(controler, becontroler);
        }

        if (controler is not { Spawned: true } || controler.Dead)
        {
            remove2pawnRelation(controler, becontroler);
        }
    }

    //检查控制者是否有控制人
    public bool hasControledSomeone(Pawn controler)
    {
        return controler != null && ControlerAndBeControler.ContainsKey(controler);
    }

    //检查即将被控制者是否已经有所属
    public bool hasBecontrolerAlreadyBecontroled(Pawn becontroler)
    {
        return becontroler != null && BeControlerAndControler.ContainsKey(becontroler);
    }

    //控制启动
    public bool ControlSomeone(Pawn controler, Pawn becontroler)
    {
        if (controler.Faction == becontroler.Faction)
        {
            return false;
        }

        if (!canBeControled(becontroler))
        {
            return false;
        }

        if (hasBecontrolerAlreadyBecontroled(becontroler))
        {
            return false;
        }

        if (hasControledSomeone(controler))
        {
            remove2pawnRelation(controler, ControlerAndBeControler.TryGetValue(controler));
        }

        add2pawnRelation(controler, becontroler);
        if (becontroler.RaceProps.IsFlesh)
        {
            YuriSoldierMakeUp.controlOtherFactionGoodWill(controler, becontroler);

            var tmp = becontroler.kindDef;
            becontroler.SetFaction(controler.Faction);
            becontroler.kindDef = tmp;

            if (becontroler.drafter != null)
            {
                becontroler.drafter.Drafted = true;
            }


            if (becontroler.AnimalOrWildMan())
            {
                return true;
            }

            var becontrolers = new List<Pawn> { becontroler };
            var tmpf = BeControlerAndBeforeFaction.TryGetValue(becontroler);

            if (becontroler.GetLord() != null)
            {
                becontroler.GetLord().Cleanup();
            }

            _ = LordMaker.MakeNewLord(tmpf,
                new LordJob_DefendBase(becontroler.Faction, becontroler.Position), becontroler.Map,
                becontrolers);
        }
        else
        {
            //TODO
            return false;
        }

        return true;
    }
}