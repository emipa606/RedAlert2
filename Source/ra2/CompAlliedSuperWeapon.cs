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
    public class CompAlliedSuperWeapon : ThingComp
    {






        private string path = "ra2/Projectile/Lighting";
        private int ticktime;
        private bool canFire;
        private Building_SuperWeapon pb;
        private List<Texture2D> list = new List<Texture2D>();
        private Material oldt = null;

        private bool hasPower() {
            CompPowerTrader cpt = this.parent.TryGetComp<CompPowerTrader>();
            if (cpt != null && cpt.PowerOn) {
                return true;
            }
        


            return false;
        }

        public override void CompTick()
        {
            base.CompTick();
          // if(pb==null) this.pb = base.parent as Building_SuperWeapon;
            this.ticktime = pb.ticks;
            this.canFire = pb.canFire;

            if (this.ticktime == 34910)
            {
                SoundStarter.PlayOneShot(SoundDef.Named("ra2_StormRead"), this.parent);
            }


        }


        public Material getShellMat() {
           // Material result = MaterialPool.MatFrom(path + "0");
            if (this.ticktime >= 34910||canFire)
            {
                if(hasPower())
                if (this.ticktime % 4 == 0 || oldt == null)
                {

                    MaterialRequest req = new MaterialRequest(list.RandomElement<Texture2D>());
                    Material result = MaterialPool.MatFrom(req);

                    oldt = result;
                    return result;
                }
               // result = MaterialPool.MatFrom(path + index);
            }

            return oldt;
        }




        public override void PostDraw()
        {
            base.PostDraw();
            Vector3 vector = base.parent.TrueCenter() + new Vector3(0, 3f, 0);
            Vector3 s = new Vector3(pb.def.graphicData.drawSize.x / 5, 1f, pb.def.graphicData.drawSize.y / 3f);
            Matrix4x4 matrix = new Matrix4x4();

            matrix.SetTRS(vector, 90f.ToQuat(), s);
            if (this.ticktime >= 34910 || canFire) {
                Material mt = getShellMat();
            Graphics.DrawMesh(MeshPool.plane10, matrix,mt, 0);


                vector = base.parent.TrueCenter() + new Vector3(-2f, 3f, 1f);
                s = new Vector3(pb.def.graphicData.drawSize.x / 10f, 1f, pb.def.graphicData.drawSize.y / 4f);
                matrix.SetTRS(vector, this.parent.Rotation.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix,mt, 0);
                vector = base.parent.TrueCenter() + new Vector3(2f, 3f, 1f);
               // s = new Vector3(pb.def.graphicData.drawSize.x / 5, 1f, pb.def.graphicData.drawSize.y / 4f);
                matrix.SetTRS(vector, this.parent.Rotation.AsQuat, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, mt, 0);
            }


        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.pb = base.parent as Building_SuperWeapon;
            list = (ContentFinder<Texture2D>.GetAllInFolder(path)).ToList<Texture2D>();
        }



    }
}
