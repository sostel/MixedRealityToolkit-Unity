using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class HistoryBase : MonoBehaviour
{
    protected Dictionary<DateTime, InputMemory> memories { get; set; }

    [SerializeField]
    protected float maxMemoryInSeconds = 3f;

    internal UnityEvent OnHistoryUpdated = new UnityEvent();

    public void Start()
    {
        // A fresh start: Erase memories
        ResetHistory();
    }

    public void Update()
    {
        ForgetOverTime();
    }

    public void ResetHistory()
    {
        memories = new Dictionary<DateTime, InputMemory>();
    }

    public abstract void Remember();

    public void Remember(DateTime timestamp, InputMemory newMemory)
    {
        if (memories == null)
        {
            ResetHistory();
        }

        int result = memories.Keys.Where(x => x > timestamp).Count();
        if (result == 0)
        {
            memories.Add(timestamp, newMemory);

            if (OnHistoryUpdated != null)
            {
                OnHistoryUpdated.Invoke();
            }
        }
    }

    protected void ForgetOverTime()
    {
        if (memories == null)
        {
            ResetHistory();
        }

        DateTime date = DateTime.UtcNow.AddSeconds(-maxMemoryInSeconds);
        List<DateTime> results = memories.Keys.Where(x => x <= date).ToList();

        for (int i = 0; i < results.Count; i++)
        {
            memories.Remove(results[i]);

            if (OnHistoryUpdated != null)
            {
                OnHistoryUpdated.Invoke();
            }
        }
    }

    public InputMemory GetMemoryAt(DateTime date)
    {
        if (memories == null)
        {
            ResetHistory();
        }

        DateTime timestamp = memories.Keys.Where(x => x <= date).Max(); // Warning: Not the most efficient!?
        InputMemory memory;
        bool check = memories.TryGetValue(timestamp, out memory);
        memory.timestamp = timestamp;

        if (check)
        {
            return memory;
        }

        return null;
    }

    public InputMemory GetMemoryBefore(DateTime date)
    {
        if (memories == null)
        {
            ResetHistory();
        }

        IEnumerable<DateTime> dates = memories.Keys.Where(x => x < date);
        if ((dates != null) && (dates.Count() > 0))
        {
            DateTime timestamp = dates.Max(); // Warning: Not the most efficient!?
            InputMemory memory;
            bool check = memories.TryGetValue(timestamp, out memory);
            memory.timestamp = timestamp;

            if (check)
            {
                return memory;
            }
        }
        return null;
    }

    public InputMemory GetMostRecentMemory()
    {
        return GetMemoryAt(DateTime.UtcNow);
    }

    public InputMemory[] GetHistory() 
    {
        return memories.Values.ToArray();
    } 
}