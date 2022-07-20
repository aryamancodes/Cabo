using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager Instance;
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
    // public GameObject lobbyPlayer;
    // public TMP_Text lobbyPlayer_text;
    public Button startButton;


    public bool firstLogin = true;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        closeAll();
        openMenu("loading");
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
    }

    public void Button_backFromJoinOrCreate()
    {
        openMenu("join-select");
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

    //Photon functions
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();
        openMenu("loading");
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
        openMenu("lobby");    
    }

    public override void OnPlayerLeftRoom(Player old)
    {
        updatePlayerList();
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
