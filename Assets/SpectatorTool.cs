using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// SpectatorTool Position Vector3(-12.2700005,1.14999998,2.86999989)

public class SpectatorTool : MonoBehaviour
{
    public float camMovementSpeed = 5f;
    public float camRotateSpeed = 1f;
    Quaternion camRot;
    GameObject ingameCanvas;
    GameObject playerContainer;
    bool controls = false;
    Text camSpeedText;
    Text moveTimeText;
    GameObject canvas;
    float moveTime = 4f;
    // Start is called before the first frame update
    void Start()
    {
        ingameCanvas = GameObject.Find("IngameCanvas");
        playerContainer = GameObject.Find("PlayerContainer");
        canvas = transform.GetChild(0).gameObject;
        camSpeedText = canvas.transform.GetChild(0).GetComponent<Text>();
        moveTimeText = canvas.transform.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Enable camera and controls
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<Camera>().enabled = !GetComponent<Camera>().enabled;
            playerContainer.SetActive(!playerContainer.activeSelf);
            controls = !controls;
        }

        if(controls)
        {
            // Increase/decrease camera movement speed with scrollwheel
            if (Mathf.Abs((int)Input.mouseScrollDelta.y) > 0)
            {
                float scrollDelta = (float)Input.mouseScrollDelta.y / 2f;

                if (camMovementSpeed + scrollDelta > 0 && camMovementSpeed + scrollDelta < 20)
                {
                    camMovementSpeed += scrollDelta;
                }
                camSpeedText.text = "Cam speed: " + camMovementSpeed.ToString("#.#");
                Debug.Log("Cam movement speed: " + camMovementSpeed);
            }

            if(Input.GetKeyDown(KeyCode.PageUp))
            {
                if(moveTime + 0.5f < 10f)
                {
                    moveTime += 0.5f;
                }
                moveTimeText.text = "Move time: " + moveTime.ToString("#.#");
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                if (moveTime - 0.5f > 0f)
                {
                    moveTime -= 0.5f;
                }
                moveTimeText.text = "Move time: " + moveTime.ToString("#.#");
            }

            // Enable/disable UI by pressing Left Alt
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                ingameCanvas.SetActive(!ingameCanvas.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                canvas.SetActive(!canvas.activeSelf);
            }


            // Increase camera rotating speed with number 1 key
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (camRotateSpeed + 1 * Time.deltaTime < 3)
                {
                    camRotateSpeed += 1 * Time.deltaTime;
                }
                Debug.Log("Cam rotate speed: " + camRotateSpeed);
            }

            // Decrease camera rotating speed with number 2 key
            if (Input.GetKey(KeyCode.Alpha2))
            {
                if (camRotateSpeed - 1 * Time.deltaTime > 0)
                {
                    camRotateSpeed -= 1 * Time.deltaTime;
                }
                Debug.Log("Cam rotate speed: " + camRotateSpeed);
            }

            // Move camera up with Space key
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.up * Time.deltaTime * camMovementSpeed);
            }

            // Move camera down with Left Shift key
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(-Vector3.up * Time.deltaTime * camMovementSpeed);
            }

            // Move camera forwards with W key
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * camMovementSpeed);
            }

            // Move camera backwards with S key
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-Vector3.forward * Time.deltaTime * camMovementSpeed);
            }

            // Move camera left with A key
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-Vector3.right * Time.deltaTime * camMovementSpeed);
            }

            // Move camera right with D key
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * Time.deltaTime * camMovementSpeed);
            }

            // Rotate camera by holding right mouse button and moving mouse
            if (Input.GetMouseButton(1))
            {
                camRot.eulerAngles += new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0f) * camRotateSpeed;
                transform.rotation = camRot;
            }

            // Reset X rotation to 0(Can be combined with camera rotating for level panning shots)
            if (Input.GetKey(KeyCode.X))
            {
                camRot.eulerAngles = new Vector3(0, camRot.eulerAngles.y, 0);
                transform.rotation = camRot;
            }

            if (Input.GetKey(KeyCode.T))
            {
                transform.localPosition = new Vector3(12.25f, -0.797999978f, -1.19000006f);
                transform.localRotation = new Quaternion(0f, 1f, 0f, 0f);
            }

            if (Input.GetKey(KeyCode.Y))
            {
                transform.localPosition = new Vector3(36.7109985f, 5.86299992f, -10.46f);
                transform.localRotation = new Quaternion(0f, -0.707106829f, 0f, 0.707106829f);
            }

            if (Input.GetKey(KeyCode.U))
            {
                transform.localPosition = new Vector3(-2.26999998f, 10.7600002f, -10.46f);
                transform.localRotation = new Quaternion(0, 0.707106829f, 0, 0.707106829f);
            }

            if (Input.GetKey(KeyCode.I))
            {
                transform.localPosition = new Vector3(-1.75199997f, 10.6820002f, -10.2049999f);
                transform.localRotation = new Quaternion(0, 1, 0, 0);
            }

            // Scripted movement
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(ScriptedMovementEvent(moveTime, Vector3.up));
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(ScriptedMovementEvent(moveTime, -Vector3.up));
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(ScriptedMovementEvent(moveTime, Vector3.forward));
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(ScriptedMovementEvent(moveTime, -Vector3.forward));
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                StartCoroutine(ScriptedMovementEvent(moveTime, Vector3.right));
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(ScriptedMovementEvent(moveTime, -Vector3.right));
            }
        }
    }

    IEnumerator ScriptedMovementEvent(float eventTime, Vector3 dir)
    {
        float timer = 0;
        while(timer < eventTime)
        {
            transform.Translate(dir * Time.deltaTime * camMovementSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
