using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour {
    public string condition = "";
    public GameObject toSpawn;

    public void Start() {
        if (condition == "" || ConditionParser.parseCondition(condition).evaluate(GameState.getGameState(gameObject))) {
            GameObject instance = Instantiate(toSpawn, transform.position, Quaternion.identity) as GameObject;
            instance.transform.parent = transform.parent;
        }

    }
}
