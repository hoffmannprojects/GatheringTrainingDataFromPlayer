using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class Helpers 
{

    /// <summary>
    /// Round to the nearest .5 value.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float RoundToPointFive (float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2f;
    }

    /// <summary>
    /// Maps a value from an original range to a new range.
    /// </summary>
    /// <returns></returns>
    public static float Map (float newFrom, float newTo, float originalFrom, float originalTo, float value)
    {
        if (value <= originalFrom)
            return newFrom;
        else if (value >= originalTo)
            return newTo;
        return (newTo - newFrom) * ((value - originalFrom) / (originalTo - originalFrom)) + newFrom;
    }
}