using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisHitEffect : MonoBehaviour
{

    float hitBackAlpha;
    float hitTopAlpha;
    float hitRightAlpha;
    float hitLeftAlpha;

    public GameObject hitBackObject;
    public GameObject hitTopObject;
    public GameObject hitRightObject;
    public GameObject hitLeftObject;

    // Start is called before the first frame update
    void Start()
    {

        hitBackAlpha = 0;
        hitTopAlpha = 0;
        hitRightAlpha = 0;
        hitLeftAlpha = 0;
        hitBackObject.GetComponent<CanvasGroup>().alpha = hitBackAlpha;
        hitTopObject.GetComponent<CanvasGroup>().alpha = hitTopAlpha;
        hitRightObject.GetComponent<CanvasGroup>().alpha = hitRightAlpha;
        hitLeftObject.GetComponent<CanvasGroup>().alpha = hitLeftAlpha;
    }



    // Update is called once per frame
    void Update()
    {

        hitBackObject.GetComponent<CanvasGroup>().alpha = hitBackAlpha;
        hitTopObject.GetComponent<CanvasGroup>().alpha = hitTopAlpha;
        hitRightObject.GetComponent<CanvasGroup>().alpha = hitRightAlpha;
        hitLeftObject.GetComponent<CanvasGroup>().alpha = hitLeftAlpha;


        hitBackAlpha -= Time.deltaTime * 5;
        hitBackAlpha = Mathf.Clamp(hitBackAlpha, 0, 1);
        hitTopAlpha -= Time.deltaTime * 5;
        hitTopAlpha = Mathf.Clamp(hitTopAlpha, 0, 1);
        hitRightAlpha -= Time.deltaTime * 5;
        hitRightAlpha = Mathf.Clamp(hitRightAlpha, 0, 1);
        hitLeftAlpha -= Time.deltaTime * 5;
        hitLeftAlpha = Mathf.Clamp(hitLeftAlpha, 0, 1);


        if (Input.GetKey(KeyCode.J))
        {
            hitBackAlpha = 1;
        }

        /*else
        {
            hitBackAlpha = 0;
        }*/

        if (Input.GetKey(KeyCode.U))
        {
            hitTopAlpha = 1;
        }

       /* else
        {
            hitTopAlpha = 0;
        }*/

        if (Input.GetKey(KeyCode.K))
        {
            hitRightAlpha = 1;
        }

        /*else
        {
            hitRightAlpha = 0;
        }*/

        if (Input.GetKey(KeyCode.H))
        {
            hitLeftAlpha = 1;
        }

        /*else
        {
            hitLeftAlpha = 0;
        }*/

    }
}