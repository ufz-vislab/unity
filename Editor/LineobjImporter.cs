using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UFZ.Rendering;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UFZ.Editor
{
    // Roughly based on https://github.com/taylorgoolsby/lineobj-importer
    [ScriptedImporter(1, "lineobj")]
    public class LineobjImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            var mainAsset = new GameObject();
            ctx.AddObjectToAsset("main obj", mainAsset);
            ctx.SetMainObject(mainAsset);
            var matProps = mainAsset.AddComponent<MaterialProperties>();
            
            // Parse object
            var v = new List<float[]>();
            var vn = new List<float[]>();
            var f = new List<int[]>();
            var e = new List<int[]>();

            using (StreamReader sr = File.OpenText(ctx.assetPath)) {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    var line = s.Trim().Split(' ');
                    if (line[0].Equals("o")) {
                        assetName = s.Remove(0, 2);
                    }
                    else if (line[0].Equals("prop"))
                    {
                        if (line[1].Equals("color"))
                        {
                            var values = Array.ConvertAll(line.Skip(3).ToArray(), float.Parse);
                            if (line[2].Equals("diffuse"))
                                matProps.SolidColor = new Color(values[0], values[1], values[2]);
                        }
                        else if (line[1].Equals("opacity"))
                            matProps.Opacity = float.Parse(line[2]);
                    }
                    else if (line[0].Equals("v")) {
                        v.Add(Array.ConvertAll(s.Split(' ').Skip(1).ToArray(), float.Parse));
                    } else if (line[0].Equals("vn")) {
                        vn.Add(Array.ConvertAll(s.Split(' ').Skip(1).ToArray(), float.Parse));
                    } else if (line[0].Equals("f")) {
                        line = s.Split(' ');
                        if (line.Length > 4) {
                            Debug.LogError("Your model must be exported with triangulated faces.");
                            continue;
                        }
                        f.Add(Array.ConvertAll(s.Split(' ').Skip(1).ToArray(), int.Parse));
                    } else if (line[0].Equals("l"))
                    {
                        // Convert 1-based obj indices to 0-based
                        // TODO: switch to 0-based? Would break general obj. compatibility
                        e.Add(Array.ConvertAll(line.Skip(1).ToArray(), int.Parse).Select(i => i - 1).ToArray());
                    }
                }
            }
            
            // Construct mesh
            Mesh mesh;
            var triangleSubmeshExists = false;
            if (f.Count > 0 && e.Count > 0) {
                mesh = new Mesh {subMeshCount = 2};
                triangleSubmeshExists = true;
            } else if (f.Count > 0) {
                mesh = new Mesh {subMeshCount = 1};
                triangleSubmeshExists = true;
            } else if (e.Count > 0) {
                mesh = new Mesh {subMeshCount = 1};
            } else {
                return;
            }

            var vertices = new Vector3[v.Count];
            for (var i = 0; i < v.Count; i++) {
                vertices[i] = new Vector3(v[i][0], v[i][1], v[i][2]);
            }
            mesh.vertices = vertices;

            // TODO: subMesh 0 is like a regular mesh which uses MeshTopology.Triangles
            //if (f.Count > 0) {
            //    int[] triangleIndices = new int[f.Count * 3];
            //    for (int i = 0; i < f.Count; i++) {
            //        string[] raw = f[i];
            //        string s1 = raw[0];
            //        string s2 = raw[1];
            //        string s3 = raw[2];
            //        if (s1.Contains("//")) {
            //            s1 = s1.Remove(s1.IndexOf("//"));
            //        }
            //        if (s2.Contains("//")) {
            //            s2 = s2.Remove(s2.IndexOf("//"));
            //        }
            //        if (s3.Contains("//")) {
            //            s3 = s3.Remove(s3.IndexOf("//"));
            //        }
            //        int v1 = int.Parse(s1) - 1;
            //        int v2 = int.Parse(s2) - 1;
            //        int v3 = int.Parse(s3) - 1;
            //        triangleIndices[i * 3] = v1;
            //        triangleIndices[i * 3 + 1] = v2;
            //        triangleIndices[i * 3 + 2] = v3;
            //    }
            //    mesh.SetIndices(triangleIndices, MeshTopology.Triangles, 0, false);
            //    mesh.RecalculateNormals();
            //}
        

            // subMesh 1 is the line mesh which uses MeshTopology.Lines
            if (e.Count > 0) {
                var indicesList = new List<int>();
                foreach (var indices in e) {
                    for (var i = 0; i < indices.Length - 1; i++) {
                        indicesList.Add(indices[i]);
                        indicesList.Add(indices[i+1]);
                    }
                }

                mesh.SetIndices(indicesList.ToArray(), MeshTopology.Lines, triangleSubmeshExists ? 1 : 0, false);
            }

            mesh.RecalculateBounds();
            mesh.name = assetName;
            var meshFilter = mainAsset.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            ctx.AddObjectToAsset("Mesh", mesh);

            mainAsset.AddComponent<MeshRenderer>();
            matProps.ColorBy = MaterialProperties.ColorMode.SolidColor;
            matProps.Lighting = MaterialProperties.LightingMode.Unlit;
        }
    }
}
