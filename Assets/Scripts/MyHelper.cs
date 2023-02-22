using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyHelper
{
    public static T[] GetRandomItems<T>(List<T> list, int n)
    {
        T[] result = new T[n];
        int length = list.Count;

        for (int i = 0; i < n; i++)
        {
            // Choose a random index between i and the end of the list
            int randomIndex = Random.Range(i, length);

            // Swap the elements at i and the random index
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;

            // Add the i-th element to the result list
            result[i] = list[i];
        }

        return result;
    }

}
