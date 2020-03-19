using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    // Token: 0x0200045A RID: 1114
    [StaticConstructorOnStartup]
    public class WeatherObject_Thunder : WeatherEvent_LightningFlash
    {
        // Token: 0x06001378 RID: 4984 RVA: 0x00094758 File Offset: 0x00092B58
        public WeatherObject_Thunder(Map map) : base(map)
        {
        }

        // Token: 0x06001379 RID: 4985 RVA: 0x0009476C File Offset: 0x00092B6C
        public WeatherObject_Thunder(Map map, IntVec3 forcedStrikeLoc) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        // Token: 0x0600137A RID: 4986 RVA: 0x00094788 File Offset: 0x00092B88
        public override void FireEvent()
        {
            base.FireEvent();
            if (!this.strikeLoc.IsValid)
            {
                this.strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(this.map) && !this.map.roofGrid.Roofed(sq), this.map, 1000);
            }
            this.boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            if (!this.strikeLoc.Fogged(this.map))
            {
                GenExplosion.DoExplosion(this.strikeLoc, this.map, 8f, DefDatabase<DamageDef>.GetNamed("ApoDamage", true), null, 1000, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                Vector3 loc = this.strikeLoc.ToVector3Shifted();
                for (int i = 0; i < 4; i++)
                {
                    MoteMaker.ThrowSmoke(loc, this.map, 1.5f);
                    MoteMaker.ThrowMicroSparks(loc, this.map);
                    MoteMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
            SoundDef.Named("ra2_StormAtk").PlayOneShot(info);
        }

        // Token: 0x0600137B RID: 4987 RVA: 0x00094899 File Offset: 0x00092C99
        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherObject_Thunder.LightningMat, base.LightningBrightness), 0);
        }

        // Token: 0x04000BF9 RID: 3065
        private IntVec3 strikeLoc = IntVec3.Invalid;

        // Token: 0x04000BFA RID: 3066
        private Mesh boltMesh;

        // Token: 0x04000BFB RID: 3067
        private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
    }
}
