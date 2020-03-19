namespace ra2
{
    using RimWorld;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using Verse;
    using Verse.Sound;

    // [StaticConstructorOnStartup]
    public class CompSovietSuperWeapon : ThingComp
    {






        private string path = "ra2/Nuke/ra2_Nuke_";
        private int ticktime;
        private bool canFire;
        private Building_SuperWeapon pb;




        public override void CompTick()
        {
            base.CompTick();
          // if(pb==null) this.pb = base.parent as Building_SuperWeapon;
            this.ticktime = pb.ticks;
            this.canFire = pb.canFire;

            if (this.ticktime == 34910)
            {
                SoundStarter.PlayOneShot(SoundDef.Named("ra2_NuclearRead"), this.parent);
            }


        }


        public Material getShellMat() {
            Material result = MaterialPool.MatFrom(path + "0");
             
            if (this.ticktime >= 34910 && this.ticktime<35000 && !canFire)
            {
                int index = 10 - (35000 - this.ticktime) / 9;
               // Log.Warning(index + "?");
                result = MaterialPool.MatFrom(path + index);

            } else if (this.ticktime>=35000||canFire) {
                result = MaterialPool.MatFrom(path + "10");
            }
            return result;
        }




        public override void PostDraw()
        {
            base.PostDraw();
            Vector3 vector = base.parent.TrueCenter() + new Vector3(0, 3f,0);
            Vector3 s = new Vector3(pb.def.graphicData.drawSize.x, 1f, pb.def.graphicData.drawSize.y);
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(vector, base.parent.Rotation.AsQuat, s);

            Graphics.DrawMesh(MeshPool.plane10, matrix, getShellMat(), 0);
            

        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.pb = base.parent as Building_SuperWeapon;
            //this.ticktime = 0;
        }



    }
}
