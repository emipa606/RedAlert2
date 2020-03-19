using System;
using System.Collections.Generic;
using System.Text;

namespace ra2
{
 
        using HugsLib.Settings;
        using System;
        using Verse;

        public class Settings
        {

            public static SettingHandle<int> droppodCargoDropPercentage;
            public static SettingHandle<bool> useGameSpeed;

            public Settings(ModSettingsPack settings)
            {
             droppodCargoDropPercentage = settings.GetHandle<int>("droppodCargoDropPercentage", "DroppodCargoDropPercentage".Translate(), "DroppodCargoDropPercentageDesc".Translate(), 30, AtLeast(), null);
             useGameSpeed = settings.GetHandle<bool>("useGameSpeed", "RA2UseGameSpeed".Translate(), "RA2UseGameSpeedDesc".Translate(), false);
        }

            private static SettingHandle.ValueIsValid AtLeast() =>
                delegate (string value) {
                    int num;
                    return (int.TryParse(value, out num) && (num <=100)&&(num>=0));
                };


         
        }
    

}
