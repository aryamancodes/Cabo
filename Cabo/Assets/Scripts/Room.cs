using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Room : MonoBehaviour
{

    public TMP_Text roomName;

    public void setRoomName(string name)
    {
        roomName.text = name;
    }
}
