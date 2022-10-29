using UnityEngine;
using Verse;

namespace ra2;

public class SovietNuclearFallen : ThingWithComps
{
    public Vector3 ExactPosition = Vector3.zero;

    public IntVec3 targetCell;

    public override void Draw()
    {
        //base.Draw();
        base.DrawAt(DrawPos, true);
    }

    public override void Tick()
    {
        base.Tick();
        Position -= new IntVec3(0, 0, 1);
        if (Position.z > targetCell.z)
        {
            return;
        }

        var bomb = (SovietNuclearStrike)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearStrike"));
        bomb.landPos = targetCell;
        GenSpawn.Spawn(bomb, targetCell, Map);
        Destroy();
    }
}