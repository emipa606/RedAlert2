using System;
using System.Collections.Generic;
using HugsLib;
using HugsLib.Utils;
using ra2.Yuri;

namespace ra2;

public class ModBaseRa2 : ModBase
{
    public static Settings settings;
    private static readonly List<Action> TickActions = [];
    public Controlstorage _controlstorage;

    public ModBaseRa2()
    {
        Instance = this;
    }


    public static ModBaseRa2 Instance { get; private set; }


    public override string ModIdentifier =>
        "Ra2";

    public override void DefsLoaded()
    {
        settings = new Settings(Settings);
    }

    public static void RegisterTickAction(Action action)
    {
        TickActions.Add(action);
    }

    public override void Tick(int currentTick)
    {
        foreach (var action in TickActions)
        {
            action();
        }

        TickActions.Clear();
    }

    public override void WorldLoaded()
    {
        _controlstorage = UtilityWorldObjectManager.GetUtilityWorldObject<Controlstorage>();
        base.WorldLoaded();
    }
}