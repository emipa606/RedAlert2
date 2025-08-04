using HarmonyLib;
using UnityEngine;
using Verse;

namespace ra2;

[HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.DrawEquipmentAndApparelExtras))]
public static class PawnRenderUtility_DrawEquipmentAndApparelExtras
{
    public static bool Prefix(Pawn pawn, Vector3 drawPos)
    {
        if (!pawn.kindDef.defName.EqualsIgnoreCase("ra2_rhinotank") &&
            !pawn.kindDef.defName.EqualsIgnoreCase("ra2_grizzlytank") &&
            !pawn.kindDef.defName.EqualsIgnoreCase("ra2_apotank"))
        {
            return true;
        }

        if (pawn.kindDef.defName.StartsWith("ra2_Yuri"))
        {
            return false;
        }

        var num = 0f;
        var aimAngle = 0f;
        if (pawn.stances.curStance is Stance_Busy { neverAimWeapon: true, focusTarg.IsValid: true } stance_Busy)
        {
            var vector = stance_Busy.focusTarg.HasThing
                ? stance_Busy.focusTarg.Thing.DrawPos
                : stance_Busy.focusTarg.Cell.ToVector3Shifted();
            if ((vector - pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
            {
                num = (vector - pawn.DrawPos).AngleFlat();
            }

            var currentEffectiveVerb = pawn.CurrentEffectiveVerb;
            if (currentEffectiveVerb is { AimAngleOverride: not null })
            {
                num = currentEffectiveVerb.AimAngleOverride.Value;
            }

            aimAngle = num;
            drawPos += new Vector3(0f, 0f, 0.4f + pawn.equipment.Primary.def.equippedDistanceOffset).RotatedBy(num);
        }
        else
        {
            if (pawn.Rotation == Rot4.South)
            {
                drawPos -= new Vector3(0f, 0f, -0.22f);
            }
            else if (pawn.Rotation == Rot4.North)
            {
                drawPos -= new Vector3(0f, 0f, -0.11f);
            }
            else if (pawn.Rotation == Rot4.East)
            {
                drawPos -= new Vector3(0.2f, 0f, -0.22f);
            }
            else if (pawn.Rotation == Rot4.West)
            {
                drawPos -= new Vector3(-0.2f, 0f, -0.22f);
            }
        }

        num = aimAngle - 90f;
        var eq = pawn.equipment.Primary;

        Mesh mesh;
        switch (aimAngle)
        {
            case > 20f and < 160f:
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
                break;
            case > 200f and < 340f:
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
                break;
            default:
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
                break;
        }

        num %= 360f;
        if (!pawn.TargetCurrentlyAimingAt.IsValid)
        {
            //Log.Warning(pp.Rotation+"/");
            switch (pawn.Rotation.AsInt)
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
        var s = new Vector3(pawn.kindDef.lifeStages[0].bodyGraphicData.drawSize.x, 1,
            pawn.kindDef.lifeStages[0].bodyGraphicData.drawSize.y); //new Vector3(5,1,5);
        matrix.SetTRS(drawPos + new Vector3(0, 1, 0), num.ToQuat(), s);
        // Graphics.DrawMesh(mesh, drawPos, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
        Graphics.DrawMesh(mesh, matrix, matSingle, 0);
        return false;
    }
}