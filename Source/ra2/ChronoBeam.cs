using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ra2
{
    public class ChronoBeam:Thing
    {
        public Matrix4x4 lineMatrix;
        public int ticks;
        public Thing caster;
        public LocalTargetInfo currentTarget;

        protected void ComputeDrawingParameters(Thing caster,LocalTargetInfo currentTarget)
        {
            Vector3 shoter = caster.Position.ToVector3()+new Vector3(0,0,0.5f);
            Vector3 target = currentTarget.Cell.ToVector3() + new Vector3(0, 0, 0.5f);

            Vector3 pos = (Vector3)((shoter + target) / 2f) + new Vector3(0, 2f, 0); ;
            float z = (target - shoter).MagnitudeHorizontal();
            Vector3 s = new Vector3(1f, 1f, z);
            Quaternion qq = Quaternion.LookRotation(target - shoter);
            this.lineMatrix.SetTRS(pos, qq, s);
            this.Draws();
        }

        public void Draws()
        {
            
           
            Graphics.DrawMesh(MeshPool.plane10, this.lineMatrix, FadedMaterialPool.FadedVersionOf(MaterialPool.MatFrom("ra2/Projectile/ChronoBeam"), this.Opacity), 0);
        }
        public override void Draw()
        {


            ComputeDrawingParameters(this.caster, this.currentTarget);
        }

        public override void Tick()
        {
            bool flag;

            // int num = this.ticks;
            // this.ticks = num + 1;
            this.ticks++;
                flag = (ticks > 60);
            if (flag)
            {
                this.Destroy(DestroyMode.Vanish);
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
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<Matrix4x4>(ref this.lineMatrix, "lineMatrix");
        }

        public float Opacity
        {
            get
            {
                return (float)Math.Sin(Math.Pow(1.0 - 1.0 * (double)this.ticks / 60, 6) * 3.1415926535897931);
            }
        }


    }
}
