using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VariableStore : MonoBehaviour {
    public abstract int GetInt(string name);
    public abstract void SetInt(string name, int value);
}