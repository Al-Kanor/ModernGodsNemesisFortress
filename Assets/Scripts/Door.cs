using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    #region Attributs publics
    public Fortress fortress;
    #endregion

    #region Méthodes privées
    void OnTriggerEnter (Collider other) {
        if ("Enemy" == other.gameObject.tag) {
            Camera.main.GetComponent<WHCameraShake> ().doShake ();
            fortress.MakeDamage (other.GetComponent<Enemy> ().strength);
            other.GetComponent<Enemy> ().Die ();
        }
    }
    #endregion
}
