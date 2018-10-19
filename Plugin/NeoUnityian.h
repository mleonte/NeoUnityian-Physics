#pragma once  

#ifdef NeoUnityian_EXPORTS  
#define NeoUnityian_API __declspec(dllexport)   
#else  
#define NeoUnityian_API __declspec(dllimport)   
#endif  


namespace NeoUnityian {
	extern "C" {
		/**
			Initializes a world for simulation
		*/
		NeoUnityian_API int world(void** worldPtr);

		/**
			Adds an object to the world based on the given vertices and tetrahedrons, which should be flattened arrays
		*/
		NeoUnityian_API int addObject(void** worldPtr, float** vertices, int* vertexCount, int** tets, int* tetCount);

		/**
			Finalizes the world, must be called prior to stepping
		*/
		NeoUnityian_API int finalize(void** worldPtr);

		/**
			Initializes a time stepper over which to update the world
		*/
		NeoUnityian_API int timeStepper(void** stepPtr, float* rate);

		/**
			Takes a step in the world
		*/
		NeoUnityian_API int step(void** worldPtr, void** stepPtr, float** vertices, int* vertexCount);

		/**
			Releases memory of given pointer
		*/
		NeoUnityian_API int releasePointer(void** ptr);
	}
}