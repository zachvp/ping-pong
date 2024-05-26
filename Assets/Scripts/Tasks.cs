using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Task
{
    public static IEnumerator Continuous(Action task)
    {
        while (true)
        {
            task();
            yield return null;
        }
    }

    public static IEnumerator Continuous(Action task, Func<bool> isActive)
    {
        while (isActive())
        {
            task();
            yield return null;
        }
    }

    public static IEnumerator Repeat(float interval, Action task)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            task();
            yield return null;
        }
    }

    public static IEnumerator FixedUpdate(Action task)
    {
        yield return new WaitForFixedUpdate();
        task();
    }

    public static IEnumerator FixedUpdate<T>(T arg0, Action<T> task)
    {
        yield return new WaitForFixedUpdate();
        task(arg0);
    }

    public static IEnumerator Delayed(float delay, Action task)
    {
        yield return new WaitForSeconds(delay);
        task();
    }

    public static IEnumerator Delayed<T>(float delay, T arg0, Action<T> task)
    {
        yield return new WaitForSeconds(delay);
        task(arg0);
    }

    public static IEnumerator Delayed<T, U>(float delay, T arg0, U arg1, Action<T, U> task)
    {
        yield return new WaitForSeconds(delay);
        task(arg0, arg1);
    }
}
