using System;
using System.Collections.Generic;
using System.Text;

namespace ra2
{

    using HugsLib;
    using HugsLib.Utils;
    using System;
    using System.Collections.Generic;
    using Yuri;

    public class ModBaseRa2 : ModBase
        {
            public static Settings settings;
            private static List<Action> TickActions = new List<Action>();


            public static ModBaseRa2 Instance { get; private set; }
            public Controlstorage _controlstorage;

        public ModBaseRa2()
        {
            Instance = this;
        }
        public override void DefsLoaded()
            {
                settings = new Settings(base.Settings);
            }
            public static void RegisterTickAction(Action action)
            {
                TickActions.Add(action);
            }

            public override void Tick(int currentTick)
            {
                foreach (Action action in TickActions)
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









        public override string ModIdentifier =>
                "Ra2";
        }
    

}
