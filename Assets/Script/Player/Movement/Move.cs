using UnityEngine;


namespace TopDown.Movement{
    [RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    private Rigidbody2D rb;
    protected Vector3 currInput;
    protected float scrollInput;
    [SerializeField] private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate(){
        // speed = scrollInput * 0.1f;
        rb.velocity = currInput * speed * Time.fixedDeltaTime;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    

}

}
