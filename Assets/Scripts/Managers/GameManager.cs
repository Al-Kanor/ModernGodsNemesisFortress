using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
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

    #region Méthodes privées
    void Start () {
        Screen.showCursor = false;
    }
    #endregion
}
