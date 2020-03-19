namespace ra2
{
    using RimWorld;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using Verse;

   
    public class CompSovietPower : CompPowerPlant
    {
        private const int BladeCount = 9;
        private const float BladeOffset = 2.36f;
        private string path = "ra2/Projectile/Lighting/";
        //public static readonly Material PrismMat = MaterialPool.MatFrom("Things/Building/Power/WatermillGenerator/WatermillGeneratorBlades");
        //public Material PrismMat;
        private bool cacheDirty = true;
        private float spinPosition;
        private float spinRate = 1f;
        private const float SpinRateFactor = 0.006666667f;

        private int ticktime;





        public override void CompTick()
        {
            base.CompTick();
          
            this.ticktime++;

           
          


        }


        public Material getMat() {
            int speed = 4;
            int index = ((this.ticktime)/speed) %4 ;
            if (this.ticktime>=(4* speed) -1) { this.ticktime = 0; }
            Material result= MaterialPool.MatFrom(path+"Lighting"+index);
            


            return result;
        }




        public override void PostDraw()
        {
            base.PostDraw();


            Vector3 vector = base.parent.TrueCenter() + new Vector3(0,2f, -0.6f); ;

                float f = this.spinPosition + ((6.283185f * 1) / 9f);
                float x = Mathf.Abs((float)(4f * Mathf.Sin(f)));
                bool flag = (f % 6.283185f) < 3.141593f;
                Vector2 vector2 = new Vector2(x, 1f);
                Vector3 s = new Vector3(0.8f, 1f, 1.1f);
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetTRS(vector , base.parent.Rotation.AsQuat, s);
            if (base.PowerOutput>0.01)
            {
                Graphics.DrawMesh(MeshPool.plane10Flip, matrix, getMat(), 0);
            }
            else {
               // Graphics.DrawMesh(MeshPool.plane10Flip, matrix, MaterialPool.MatFrom(path + "Lightning0"), 0);
            }
               







               
          // Log.Warning("now index is "+getMat());
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.spinPosition = Rand.Range((float)0f, (float)15f);
            this.ticktime = 0;
        }







        public new CompProperties_Power Props
        {
            get
            {
                return (CompProperties_Power)base.props;
            }
        }









    }
}
