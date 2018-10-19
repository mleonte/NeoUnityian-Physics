using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils {

	public static IEnumerable<float> ToFloats(this IEnumerable<Vector3> vectors)
    {
        foreach (var v in vectors)
        {
            yield return v.x;
            yield return v.y;
            yield return v.z;
        }
    }

    public static IEnumerable<Vector3> ToVectors(this IEnumerable<float> floats)
    {
        for (int i = 0; i < floats.Count(); i += 3)
            yield return new Vector3(
                floats.ElementAt(i), 
                floats.ElementAt(i + 1), 
                floats.ElementAt(i + 2));
    }
}
