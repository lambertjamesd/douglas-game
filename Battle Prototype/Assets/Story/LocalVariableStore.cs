using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntegerValue
{
    public string name;
    public int value;
}

[System.Serializable]
public class BooleanValue
{
    public string name;
    public bool value;
}

public class LocalVariableStore : VariableStore {
    public IntegerValue[] initialIntegers;
    private Dictionary<string, int> intStorage = new Dictionary<string, int>();

    public BooleanValue[] booleanIntegers;
    private Dictionary<string, bool> boolStorage = new Dictionary<string, bool>();

    public void Start()
    {
        foreach (IntegerValue value in initialIntegers)
        {
            intStorage[value.name] = value.value;
        }

        foreach (BooleanValue value in booleanIntegers)
        {
            boolStorage[value.name] = value.value;
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

    public override bool GetBool(string name)
    {
        if (boolStorage.ContainsKey(name))
        {
            return boolStorage[name];
        }
        else
        {
            return false;
        }
    }

    public override void SetBool(string name, bool value)
    {
        boolStorage[name] = value;
    }
}
