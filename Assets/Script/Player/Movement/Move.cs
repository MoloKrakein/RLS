using UnityEngine;


namespace TopDown.Movement{
    [RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    private Rigidbody2D rb;
    protected Vector3 currInput;
    [SerializeField] private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate(){
        rb.velocity = currInput * speed * Time.fixedDeltaTime;
    }
}

}
