using RimWorld;
using UnityEngine;
using Verse;

namespace ra2
{

    public class Projectile_Line : Projectile_Custom
    {
        private int age;
       
        public Matrix4x4 lineMatrix;
        public Vector3 lineScale = new Vector3(1f, 1f, 1f);


        protected void ComputeDrawingParameters()
        {
            Vector3 pos = (Vector3)((base.origin + base.destination) / 2f) + new Vector3(0,2f,0); ;
            float z = (base.destination - base.origin).MagnitudeHorizontal();
            Vector3 s = new Vector3(1f, 1f, z);
            this.lineMatrix.SetTRS(pos, this.ExactRotation, s);
        }

        public override void Draw()
        {
            Graphics.DrawMesh(MeshPool.plane10, this.lineMatrix, FadedMaterialPool.FadedVersionOf(base.def.DrawMatSingle, this.LineBrightness), 0);
            base.Comps_PostDraw();
        }

        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
        
                MoteMaker.ThrowMicroSparks(base.destination, map);
            
            base.Impact(hitThing);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
           
        }

        public override void Tick()
        {
            if (this.age == 0)
            {
                this.ComputeDrawingParameters();
            }
            base.Tick();
            this.age++;
        }

        protected float LineBrightness
        {
            get
            {
                if (this.age <= 3)
                {
                    return (((float)this.age) / 3f);
                }
                return (1f - (((float)this.age) / ((float)Rand.Range(15, 60))));
            }
        }
    }
}
