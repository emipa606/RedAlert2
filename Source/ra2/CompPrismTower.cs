using RimWorld;
using UnityEngine;
using Verse;

namespace ra2;

[StaticConstructorOnStartup]
public class CompPrismTower : ThingComp
{
    private const int BladeCount = 9;
    private const float BladeOffset = 2.36f;

    private const float SpinRateFactor = 0.006666667f;
    private readonly string path = "ra2/Prism tower/";

    //public static readonly Material PrismMat = MaterialPool.MatFrom("Things/Building/Power/WatermillGenerator/WatermillGeneratorBlades");
    //public Material PrismMat;
    private bool cacheDirty = true;
    private float spinPosition;
    private float spinRate = 1f;

    private int ticktime;


    private CompProperties_PrismTower Props => (CompProperties_PrismTower)props;

    private void ClearCache()
    {
        cacheDirty = true;
    }


    public override void CompTick()
    {
        base.CompTick();

        ticktime++;
    }


    private Material getMat()
    {
        var speed = Props.speed;
        var index = ticktime / speed % 12;
        if (ticktime >= (12 * speed) - 1)
        {
            ticktime = 0;
        }

        var result = MaterialPool.MatFrom(path + index);


        return result;
    }


    public override void PostDraw()
    {
        base.PostDraw();


        var vector = parent.TrueCenter() + new Vector3(0, 3f, Props.size.x / 1.5f);

        var f = spinPosition + (6.283185f * 1 / 9f);
        var x = Mathf.Abs(4f * Mathf.Sin(f));
        _ = f % 6.283185f < 3.141593f;
        _ = new Vector2(x, 1f);
        var s = new Vector3(Props.size.x, 1f, Props.size.y);
        var matrix = new Matrix4x4();
        matrix.SetTRS(vector, parent.Rotation.AsQuat, s);
        if (hasPower() && !isATK())
        {
            Graphics.DrawMesh(MeshPool.plane10Flip, matrix, getMat(), 0);
        }
        else
        {
            Graphics.DrawMesh(MeshPool.plane10Flip, matrix, MaterialPool.MatFrom($"{path}0"), 0);
        }


        // Log.Warning("now index is "+getMat());
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        spinPosition = Rand.Range(0f, 15f);
        ticktime = 0;
    }


    private bool isATK()
    {
        return ((Building_TurretGun)parent).CurrentTarget != null;
    }

    private bool hasPower()
    {
        var power = parent.TryGetComp<CompPowerTrader>();
        return power is { PowerOn: true };
    }
}