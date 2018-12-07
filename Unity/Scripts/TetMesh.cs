using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using g3;
using System.Linq;
using System.Text.RegularExpressions;

[RequireComponent(typeof(MeshFilter))]
public class TetMesh : MonoBehaviour {
    string vertexPath;
    string tetPath;
    string facePath;
    public Mesh mesh;
    public float[] verticies;
    public int[] tets;
    public int[] faces;
    string meshHash;
    public IntPtr objPtr;
    bool needsRegen;

    // Use this for initialization
    void Awake ()
    {
    }

    public void Generate()
    {
        verticies = null;
        tets = null;
        faces = null;
        meshHash = mesh.GetHashCode().ToString();
        string OFF = WriteOFF();
        WriteTetFiles(OFF);
        ParseMeshFiles();
        MeshReplace();
    }

    public void Load()
    {
        ParseMeshFiles();
        MeshReplace();
    }

    public void ApplyMovement(float[] displacements)
    {
        float[] total = new float[verticies.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            total[i] = verticies[i] + displacements[i];
        }
        GetComponent<MeshFilter>().mesh.vertices = total.ToVectors().ToArray();
    }

    void MeshReplace()
    {
        Mesh newMesh = new Mesh();

        //The triangles tend to come out reversed, so we need to fix them
        DMesh3 dmesh3 = DMesh3Builder.Build<float, int, float>(verticies, faces);
        MeshNormals.QuickCompute(dmesh3);
        dmesh3.ReverseOrientation(false);

        newMesh.vertices = verticies.ToVectors().ToArray();
        newMesh.triangles = dmesh3.TrianglesBuffer.ToArray();
        for (int i = 0; i < mesh.uv.Length; i++)
        {
            newMesh.uv[i] = mesh.uv[i];
        }
        newMesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = newMesh;
    }

    void ParseMeshFiles()
    {
        using (StreamReader sr = new StreamReader(vertexPath))
        {
            string[] line = Regex.Replace(sr.ReadLine(), @"\s+", " ").Trim().Split();
            if (line[1] != "3") throw new InvalidOperationException("Only [X, Y, Z] coordinates are supported.");

            int numVertices = int.Parse(line[0]);
            verticies = new float[numVertices * 3];
            for (int i = 0; i < numVertices * 3; i += 3)
            {
                line = Regex.Replace(sr.ReadLine(), @"\s+", " ").Trim().Split();
                if (line[0] == "#") break;
                verticies[i] = float.Parse(line[1]);
                verticies[i + 1] = float.Parse(line[2]);
                verticies[i + 2] = float.Parse(line[3]);
            }
        }

        using (StreamReader sr = new StreamReader(tetPath))
        {
            string[] line = Regex.Replace(sr.ReadLine(), @"\s+", " ").Trim().Split();
            if (line[1] != "4") throw new InvalidOperationException("Only tetrahedrals are supported.");

            int numTets = int.Parse(line[0]);
            tets = new int[numTets * 4];
            for (int i = 0; i < numTets * 4; i += 4)
            {
                line = Regex.Replace(sr.ReadLine(), @"\s+", " ").Trim().Split();
                if (line[0] == "#") break;
                tets[i] = int.Parse(line[1]);
                tets[i + 1] = int.Parse(line[2]);
                tets[i + 2] = int.Parse(line[3]);
                tets[i + 3] = int.Parse(line[4]);
            }
        }

        using (StreamReader sr = new StreamReader(facePath))
        {
            string[] line = Regex.Replace(sr.ReadLine(), @"\s+", " ").Trim().Split();

            int numFaces = int.Parse(line[0]);
            faces = new int[numFaces * 3];
            for (int i = 0; i < numFaces * 3; i += 3)
            {
                line = Regex.Replace(sr.ReadLine(), @"\s+", " ").Trim().Split();
                if (line[0] == "#") break;
                faces[i] = int.Parse(line[1]);
                faces[i + 1] = int.Parse(line[2]);
                faces[i + 2] = int.Parse(line[3]);
            }
        }
    }

    string WriteOFF()
    {
        IEnumerable<float> coords = mesh.vertices.ToFloats();
        DMesh3 dmesh3 = DMesh3Builder.Build<float, int, float>(coords.ToArray(), mesh.triangles);
        WriteMesh writeMesh = new WriteMesh
        {
            Mesh = dmesh3,
            Name = meshHash
        };

        OFFWriter OFFwriter = new OFFWriter();
        string OFF = Application.dataPath + "/VolumetricMeshes/" + meshHash + "/" + meshHash + ".off";
        Directory.CreateDirectory(Application.dataPath + "/VolumetricMeshes/" + meshHash + "/");
        FileStream OFFfile = File.OpenWrite(OFF);
        TextWriter textWriter = new StreamWriter(OFFfile);
        WriteOptions options = new WriteOptions() { RealPrecisionDigits = 7 };
        
        OFFwriter.Write(textWriter, new List<WriteMesh>() { writeMesh }, options);
        textWriter.Flush();
        textWriter.Close();

        return OFF;
    }

    void WriteTetFiles(string OFF)
    {
        Process p = new Process();
        p.StartInfo.FileName = Application.dataPath + "/Utilities/Tetgen.exe";
        p.StartInfo.Arguments = " -pqa0.5 " + OFF;
        p.Start();
        p.WaitForExit();
        vertexPath = Application.dataPath + "/VolumetricMeshes/" + meshHash + "/" + meshHash + ".1.node";
        tetPath = Application.dataPath + "/VolumetricMeshes/" + meshHash + "/" + meshHash + ".1.ele";
        facePath = Application.dataPath + "/VolumetricMeshes/" + meshHash + "/" + meshHash + ".1.face";
    }

    private void OnDestroy()
    {
        //Destroy(origMesh);
    }
}
