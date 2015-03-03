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

    #region Accesseurs
    public int Life {
        get { return life; }
        set {
            life = value;
            UpdateUI ();
        }
    }
    #endregion

    #region Méthodes publiques
    public void MakeDamage (int damage) {
        /*
        life = Mathf.Clamp (life - damage, 0, lifeMax);
        UpdateUI ();

        if (life <= 0) {
            Destroy (gameObject);
        }
        */
        MultiplayerManager.instance.SendFortressDamage (damage);
    }

    public void UpdateUI () {
        lifeUI.GetComponent<Text> ().text = "" + life;
        lifeMaxUI.GetComponent<Text> ().text = "" + lifeMax;
    }
    #endregion

    #region Méthodes privées
    void FixedUpdate () {
        
    }

    void Start () {
        lifeMax = life;
        UpdateUI ();
    }
    #endregion
}