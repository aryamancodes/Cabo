using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public void open()
    {
        gameObject.SetActive(true);
    }

    public void close()
    {
        gameObject.SetActive(false);
    }

}
