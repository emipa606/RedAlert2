using System;
using UnityEngine;
using Verse;

namespace ra2
{
    public class ChronoBeam : Thing
    {
        public Thing caster;
        public LocalTargetInfo currentTarget;
        public Matrix4x4 lineMatrix;
        public int ticks;

        public float Opacity => (float) Math.Sin(Math.Pow(1.0 - (1.0 * ticks / 60), 6) * 3.1415926535897931);

        protected void ComputeDrawingParameters(Thing caster, LocalTargetInfo currentTarget)
        {
            var shoter = caster.Position.ToVector3() + new Vector3(0, 0, 0.5f);
            var target = currentTarget.Cell.ToVector3() + new Vector3(0, 0, 0.5f);

            var pos = ((shoter + target) / 2f) + new Vector3(0, 2f, 0);
            var z = (target - shoter).MagnitudeHorizontal();
            var s = new Vector3(1f, 1f, z);
            var qq = Quaternion.LookRotation(target - shoter);
            lineMatrix.SetTRS(pos, qq, s);
            Draws();
        }

        public void Draws()
        {
            Graphics.DrawMesh(MeshPool.plane10, lineMatrix,
                FadedMaterialPool.FadedVersionOf(MaterialPool.MatFrom("ra2/Projectile/ChronoBeam"), Opacity), 0);
        }

        public override void Draw()
        {
            ComputeDrawingParameters(caster, currentTarget);
        }

        public override void Tick()
        {
            // int num = this.ticks;
            // this.ticks = num + 1;
            ticks++;
            if (ticks > 60)
            {
                Destroy();
            }
        }

        public void Setup(Thing caster, LocalTargetInfo currentTarget)
        {
            this.caster = caster;
            this.currentTarget = currentTarget;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref lineMatrix, "lineMatrix");
        }
    }
}