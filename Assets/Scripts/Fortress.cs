using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fortress : MonoBehaviour {
    #region Attributs publics
    public int life = 100;
    public GameObject lifeUI;
    public GameObject lifeMaxUI;
    #endregion

    #region Attributs privés
    private int lifeMax;
    #endregion

    #region Méthodes publiques
    public void UpdateUI () {
        lifeUI.GetComponent<Text> ().text = "" + life;
        lifeMaxUI.GetComponent<Text> ().text = "" + lifeMax;
    }
    #endregion

    #region Méthodes privées
    void FixedUpdate () {
        
    }

    void Shake () {
        
    }

    void Start () {
        lifeMax = life;
        UpdateUI ();
    }
    #endregion
}