using HarmonyLib;
using UnityEngine;
using Verse;

namespace ra2
{
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming", typeof(Thing), typeof(Vector3), typeof(float))]
    public static class Harmony_Tank_WeaponSize
    {
        public static bool Prefix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            var traverse = Traverse.Create(__instance);
            var pp = traverse.Field("pawn").GetValue<Pawn>();
            if (!pp.kindDef.defName.EqualsIgnoreCase("ra2_rhinotank") &&
                !pp.kindDef.defName.EqualsIgnoreCase("ra2_grizzlytank") &&
                !pp.kindDef.defName.EqualsIgnoreCase("ra2_apotank"))
            {
                return true;
            }

            if (pp.stances.curStance is Stance_Busy {neverAimWeapon: false} stance_Busy &&
                stance_Busy.focusTarg.IsValid)
            {
                drawLoc -= new Vector3(0f, 0f, 0.4f).RotatedBy(aimAngle);
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

            var num = aimAngle - 90f;


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

            Material matSingle;
            if (eq.Graphic is Graphic_StackCount graphic_StackCount)
            {
                matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
            }
            else
            {
                matSingle = eq.Graphic.MatSingle;
            }

            var matrix = new Matrix4x4();
            var s = new Vector3(pp.kindDef.lifeStages[0].bodyGraphicData.drawSize.x, 1,
                pp.kindDef.lifeStages[0].bodyGraphicData.drawSize.y); //new Vector3(5,1,5);
            matrix.SetTRS(drawLoc + new Vector3(0, 1, 0), num.ToQuat(), s);
            // Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
            Graphics.DrawMesh(mesh, matrix, matSingle, 0);
            return false;
        }
    }
}