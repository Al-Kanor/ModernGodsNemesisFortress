using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    #region Attributs publics
    public int life = 3;
    public int strength = 5;
    public Transform explosion;
    #endregion

    #region Attributs privés
    private NavMeshAgent navMeshAgent;
    private Transform target;
    #endregion

    #region Méthodes publiques
    void MakeDamage (int damage) {
        life -= damage;

        if (life <= 0) {
            Die ();
        }
    }
    #endregion

    #region Méthodes privées
    void Die () {
        Instantiate (explosion, transform.position, transform.rotation);
        Destroy (gameObject);
    }

    void OnCollisionEnter (Collision other) {
        if ("Fortress" == other.gameObject.tag) {
            other.gameObject.GetComponent<Fortress> ().life -= strength;
            Die ();
        }
    }

    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent> ();
        target = GameObject.FindGameObjectWithTag ("Fortress").transform;
        navMeshAgent.SetDestination (target.position);
    }

    void FixedUpdate () {
        
    }
    #endregion
}