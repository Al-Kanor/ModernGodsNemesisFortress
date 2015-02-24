using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
    #region enum publics
    public enum SoundName {
        SHOOT
    };
    #endregion

    #region Attribut privés
    private AudioSource[] sounds;
    #endregion

    #region Singleton
    static SoundManager m_instance;
    static public SoundManager instance { get { return m_instance; } }
    void Awake () {
        if (null == instance) {
            m_instance = this;
        }
        DontDestroyOnLoad (this);
        sounds = GetComponents<AudioSource> ();
    }
    #endregion

    #region Méthodes publiques
    public void PlaySound (SoundName soundName) {
        ToggleSound (soundName, true);
    }

    public void StopSound (SoundName soundName) {
        ToggleSound (soundName, false);
    }
    #endregion

    #region Méthodes privées
    void ToggleSound (SoundName soundName, bool play) {
        int soundIndex = -1;

        switch (soundName) {
            case SoundName.SHOOT:
                soundIndex = 0;
                break;
        }

        if (play) {
            sounds[soundIndex].Play ();
        }
        else {
            sounds[soundIndex].Stop ();
        }
    }
    #endregion
}
