using RimWorld;
using Verse;

namespace ra2;

public class Building_CustomTurretGun : Building_TurretGun
{
    protected new TurretTop_CustomSize top;
    protected CompTurretTopSize topSizeComp;

    public Building_CustomTurretGun()
    {
        top = new TurretTop_CustomSize(this);
    }


    public bool IsStun => stunner.Stunned;


    public CompTurretTopSize TopSizeComp =>
        topSizeComp;

    public override void Draw()
    {
        top.DrawTurret();
        Comps_PostDraw();
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        topSizeComp = GetComp<CompTurretTopSize>();
    }


    public override void Tick()
    {
        base.Tick();


        if (powerComp is { PowerOn: false } || mannableComp is { MannedNow: false } ||
            !Spawned)
        {
            return;
        }

        GunCompEq.verbTracker.VerbsTick();
        if (!stunner.Stunned && GunCompEq.PrimaryVerb.state != VerbState.Bursting)
        {
            top.TurretTopTick();
        }
    }
}