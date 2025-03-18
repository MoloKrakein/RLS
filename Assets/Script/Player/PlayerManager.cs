using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the player prefab
    public Camera CameraManager; // Reference to the CameraManager object
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // make the camera follow the player
        CameraManager.transform.position = new Vector3(playerPrefab.transform.position.x, playerPrefab.transform.position.y, -10);
        
        // when mouse touching the edge of the screen, move the camera a bit
        if (Input.mousePosition.x >= Screen.width - 1)
        {
            CameraManager.transform.position += new Vector3(0.1f, 0, 0);
        }
        if (Input.mousePosition.x <= 1)
        {
            CameraManager.transform.position -= new Vector3(0.1f, 0, 0);
        }
        if (Input.mousePosition.y >= Screen.height - 1)
        {
            CameraManager.transform.position += new Vector3(0, 0.1f, 0);
        }
        if (Input.mousePosition.y <= 1)
        {
            CameraManager.transform.position -= new Vector3(0, 0.1f, 0);
        }
        
    }
}
