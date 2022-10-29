using System.Reflection;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2;

public class SovietNuclearLeaving : ThingWithComps
{
    public int destinationTile = -1;

    //public IntVec3 targetCell;
    public Vector3 ExactPosition = Vector3.zero;

    private MapParent mapParent;
    private float speed = 0.001f;

    public IntVec3 targetCell = IntVec3.Zero;

    public override Vector3 DrawPos => ExactPosition;


    public override void Tick()
    {
        base.Tick();
        if (ExactPosition == Vector3.zero)
        {
            SoundDef.Named("ra2_NuclearAlert").PlayOneShotOnCamera();
            SoundDef.Named("ra2_NuclearLaunch").PlayOneShotOnCamera();
            ExactPosition = this.TrueCenter() + new Vector3(0.5f, 10, 5);
        }

        if (speed < 1f)
        {
            speed += 0.002f;
        }

        ExactPosition += new Vector3(0, 0, speed);
        FleckMaker.ThrowExplosionCell((ExactPosition + new Vector3(0, 0, -5)).ToIntVec3(), Map,
            FleckDefOf.ExplosionFlash, new Color(1, 0, 0));
        if (!(ExactPosition.z >= Map.Size.z))
        {
            return;
        }

        Booom();
        Destroy();
    }


    public void Booom()
    {
        if (!Find.WorldObjects.AnyMapParentAt(destinationTile))
        {
            Messages.Message("SWeaponDestoryBase".Translate(), MessageTypeDefOf.NeutralEvent);
            var worldObject = Find.WorldObjects.WorldObjectAt<WorldObject>(destinationTile);
            if (worldObject?.Faction != null && !worldObject.Faction.IsPlayer)
            {
                worldObject.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999);
            }

            var peaceTalks = Find.WorldObjects.WorldObjectAt<PeaceTalks>(destinationTile);
            if (peaceTalks == null)
            {
                return;
            }

            try
            {
                peaceTalks.GetType()
                    .GetMethod("Outcome_Disaster", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(peaceTalks, null);
            }
            catch
            {
                // ignored
            }

            Find.WorldObjects.Remove(peaceTalks);

            return;
        }


        mapParent = Find.WorldObjects.MapParentAt(destinationTile);
        if (mapParent.Faction is { IsPlayer: false })
        {
            mapParent.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -999);
        }

        if (mapParent.HasMap)
        {
            var bomb = (SovietNuclearFallen)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearFallen"));
            bomb.targetCell = targetCell;
            bomb.ExactPosition = new Vector3(targetCell.x, 10, mapParent.Map.Size.z - 10);
            GenSpawn.Spawn(bomb, bomb.ExactPosition.ToIntVec3(), mapParent.Map);
            return;
        }

        Messages.Message("SWeaponDestoryBase".Translate(), mapParent, MessageTypeDefOf.NegativeEvent);
        SoundDef.Named("ra2_NuclearExplode").PlayOneShotOnCamera();
        Settlement settlementBase;
        if ((settlementBase = mapParent as Settlement) != null)
        {
            if (settlementBase.Faction == Faction.OfPlayer)
            {
                return;
            }

            foreach (var faction in Find.FactionManager.AllFactionsListForReading)
            {
                if (faction != Faction.OfPlayer)
                {
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, -10);
                }
            }

            try
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("LetterFactionBaseDefeated".Translate(settlementBase.Label,
                    TimedForcedExit.GetForceExitAndRemoveMapCountdownTimeLeftString(60000)));

                Find.LetterStack.ReceiveLetter("LetterLabelFactionBaseDefeated".Translate(),
                    stringBuilder.ToString(), LetterDefOf.PositiveEvent, new GlobalTargetInfo(settlementBase.Tile));
                var destroyedSettlement =
                    (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
                destroyedSettlement.Tile = settlementBase.Tile;
                Find.WorldObjects.Add(destroyedSettlement);
            }
            catch
            {
                Log.Warning("Failed to end settlement");
            }
        }

        Find.WorldObjects.Remove(mapParent);
    }
}