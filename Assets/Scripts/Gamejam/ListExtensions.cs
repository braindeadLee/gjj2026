using UnityEngine;
using System.Collections.Generic;

public static class ListExtensions
{
    /// <summary>
    /// Shuffles the elements of an IList<T> in place using the Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <param name="list">The list to shuffle.</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            // Random.Range for integers has an exclusive maximum value.
            // n + 1 ensures the potential index includes the current n.
            int k = Random.Range(0, n + 1); 
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
