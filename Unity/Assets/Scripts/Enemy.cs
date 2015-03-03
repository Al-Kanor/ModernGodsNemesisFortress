using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    #region Attributs publics
    public int life = 3;
    public int strength = 5;
    public Transform explosion;
    #endregion

    #region Attributs privés
    private int id;
    private NavMeshAgent navMeshAgent;
    private Transform target;
    #endregion

    #region Accesseurs
    public int Id {
        get { return id; }
        set { id = value; }
    }
    #endregion

    #region Méthodes publiques
    public void ApplyDamage (int damage) {
        life -= damage;

        if (life <= 0) {
            Die ();
        }
    }

    public void Die () {
        Instantiate (explosion, transform.position, transform.rotation);
        Destroy (gameObject);
    }

    public void ReceiveDamage (int damage) {
        MultiplayerManager.instance.SendEnemyDamage (id, damage);
    }
    #endregion

    #region Méthodes privées
    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent> ();
        target = GameObject.Find ("Door").transform;
        navMeshAgent.SetDestination (target.position);
    }

    void FixedUpdate () {
        
    }
    #endregion
}