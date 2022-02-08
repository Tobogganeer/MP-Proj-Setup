using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer// : IDisposable
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager instance;

        private List<Timer> timers = new List<Timer>();
        private Queue<Timer> toAdd = new Queue<Timer>();
        private Queue<Timer> toRemove = new Queue<Timer>();

        public void AddTimer(Timer timer)
        {
            toAdd.Enqueue(timer);
        }

        public void RemoveTimer(Timer timer)
        {
            toRemove.Enqueue(timer);
        }

        private void Update()
        {
            while (toAdd.Count > 0)
            {
                Timer timer = toAdd.Dequeue();
                if (!timers.Contains(timer)) timers.Add(timer);
            }

            while (toRemove.Count > 0)
            {
                Timer timer = toRemove.Dequeue();
                if (timers.Contains(timer)) timers.Remove(timer);
            }

            toAdd.Clear();

            toRemove.Clear();

            foreach (Timer timer in timers)
            {
                timer.Update();
            }
        }

        public void ClearTimers(int id)
        {
            if (id == -1) return;

            foreach (Timer timer in timers)
            {
                if (timer.id == id) RemoveTimer(timer);
            }
        }
    }

    Action action;
    float time;
    bool isDestroyed;
    int id;

    private Timer(Action action, float afterTime)
    {
        CheckManagerInstance();

        this.action = action;
        time = afterTime;
        isDestroyed = false;
        id = -1;

        TimerManager.instance.AddTimer(this);
    }

    private Timer(Action action, float afterTime, int id)
    {
        CheckManagerInstance();

        this.action = action;
        time = afterTime;
        isDestroyed = false;
        this.id = id;

        TimerManager.instance.AddTimer(this);
    }

    /// <summary>
    /// Runs the specified action <paramref name="action"/> after time <paramref name="time"/>.
    /// </summary>
    /// <param name="action">The action to run on completion</param>
    /// <param name="time">The time to run the action after</param>
    public static void Run(Action action, float time)
    {
        new Timer(action, time);
    }

    /// <summary>
    /// Runs the specified action <paramref name="action"/> after time <paramref name="time"/>.
    /// </summary>
    /// <param name="action">The action to run on completion</param>
    /// <param name="time">The time to run the action after</param>
    /// <param name="id">The id of the timer (for referencing later)</param>
    public static void Run(Action action, float time, int id)
    {
        new Timer(action, time, id);
    }

    public void Update()
    {
        if (!isDestroyed)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                action?.Invoke();
                Dispose();
            }
        }
    }

    public void Dispose()
    {
        //Dispose(true);
        isDestroyed = true;
        TimerManager.instance.RemoveTimer(this);

        GC.SuppressFinalize(this);
    }

    public static void ClearTimers(int id)
    {
        if (id == -1) return;

        CheckManagerInstance();

        TimerManager.instance.ClearTimers(id);
    }

    private static void CheckManagerInstance()
    {
        if (TimerManager.instance == null)
        {
            TimerManager.instance = new GameObject("TimerManager", typeof(TimerManager)).GetComponent<TimerManager>();
        }
    }

    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!isDestroyed)
    //    {
    //        if (!disposing)
    //        {
    //
    //        }
    //
    //        isDestroyed = true;
    //    }
    //}
}
