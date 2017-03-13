using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState {
    private Dictionary<string, bool> booleanTypes = new Dictionary<string, bool>();
    private Dictionary<string, double> numberTypes = new Dictionary<string, double>();
    private Dictionary<string, string> stringTypes = new Dictionary<string, string>();

    public void setBoolean(string name, bool boolValue) {
        booleanTypes[name] = boolValue;
    }

    public bool getBoolean(string name) {
        if (booleanTypes.ContainsKey(name)) {
            return booleanTypes[name];
        } else {
            return false;
        }
    }

    public void setNumber(string name, double numberValue) {
        numberTypes[name] = numberValue;
    }

    public double getNumber(string name) {
        if (numberTypes.ContainsKey(name)) {
            return numberTypes[name];
        } else {
            return 0.0;
        }
    }

    public static GameState getGameState(GameObject forObject) {
        var result = new GameState();
        result.setBoolean("has-foo", true);
        return result;
    }
}