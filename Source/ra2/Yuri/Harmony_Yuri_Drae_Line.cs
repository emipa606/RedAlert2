using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ra2.Yuri;

[HarmonyPatch(typeof(SelectionDrawer), "DrawSelectionOverlays", new Type[] { })]
public static class Harmony_Yuri_Drae_Line
{
    public static void Postfix()
    {
        foreach (var obj in Find.Selector.SelectedObjects)
        {
            if (obj is not Thing thing)
            {
                continue;
            }

            if (thing is not Pawn key)
            {
                continue;
            }

            if (ModBaseRa2.Instance._controlstorage.BeControlerAndControler.ContainsKey(key))
            {
                var controler =
                    ModBaseRa2.Instance._controlstorage.BeControlerAndControler.TryGetValue(key);
                if (controler.Map == key.Map)
                {
                    HighDrawLineBetween(key.Position.ToVector3(), controler.Position.ToVector3(),
                        MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent,
                            Color.magenta));
                }
            }
            else if (ModBaseRa2.Instance._controlstorage.ControlerAndBeControler.ContainsKey(key))
            {
                var controler = key;
                var becontroler =
                    ModBaseRa2.Instance._controlstorage.ControlerAndBeControler.TryGetValue(controler);
                if (controler.Map == becontroler.Map)
                {
                    HighDrawLineBetween(becontroler.Position.ToVector3(), controler.Position.ToVector3(),
                        MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent,
                            Color.magenta));
                }
            }
        }
    }

    public static void HighDrawLineBetween(Vector3 A, Vector3 B, Material mat)
    {
        if (Mathf.Abs(A.x - B.x) < 0.01f && Mathf.Abs(A.z - B.z) < 0.01f)
        {
            return;
        }

        var pos = (A + B) / 2f;
        if (A == B)
        {
            return;
        }

        A.y = B.y;
        var z = (A - B).MagnitudeHorizontal();
        var q = Quaternion.LookRotation(A - B);
        var s = new Vector3(0.2f, 1f, z);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos + new Vector3(0, 13, 0), q, s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
    }
}