using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ParticleTesterScript : MonoBehaviour
{

    public ParticleSystem blood;


    [SerializeField] LayerMask mask; // Mask you want the blood splats to collide with. Use "Geometry" layer.
    public GameObject particleSystemHolder;
    public TextMeshProUGUI selectedText;

    public List<ParticleSystem> particleSystems;
    public ParticleSystem[] enemyDeathBloodBursts;
    List<GameObject> enemyList = new List<GameObject>();
    public DecalHandler decalHandler;
    int selected = 0;
    BloodBurst selectedGibBurst = 0;
    public bool slow = false;

    public bool testControls = true;

    float playbackSpeed = 1f;

    List<GameObject> playingSystems = new List<GameObject>();

    Color32 bulletholeColor1 = new Color32(11, 11, 11, 255);
    Color32 bulletholeColor2 = new Color32(30, 28, 21, 255);
    Color32 bulletholeColor3 = new Color32(19, 19, 19, 255);
    Color32 bulletholeColor4 = new Color32(52, 47, 32, 255);
    Color32 bulletholeColor5 = new Color32(27, 25, 20, 255);
    Color32 bulletholeColor6 = new Color32(27, 25, 19, 255);

    public void SpawnParticleSystem(int effect, RaycastHit hit, Vector3 rayDir)
    {
        
        ParticleSystem particleSystem = Instantiate(particleSystems[effect]);

        particleSystem.gameObject.SetActive(true);
        particleSystem.transform.position = hit.point + hit.normal * 0.1f;

        Color bulletHoleImpactColorDelta = new Color(50f / 255f, 50f / 255f, 50f / 255f, 0);
        //ParticleSystem.SubEmittersModule subEmittersModule = particleSystem.subEmitters;
        //ParticleSystemSubEmitterProperties subEmProps = new ParticleSystemSubEmitterProperties();
        //subEmittersModule.SetSubEmitterProperties()
        //if (particleSystem.transform.GetChild(0) != null)
        //{
        //    //ParticleSystem bloodCol = blood.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        //    //bloodCol.randomSeed = bloodCol.randomSeed;
        //    //ParticleSystem bloodPS = Instantiate(blood);
        //    //bloodPS.transform.position = hit.point + hit.normal * 0.1f;
        //    //bloodPS.Play();
        //    //return;
        //    //ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        //    //particleSystem.GetParticles(particles);

        //    //for (int i = 0; i < particles.Length; i++)
        //    //{
        //    //    ParticleSystem.Particle[] subParticles = new ParticleSystem.Particle[];
        //    //}

        //    ParticleSystem bloodCollider = particleSystem.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        //    Debug.Log(particleSystem.randomSeed);
        //    //bloodCollider.randomSeed = particleSystem.randomSeed;
        //    Debug.Log("Col: " + bloodCollider.randomSeed);
        //    //bloodCollider.startSpeed = particleSystem.startSpeed;
        //    //Debug.Log(particleSystem.startSpeed);
        //    //Debug.Log("col: " + bloodCollider.startSpeed);
        //    //bloodCollider.startLifetime = particleSystem.startLifetime;
        //    //Debug.Log(particleSystem.startLifetime);
        //    //Debug.Log("col: " + bloodCollider.startLifetime);
        //}


        //Debug.Log(particleSystem.name);
        if (particleSystem.name.Contains("BulletImpactPS"))
        {
            
            Color32 surfaceColor = GetPixelColor(hit);
            surfaceColor.a = 255;

            Debug.Log("SF color: " + surfaceColor);
            if (surfaceColor.r > 0 || surfaceColor.g > 0 || surfaceColor.b > 0 && surfaceColor.a > 0)
            {

                ParticleSystem.MainModule main = particleSystem.main;
                if (surfaceColor.ColorEq(bulletholeColor1) || surfaceColor.ColorEq(bulletholeColor2) || surfaceColor.ColorEq(bulletholeColor3) || surfaceColor.ColorEq(bulletholeColor4)
                    || surfaceColor.ColorEq(bulletholeColor5))
                {
                    main.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor + bulletHoleImpactColorDelta);
                }
                else
                {
                    main.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                    
                }

                //particleSystem.startColor = surfaceColor;

                //ParticleSystem.TrailModule trails = particleSystem.trails;
                //ParticleSystem.MinMaxGradient trailColorModule = particleSystem.trails.colorOverLifetime;
                //trailColorModule.color = surfaceColor;
                //trails.colorOverLifetime = trailColorModule;
                //UnityEngine.Debug.Log("particle color: " + particleSystem.startColor);
                //UnityEngine.Debug.Log("surface color: " + surfaceColor);
                //UnityEngine.Debug.Log("trail color: " + particleSystem.trails.colorOverLifetime.color);
            }

            if (surfaceColor.r < 60 && surfaceColor.g < 60 && surfaceColor.b < 60 && surfaceColor.a > 0 && !surfaceColor.ColorEq(bulletholeColor1) && !surfaceColor.ColorEq(bulletholeColor2) && !surfaceColor.ColorEq(bulletholeColor3) && !surfaceColor.ColorEq(bulletholeColor4)
                    && !surfaceColor.ColorEq(bulletholeColor5))
            {
                ParticleSystem impactSplashSparkles = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactSplashSparklesPS")));
                impactSplashSparkles.gameObject.SetActive(true);
                impactSplashSparkles.transform.position = hit.point + hit.normal * 0.01f;
                impactSplashSparkles.transform.forward = hit.normal;
                //ParticleSystem.MainModule splashMain = impactSplashSparkles.main;
                //splashMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                //impactSplash.startColor = surfaceColor;
                impactSplashSparkles.Play();

                ParticleSystem impactReflectSparkles = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactReflectSparklesPS")));

                impactReflectSparkles.gameObject.SetActive(true);
                impactReflectSparkles.transform.position = hit.point + hit.normal * 0.01f;
                Vector3 reflectionVector = Vector3.Reflect(rayDir, hit.normal);
                //ParticleSystem.MainModule reflectMain = impactReflectSparkels.main;
                //reflectMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                impactReflectSparkles.transform.forward = reflectionVector;//hit.normal;
                impactReflectSparkles.Play();
            }
            else
            {
                ParticleSystem impactSplash = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactSplashPS")));

                impactSplash.gameObject.SetActive(true);
                impactSplash.transform.position = hit.point + hit.normal * 0.01f;
                impactSplash.transform.forward = hit.normal;
                ParticleSystem.MainModule splashMain = impactSplash.main;

                if (surfaceColor.ColorEq(bulletholeColor1) || surfaceColor.ColorEq(bulletholeColor2) || surfaceColor.ColorEq(bulletholeColor3) || surfaceColor.ColorEq(bulletholeColor4)
                     || surfaceColor.ColorEq(bulletholeColor5))
                {
                    splashMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor + bulletHoleImpactColorDelta);
                }
                else
                {
                    splashMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                    //impactSplash.startColor = surfaceColor;
                }

                impactSplash.Play();


                ParticleSystem impactReflect = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactReflectPS")));

                impactReflect.gameObject.SetActive(true);
                impactReflect.transform.position = hit.point + hit.normal * 0.01f;
                Vector3 reflectionVector = Vector3.Reflect(rayDir, hit.normal);
                ParticleSystem.MainModule reflectMain = impactReflect.main;
                impactReflect.transform.forward = reflectionVector;//hit.normal;

                if (surfaceColor.ColorEq(bulletholeColor1) || surfaceColor.ColorEq(bulletholeColor2) || surfaceColor.ColorEq(bulletholeColor3) || surfaceColor.ColorEq(bulletholeColor4)
                     || surfaceColor.ColorEq(bulletholeColor5))
                {
                    reflectMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor + bulletHoleImpactColorDelta);
                }
                else
                {
                    reflectMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));

                }

                impactReflect.Play();
            }
            
            
            //ParticleSystem impactReflect = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactReflectPS")));
            //ParticleSystem impactReflectSparkels = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactReflectSparklesPS")));
            ParticleSystem impactResidue = Instantiate(particleSystems.Find(x => x.name.Contains("BulletImpactResiduePS")));

            ParticleSystem dustEffect = Instantiate(particleSystems.Find(x => x.name.Contains("DustEffectSmallPS")));

            

            

            ParticleSystem.MainModule residueMain = impactResidue.main;

            if (surfaceColor.ColorEq(bulletholeColor1) || surfaceColor.ColorEq(bulletholeColor2) || surfaceColor.ColorEq(bulletholeColor3) || surfaceColor.ColorEq(bulletholeColor4)
                     || surfaceColor.ColorEq(bulletholeColor5))
            {
                residueMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor + bulletHoleImpactColorDelta);
            }
            else
            {
                residueMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
            }
            //residueMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
            //impactResidue.startColor = surfaceColor;
            impactResidue.gameObject.SetActive(true);
            impactResidue.transform.position = hit.point + hit.normal * 0.01f;
            impactResidue.transform.forward = hit.normal;
            impactResidue.Play();

            dustEffect.gameObject.SetActive(true);
            dustEffect.transform.position = hit.point + hit.normal * 0.13f;// Bring outwards from the surface to avoid clipping
            dustEffect.transform.position += new Vector3(0f, 0.2f, 0f);
            dustEffect.transform.forward = hit.normal;
            dustEffect.Play();

            decalHandler.PaintBulletHole(hit);
        }

        if(particleSystem.name.Contains("Strike"))
        {
            particleSystem.transform.forward = rayDir;
        }
        else
        {
            particleSystem.transform.forward = hit.normal;
        }

        particleSystem.Play();

        if (slow)
        {
            particleSystem.playbackSpeed = playbackSpeed;
        }
        else
        {
            particleSystem.playbackSpeed = 1f;
        }
        //playingSystems.Add(particleSystem.gameObject);
    }


    public Color32 GetPixelColor(RaycastHit hit)
    {
        Color32 pixelColor = new Color32(0, 0, 0, 0);
        Renderer rend = hit.transform.GetComponent<Renderer>();
        if (rend != null)
        {
            Texture2D groundTex = (Texture2D)hit.transform.GetComponent<Renderer>().material.GetTexture("_BaseMap");

            if (groundTex != null && groundTex.isReadable)
            {
                pixelColor = (Color32)groundTex.GetPixel((int)(hit.textureCoord.x * groundTex.width), (int)(hit.textureCoord.y * groundTex.height));
                //Debug.Log(pixelColor);
                return pixelColor;
            }
        }
        //Debug.Log("Couldnt get texture pixel");
        return pixelColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] gos = FindObjectsOfType<GameObject>();

        foreach(GameObject go in gos)
        {
            if(go.CompareTag("Enemy"))
            {
                enemyList.Add(go);
            }
        }

        decalHandler = GameObject.Find("DecalHandler").GetComponent<DecalHandler>();

        for (int i = 0; i < particleSystemHolder.transform.childCount; i++)
        {
            particleSystems.Add(particleSystemHolder.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
        selectedText.text = particleSystems[selected].name.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(slow)
        {
            ParticleSystem[] playingSystems = FindObjectsOfType<ParticleSystem>();

            foreach (ParticleSystem ps in playingSystems)
            {
                ps.playbackSpeed = playbackSpeed;
            }
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if(!slow)
        //    {
        //        Time.timeScale = 0.1f;
        //        slow = true;
        //    }
        //    else
        //    {
        //        Time.timeScale = 1f;
        //        slow = false;
        //    }
        //}

        if (testControls)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ParticleSystem[] playingSystems = FindObjectsOfType<ParticleSystem>();

                foreach (ParticleSystem ps in playingSystems)
                {
                    if (!slow)
                    {
                        if (ps != null)
                        {
                            playbackSpeed = 0.1f;
                            ps.playbackSpeed = playbackSpeed;
                        }
                    }
                    else
                    {
                        if (ps != null)
                        {
                            playbackSpeed = 1f;
                            ps.playbackSpeed = playbackSpeed;
                        }
                    }
                }
                slow = !slow;
                //foreach(GameObject go in playingSystems)
                //{
                //    if(go != null)
                //    {
                //        if (!slow)
                //        {
                //            ParticleSystem ps = go.GetComponent<ParticleSystem>();
                //            if(ps != null)
                //            {
                //                ps.playbackSpeed = 0.1f;
                //                //ps.playbackSpeed = 0.2f;
                //                slow = true;
                //            }
                //        }
                //        else
                //        {
                //            ParticleSystem ps = go.GetComponent<ParticleSystem>();
                //            if(ps != null)
                //            {
                //                ps.playbackSpeed = 1f;
                //                //ps.playbackSpeed = 1f;
                //                slow = false;
                //            }
                //        }
                //    }
                //}
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (slow)
                {
                    playbackSpeed *= 0.5f;
                    ParticleSystem[] playingSystems = FindObjectsOfType<ParticleSystem>();

                    foreach (ParticleSystem ps in playingSystems)
                    {
                        ps.playbackSpeed = playbackSpeed;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (slow)
                {
                    playbackSpeed *= 1.5f;
                    ParticleSystem[] playingSystems = FindObjectsOfType<ParticleSystem>();

                    foreach (ParticleSystem ps in playingSystems)
                    {
                        ps.playbackSpeed = playbackSpeed;
                    }
                }
            }


            if (Mathf.Abs((int)Input.mouseScrollDelta.y) > 0)
            {
                int scrollDelta = (int)Input.mouseScrollDelta.y * -1;
                //Debug.Log(Input.mouseScrollDelta.y);
                if ((selected + scrollDelta) >= 0 && (selected + scrollDelta) < particleSystems.Count)
                {
                    selected += scrollDelta;
                }

                if (((int)selectedGibBurst + scrollDelta) >= 0 && ((int)selectedGibBurst + scrollDelta) < enemyDeathBloodBursts.Length)
                {
                    selectedGibBurst += scrollDelta;
                }
                Debug.Log("[" + selected + "] " + particleSystems[selected].name);
                selectedText.text = "[" + selected + "] " + particleSystems[selected].name.ToString();
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                {
                    if (particleSystems[selected] != null)
                    {
                        Vector3 rayDir = (Camera.main.ScreenPointToRay(Input.mousePosition).direction);
                        //SpawnParticleSystem(selected, hit, rayDir);
                        //decalHandler.RocketWallHit(hit);
                        if (hit.transform.gameObject.CompareTag("Enemy"))
                        {
                            decalHandler.EnemyGib(hit.transform, selectedGibBurst);
                            hit.transform.gameObject.SetActive(false);
                            //Destroy(hit.transform.gameObject);
                        }
                    }
                }
            }

            if(Input.GetKeyDown(KeyCode.G))
            {
                foreach(GameObject enemy in enemyList)
                {
                    enemy.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
