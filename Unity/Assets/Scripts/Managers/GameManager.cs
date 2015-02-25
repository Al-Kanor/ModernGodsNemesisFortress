using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {
    #region Attributs publics
    public GameObject scoreUI;
    #endregion

    #region Attributs privés
    private int score = -1;
    #endregion

    #region Singleton
    static GameManager m_instance;
    static public GameManager instance { get { return m_instance; } }

    void Awake () {
        if (null == instance) {
            m_instance = this;
        }

        DontDestroyOnLoad (this);
    }
    #endregion

    #region Méthodes publiques
    
    #endregion

    #region Méthodes privées
    void Start () {
        Screen.showCursor = false;
        StartCoroutine ("UpdateScore");
    }

    IEnumerator UpdateScore () {
        do {
            score++;
            scoreUI.GetComponent<Text> ().text = "" + score;
            yield return new WaitForSeconds (1);
        } while (true);
    }
    #endregion
}
