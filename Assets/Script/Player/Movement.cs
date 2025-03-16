using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller; // Reference to the CharacterController
    public float speed = 5f; // Movement speed

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

        // Handle rotation to face the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}