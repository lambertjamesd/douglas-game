using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour {
    public string condition;
    public GameObject toSpawn;

    public void Start() {
        GameObject instance = Instantiate(toSpawn, transform.position, Quaternion.identity) as GameObject;
        instance.transform.parent = transform.parent;
    }
}
