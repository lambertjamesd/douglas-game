using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	public GameObject enemy;
	public float spawnDelay = float.PositiveInfinity;
	public float initialDelay = 0.0f;

	void Start() {
		if (spawnDelay != float.PositiveInfinity) {
			StartCoroutine(SpawnAway());
		} else {
			CreateEnemy();
		}
	}

	private void CreateEnemy() {
		Instantiate(enemy, transform.position, transform.rotation);
	}

	private IEnumerator SpawnAway() {
		yield return new WaitForSeconds(spawnDelay + initialDelay);
		while (gameObject != null) {
			CreateEnemy();
			yield return new WaitForSeconds(spawnDelay);
		}
	}
}
