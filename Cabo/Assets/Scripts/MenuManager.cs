using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

/* 
    Class attached to the Main Menu scene, handles the setting of usernames,
    as well as the creating, joining and leaving of a room. 
*/

public class MenuManager : MonoBehaviourPunCallbacks
{
    public List<Menu> menus;
    public TMP_InputField username_input;
    public TMP_InputField roomname_input;
    public Room room_prefab;
    public List<Room> roomList = new List<Room>();
    public LobbyPlayer player_prefab;
    public List<LobbyPlayer> playerList = new List<LobbyPlayer>();
    public GameObject roomListMenu;
    public TMP_Text roomname_text;
    public TMP_Text lobbyname_text;
    public TMP_Text error_text;
    public GameObject lobby; 
    public Button startButton;
    public GameObject regions;
    public string newRegion;
    public TMP_Text regionHint_text;
    public TMP_Text regionPing_text;
    public TMP_Text roomRegionPing_text;


    public static bool firstLogin = true;

    void Start()
    {
        ServerSettings.ResetBestRegionCodeInPreferences();	
        PhotonNetwork.ConnectUsingSettings();
        GameManager.Instance.localSetGameState(GameState.NONE);
        closeAll();
        openMenu("loading");
    }

    string getCurrentRegion()
    {
        //each region can be uniquely identifued based on the first 2 chars
        switch(PhotonNetwork.CloudRegion[0..2])
        {
            case "as":
                return "Singapore:";
            case "au":
                return "Australia:";
            case "us":
                return "USA:";
            case "eu":
                return "Netherlands:";
            case "ca":
                return "Canada:";
            case "in":
                return "India:";
        }
        return "";
    }

    //Menu button functions
     public void Button_usernameClicked()
    {
        if(string.IsNullOrEmpty(username_input.text))
        {
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = username_input.text;
        openMenu("join-select");
    }

    public void Button_createClicked()
    {
        openMenu("roomname");
    }

    public void Button_settingsClicked()
    {
        openMenu("settings");
        string currRegion = PhotonNetwork.CloudRegion;
        Debug.Log("THE FIXED REGION IS " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
        //Fixed region adds two extra chars to the CloudRegion, so remove those
        if(currRegion.Length > 3){ currRegion = PhotonNetwork.CloudRegion[0..^2]; }
        regions.transform.Find(currRegion).GetComponent<Toggle>().isOn = true;
    }

    public void Button_createRoomClicked()
    {
        if(string.IsNullOrEmpty(roomname_input.text))
        {
            return;
        }
        RoomOptions options = new RoomOptions(){IsOpen = true, IsVisible = true, MaxPlayers = 2};
        PhotonNetwork.CreateRoom(roomname_input.text, options);
        Room newRoom = Instantiate(room_prefab);
        newRoom.gameObject.transform.SetParent(roomListMenu.transform, false);
        newRoom.setRoomName(roomname_input.text);
        newRoom.name = roomname_input.text;
        roomList.Add(newRoom);
    }

    public void Button_joinRoomClicked()
    {  
        openMenu("roomlist");
        regionHint_text.text = "Available rooms in the region " + getCurrentRegion();
    }

    public void Button_backFromJoinOrCreate()
    {
        //change region once settings menu is exited
        if(newRegion != null)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = newRegion;
            PhotonNetwork.ConnectUsingSettings();
        }
        else { openMenu("join-select"); }
    }

    public void Button_leaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        openMenu("loading");
    }

    public void Button_userRoomClicked()
    {
        string currRoom = EventSystem.current.currentSelectedGameObject.name;
        PhotonNetwork.JoinRoom(currRoom); 
        openMenu("loading");
    }

    public void Button_startGameClicked()
    {
        if (PhotonNetwork.IsMasterClient) { PhotonNetwork.LoadLevel("Play Level"); }
    }

    public void setRegion()
    {
        Toggle[] toggles = regions.GetComponentsInChildren<Toggle>();
        foreach (Toggle child in toggles)
        {
            if(child.isOn) 
            { 
                child.GetComponent<Image>().color = Color.green; 
                if(!PhotonNetwork.CloudRegion.Contains(child.name)) { newRegion = child.name; }
            }
            else { child.GetComponent<Image>().color = Color.white; }
        }
    }

    //Photon functions
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master in region " + PhotonNetwork.CloudRegion + " with ping " + PhotonNetwork.GetPing());        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();
        openMenu("loading");
        //reset region and set ping indicators
        newRegion = null;
        string pingText = getCurrentRegion() + "\n" + PhotonNetwork.GetPing() + " ms";
        regionPing_text.text = pingText;
        roomRegionPing_text.text = pingText;
    } 

    public override void OnJoinedLobby()
    {
        if(firstLogin)
        {
            openMenu("username");
            firstLogin = false;
        }
        else
        {
            openMenu("join-select");
        }
    }

     public void updatePlayerList()
    {

        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }
        
        foreach(LobbyPlayer oldPlayer in playerList)
        {
            Destroy(oldPlayer.gameObject);
        }
        playerList.Clear();

        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient){
            startButton.interactable = true;
        }
        else { startButton.interactable = false; }

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            LobbyPlayer newPlayer = Instantiate(player_prefab);
            newPlayer.setName(player.Value.NickName);
            newPlayer.transform.SetParent(lobby.transform, false);
            playerList.Add(newPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        lobbyname_text.text = PhotonNetwork.CurrentRoom.Name;
        updatePlayerList();
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
        openMenu("lobby");

    }

    public override void OnPlayerLeftRoom(Player old)
    {
        updatePlayerList();
        startButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        lobbyname_text.text = PhotonNetwork.CurrentRoom.Name;
        updatePlayerList();
        openMenu("lobby"); 
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
	{
		error_text.text = "Creating room failed: " + message;
		openMenu("error");
	}

    public override void OnJoinRoomFailed(short returnCode, string message)
	{
		error_text.text = "Joining room failed: " + message;
		openMenu("error");
	}

    public override void OnRoomListUpdate(List<RoomInfo> photonRoomList)
    {
        //clear out room list
        foreach(Room oldRoom in roomList)
        {
            Destroy(oldRoom.gameObject);
        }
        roomList.Clear();

        //update all available rooms
        foreach(RoomInfo room in photonRoomList)
        {
            if(!room.RemovedFromList)
            {
                Room newRoom = Instantiate(room_prefab);
                newRoom.gameObject.transform.SetParent(roomListMenu.transform, false);
                newRoom.setRoomName(room.Name);
                newRoom.name = room.Name;
                roomList.Add(newRoom);
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if(newRegion != null ) { openMenu("loading"); }
        else 
        { 
            error_text.text = "Lost connection to servers. Please restart the game"; 
            openMenu("error");
        }
    }

    //menu helper functions
    public void openMenu(string name)
    {
        foreach(Menu menu in menus)
        {
            if (menu.menuName == name)
            {
                menu.open();
            }
            else
            {
                menu.close();
            }
        }
    }

    public void closeMenu(Menu menu)
    {
        menu.close();
    }

    public void closeAll()
    {
        foreach(Menu menu in menus)
        {
            menu.close();
        }
    }

}
