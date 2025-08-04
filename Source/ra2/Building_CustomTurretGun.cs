using RimWorld;
using UnityEngine;
using Verse;

namespace ra2;

public class Building_CustomTurretGun : Building_TurretGun
{
    private new readonly TurretTop_CustomSize top;

    public Building_CustomTurretGun()
    {
        top = new TurretTop_CustomSize(this);
    }


    public bool IsStun => IsStunned;


    public CompTurretTopSize TopSizeComp { get; private set; }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        top.DrawTurret();
        Comps_PostDraw();
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        TopSizeComp = GetComp<CompTurretTopSize>();
    }


    protected override void Tick()
    {
        base.Tick();


        if (powerComp is { PowerOn: false } || mannableComp is { MannedNow: false } ||
            !Spawned)
        {
            return;
        }

        GunCompEq.verbTracker.VerbsTick();
        if (!IsStunned && GunCompEq.PrimaryVerb.state != VerbState.Bursting)
        {
            top.TurretTopTick();
        }
    }
}