using UnityEngine;
using System.Collections;
using PlayerIOClient;
using System.Collections.Generic;

public class MultiplayerManager : MonoBehaviour {
    #region Singleton
	static MultiplayerManager mInst;
	static public MultiplayerManager instance { get { return mInst; } }		
	void Awake () {
		if (mInst == null) mInst = this;
		DontDestroyOnLoad(this); 		
	}
    #endregion

    //PlayerIO stuff
	private Connection pioconnection;
	private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>(); //  Messsage queue implementation
	private bool joinedroom = false;
	private PlayerIOClient.Client pioclient;
	public bool isConnected { get { return pioconnection!=null? pioconnection.Connected : false;}}
	public string userId = "";
	
	public bool developmentServer;
	public bool localhost;
    public string ipDevServ = "192.168.1.3";
	
	//Here it begins 
	public void StartConnection(){
		
		string playerId = SystemInfo.deviceUniqueIdentifier;
		
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
	
	void SuccessfullConnect(Client client){
		Debug.Log("Successfully connected to Player.IO");
		
		if(developmentServer) {
			client.Multiplayer.DevelopmentServer = new ServerEndpoint(System.String.IsNullOrEmpty(ipDevServ) ? "192.168.1.96" : ipDevServ,8184);
		}
		if(localhost) {
			client.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1",8184);
		}
		
		//Create or join the room	
		string roomId = "RoomType";
		if(string.IsNullOrEmpty(roomId)){
			roomId = userId;	
		}
		
		client.Multiplayer.CreateJoinRoom (
			roomId,	//Room is the Alliance of the player 
			"Modern-Gods-Genesis",					//The room type started on the server
			false,									//Should the room be visible in the lobby?
			null,   
			null,
			delegate(Connection connection) {
			    Debug.Log("Joined Room : " + roomId);
			    // We successfully joined a room so set up the message handler
			    pioconnection = connection;
			    pioconnection.OnMessage += Handlemessage;
			    pioconnection.OnDisconnect += disconnected; 
			    joinedroom = true;	
		    },
		    delegate(PlayerIOError error) {
			    Debug.LogError("Error Joining Room: " + error.ToString());
		    }
		);
		
		pioclient = client;
	}

	public void Disconnect(){
		if (!pioconnection.Connected) return;
		pioconnection.Disconnect();	
	}
	
	public void disconnected(object sender, string error){
		Debug.LogWarning("Disconnected !"); 	
	}
	
	void FixedUpdate() {
		// process message queue
		foreach(PlayerIOClient.Message m in msgList) {
			Debug.Log(Time.time + " - Message received from server " + m.ToString());
			switch(m.Type) {
				//Basic connection/deconnection
				
				//Lobby Messages
			case "PlayerJoined":
				Debug.Log("PlayerJoined : " + m.GetString(0));
				break; 
			case "PlayerLeft":
				Debug.Log("PlayerLeft : " + m.GetString(0));	
				break;
			case "gameStarted" :
	
				break;
			case "Chat" :
				Debug.Log(m.GetString(0) + ":" + m.GetString(1));
				break;
			}
		}
		
		// clear message queue after it's been processed
		msgList.Clear();
	}
	
	void Handlemessage(object sender, PlayerIOClient.Message m) {
		msgList.Add(m);
	}
	
	void JoinGameRoom (string roomId)
	{
		pioclient.Multiplayer.CreateJoinRoom (
			roomId,				//Room is the Alliance of the player 
			"RoomType",							//The room type started on the server
			false,									//Should the room be visible in the lobby?
			null,
			null,
			delegate(Connection connection) {
			    Debug.Log("Joined Room : " + roomId);
			    // We successfully joined a room so set up the message handler
			    pioconnection = connection;
			    pioconnection.OnMessage += Handlemessage;
			    pioconnection.OnDisconnect += disconnected; 
			    joinedroom = true;
		    },
		    delegate(PlayerIOError error) {			
			    Debug.LogError("Error Joining Room: " + error.ToString());
		    }
		);
	}
	
	// Use this for initialization
	void Start () {
		StartConnection ();
	}
	
	void OnLevelWasLoaded(int level) {
		if (Application.loadedLevelName.Equals ("Game")) {
			
		}
	}
	
	// METHODS SENT TO SERVER
	public void SendStart(){
		Debug.Log ("Sending Start to Server");
		pioconnection.Send("Start");
	}
	
	public void SendChat(string text){
		pioconnection.Send ("Chat", text);
	}
}
