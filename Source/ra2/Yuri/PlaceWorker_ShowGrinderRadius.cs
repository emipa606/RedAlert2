using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ra2.Yuri
{
    public class PlaceWorker_ShowGrinderRadius : PlaceWorker
    {
        // Token: 0x06004769 RID: 18281 RVA: 0x00216080 File Offset: 0x00214480
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            var unused = Find.CurrentMap;
            // drawit(center.ToVector3()+new Vector3(0.5f,0,1),7f, MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.red));
            GenDraw.DrawFieldEdges(CellsAround(center, Find.CurrentMap), Color.magenta);
        }

        public List<IntVec3> CellsAround(IntVec3 pos, Map map)
        {
            var result = new List<IntVec3>();
            if (!pos.InBounds(map))
            {
                return result;
            }

            var region = pos.GetRegion(map, RegionType.Set_All);
            if (region == null)
            {
                return result;
            }

            RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
            {
                foreach (var item in r.Cells)
                {
                    if (item.InHorDistOf(pos, 6f))
                    {
                        result.Add(item);
                    }
                }

                return false;
            }, 13, RegionType.Set_All);
            return result;
        }

        /*
        public void drawit(Vector3 center, float radius, Material material) {
            int num = Mathf.Clamp(Mathf.RoundToInt(24f * radius), 12, 48);
            float num2 = 0f;
            float num3 = 6.28318548f / (float)num;
            Vector3 vector = center;
            Vector3 a = center;
            for (int i = 0; i < num + 2; i++)
            {
                if (i >= 2)
                {
               
                    Vector3 A = a;
                    Vector3 B = vector;
                    Material mat = material;
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
                    matrix.SetTRS(pos, q, s);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
                }
                a = vector;
                vector = center;
                vector.x += Mathf.Cos(num2) * radius;
                vector.z += Mathf.Sin(num2) * radius;
                num2 += num3;
            }
        }


        */
    }
}