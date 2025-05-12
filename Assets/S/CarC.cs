using UnityEngine;

public class CarC : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;
    private PathManager pathManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pathManager = FindObjectOfType<PathManager>();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed;
        float turn = Input.GetAxis("Horizontal") * turnSpeed;

        // ����/����
        rb.MovePosition(rb.position + transform.forward * move * Time.fixedDeltaTime);
        // �¿� ȸ��
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * Time.fixedDeltaTime, 0f));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            pathManager.SpawnNewDestination();
        }
    }
}