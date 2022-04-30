using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private Camera camera;

    float rotX;
    float rotY;

    [SerializeField]
    float moveSpeed = 1.0f;
    [SerializeField]
    float mouseSensitivity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        rotX = transform.rotation.eulerAngles.x;
        rotY = transform.rotation.eulerAngles.y;

        camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        float xMove = Input.GetAxis("Horizontal") * moveSpeed;
        float yMove = Input.GetAxis("YMove") * moveSpeed;
        float zMove = Input.GetAxis("Vertical") * moveSpeed;

        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY += Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotY = Mathf.Clamp(rotY, -90, 90);

        transform.position += (xMove * camera.transform.right.normalized + zMove * camera.transform.forward.normalized + yMove * Vector3.up);
        camera.transform.rotation = Quaternion.AngleAxis(rotX, Vector3.up) * Quaternion.AngleAxis(rotY, Vector3.left);
    }
}
