using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy
public class GroundShadow : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject shadow;
    GameObject shadowObj;
    LayerMask mask;

    void Start()
    {
        mask = LayerMask.GetMask("Geometry");
        //shadowObj = Instantiate(shadow);
        //shadowObj.transform.parent = transform;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out groundRay, 1000, mask))
        {
            if (shadowObj != null)
            {
                shadowObj.transform.position = new Vector3(groundRay.point.x, groundRay.point.y + 0.0001f, groundRay.point.z);
            }
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * groundRay.distance, Color.red);
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * (transform.lossyScale.y + 0.3f), Color.red);
        }
    }

    RaycastHit groundRay;
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out groundRay, 1000, mask))
        {
            if (shadowObj != null)
            {
                shadowObj.transform.position = new Vector3(groundRay.point.x, groundRay.point.y + 0.0001f, groundRay.point.z);
            }
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * groundRay.distance, Color.red);
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * (transform.lossyScale.y + 0.3f), Color.red);
        }
    }
    private void OnEnable()
    {
        if (shadowObj == null)
        {
            //Debug.Log("DOIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            shadowObj = Instantiate(shadow);
            shadowObj.transform.parent = transform;
        }
    }
    private void OnDisable()
    {
        if (shadowObj != null)
        {
            Destroy(shadowObj);
        }
    }
    private void OnDestroy()
    {
        if (shadowObj != null)
        {
            Destroy(shadowObj);
        }
    }
}
