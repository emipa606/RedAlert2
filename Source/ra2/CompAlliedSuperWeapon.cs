using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

// [StaticConstructorOnStartup]
public class CompAlliedSuperWeapon : ThingComp
{
    private readonly string path = "ra2/Projectile/Lighting";
    private bool canFire;
    private List<Texture2D> list = [];
    private Material oldt;
    private Building_SuperWeapon pb;
    private int ticktime;

    private bool hasPower()
    {
        var cpt = parent.TryGetComp<CompPowerTrader>();
        return cpt is { PowerOn: true };
    }

    public override void CompTick()
    {
        base.CompTick();
        // if(pb==null) this.pb = base.parent as Building_SuperWeapon;
        ticktime = pb.ticks;
        canFire = pb.canFire;

        if (ticktime == 34910)
        {
            SoundDef.Named("ra2_StormRead").PlayOneShot(parent);
        }
    }


    private Material getShellMat()
    {
        // Material result = MaterialPool.MatFrom(path + "0");
        if (ticktime < 34910 && !canFire)
        {
            return oldt;
        }

        if (!hasPower() || ticktime % 4 != 0 && oldt != null)
        {
            return oldt;
        }

        var req = new MaterialRequest(list.RandomElement());
        var result = MaterialPool.MatFrom(req);

        oldt = result;
        return result;

        // result = MaterialPool.MatFrom(path + index);
    }


    public override void PostDraw()
    {
        base.PostDraw();
        var vector = parent.TrueCenter() + new Vector3(0, 3f, 0);
        var s = new Vector3(pb.def.graphicData.drawSize.x / 5, 1f, pb.def.graphicData.drawSize.y / 3f);
        var matrix = new Matrix4x4();

        matrix.SetTRS(vector, 90f.ToQuat(), s);
        if (ticktime < 34910 && !canFire)
        {
            return;
        }

        var mt = getShellMat();
        Graphics.DrawMesh(MeshPool.plane10, matrix, mt, 0);


        vector = parent.TrueCenter() + new Vector3(-2f, 3f, 1f);
        s = new Vector3(pb.def.graphicData.drawSize.x / 10f, 1f, pb.def.graphicData.drawSize.y / 4f);
        matrix.SetTRS(vector, parent.Rotation.AsQuat, s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, mt, 0);
        vector = parent.TrueCenter() + new Vector3(2f, 3f, 1f);
        // s = new Vector3(pb.def.graphicData.drawSize.x / 5, 1f, pb.def.graphicData.drawSize.y / 4f);
        matrix.SetTRS(vector, parent.Rotation.AsQuat, s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, mt, 0);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        pb = parent as Building_SuperWeapon;
        list = ContentFinder<Texture2D>.GetAllInFolder(path).ToList();
    }
}