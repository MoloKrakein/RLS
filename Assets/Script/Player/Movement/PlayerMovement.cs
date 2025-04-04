
using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDown.Movement
{
    [RequireComponent(typeof(PlayerInput))]

    public class PlayerMovement : Move
    {
        private void OnMove(InputValue value)
        {
            Vector3 playerInput = new Vector3(value.Get<Vector2>().x, value.Get<Vector2>().y, 0);
            currInput = playerInput;
        }

        private void SetSpeed(InputValue value){
            Vector2 playerInput = Mouse.current.scroll.ReadValue();
            scrollInput = playerInput;

        
        }


   
    }
}
