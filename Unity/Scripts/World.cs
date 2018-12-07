using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class World {

    public float[] displacements;

    private IntPtr worldPtr;
    private IntPtr stepPtr;
    float timeStepRate;
    IntPtr[] objects;
    IntPtr objPtr;

    /**
        Initializes a world for simulation
    */
    [DllImport("NeoUnityian")]
    static extern int world(ref IntPtr world);

    /**
        Adds an object to the world using the given vertices and tetrahedrons, which should be flattened arrays
    */
    [DllImport("NeoUnityian")]
    static extern int addObject(ref IntPtr world, ref IntPtr obj, ref float[] vertices, ref int vertexCount, ref int[] tets, ref int tetCount);

    /**
        Finalizes the world, must be called prior to stepping
    */
    [DllImport("NeoUnityian")]
    static extern int finalize(ref IntPtr world);

    /**
        Initializes a time stepper over which to update the world
    */
    [DllImport("NeoUnityian")]
    static extern int timeStepper(ref IntPtr stepper, ref IntPtr world, ref IntPtr obj, ref float rate);

    /**
        Takes a step in the world
    */
    [DllImport("NeoUnityian")]
    static extern int step(ref IntPtr worldPtr, ref IntPtr stepPtr, ref float dt, ref float[] vertices, ref int vertexCount);

    /**
        Releases memory of given pointer
    */
    [DllImport("NeoUnityian")]
    static extern int releasePointer(ref IntPtr ptr);

    public World(float timeStepRate)
    {
        worldPtr = new IntPtr();
        stepPtr = new IntPtr();
        objPtr = new IntPtr();
        this.timeStepRate = timeStepRate;
        if (world(ref worldPtr) != 0 || worldPtr.ToInt64() == 0)
            throw new Exception("World not initialized!");
        Debug.Log("World setup successful");
        objects = new IntPtr[1];
    }

    public IntPtr AddObject(float[] vertices, int[] tets)
    {
        int vertexCount = vertices.Length;
        int tetCount = tets.Length;
        objects[0] = objPtr;

        addObject(ref worldPtr, ref objPtr, ref vertices, ref vertexCount, ref tets, ref tetCount);
        return objPtr;
    }

    public void FinalizeWorld()
    {
        finalize(ref worldPtr);
        if (timeStepper(ref stepPtr, ref worldPtr, ref objPtr, ref timeStepRate) != 0 || stepPtr.ToInt64() == 0)
            throw new Exception("Stepper not initalized!");
    }
    
    public void Step()
    {
        int vertexCount = displacements.Length;
        float dt = Time.deltaTime;
        
        if (step(ref worldPtr, ref stepPtr, ref dt, ref displacements, ref vertexCount) != 0)
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
