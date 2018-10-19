cmake_minimum_required (VERSION 3.2)
include(${CMAKE_ROOT}/Modules/ExternalProject.cmake)

project (NeoUnityian)

add_library(NeoUnityian SHARED NeoUnityian.cpp)

target_link_libraries(NeoUnityian Core Base FEM UI ${GAUSS_LIBS})