using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
    #region Attributs publics
    public int damage = 1;  // Dégats du tir
    public float rate = 0.3f;   // Cadence de tir
    public float strength = 1000;   // Force de l'impact sur la cible
    public float scope = 1000; // Portée du tir
    public GameObject impact;   // Particule de l'impact
    public Fortress fortress;
    #endregion

    #region Attribut privés
    private bool canFire = true;
    private float timer;
    #endregion

    #region Méthodes privées
    void Fire () {
		Vector3 direction = transform.TransformDirection (Vector3.forward);
		RaycastHit hit;
		
		if (Physics.Raycast(transform.position, direction, out hit, scope)) {
			Quaternion tempRot = Quaternion.FromToRotation (Vector3.up, hit.normal);
			Instantiate (impact, hit.point, tempRot);

            switch (hit.collider.gameObject.tag) {
                case "Enemy":
                    hit.collider.gameObject.GetComponent<Enemy> ().ReceiveDamage (damage);
                    break;
                case "Fortress":
                    fortress.ReceiveDamage (damage);
                    break;
            }
        }

        SoundManager.instance.PlaySound (SoundManager.SoundName.SHOOT);
    }

    void Start () {
        timer = rate;
    }
        
    void FixedUpdate () {
        if (timer <= 0) {
            canFire = true;
        }
        else {
            timer -= Time.fixedDeltaTime;
        }

        if (Input.GetButton ("Fire1")) {
            if (canFire) {
                Fire ();
                canFire = false;
                timer = rate;
            }
        }
    }
    #endregion
}
