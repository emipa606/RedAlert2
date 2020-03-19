using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ra2
{

      [HarmonyPatch(typeof(PawnRenderer), "CarryWeaponOpenly", new Type[]
    {

    })]
    public static class Harmony_Tank_WeaponAim
    {
        public static bool Prefix(PawnRenderer __instance, ref bool __result)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pp = traverse.Field("pawn").GetValue<Pawn>();
            if (pp.kindDef.defName.EqualsIgnoreCase("ra2_rhinotank") || pp.kindDef.defName.EqualsIgnoreCase("ra2_grizzlytank") || pp.kindDef.defName.EqualsIgnoreCase("ra2_apotank")) {
                __result = true;
                return false;
            }
            //return (pp.carryTracker == null || pp.carryTracker.CarriedThing == null) && (pp.Drafted || (pp.CurJob != null && pp.CurJob.def.alwaysShowWeapon) || (pp.mindState.duty != null && pp.mindState.duty.def.alwaysShowWeapon));
            return true;

        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment", new Type[]
  {
      typeof(Vector3)
  })]
    public static class Harmony_Tank_WeaponYuriDRAW
    {
        public static bool Prefix(PawnRenderer __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pp = traverse.Field("pawn").GetValue<Pawn>();
            if (pp.kindDef.defName.StartsWith("ra2_Yuri"))
            {
               
                return false;
            }
            //return (pp.carryTracker == null || pp.carryTracker.CarriedThing == null) && (pp.Drafted || (pp.CurJob != null && pp.CurJob.def.alwaysShowWeapon) || (pp.mindState.duty != null && pp.mindState.duty.def.alwaysShowWeapon));
            return true;

        }
    }


    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming", new Type[]
{
    typeof(Thing),
    typeof(Vector3),
    typeof(float)
})]
    public static class Harmony_Tank_WeaponSize
    {
        public static bool Prefix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pp = traverse.Field("pawn").GetValue<Pawn>();
            if (pp.kindDef.defName.EqualsIgnoreCase("ra2_rhinotank") || pp.kindDef.defName.EqualsIgnoreCase("ra2_grizzlytank") || pp.kindDef.defName.EqualsIgnoreCase("ra2_apotank"))
            {
                Stance_Busy stance_Busy = pp.stances.curStance as Stance_Busy;
                if (stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid) {
                    drawLoc -=new Vector3(0f, 0f, 0.4f).RotatedBy(aimAngle);
                }
                else
                {

                    if (pp.Rotation == Rot4.South)
                    {
                        drawLoc -= new Vector3(0f, 0f, -0.22f);


                    }
                    else if (pp.Rotation == Rot4.North)
                    {
                        drawLoc -= new Vector3(0f, 0f, -0.11f);


                    }
                    else if (pp.Rotation == Rot4.East)
                    {
                        drawLoc -= new Vector3(0.2f, 0f, -0.22f);


                    }
                    else if (pp.Rotation == Rot4.West)
                    {
                        drawLoc -= new Vector3(-0.2f, 0f, -0.22f);

                    }
                }

                float num = aimAngle - 90f;

               





            Mesh mesh;
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }

                num %= 360f;
                if (!pp.TargetCurrentlyAimingAt.IsValid)
                {
                    //Log.Warning(pp.Rotation+"/");
                    switch (pp.Rotation.AsInt)
                    {
                        case 0:
                            num = -90f;
                            break;
                        case 1:
                            num = 0f;
                            break;
                        case 2:
                            num = 90f;
                            break;
                        case 3:
                            num = 0f;
                            break;
                        default:
                            num = 0;
                            break;
                    }
                    

                   // num =pp.Rotation.AsAngle- 90f;
                }
                Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
            Material matSingle;
            if (graphic_StackCount != null)
            {
                matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
            }
            else
            {
                matSingle = eq.Graphic.MatSingle;
            }
                Matrix4x4 matrix = new Matrix4x4();
                Vector3 s = new Vector3(pp.kindDef.lifeStages[0].bodyGraphicData.drawSize.x,1, pp.kindDef.lifeStages[0].bodyGraphicData.drawSize.y); //new Vector3(5,1,5);
                matrix.SetTRS(drawLoc+new Vector3(0,1,0), num.ToQuat(), s);
                // Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
                Graphics.DrawMesh(mesh, matrix, matSingle, 0);
                return false;
            }




            return true;
        }
    }


 

}
