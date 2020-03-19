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

    [StaticConstructorOnStartup]
    public class CompPrismTower : ThingComp
    {
        private const int BladeCount = 9;
        private const float BladeOffset = 2.36f;
        private string path = "ra2/Prism tower/";
        //public static readonly Material PrismMat = MaterialPool.MatFrom("Things/Building/Power/WatermillGenerator/WatermillGeneratorBlades");
        //public Material PrismMat;
        private bool cacheDirty = true;
        private float spinPosition;
        private float spinRate = 1f;
        private const float SpinRateFactor = 0.006666667f;

        private int ticktime;

        private void ClearCache()
        {
            this.cacheDirty = true;
        }



        public override void CompTick()
        {
            base.CompTick();
          
            this.ticktime++;

           
          


        }


        public Material getMat() {
            int speed = this.Props.speed;
            int index = ((this.ticktime)/speed) %12 ;
            if (this.ticktime>=(12* speed) -1) { this.ticktime = 0; }
            Material result= MaterialPool.MatFrom(path+index);
            


            return result;
        }




        public override void PostDraw()
        {
            base.PostDraw();


            Vector3 vector = base.parent.TrueCenter() + new Vector3(0,3f, (this.Props.size.x)/1.5f); ;

                float f = this.spinPosition + ((6.283185f * 1) / 9f);
                float x = Mathf.Abs((float)(4f * Mathf.Sin(f)));
                bool flag = (f % 6.283185f) < 3.141593f;
                Vector2 vector2 = new Vector2(x, 1f);
                Vector3 s = new Vector3(this.Props.size.x, 1f, this.Props.size.y);
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetTRS(vector , base.parent.Rotation.AsQuat, s);
            if (hasPower()&&!isATK())
            {
                Graphics.DrawMesh(MeshPool.plane10Flip, matrix, getMat(), 0);
            }
            else {
                Graphics.DrawMesh(MeshPool.plane10Flip, matrix, MaterialPool.MatFrom(path + "0"), 0);
            }
               







               
          // Log.Warning("now index is "+getMat());
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.spinPosition = Rand.Range((float)0f, (float)15f);
            this.ticktime = 0;
        }







        public CompProperties_PrismTower Props
        {
            get
            {
                return (CompProperties_PrismTower)base.props;
            }
        }




        private bool isATK() {
            return (((Building_TurretGun)(base.parent)).CurrentTarget!=null) ? true : false;
         
        }

        private bool hasPower() {
            CompPowerTrader power = base.parent.TryGetComp<CompPowerTrader>();
            if (power != null && power.PowerOn) {
                return true;
            }
            Building_CustomTurretGun bc = this.parent as Building_CustomTurretGun;
            if (bc.IsStun) { return false; }
            return false;
        }




    }
}
