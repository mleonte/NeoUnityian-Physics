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
    private bool started = false;

    // Use this for initialization
    void Awake () {
        displacementCount = 0;
        world = new World(timeStepRate);
        foreach (var obj in objects)
        {
            obj.objPtr = world.AddObject(obj.verticies, obj.tets);
            displacementCount += obj.verticies.Length;
        }
        world.FinalizeWorld();
        world.displacements = new float[displacementCount];
    }

    // Update is called once per frame
    void Update () {
        if (started)
        {
            world.Step();
            int i = 0;
            foreach (var obj in objects)
            {
                obj.ApplyMovement(world.displacements.Skip(i).Take(obj.verticies.Length).ToArray());
                i += obj.verticies.Length;
            }
        }
    }

    public void StartSimulation()
    {
        started = true;
    }

    public void PauseSimulation()
    {
        started = false;
    }
}
