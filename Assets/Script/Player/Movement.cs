using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller; // Reference to the CharacterController
    public float speed = 5f; // Movement speed
    // Reference to the MidPoint object
    public GameObject midPoint;

    void Start()
    {
        controller = GetComponent<CharacterController>(); // Get the CharacterController component
    }
    void Update()
    {
        // Handle movement
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(x, y, 0).normalized; // Normalize to prevent faster diagonal movement
        controller.Move(movement * speed * Time.deltaTime); // Move the player using CharacterController

        // Handle rotation
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the mouse position
        Vector3 lookDir = mousePos - transform.position; // Calculate the direction to look at
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg; // Calculate the angle
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Rotate the player

    

        


        
    }
}