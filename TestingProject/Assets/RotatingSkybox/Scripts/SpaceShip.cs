using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    public float xMin = -2.5f;
    public float xMax = 2.5f;

    public float yMin = -4.5f;
    public float yMax = 4.5f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0f) * speed * Time.deltaTime;
        transform.position += movement;

        float x = Mathf.Clamp(transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(transform.position.y, yMin, yMax);
        transform.position = new Vector3(x, y, 0f);
    }
}
