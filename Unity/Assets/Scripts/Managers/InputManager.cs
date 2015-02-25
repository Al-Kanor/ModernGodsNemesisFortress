using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    #region Méthodes privées
    void FixedUpdate () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
    }
    #endregion
}
