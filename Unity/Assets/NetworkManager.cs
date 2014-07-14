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
	
	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";
	string ip = "193.11.162.163";
	string port = "25000";

	private void StartServer()
	{
		Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		//		MasterServer.ipAddress ="127.0.0.1";
	}
	
	void OnServerInitialized()
	{
//		Screen.showCursor = false;
//		Screen.lockCursor = true;
		Debug.Log("Server Initializied");
		SpawnPlayer();
	}
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			ip = GUI.TextArea(new Rect(0,0,100,25),ip,200);
			port = GUI.TextArea(new Rect(100,0,50,25),port,200);
//			if (GUI.Button(new Rect(0, 0, 100,75), "Farmer"))
//				classType = 0;
//			if (GUI.Button(new Rect(100, 0, 100, 75), "Builder"))
//				classType = 1;
//			if (GUI.Button(new Rect(200, 0, 100, 75), "Engineer"))
//				classType = 2;
//			if (GUI.Button(new Rect(300, 0, 100, 75), "Alchemist"))
//				classType = 3;
			
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
					Debug.Log(hostList[i].ip + " " + hostList[i].port);
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
//		Screen.showCursor = false;
//		Screen.lockCursor = true;
		SpawnPlayer();
	}
	
	
	private void SpawnPlayer()
	{
		GameObject p0 = (GameObject)Network.Instantiate(player,new Vector3(0,-0.35f,0),Quaternion.identity,0);
//		if ( classType == 0 )
//			p0.GetComponent<myPlayer>().classType = myPlayer.FARMER;
//		if ( classType == 1 )
//			p0.GetComponent<myPlayer>().classType = myPlayer.BUILDER;
//		if ( classType == 2 )
//			p0.GetComponent<myPlayer>().classType = myPlayer.ENGINEER;
//		if ( classType == 3 )
//			p0.GetComponent<myPlayer>().classType = myPlayer.ALCHEMIST;
	}
}
