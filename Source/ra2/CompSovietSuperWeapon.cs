using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

// [StaticConstructorOnStartup]
public class CompSovietSuperWeapon : ThingComp
{
    private readonly string path = "ra2/Nuke/ra2_Nuke_";
    private bool canFire;
    private Building_SuperWeapon pb;
    private int ticktime;


    public override void CompTick()
    {
        base.CompTick();
        // if(pb==null) this.pb = base.parent as Building_SuperWeapon;
        ticktime = pb.ticks;
        canFire = pb.canFire;

        if (ticktime == 34910)
        {
            SoundDef.Named("ra2_NuclearRead").PlayOneShot(parent);
        }
    }


    private Material getShellMat()
    {
        var result = MaterialPool.MatFrom($"{path}0");

        if (ticktime is >= 34910 and < 35000 && !canFire)
        {
            var index = 10 - ((35000 - ticktime) / 9);
            // Log.Warning(index + "?");
            result = MaterialPool.MatFrom(path + index);
        }
        else if (ticktime >= 35000 || canFire)
        {
            result = MaterialPool.MatFrom($"{path}10");
        }

        return result;
    }


    public override void PostDraw()
    {
        base.PostDraw();
        var vector = parent.TrueCenter() + new Vector3(0, 3f, 0);
        var s = new Vector3(pb.def.graphicData.drawSize.x, 1f, pb.def.graphicData.drawSize.y);
        var matrix = new Matrix4x4();
        matrix.SetTRS(vector, parent.Rotation.AsQuat, s);

        Graphics.DrawMesh(MeshPool.plane10, matrix, getShellMat(), 0);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        pb = parent as Building_SuperWeapon;
        //this.ticktime = 0;
    }
}