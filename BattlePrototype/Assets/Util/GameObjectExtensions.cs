using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions {
    public static T GetComponentInHeirarchy<T>(this GameObject self) where T : class
    {
        GameObject current = self;

        while (current != null)
        {
            T result = current.GetComponent<T>();

            if (result != null)
            {
                return result;
            }

            if (current.transform.parent != null)
            {
                current = current.transform.parent.gameObject;
            }
            else
            {
                current = null;
            }
        }

        return null;
    }
}
