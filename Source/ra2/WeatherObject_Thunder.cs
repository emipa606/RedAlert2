using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    // Token: 0x0200045A RID: 1114
    [StaticConstructorOnStartup]
    public class WeatherObject_Thunder : WeatherEvent_LightningFlash
    {
        // Token: 0x04000BFB RID: 3067
        private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

        // Token: 0x04000BFA RID: 3066
        private Mesh boltMesh;

        // Token: 0x04000BF9 RID: 3065
        private IntVec3 strikeLoc = IntVec3.Invalid;

        // Token: 0x06001378 RID: 4984 RVA: 0x00094758 File Offset: 0x00092B58
        public WeatherObject_Thunder(Map map) : base(map)
        {
        }

        // Token: 0x06001379 RID: 4985 RVA: 0x0009476C File Offset: 0x00092B6C
        public WeatherObject_Thunder(Map map, IntVec3 forcedStrikeLoc) : base(map)
        {
            strikeLoc = forcedStrikeLoc;
        }

        // Token: 0x0600137A RID: 4986 RVA: 0x00094788 File Offset: 0x00092B88
        public override void FireEvent()
        {
            base.FireEvent();
            if (!strikeLoc.IsValid)
            {
                strikeLoc = CellFinderLoose.RandomCellWith(sq => sq.Standable(map) && !map.roofGrid.Roofed(sq), map);
            }

            boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            if (!strikeLoc.Fogged(map))
            {
                GenExplosion.DoExplosion(strikeLoc, map, 8f, DefDatabase<DamageDef>.GetNamed("ApoDamage"), null, 1000);
                var loc = strikeLoc.ToVector3Shifted();
                for (var i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(loc, map, 1.5f);
                    FleckMaker.ThrowMicroSparks(loc, map);
                    FleckMaker.ThrowLightningGlow(loc, map, 1.5f);
                }
            }

            var info = SoundInfo.InMap(new TargetInfo(strikeLoc, map));
            SoundDef.Named("ra2_StormAtk").PlayOneShot(info);
        }

        // Token: 0x0600137B RID: 4987 RVA: 0x00094899 File Offset: 0x00092C99
        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather),
                Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, LightningBrightness), 0);
        }
    }
}