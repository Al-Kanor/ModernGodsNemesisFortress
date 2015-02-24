using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    #region Attributs publics
    public Fortress fortress;
    #endregion

    #region Méthodes privées
    void OnTriggerEnter (Collider other) {
        if ("Enemy" == other.gameObject.tag) {
            fortress.life--;
            fortress.UpdateUI ();
            other.GetComponent<Enemy> ().Die ();
        }
    }
    #endregion
}
