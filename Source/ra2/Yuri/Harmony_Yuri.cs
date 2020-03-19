using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ra2.Yuri
{
    // 尤里光头以及狂兽人肤色预定
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[] { })]
    public static class Harmony_Yuri_Hair
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);

            Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();

            if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuriyuri"))
            {
                if (pawn.story != null)
                {
                    pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved", true);
                    pawn.story.bodyType = BodyTypeDefOf.Thin;
                    pawn.gender = Gender.Male;
                    pawn.story.melanin = 0f;
                }
            }
            else if (pawn.kindDef.defName.EqualsIgnoreCase("ra2_yuribrute")) {
                if (pawn.story != null) {
                    pawn.story.melanin = 10f;
                    pawn.story.bodyType = BodyTypeDefOf.Hulk;
                    
                }
            }


        }


    }
    // 时刻检查控制
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[]{})]
    public static class Harmony_Yuri_Update
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);

           Pawn pawn = traverse.Field("pawn").GetValue<Pawn>();
    
           ModBaseRa2.Instance._controlstorage.checkBeControlerExist(pawn);
            ModBaseRa2.Instance._controlstorage.checkControlerExist(pawn);
          

        }

    }
    //绘画选择线条
    [HarmonyPatch(typeof(SelectionDrawer), "DrawSelectionOverlays", new Type[] { })]
    public static class Harmony_Yuri_Drae_Line
    {
        public static void Postfix()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
               
                Thing thing = obj as Thing;
                if (thing != null)
                {
                    if (thing is Pawn) {
                        if (ModBaseRa2.Instance._controlstorage.BeControlerAndControler.ContainsKey(thing as Pawn)) {
                            Pawn becontroler = thing as Pawn;
                            Pawn controler = ModBaseRa2.Instance._controlstorage.BeControlerAndControler.TryGetValue(becontroler);
                            if(controler.Map==becontroler.Map)
                            HighDrawLineBetween(thing.Position.ToVector3(),controler.Position.ToVector3(), MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.magenta));
                        }else 
                        if (ModBaseRa2.Instance._controlstorage.ControlerAndBeControler.ContainsKey(thing as Pawn))
                        {
                            Pawn controler = thing as Pawn;
                            Pawn becontroler = ModBaseRa2.Instance._controlstorage.ControlerAndBeControler.TryGetValue(controler);
                            if (controler.Map == becontroler.Map)
                               HighDrawLineBetween(becontroler.Position.ToVector3(), controler.Position.ToVector3(), MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.magenta));
                        }



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
            Vector3 pos = (A + B) / 2f;
            if (A == B)
            {
                return;
            }
            A.y = B.y;
            float z = (A - B).MagnitudeHorizontal();
            Quaternion q = Quaternion.LookRotation(A - B);
            Vector3 s = new Vector3(0.2f, 1f, z);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos+new Vector3(0,13,0), q, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
        }

    }

}
