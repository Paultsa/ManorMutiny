using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLight : MonoBehaviour
{

    [SerializeField]
    AnimationCurve intensityCurve;

    Light explosionLight;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        explosionLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        explosionLight.intensity = intensityCurve.Evaluate(timer) * 50;
        explosionLight.range = intensityCurve.Evaluate(timer) * 17;
    }
}
