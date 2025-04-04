
using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDown.Movement
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : Move
    {
        private float baseSpeed = 400f;
        [SerializeField] private float speedIncrement = 1f;
        [SerializeField] private float maxSpeed = 1000f; // Maximum speed limit
        [SerializeField] private float minSpeed = 100f; // Minimum speed limit
        private void OnMove(InputValue value)
        {
            Vector3 playerInput = new Vector3(value.Get<Vector2>().x, value.Get<Vector2>().y, 0);
            currInput = playerInput;
        }

        private void OnScroll(InputValue value){
            float scrollDelta = value.Get<Vector2>().y;
            baseSpeed += scrollDelta * speedIncrement;
            baseSpeed = Mathf.Clamp(baseSpeed, minSpeed, maxSpeed); // Limit the speed to a maximum value

            SetSpeed(baseSpeed);

        
        }


   
    }
}
