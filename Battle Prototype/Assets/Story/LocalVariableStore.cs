using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntegerValue
{
    public string name;
    public int value;
}

public class LocalVariableStore : VariableStore {
    public IntegerValue[] initialIntegers;
    private Dictionary<string, int> intStorage = new Dictionary<string, int>();

    public void Start()
    {
        foreach (IntegerValue value in initialIntegers)
        {
            intStorage[value.name] = value.value;
        }
    }

    public override int GetInt(string name)
    {
        if (intStorage.ContainsKey(name))
        {
            return intStorage[name];
        }
        else
        {
            return 0;
        }
    }

    public override void SetInt(string name, int value)
    {
        intStorage[name] = value;
    }
}
