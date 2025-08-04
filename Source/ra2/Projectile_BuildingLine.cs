using RimWorld;
using UnityEngine;
using Verse;

namespace ra2;

public class Projectile_BuildingLine : Projectile_Custom
{
    private int age;

    private Matrix4x4 lineMatrix;
    public Vector3 lineScale = new(1f, 1f, 1f);

    private float LineBrightness
    {
        get
        {
            if (age <= 3)
            {
                return age / 3f;
            }

            return 1f - (age / (float)Rand.Range(15, 60));
        }
    }


    private void ComputeDrawingParameters()
    {
        var pos = ((origin + destination) / 2f) + new Vector3(0, 2f, 1);
        var z = (destination - origin).MagnitudeHorizontal();
        var s = new Vector3(1f, 1f, z);

        lineMatrix.SetTRS(pos, ExactRotation, s);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        Graphics.DrawMesh(MeshPool.plane10, lineMatrix,
            FadedMaterialPool.FadedVersionOf(def.DrawMatSingle, LineBrightness), 0);
        Comps_PostDraw();
    }

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        var map = Map;

        FleckMaker.ThrowMicroSparks(destination, map);

        base.Impact(hitThing, blockedByShield);
    }

    protected override void Tick()
    {
        if (age == 0)
        {
            ComputeDrawingParameters();
        }

        base.Tick();
        age++;
    }
}