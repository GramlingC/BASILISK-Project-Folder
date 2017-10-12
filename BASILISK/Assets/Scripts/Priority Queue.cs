using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{

    Dictionary<T, int> elements = new Dictionary<T, int>();
    List<T> visibleElements = new List<T>();


    // Use this for initialization

    public KeyValuePair<T, int> Dequeue()
    {
        T lowestk = visibleElements[0];
        int lowestf = 100;

        bool begun = false;

        foreach (T k in elements.Keys)
        {
            if (!begun)
            {
                lowestf = elements[k];
                lowestk = k;
                begun = true;
            }
            if (elements[k] < lowestf)
            {
                lowestf = elements[k];
                lowestk = k;
            }
        }

        elements.Remove(lowestk);
        visibleElements.Remove(lowestk);
        return new KeyValuePair<T,int>(lowestk,lowestf);
    }

    public void Enqueue(KeyValuePair<T, int> kv)
    {
        elements[kv.Key] = kv.Value;

        if (!visibleElements.Contains(kv.Key))
            visibleElements.Add(kv.Key);
    }

    public int Count()
    {
        return elements.Count;
    }
}