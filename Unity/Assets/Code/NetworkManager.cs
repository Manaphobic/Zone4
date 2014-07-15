using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
		
	public GameObject player;

	// Use this for initialization
	void Start () 
	{
		Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private const string typeName = "Zone4-zfxFR";
	private string gameName = "";
	string ip = "193.11.162.163";
//	string ip = "193.10.185.141";
	string port = "25000";

	private void StartServer()
	{
		Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		gameName = RandomRoomName();
		MasterServer.RegisterHost(typeName, gameName);
		//		MasterServer.ipAddress ="127.0.0.1";
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		SpawnPlayer();
	}
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			ip = GUI.TextArea(new Rect(0,0,100,25),ip,200);
			port = GUI.TextArea(new Rect(100,0,50,25),port,200);
			
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();

			if (GUI.Button(new Rect(100, 400, 250, 100), "JoinServer by ip"))
				JoinByIP();

			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					//Debug.Log(hostList[i].ip + " " + hostList[i].port);
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
	
	
	private HostData[] hostList;
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	private void JoinByIP()
	{
		Network.Connect(ip, System.Int32.Parse(port));
	}

	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		SpawnPlayer();
	}

	private void SpawnPlayer()
	{
//		GameObject p0 = (GameObject)Network.Instantiate(player,new Vector3(0,-0.35f,0),Quaternion.identity,0);
		Network.Instantiate(player,new Vector3(0,-0.35f,0),Quaternion.identity,0);
	}

	private string characters = "abcdefghijklmnopqrstuvwxyz1234567890";
	private string RandomRoomName()	
	{
		string rtn = "";
		for( int i = 0; i < 5; i++ )
		{
			rtn += characters[Random.Range( 0, characters.Length )]; 
		}
		return rtn;
	}
}







