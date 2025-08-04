using RimWorld;
using UnityEngine;
using Verse;

namespace ra2;

public class CompSovietPower : CompPowerPlant
{
    private const int BladeCount = 9;
    private const float BladeOffset = 2.36f;

    private const float SpinRateFactor = 0.006666667f;
    private readonly string path = "ra2/Projectile/Lighting/";

    //public static readonly Material PrismMat = MaterialPool.MatFrom("Things/Building/Power/WatermillGenerator/WatermillGeneratorBlades");
    //public Material PrismMat;
    private bool cacheDirty = true;
    private float spinPosition;
    private float spinRate = 1f;

    private int ticktime;


    public new CompProperties_Power Props => (CompProperties_Power)props;


    public override void CompTick()
    {
        base.CompTick();

        ticktime++;
    }


    private Material getMat()
    {
        var speed = 4;
        var index = ticktime / speed % 4;
        if (ticktime >= (4 * speed) - 1)
        {
            ticktime = 0;
        }

        var result = MaterialPool.MatFrom($"{path}Lighting{index}");


        return result;
    }


    public override void PostDraw()
    {
        base.PostDraw();


        var vector = parent.TrueCenter() + new Vector3(0, 2f, -0.6f);

        var f = spinPosition + (6.283185f * 1 / 9f);
        var x = Mathf.Abs(4f * Mathf.Sin(f));
        _ = f % 6.283185f < 3.141593f;
        _ = new Vector2(x, 1f);
        var s = new Vector3(0.8f, 1f, 1.1f);
        var matrix = new Matrix4x4();
        matrix.SetTRS(vector, parent.Rotation.AsQuat, s);
        if (PowerOutput > 0.01)
        {
            Graphics.DrawMesh(MeshPool.plane10Flip, matrix, getMat(), 0);
        }


        // Log.Warning("now index is "+getMat());
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        spinPosition = Rand.Range(0f, 15f);
        ticktime = 0;
    }
}