using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ra2
{
    public enum SuperWeaponType {
        Soviet,
        Allied,
        Yuri
    }

    public class Building_SuperWeapon:Building
    {
        public bool canFire = false;
        public int ticks = 0;
        private int MaxLaunchDistance = 350;
       // private SuperWeaponType type;

        protected StunHandler stunner;


        public Building_SuperWeapon(){
            this.stunner = new StunHandler(this);
           // this.type = getType();
            
        }


        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();
            int left = (36000 - this.ticks) / 60;
            sb.Append(base.GetInspectString());
            if (!canFire)
            {
                sb.Append("\n"+"LeftSuperWeaponTime".Translate() + ":" + left/60 + "m"+ (left%60)+"s");
            }
            else {
                sb.Append("\n"+"ReadytoLaunch".Translate());
            }
          




            // String sb = base.GetInspectString() + "\n" + Translator.Translate("restoreBullet") + ":" + this.restoreBullet + "\n" + Translator.Translate("restoreProgress") + ":" + (this.ticks);
            return sb.ToString();

        }
        private bool hasPower() {
            CompPowerTrader powerComp = this.TryGetComp<CompPowerTrader>();
            if ((((powerComp == null) || powerComp.PowerOn) && this.Spawned))
            {
                if (!stunner.Stunned)
                {
                    return true;
                }
            }
            return false;
        }

        private bool canTickAdd() {
            CompPowerTrader powerComp = this.TryGetComp<CompPowerTrader>();
            if ((((powerComp == null) || powerComp.PowerOn) &&this.Spawned))
            {
                if (!stunner.Stunned)
                {
                    return !canFire;
                }
            }
          //  if (canFire) return false;

            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
            Scribe_Values.Look<bool>(ref this.canFire, "canFire", false, false);
            //Scribe_Values.Look<SuperWeaponType>(ref this.type, "canFire", SuperWeaponType.Soviet, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (canTickAdd()) ticks++;





            if (ticks > 36000) {
                canFire = true;
                ticks = 0;
            }
            
        }

        private SuperWeaponType getType()
        {
            if (this.def.defName == "ra2_SovietSuperWeapon") return SuperWeaponType.Soviet;
            else if (this.def.defName == "ra2_AlliedSuperWeapon") return SuperWeaponType.Allied;

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
                        this.canFire = true;
                        this.ticks = 0;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "AlmostDone",
                    action = delegate
                    {
                        this.ticks = 34819;
                        this.canFire = false;
                       
                    }
                };
            }

            yield return new Command_Action
            {
                defaultLabel = ("SuperWeaponDesc." + getType()).Translate(),
                defaultDesc = ("SuperWeaponDesc." + getType()).Translate(),
                icon = ContentFinder<Texture2D>.Get("ra2/World/ra2_SuperWeaponIcon_"+getType()),
                disabled=!canFire || !hasPower(),
                action = delegate
                {
                    this.StartChoosingDestination();
                }
            };


            yield break;


        }

        private void StartChoosingDestination()
        {
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this));
            Find.WorldSelector.ClearSelection();
            int tile = base.Map.Tile;
            Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, null, true, delegate
            {
                if (this.MaxLaunchDistance < 200)
                {
                    GenDraw.DrawWorldRadiusRing(tile, this.MaxLaunchDistance);
                }
            }, delegate (GlobalTargetInfo target)
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
                Messages.Message("SWeaponTargetInvalid".Translate(), MessageTypeDefOf.RejectInput, true);
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
            MapParent mapParent = target.WorldObject as MapParent;
            if (mapParent != null && mapParent.HasMap)
            {
                Map map = mapParent.Map;
                Current.Game.CurrentMap = map;
                Targeter targeter = Find.Targeter;
                Action actionWhenFinished = delegate
                {
                    if (Find.Maps.Contains(base.Map))
                    {
                        Current.Game.CurrentMap = base.Map;
                    }
                };
                /*
                int range = 0;
                switch (getType()) {
                    case SuperWeaponType.Soviet:range = 56;break;
                    case SuperWeaponType.Allied: range = 30; break;
                    default:range=10;break;
                }
                */

                Texture2D ic = ContentFinder<Texture2D>.Get("ra2/World/ra2_SuperWeaponIcon_" + getType());
                TargetingParameters tg = new TargetingParameters();
                tg.canTargetLocations = true;
                tg.canTargetItems = true;
                tg.canTargetBuildings = true;
                tg.canTargetPawns = true;
                tg.canTargetFires = true;
                targeter.BeginTargeting(tg, delegate (LocalTargetInfo x)
                {
                    //GenDraw.DrawRadiusRing(UI.MouseCell(), range);
                    this.TryLaunch(x.ToGlobalTargetInfo(map));
                }, null, actionWhenFinished, ic);
                return true;
            }
            this.TryLaunch(target);
            return true;
        }

        public void TryLaunch(GlobalTargetInfo target)
        {
            if (!this.Spawned)
            {
                Log.Error("Tried to launch  but it's unspawned.", false);
                return;
            }
           


            Map map = this.Map;
            int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, target.Tile, true, int.MaxValue);
            if (num > this.MaxLaunchDistance)
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
            this.canFire = false;
            this.ticks = 0;

            if(getType() == SuperWeaponType.Soviet)
            SuperWeaponAction.Soviet(target,this.Position,map);
            else if (getType() == SuperWeaponType.Allied)
                SuperWeaponAction.Allied(target, this.Position, map,target.Tile);




            //   }
            CameraJumper.TryHideWorld();
        }
        //public static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("ra2/World/SovietIcon", true);

    }
}
