using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateMainMenu : MonoBehaviour
{
    public List<Sprite> sprites;
    public Image image;
    public RectTransform panelLeft;
    public RectTransform panelRight;

    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        //use the bottom left corner as a reference for spawning
        Vector3 leftReference = GetBottomLeftCorner(panelLeft);
        Vector3 rightReference = GetBottomLeftCorner(panelRight);
        bool even = true;
        foreach(var sprite in sprites)
        {
            if(even)
            {
                var spawnPositionLeft = leftReference - new Vector3(Random.Range(0, panelLeft.rect.x), Random.Range(0, panelRight.rect.y), 0);
                var spawnRotation =  Quaternion.Euler(new Vector3(0,0, Random.Range(-360f, 360f)));
                var child = Instantiate(image, spawnPositionLeft, spawnRotation, panelLeft);
                child.sprite = sprite;
                even = false;            

            }

            else
            {
               var spawnPositionRight = rightReference - new Vector3(Random.Range(0, panelLeft.rect.x), Random.Range(0, panelRight.rect.y), 0);
                var spawnRotation =  Quaternion.Euler(new Vector3(0,0, Random.Range(-360f, 360f)));
                var child = Instantiate(image, spawnPositionRight, spawnRotation, panelRight);
                child.sprite = sprite;
                even = true;       
            }
        }
    }

    Vector3 GetBottomLeftCorner(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        return v[0];
    }
}
