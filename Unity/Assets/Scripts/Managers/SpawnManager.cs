using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {
    #region Attributs publics
    public float rate = 1;
    public int proba = 10;    // [proba]% de chance de spawn un ennemi toutes les [rate] secondes
    public GameObject simpleEnemyPrefab;
    public GameObject giantEnemyPrefab;
    #endregion

    #region Singleton
    static SpawnManager m_instance;
    static public SpawnManager instance { get { return m_instance; } }
    void Awake () {
        if (null == instance) {
            m_instance = this;
        }
        DontDestroyOnLoad (this);
    }
    #endregion

    #region Méthodes privées
    void FixedUpdate () {
        
    }

    IEnumerator SpawnEnemy () {
        do {
            int rand = Random.Range (0, 100);
            if (rand < proba) {
                float x, z;
                /*
                 * Coordonnées de spawn (Carte vue du ciel, abscisses = x, ordonnées = z) :
                 * -100, [-100, 100] => Colonne de gauche
                 * 100, [-100, 100] => Colonne de droite
                 * [-100, 100], -100 => Ligne du bas
                 * [-100, 100], 100 => Ligne du haut
                 */
                if (0 == Random.Range (0, 2)) {
                    // 1 chance sur 2 de spawn sur une colonne
                    x = 0 == Random.Range (0, 2) ? -100 : 100;  // Colonne gauche / droite
                    z = Random.Range (-100, 100);
                }
                else {
                    // 1 chance sur 2 de spawn sur une ligne
                    x = Random.Range (-100, 100);
                    z = 0 == Random.Range (0, 2) ? -100 : 100;  // Ligne haut / bas
                }

                if (Random.Range (0, 100) < 90) {   // 90% de chance de spawn un ennemi simple
                    Instantiate (simpleEnemyPrefab, new Vector3 (x, 0, z), Quaternion.identity);
                }
                else {  // 10% de chance de spawn un ennemi géant
                    Instantiate (giantEnemyPrefab, new Vector3 (x, 0, z), Quaternion.identity);
                }
            }
            yield return new WaitForSeconds (rate);
        } while (true);
    }

    void Start () {
        StartCoroutine ("SpawnEnemy");
    }
    #endregion
}
