using HugsLib.Settings;
using Verse;

namespace ra2
{
    public class Settings
    {
        public static SettingHandle<int> droppodCargoDropPercentage;
        public static SettingHandle<bool> useGameSpeed;

        public Settings(ModSettingsPack settings)
        {
            droppodCargoDropPercentage = settings.GetHandle("droppodCargoDropPercentage",
                "DroppodCargoDropPercentage".Translate(), "DroppodCargoDropPercentageDesc".Translate(), 30, AtLeast());
            useGameSpeed = settings.GetHandle<bool>("useGameSpeed", "RA2UseGameSpeed".Translate(),
                "RA2UseGameSpeedDesc".Translate());
        }

        private static SettingHandle.ValueIsValid AtLeast()
        {
            return value => int.TryParse(value, out var num) && num <= 100 && num >= 0;
        }
    }
}