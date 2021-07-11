using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ra2
{
    public class Building_SuperWeapon : Building
    {
        private readonly int MaxLaunchDistance = 350;

        public bool canFire;
        // private SuperWeaponType type;

        protected StunHandler stunner;
        public int ticks;


        public Building_SuperWeapon()
        {
            stunner = new StunHandler(this);
            // this.type = getType();
        }


        public override string GetInspectString()
        {
            var sb = new StringBuilder();
            var left = (36000 - ticks) / 60;
            sb.Append(base.GetInspectString());
            if (!canFire)
            {
                sb.Append("\n" + "LeftSuperWeaponTime".Translate() + ":" + (left / 60) + "m" + (left % 60) + "s");
            }
            else
            {
                sb.Append("\n" + "ReadytoLaunch".Translate());
            }


            // String sb = base.GetInspectString() + "\n" + Translator.Translate("restoreBullet") + ":" + this.restoreBullet + "\n" + Translator.Translate("restoreProgress") + ":" + (this.ticks);
            return sb.ToString();
        }

        private bool hasPower()
        {
            var powerComp = this.TryGetComp<CompPowerTrader>();
            if (powerComp != null && !powerComp.PowerOn || !Spawned)
            {
                return false;
            }

            if (!stunner.Stunned)
            {
                return true;
            }

            return false;
        }

        private bool canTickAdd()
        {
            var powerComp = this.TryGetComp<CompPowerTrader>();
            if (powerComp != null && !powerComp.PowerOn || !Spawned)
            {
                return false;
            }

            if (!stunner.Stunned)
            {
                return !canFire;
            }
            //  if (canFire) return false;

            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref canFire, "canFire");
            //Scribe_Values.Look<SuperWeaponType>(ref this.type, "canFire", SuperWeaponType.Soviet, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (canTickAdd())
            {
                ticks++;
            }


            if (ticks <= 36000)
            {
                return;
            }

            canFire = true;
            ticks = 0;
        }

        private SuperWeaponType getType()
        {
            if (def.defName == "ra2_SovietSuperWeapon")
            {
                return SuperWeaponType.Soviet;
            }

            if (def.defName == "ra2_AlliedSuperWeapon")
            {
                return SuperWeaponType.Allied;
            }

            return SuperWeaponType.Soviet;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            /*
            foreach (Gizmo gz in this.GetGizmos()) {
                yield return gz;
            }
            */
            if (DebugSettings.godMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "fill",
                    action = delegate
                    {
                        canFire = true;
                        ticks = 0;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "AlmostDone",
                    action = delegate
                    {
                        ticks = 34819;
                        canFire = false;
                    }
                };
            }

            yield return new Command_Action
            {
                defaultLabel = ("SuperWeaponDesc." + getType()).Translate(),
                defaultDesc = ("SuperWeaponDesc." + getType()).Translate(),
                icon = ContentFinder<Texture2D>.Get("ra2/World/ra2_SuperWeaponIcon_" + getType()),
                disabled = !canFire || !hasPower(),
                action = StartChoosingDestination
            };
        }

        private void StartChoosingDestination()
        {
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this));
            Find.WorldSelector.ClearSelection();
            var tile = Map.Tile;
            Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, true, null, true, delegate
            {
                if (MaxLaunchDistance < 200)
                {
                    GenDraw.DrawWorldRadiusRing(tile, MaxLaunchDistance);
                }
            }, delegate(GlobalTargetInfo target)
            {
                if (!target.IsValid)
                {
                    return "SWeaponTargetInvalid".Translate();
                }

                /*
                int num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile, true, int.MaxValue);
                if (this.MaxLaunchDistance < 200 && num > this.MaxLaunchDistance)
                {
                    return "SWeaponRangeBad".Translate();
                }
                */
                return null;
            });
        }

        public bool ChoseWorldTarget(GlobalTargetInfo target)
        {
            if (!target.IsValid)
            {
                Messages.Message("SWeaponTargetInvalid".Translate(), MessageTypeDefOf.RejectInput);
                return false;
            }

            /*
            int num = Find.WorldGrid.TraversalDistanceBetween(base.Map.Tile, target.Tile, true, int.MaxValue);
            if (this.MaxLaunchDistance < 200 && num > this.MaxLaunchDistance)
            {
                Messages.Message("MissileRangeBad".Translate(), MessageTypeDefOf.RejectInput, true);
                return false;
            }
            */
            if (target.WorldObject is MapParent {HasMap: true} mapParent)
            {
                var map = mapParent.Map;
                Current.Game.CurrentMap = map;
                var targeter = Find.Targeter;

                void ActionWhenFinished()
                {
                    if (Find.Maps.Contains(Map))
                    {
                        Current.Game.CurrentMap = Map;
                    }
                }
                /*
                int range = 0;
                switch (getType()) {
                    case SuperWeaponType.Soviet:range = 56;break;
                    case SuperWeaponType.Allied: range = 30; break;
                    default:range=10;break;
                }
                */

                var ic = ContentFinder<Texture2D>.Get("ra2/World/ra2_SuperWeaponIcon_" + getType());
                var tg = new TargetingParameters
                {
                    canTargetLocations = true,
                    canTargetItems = true,
                    canTargetBuildings = true,
                    canTargetPawns = true,
                    canTargetFires = true
                };
                targeter.BeginTargeting(tg, delegate(LocalTargetInfo x)
                {
                    //GenDraw.DrawRadiusRing(UI.MouseCell(), range);
                    TryLaunch(x.ToGlobalTargetInfo(map));
                }, null, ActionWhenFinished, ic);
                return true;
            }

            TryLaunch(target);
            return true;
        }

        public void TryLaunch(GlobalTargetInfo target)
        {
            if (!Spawned)
            {
                Log.Error("Tried to launch  but it's unspawned.");
                return;
            }


            var map = Map;
            var num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, target.Tile);
            if (num > MaxLaunchDistance)
            {
                return;
            }

            //this.Transporter.TryRemoveLord(map);
            //  int groupID = this.Transporter.groupID;
            // float amount = Mathf.Max(CompLaunchable.FuelNeededToLaunchAtDist((float)num), 1f);
            //  for (int i = 0; i < transportersInGroup.Count; i++)
            //   {
            // CompTransporter compTransporter = transportersInGroup[i];
            //  Building fuelingPortSource = compTransporter.Launchable.FuelingPortSource;
            // if (fuelingPortSource != null)
            //  {
            //     fuelingPortSource.TryGetComp<CompRefuelable>().ConsumeFuel(amount);
            //  }
            //  ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();
            canFire = false;
            ticks = 0;

            if (getType() == SuperWeaponType.Soviet)
            {
                SuperWeaponAction.Soviet(target, Position, map);
            }
            else if (getType() == SuperWeaponType.Allied)
            {
                SuperWeaponAction.Allied(target, Position, map, target.Tile);
            }


            //   }
            CameraJumper.TryHideWorld();
        }

        //public static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("ra2/World/SovietIcon", true);
    }
}