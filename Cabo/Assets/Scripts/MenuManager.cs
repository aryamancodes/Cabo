using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager Instance;
    public List<Menu> menus;
    public TMP_InputField username_input;
    public TMP_InputField roomname_input;
    public GameObject roomList;
    public Button room;
    public TMP_Text roomname_text;
    public TMP_Text lobbyname_text;
    public GameObject lobby;
    public GameObject lobbyPlayer;
    public TMP_Text lobbyPlayer_text;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Debug.Log("Connecting to master");
        //PhotonNetwork.ConnectUsingSettings();
        closeAll();
        //REMOVE ONCE ROGERS STOPS FUCKING BEING ASS
        openMenu("username");
    }

    //Menu button functions
     public void Button_usernameClicked()
    {
        if(string.IsNullOrEmpty(username_input.text))
        {
            return;
        }
        openMenu("join-select");
    }

    public void Button_createClicked()
    {
        openMenu("roomname");
    }

    public void Button_createRoomClicked()
    {
        //TODO: Check for duplicate room names
        if(string.IsNullOrEmpty(roomname_input.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomname_input.text);
        Button newRoom = Instantiate(room, new Vector2(0,0), Quaternion.Euler(new Vector3(0,0,90)));
        TMP_Text newRoomText = Instantiate(roomname_text, new Vector2(0,0), Quaternion.Euler(new Vector3(0,0,-90))); 
        newRoom.name = roomname_input.text;
        newRoomText.text = roomname_input.text;
        newRoom.transform.SetParent(roomList.transform, false);
        newRoomText.transform.SetParent(newRoom.transform, false);
    }

    public void Button_joinRoomClicked()
    {  
        openMenu("roomlist");
    }

    public void Button_backFromJoinOrCreate()
    {
        openMenu("join-select");
    }


    //TODO: CAN THESE 2 FNS BE ONE W PHOTON?
    public void Button_userRoomClicked()
    {
        lobbyname_text.text = EventSystem.current.currentSelectedGameObject.name;
        GameObject newPlayer = Instantiate(lobbyPlayer, new Vector2(0,0), Quaternion.Euler(new Vector3(0,0,90)));
        TMP_Text newPlayerText =  Instantiate(lobbyPlayer_text, new Vector2(0,0), Quaternion.Euler(new Vector3(0,0,-90))); 
        newPlayerText.text = username_input.text;
        newPlayer.transform.SetParent(lobby.transform, false);
        newPlayerText.transform.SetParent(newPlayer.transform, false);
        openMenu("lobby");
    }

    public void Button_userRoomCreated()
    {
        lobbyname_text.text = roomname_input.text;
        GameObject newPlayer = Instantiate(lobbyPlayer, new Vector2(0,0), Quaternion.Euler(new Vector3(0,0,90)));
        TMP_Text newPlayerText =  Instantiate(lobbyPlayer_text, new Vector2(0,0), Quaternion.Euler(new Vector3(0,0,-90))); 
        newPlayerText.text = username_input.text;
        newPlayer.transform.SetParent(lobby.transform, false);
        newPlayerText.transform.SetParent(newPlayer.transform, false);
        openMenu("lobby");
    }

    //Photon functions
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        openMenu("username");
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
