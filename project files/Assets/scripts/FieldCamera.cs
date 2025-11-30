using UnityEngine;

public class FieldCamera : MonoBehaviour
{
    public bool isBeingControlled;
    public float moveSpeed;

    void Update()
    {
        if (isBeingControlled)
        {
            if (Input.GetMouseButton(0)) {
                transform.position += transform.right * moveSpeed * Time.deltaTime * Input.GetAxis("Mouse X") +
                transform.up * moveSpeed * Time.deltaTime * Input.GetAxis("Mouse Y");
            }
            transform.position += transform.forward * moveSpeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetMouseButton(1)) {
                transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * Time.deltaTime * 50, Space.Self);
            }
        }
    }
}
