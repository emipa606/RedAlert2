using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

[StaticConstructorOnStartup]
public class WeatherObject_Thunder : WeatherEvent_LightningFlash
{
    private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

    private Mesh boltMesh;

    private IntVec3 strikeLoc = IntVec3.Invalid;

    public WeatherObject_Thunder(Map map) : base(map)
    {
    }

    public WeatherObject_Thunder(Map map, IntVec3 forcedStrikeLoc) : base(map)
    {
        strikeLoc = forcedStrikeLoc;
    }

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

    public override void WeatherEventDraw()
    {
        Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather),
            Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, LightningBrightness), 0);
    }
}