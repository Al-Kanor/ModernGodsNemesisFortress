using UnityEngine;
using System.Collections;
using PlayerIOClient;
using System.Collections.Generic;

public class MultiplayerManager : MonoBehaviour {
    #region Singleton
	static MultiplayerManager mInst;
	static public MultiplayerManager instance { get { return mInst; } }		
	void Awake () {
        if (null == mInst) {
            mInst = this;
        }
		DontDestroyOnLoad(this); 		
	}
    #endregion

    #region Attributs publics
    public bool debug = false;
    public string userId = "";
    public bool localhost;
    public bool developmentServer;
    public string ipDevServ = "192.168.1.3";
    public float serverRate = 0.1f;
    #endregion

    #region Attributs privés
    private Connection pioConnection;
	private List<PlayerIOClient.Message> messages = new List<PlayerIOClient.Message> ();
	private bool joinedRoom = false;
	private PlayerIOClient.Client pioClient;
    private Player player;
    private Fortress fortress;
    #endregion

    #region Accesseurs
    public bool IsConnected {
        get { return pioConnection != null ? pioConnection.Connected : false; }
    }
    #endregion

    #region Méthodes publiques
    public void Disconnect () {
        if (!pioConnection.Connected) return;
        pioConnection.Disconnect ();
    }

    public void Disconnected (object sender, string error) {
        Debug.LogWarning ("Disconnected !");
    }

    public void SendChat (string text) {
        pioConnection.Send ("Chat", text);
    }

    public void SendEnemyDamage (int enemyId, int damage) {
        pioConnection.Send ("Enemy Damage", enemyId, damage);
    }

    public void SendFortressDamage (int damage) {
        pioConnection.Send ("Fortress Damage", damage);
    }

    public void SendStart () {
        Debug.Log ("Sending Start to Server");
        pioConnection.Send ("Start");
    }
	
    public void StartConnection () {
        string playerId;

        if (debug) {
            // SystemInfo.deviceUniqueIdentifier renvoie le même id si la machine lance plusieurs clients
            // En mode debug on génère donc un id aléatoire en utilisant Random.Range()
            playerId = "" + Random.Range (1, 1000000);
        }
        else {
            playerId = SystemInfo.deviceUniqueIdentifier;
        }

		//user is just using this device with no account
		Debug.Log ("Annonymous connect : " + playerId);	
		userId = playerId;
		PlayerIOClient.PlayerIO.Connect (
            "nemesis-fortress-d6ukh1q69kgyacgrmn52pw",	// Game id 
			"public",							// The id of the connection, as given in the settings section of the admin panel. By default, a connection with id='public' is created on all games.
			playerId,							// The id of the user connecting. 
			null,								// If the connection identified by the connection id only accepts authenticated requests, the auth value generated based on UserId is added here
			null,
			null,				
			delegate(Client client) { 
			    SuccessfullConnect (client);
		    },
		    delegate(PlayerIOError error) {
                Debug.Log ("Error connecting: " + error.ToString ());
		    }
		);
	}
    #endregion

    #region Méthode privées
    void FixedUpdate () {
        // Process message queue
        foreach (PlayerIOClient.Message message in messages) {
            //Debug.Log ("Message from server : " + message.ToString ());
            switch (message.Type) {
                case "Player Left":
                    Debug.Log ("PlayerLeft : " + message.GetString (0));
                    break;
                case "Chat":
                    Debug.Log (message.GetString (0) + ":" + message.GetString (1));
                    break;
                case "Debug":
                    Debug.Log ("Message from server : " + message.GetString (0));
                    break;
                case "Enemy Damage":
                    GameObject enemyObject = GameObject.Find ("Enemy" + message.GetInt (0));
                    if (null != enemyObject) {
                        enemyObject.GetComponent<Enemy> ().ApplyDamage (message.GetInt (1));
                    }
                    break;
                case "Fortress Damage":
                    fortress.Life = message.GetInt (0);
                    break;
                case "Position":
                    break;
                case "Enemy Spawn":
                    SpawnManager.instance.SpawnEnemy (message.GetString (0), message.GetInt(1), float.Parse(message.GetString (2)), float.Parse(message.GetString (3)));
                    break;
                case "Player Joined":
                    Debug.Log (message.GetString (0) + " has joined !");
                    break;
                case "Player Position":
                    Debug.Log ("Position of " + message.GetString (0) + " : (" + message.GetFloat (1) + ", " + message.GetFloat (2) + ", " + message.GetFloat (3) + ")");
                    break;
            }
        }

        // clear message queue after it's been processed
        messages.Clear ();
    }

    void HandleMessage (object sender, PlayerIOClient.Message message) {
        messages.Add (message);
    }

    void JoinGameRoom (string roomId) {
        pioClient.Multiplayer.CreateJoinRoom (
            roomId,
            "RoomType",			// The room type started on the server
            false,				// Should the room be visible in the lobby?
            null,
            null,
            delegate (Connection connection) {
                Debug.Log ("Joined Room : " + roomId);
                // We successfully joined a room so set up the message handler
                pioConnection = connection;
                pioConnection.OnMessage += HandleMessage;
                pioConnection.OnDisconnect += Disconnected;
                joinedRoom = true;
            },
            delegate (PlayerIOError error) {
                Debug.LogError ("Error Joining Room : " + error.ToString ());
            }
        );
    }

    void OnLevelWasLoaded (int level) {
        if (Application.loadedLevelName.Equals ("Level")) {

        }
    }

    IEnumerator SendPlayerPosition () {
        do {
            if (joinedRoom) {
                pioConnection.Send ("Player Position", player.transform.position.x, player.transform.position.y, player.transform.position.z, player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z);
            }
            yield return new WaitForSeconds (0.2f);
        } while (true);
    }

    void Start () {
        player = GameObject.Find ("Player").GetComponent<Player> ();
        fortress = GameObject.Find ("Fortress").GetComponent<Fortress> ();
        StartConnection ();
        StartCoroutine ("SendPlayerPosition");
    }
	
    void SuccessfullConnect (Client client) {
        Debug.Log ("Successfully connected to Player.IO");

        if (developmentServer) {
            client.Multiplayer.DevelopmentServer = new ServerEndpoint (System.String.IsNullOrEmpty (ipDevServ) ? "192.168.1.96" : ipDevServ, 8184);
        }

        if (localhost) {
            client.Multiplayer.DevelopmentServer = new ServerEndpoint ("127.0.0.1", 8184);
        }

        // Create or join the room	
        string roomId = "Fortress";
        if (string.IsNullOrEmpty (roomId)) {
            roomId = userId;
        }

        client.Multiplayer.CreateJoinRoom (
            roomId,	//Room is the Alliance of the player 
            "NemesisFortress",					//The room type started on the server
            false,									//Should the room be visible in the lobby?
            null,
            null,
            delegate (Connection connection) {
                Debug.Log ("Joined Room : " + roomId);
                // We successfully joined a room so set up the message handler
                pioConnection = connection;
                pioConnection.OnMessage += HandleMessage;
                pioConnection.OnDisconnect += Disconnected;
                joinedRoom = true;
            },
            delegate (PlayerIOError error) {
                Debug.LogError ("Error Joining Room: " + error.ToString ());
            }
        );

        pioClient = client;
    }
    #endregion
}
