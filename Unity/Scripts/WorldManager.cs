using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    public TetMesh[] objects;
    public float timeStepRate;
    private World world;
    private int displacementCount;
    private float[] displacements;

    // Use this for initialization
    void Awake () {
        displacementCount = 0;
        world = new World();
        foreach (var obj in objects)
        {
            world.AddObject(ref obj.verticies, ref obj.tets);
            displacementCount += obj.verticies.Length;
        }
        world.FinalizeWorld();
        displacements = new float[displacementCount];
    }

    // Update is called once per frame
    void Update () {
        world.Step(displacements);
        int i = 0;
        foreach (var obj in objects)
        {
            obj.ApplyMovement(displacements.Skip(i).Take(obj.verticies.Length).ToArray());
            i += obj.verticies.Length;
        }
    }
}
