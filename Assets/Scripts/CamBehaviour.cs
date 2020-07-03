using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CamBehaviour : MonoBehaviour
{
    private GameObject camObject;
    private MazeGenerator mazeGen;

    private float positionX;
    private float positionY;

    void Start()
    {
        mazeGen = GameObject.Find("Maze Generator").GetComponent<MazeGenerator>();
        camObject = GameObject.Find("Main Camera");
        CreateSize();
    }

    public void CreateSize()
    {
        positionX = mazeGen.xSize;
        positionY = mazeGen.ySize;

        if (positionY > 1 || positionX > 1)
        {
            camObject.transform.position = new Vector3(-0.5f + (positionX * 0.5f), -0.95f + (0.24f * positionY), -10.0f);

            if (positionY > positionX)
            {
                camObject.GetComponent<Camera>().orthographicSize = 0.25f + (0.8f * positionY);
            }
            else
            {
                camObject.GetComponent<Camera>().orthographicSize = 0.25f + (0.8f * positionX);
            }
        }

        // Both 1
        if (positionY == 1 && positionX == 1)
        {
            camObject.transform.position = new Vector3(0.0f, -0.75f, -10.0f);
            camObject.GetComponent<Camera>().orthographicSize = 1;
        }
    }
}
