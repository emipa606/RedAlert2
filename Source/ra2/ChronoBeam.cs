using System;
using UnityEngine;
using Verse;

namespace ra2;

public class ChronoBeam : Thing
{
    private Thing caster;
    private LocalTargetInfo currentTarget;
    private Matrix4x4 lineMatrix;
    private int ticks;

    private float Opacity => (float)Math.Sin(Math.Pow(1.0 - (1.0 * ticks / 60), 6) * 3.1415926535897931);

    private void ComputeDrawingParameters(Thing thing, LocalTargetInfo localTargetInfo)
    {
        var shoter = thing.Position.ToVector3() + new Vector3(0, 0, 0.5f);
        var target = localTargetInfo.Cell.ToVector3() + new Vector3(0, 0, 0.5f);

        var pos = ((shoter + target) / 2f) + new Vector3(0, 2f, 0);
        var z = (target - shoter).MagnitudeHorizontal();
        var s = new Vector3(1f, 1f, z);
        var qq = Quaternion.LookRotation(target - shoter);
        lineMatrix.SetTRS(pos, qq, s);
        Draws();
    }

    private void Draws()
    {
        Graphics.DrawMesh(MeshPool.plane10, lineMatrix,
            FadedMaterialPool.FadedVersionOf(MaterialPool.MatFrom("ra2/Projectile/ChronoBeam"), Opacity), 0);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        ComputeDrawingParameters(caster, currentTarget);
    }

    protected override void Tick()
    {
        // int num = this.ticks;
        // this.ticks = num + 1;
        ticks++;
        if (ticks > 60)
        {
            Destroy();
        }
    }

    public void Setup(Thing thing, LocalTargetInfo localTargetInfo)
    {
        caster = thing;
        currentTarget = localTargetInfo;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticks, "ticks");
        Scribe_Values.Look(ref lineMatrix, "lineMatrix");
    }
}