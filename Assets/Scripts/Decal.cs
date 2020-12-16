using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.HighDefinition;

// Eelis

public class Decal : MonoBehaviour
{

    // Time to live
    public float ttl = 10f;
    float aliveTime = 0f;
    public bool bloodSplat = false;
    SpriteRenderer spriteRenderer;
    //DecalProjector projector;
    public bool destroy = false;
    public float fadeTime = 5f;
    AnimationCurve sizeOverTime;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sizeOverTime = AnimationCurve.Linear(0, Random.Range(0.075f, 0.1f), 10, Random.Range(0.1f, 0.17f));
        transform.localScale = new Vector3(sizeOverTime.Evaluate(aliveTime), sizeOverTime.Evaluate(aliveTime), sizeOverTime.Evaluate(aliveTime));
        spriteRenderer.enabled = true;
        //projector = GetComponent<DecalProjector>();
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime += Time.deltaTime;
        ttl -= Time.deltaTime;
        if(bloodSplat)
        {
            if(spriteRenderer != null)
            {
                transform.localScale = new Vector3(sizeOverTime.Evaluate(aliveTime), sizeOverTime.Evaluate(aliveTime), sizeOverTime.Evaluate(aliveTime));
            }
        }
        //if (bloodSplat)
        //{
        //    if (transform.localScale.x < 0.2f)
        //    {
        //        transform.localScale += new Vector3(0.01f, 0.01f, 0) * Time.deltaTime;
        //    }
        //}
        if (destroy)
        {
            fadeTime -= Time.deltaTime;
            if (fadeTime > 0)
            {
                if (bloodSplat)
                {
                    if(spriteRenderer != null)
                    {
                        spriteRenderer.color -= new Color(0, 0, 0, 63.75f / 255) * Time.deltaTime;
                    }
                    //if (projector != null)
                    //{
                    //    projector.fadeFactor -= Time.deltaTime / 5f;
                    //}
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //if (spriteRenderer != null)
        //{
        //    // Fade out
        //    if (ttl < 4f)
        //    {
        //        spriteRenderer.color -= new Color(0, 0, 0, 63.75f / 255) * Time.deltaTime;
        //    }
        //}


        if (ttl < 0)
        {
            Destroy(gameObject);
        }
    }
}
