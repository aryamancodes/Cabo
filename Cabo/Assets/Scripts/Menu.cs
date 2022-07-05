using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    private bool isOpen;
    public void open()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }

    public void close()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }

}
