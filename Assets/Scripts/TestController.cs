using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public Rigidbody rb;
    public Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * 500);
        }
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * -1;

        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(moveX, 0, moveZ);
        transform.Rotate(0, mouseX, 0);
        mainCam.transform.Rotate(mouseY, 0, 0);

        if (mainCam.transform.eulerAngles.x > 70 && mainCam.transform.eulerAngles.x < 180)
        {
            mainCam.transform.eulerAngles = new Vector3(70, mainCam.transform.eulerAngles.y, 0);
        }
        if (mainCam.transform.eulerAngles.x < 310 && mainCam.transform.eulerAngles.x > 100)
        {
            mainCam.transform.eulerAngles = new Vector3(310, mainCam.transform.eulerAngles.y, 0);
        }
    }
}
