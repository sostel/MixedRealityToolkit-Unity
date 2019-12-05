﻿using System;
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
        // Remember();
        ForgetOverTime();

        if (OnHistoryUpdated != null)
        {
            OnHistoryUpdated.Invoke();
        }
    }

    public void ResetHistory()
    {
        memories = new Dictionary<DateTime, InputMemory>();
    }

    public abstract void Remember();

    public void Remember(DateTime timestamp, InputMemory newMemory)
    {
        int result = memories.Keys.Where(x => x > timestamp).Count();

        if (result == 0)
        {
            memories.Add(timestamp, newMemory);
        }
    }

    protected void ForgetOverTime()
    {
        DateTime date = DateTime.UtcNow.AddSeconds(-maxMemoryInSeconds);
        List<DateTime> results = memories.Keys.Where(x => x <= date).ToList();

        for (int i = 0; i < results.Count; i++)
        {
            memories.Remove(results[i]);
        }
    }

    public InputMemory GetMemoryAt(DateTime date)
    {
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

    public InputMemory GetMostRecentMemory()
    {
        return GetMemoryAt(DateTime.UtcNow);
    }
}