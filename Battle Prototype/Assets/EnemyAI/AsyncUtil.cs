using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AsyncManager
{
    private Stack<IEnumerator> enumerators = new Stack<IEnumerator>();
    private object lastValue = null;

    public AsyncManager(IEnumerator enumerator)
    {
        this.enumerators.Push(enumerator);
    }

    private bool HasNext()
    {
        return enumerators.Count > 0;
    }

    public bool Next()
    {
        while (HasNext() && !enumerators.Peek().MoveNext())
        {
            enumerators.Pop();
        }

        if (HasNext())
        {
            object result = enumerators.Peek().Current;

            if (result is IEnumerator)
            {
                enumerators.Push((IEnumerator)result);
            }

            lastValue = result;

            return true;
        }

        return false;
    }

    public IEnumerator Finish()
    {
        while (Next())
        {
            yield return lastValue;
        }
    }
}

public static class AsyncUtil
{
    public static IEnumerator Race(IEnumerable<IEnumerator> allEnumerators)
    {
        AsyncManager[] asyncManagers = allEnumerators.Select(select => new AsyncManager(select)).ToArray();

        while (asyncManagers.All(async => async.Next()))
        {
            yield return null;
        }
    }

    public static IEnumerator All(IEnumerable<IEnumerator> allEnumerators)
    {
        AsyncManager[] asyncManagers = allEnumerators.Select(select => new AsyncManager(select)).ToArray();

        while (asyncManagers.Any(async => async.Next()))
        {
            yield return null;
        }
    }

    public static IEnumerator Pause(float delay)
    {
        float end = Time.time + delay;

        while (end > Time.time)
        {
            yield return null;
        }
    }

    public static IEnumerator WaitUntil(System.Func<bool> until)
    {
        while (!until())
        {
            yield return null;
        }
    }

    public static IEnumerator HandleNested(IEnumerator over)
    {
        AsyncManager manager = new AsyncManager(over);

        while (manager.Next())
        {
            yield return null;
        }
    }
}
