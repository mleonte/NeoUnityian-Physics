using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class World {

    private IntPtr worldPtr;
    private IntPtr stepPtr;
    float timeStepRate = 0.5f;
    public float[] displacements;

    /**
        Initializes a world for simulation
    */
    [DllImport("NeoUnityian")]
    static extern int world(ref IntPtr world);

    /**
        Adds an object to the world using the given vertices and tetrahedrons, which should be flattened arrays
    */
    [DllImport("NeoUnityian")]
    static extern int addObject(ref IntPtr world, ref float[] vertices, ref int vertexCount, ref int[] tets, ref int tetCount);

    /**
        Finalizes the world, must be called prior to stepping
    */
    [DllImport("NeoUnityian")]
    static extern int finalize(ref IntPtr world);

    /**
        Initializes a time stepper over which to update the world
    */
    [DllImport("NeoUnityian")]
    static extern int timeStepper(ref IntPtr stepper, ref float rate);

    /**
        Takes a step in the world
    */
    [DllImport("NeoUnityian")]
    static extern int step(ref IntPtr worldPtr, ref IntPtr stepPtr, ref float[] vertices, ref int vertexCount);

    /**
        Releases memory of given pointer
    */
    [DllImport("NeoUnityian")]
    static extern int releasePointer(ref IntPtr ptr);

    public World(float timeStepRate)
    {
        worldPtr = new IntPtr();
        stepPtr = new IntPtr();
        if (world(ref worldPtr) != 0 || worldPtr.ToInt64() == 0)
            throw new Exception("World not initialized!");
        if (timeStepper(ref stepPtr, ref timeStepRate) != 0 || stepPtr.ToInt64() == 0)
            throw new Exception("Stepper not initalized!");
        Debug.Log("World setup successful");
    }

    public void AddObject(float[] vertices, int[] tets)
    {
        int vertexCount = vertices.Length;
        int tetCount = tets.Length;

        addObject(ref worldPtr, ref vertices, ref vertexCount, ref tets, ref tetCount);
    }

    public void FinalizeWorld()
    {
        finalize(ref worldPtr);
    }
    
    public void Step()
    {
        int vertexCount = displacements.Length;
        
        if (step(ref worldPtr, ref stepPtr, ref displacements, ref vertexCount) != 0)
            throw new Exception("Step unsuccessful");

    }

    ~World()
    {
        releasePointer(ref worldPtr);
        releasePointer(ref stepPtr);
    }

    public static implicit operator bool(World foo)
    {
        return !object.ReferenceEquals(foo, null);
    }
}
