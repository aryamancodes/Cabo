using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyPlayer : MonoBehaviourPunCallbacks
{

    public TMP_Text playerName;

    public void setName(string name)
    {
        playerName.text = name;
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

}
