using System.Collections.Generic;
using UnityEngine;


public abstract class RuntimeSetSO<T> : ScriptableObject where T : Component
{
    protected readonly HashSet<T> items = new();
    public IReadOnlyCollection<T> Items => items;

    public void Add(T item)
    {
        items.Add(item);
    }

    public void Remove(T item)
    {
        items.Remove(item);
    }

    public void Clear()
    {
        items.Clear();
    }
}
