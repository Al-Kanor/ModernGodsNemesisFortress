using UnityEngine;
using System.Collections;

public class SpawnZone : MonoBehaviour {
    #region Attributs publics
    public float rate = 1;
    public int proba = 10;    // [proba]% de chance de spawn un ennemi toutes les [rate] secondes
    public GameObject enemyPrefab;
    #endregion

    #region Méthodes privées
    void FixedUpdate () {
        
    }

    IEnumerator SpawnEnemy () {
        do {
            int rand = Random.Range (0, 100);
            if (rand < proba) {
                Instantiate (enemyPrefab, transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds (rate);
        } while (true);
    }

    void Start () {
        StartCoroutine ("SpawnEnemy");
    }
    #endregion
}
