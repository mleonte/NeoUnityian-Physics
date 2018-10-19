#include "NeoUnityian.h"
#include <functional>
#include <iostream>
#include <fstream>

#include <Qt3DIncludes.h>
#include <GaussIncludes.h>
#include <FEMIncludes.h>

//Any extra things I need such as constraints
#include <ConstraintFixedPoint.h>
#include <TimeStepperEulerImplicitLinear.h>

using namespace Gauss;
using namespace FEM;
using namespace ParticleSystem; //For Force Spring

/* Tetrahedral finite elements */

//typedef physical entities I need

//typedef scene
typedef PhysicalSystemFEM<double, NeohookeanTet> FEMLinearTets;

typedef World<double, std::tuple<FEMLinearTets *, PhysicalSystemParticleSingle<double> *>,
	std::tuple<ForceSpringFEMParticle<double> *>,
	std::tuple<ConstraintFixedPoint<double> *> > MyWorld;
typedef TimeStepperEulerImplicitLinear<double, AssemblerEigenSparseMatrix<double>,
	AssemblerEigenVector<double> > MyTimeStepper;


namespace NeoUnityian {

	void getTetFromArrays(Eigen::MatrixXd &V, Eigen::MatrixXi &F, float** vertices, int* vertexCount, int** tets, int* tetCount) {
		V.resize(*vertexCount / 3, 3);

		for (int i = 0; i < *vertexCount; i += 1) {
			V(i / 3, i % 3) = (*vertices)[i];
		}

		F.resize(*tetCount / 4, 4);

		for (int j = 0; j < *tetCount; j += 1) {
			F(j / 4, j % 4) = (*tets)[j];
		}
	}

	int world(void** worldPtr) {
		MyWorld* world = new MyWorld();

		void* voidWorld = (static_cast<void*>(world));
		*worldPtr = voidWorld;
		if (worldPtr != NULL) return 0;
		return -1;
	}

	int addObject(void** worldPtr, float** vertices, int* vertexCount, int** tets, int* tetCount) {
		MyWorld* world = (static_cast<MyWorld*>(*worldPtr));

		Eigen::MatrixXd V;

		Eigen::MatrixXi F;

		getTetFromArrays(V, F, vertices, vertexCount, tets, tetCount);

		FEMLinearTets *test = new FEMLinearTets(V, F);

		world->addSystem(test);
		fixDisplacementMin(*world, test);

		return -1;
	}

	int finalize(void** worldPtr) {
		MyWorld* world = (static_cast<MyWorld*>(*worldPtr));

		world->finalize();

		auto q = mapStateEigen(*world);
		q.setZero();

		return 0;
	}

	int timeStepper(void** stepPtr, float* rate) {
		MyTimeStepper* stepper = new MyTimeStepper(*rate);
		void* voidStep = (static_cast<void*>(stepper));
		*stepPtr = voidStep;
		if (stepPtr != NULL) return 0;
		return -1;
	}

	int step(void** worldPtr, void** stepPtr, float** vertices, int* vertexCount) {
		MyTimeStepper* stepper = (static_cast<MyTimeStepper*>(*stepPtr));
		MyWorld* world = (static_cast<MyWorld*>(*worldPtr));
		stepper->step(*world);

		auto displacement = mapStateEigen<0>(*world);
		for (int i = 0; i < *vertexCount; i++)
			(*vertices)[i] = displacement[i];

		return 0;
	}

	int releasePointer(void** ptr)
	{
		delete *ptr;
		*ptr = NULL;
		return 0;
	}
}
