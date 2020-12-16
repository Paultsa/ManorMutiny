using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Linq;

// Eelis

public class BloodCollisionLW : MonoBehaviour
{
    GameObject decalHolder; // Holder/parent object for the bloodSplat prefabs
    GameObject bloodSplashHolder; // Holder/parent object for the bloodSplash prefabs

    DecalHandler decalHandler;

    ParticleSystem particle;

    //[SerializeField] Sprite[] gibSprites;

    List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();

    [SerializeField] LayerMask mask; // Mask you want the blood splats to collide with. Use "Geometry" layer.

    public bool bodyPartSplatter = false;

    [SerializeField] ParticleSystem bloodSplash;

    int maxSplats = 150; // Max amount of splats for each particle system, not for single particle
    int maxTexturePaints = 150;
    int paintCount = 0;
    int localPaintsInProgress = 0;
    int splatCount = 0;
    int splashCount = 0;
    int maxSplashes = 150;
    int collisionCount = 0;
    int maxCollisions = 150; // Max collisions after which particle system stops sending collsion messages and lowers collision quality

    public float paintProbability = 1f;

    private List<Vector4> customData = new List<Vector4>();

    public Texture2D[] bloodSplatSprites;


    public List<Dictionary<int, Color32>> bloodSplatDictionaries = new List<Dictionary<int, Color32>>();

    bool lowQuality = false;

    //Color32 bloodPixel = new Color32(156, 20, 28, 255);
    //Color32 bloodPixel = new Color32(129, 0, 7, 255);
    //Color32 bloodPixel = new Color32(121, 13, 0, 255);
    Color32 bloodPixel = new Color32(138, 3, 3, 255);

    // TODO: Unify queues and calls with DecalHandler queue(only use 1)
    Queue<RaycastHit> paintQueue = new Queue<RaycastHit>();

    float maxTimePerInstance = 3f; // ms
    float timeInInstance = 0f;

    //KostinKoodi
    [FMODUnity.EventRef]
    public string GibSoundPath = "";
    FMOD.Studio.EventInstance GibSoundInstance;

    public struct wallDripStruct
    {
        public Texture2D t;
        public Vector2 v;
        public int type;
    }

    private void Awake()
    {

        // Only allow debug messages in editor
#if UNITY_EDITOR
        UnityEngine.Debug.unityLogger.logEnabled = true;
#else
    UnityEngine.Debug.unityLogger.logEnabled = false;
#endif

    }

    // Start is called before the first frame update
    void Start()
    {
        decalHolder = GameObject.Find("DecalHolder");
        bloodSplashHolder = GameObject.Find("BloodSplashHolder");
        decalHandler = GameObject.Find("DecalHandler").GetComponent<DecalHandler>();
        //loadTexture(lowResDecal);
        particle = GetComponent<ParticleSystem>();

        bloodSplatDictionaries = decalHandler.bloodSplatDictionaries;
        bloodSplatSprites = decalHandler.bloodSplatSprites;
    }



    private void Update()
    {
        timeInInstance = 0;

        if (!GlobalVariables.instance.paintInProgress && paintQueue.Count > 0)
        {
            StartCoroutine(PaintTextureAsyncTimed(paintQueue.Dequeue())); // Make sure to use the same function for dequeue
        }
    }

    IEnumerator PaintTextureAsyncTimed(RaycastHit hit) // paintInprogress causes problems
    {
        GlobalVariables.instance.paintInProgress = true;
        GlobalVariables.instance.globalPaintsInProgress++;
        localPaintsInProgress++;

        int frames = 0;
        float maxTimePerFrame = 4f;

        Stopwatch stopWatch = new Stopwatch();
        Stopwatch startToEndStopwatch = new Stopwatch();

        startToEndStopwatch.Start();
        stopWatch.Start();

        Renderer rend = hit.transform.GetComponent<Renderer>();

        int bloodType = Random.Range(0, bloodSplatSprites.Length);

        if (rend != null)
        {
            Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");

            if (originalTex != null && originalTex.isReadable)
            {
                float elapsedMs = 0;

                Texture2D brush = bloodSplatSprites[bloodType];

                int orgTexWidth = originalTex.width;
                int orgTexHeight = originalTex.height;

                int brushWidth = brush.width;
                int brushHeight = brush.height;

                //Debug.Log("brush width: " + brushWidth);

                int uvx = (int)(hit.textureCoord.x * orgTexWidth);
                int uvy = (int)(hit.textureCoord.y * orgTexHeight);
                //UnityEngine.Debug.Log("tex cord: " + hit.textureCoord);
                //Debug.Log("x: " + uvx + " y: " + uvy);

                int paintStartX = uvx - (brushWidth / 2);
                int paintStartY = uvy - (brushHeight / 2);

                Texture2D modifiedTex;
                bool createNewTex = false;

                // If texture has not been edited/copied yet, create a new one
                if (originalTex.updateCount == 0)
                {
                    createNewTex = true;
                    modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
                    modifiedTex.filterMode = FilterMode.Point;
                    Color32[] newColors = originalTex.GetPixels32();

                    elapsedMs = (float)stopWatch.ElapsedTicks / (float)10000f; // Tick is 100ns. 10K ticks = 1ms
                    if (elapsedMs > maxTimePerFrame)
                    {
                        frames++;
                        stopWatch.Reset();
                        startToEndStopwatch.Stop();
                        yield return null;
                        startToEndStopwatch.Start();
                        stopWatch.Start();
                    }

                    modifiedTex.SetPixels32(newColors);

                    elapsedMs = (float)stopWatch.ElapsedTicks / (float)10000f; // Tick is 100ns. 10K ticks = 1ms
                    if (elapsedMs > maxTimePerFrame)
                    {
                        frames++;
                        stopWatch.Reset();
                        startToEndStopwatch.Stop();
                        yield return null;
                        startToEndStopwatch.Start();
                        stopWatch.Start();
                    }
                }
                // If texture has been already edited/copied, edit existing texture rather than creating a new one
                else
                {
                    modifiedTex = originalTex;
                }

                for (int i = 0; i < brushWidth; i++)
                {
                    elapsedMs = (float)stopWatch.ElapsedTicks / (float)10000f; // Tick is 100ns. 10K ticks = 1ms
                    if (elapsedMs > maxTimePerFrame)
                    {
                        frames++;
                        stopWatch.Reset();
                        startToEndStopwatch.Stop();
                        yield return null;
                        startToEndStopwatch.Start();
                        stopWatch.Start();
                    }
                    for (int j = 0; j < brushHeight; j++)
                    {
                        //int pos = j * decalWidth + i;
                        //Color32 color = decalColorData[pos];
                        if (paintStartX + i >= 0 && paintStartX + i < orgTexWidth && paintStartY + j >= 0 && paintStartY + j < orgTexHeight)
                        {

                            Color32 c;
                            if (bloodSplatDictionaries[bloodType].TryGetValue(j * brushWidth + i, out c))
                            {
                                modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
                            }
                        }
                    }
                }

                modifiedTex.Apply();

                if (createNewTex)
                {
                    rend.material.SetTexture("_BaseMap", modifiedTex);
                }
                else
                {

                }

                if (Mathf.Abs(rend.transform.rotation.eulerAngles.x) > 75)
                {
                    DecalHandler.wallDripStruct wds = new DecalHandler.wallDripStruct();
                    wds.tex = modifiedTex;
                    wds.uv = new Vector2(uvx, uvy);
                    wds.bloodType = bloodType;

                    decalHandler.wallDripQueue.Enqueue(wds);
                }
            }
        }
        timeInInstance += (float)startToEndStopwatch.ElapsedTicks / (float)10000f;

        if (timeInInstance > maxTimePerInstance)
        {
            yield return null;
        }
        GlobalVariables.instance.paintInProgress = false; // Sometimes this object gets destroyed before the coroutine reaches this point
        GlobalVariables.instance.globalPaintsInProgress--;
        localPaintsInProgress--;
    }


    public void OnParticleSystemStopped()
    {
        GlobalVariables.instance.paintInProgress = false;
        GlobalVariables.instance.globalPaintsInProgress -= localPaintsInProgress;
        //UnityEngine.Debug.Log("Callback destroy");
        Destroy(gameObject);
    }


    private void OnDestroy()
    {
    }

    private void OnParticleCollision(GameObject other)
    {
        collisionCount++;
        //UnityEngine.Debug.Log("Col count: " + collisionCount);
        if (collisionCount > maxCollisions && !lowQuality)
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            var coll = ps.collision;
            coll.sendCollisionMessages = false;
            coll.quality = ParticleSystemCollisionQuality.High;
            coll.enableDynamicColliders = false;
            lowQuality = true;
        }

        if (other != null)
        {
            //UnityEngine.Debug.Log("particle collision");
            ParticlePhysicsExtensions.GetCollisionEvents(particle, other, collisions);

            foreach (ParticleCollisionEvent col in collisions) // Add velocity check right here?
            {

                Quaternion rot = new Quaternion();

                if (col.normal != Vector3.zero)
                {
                    rot = Quaternion.LookRotation(col.normal, Vector3.up);
                }

                if (Random.Range(0f, 1f) < paintProbability)
                {
                    // Texture painting
                    if (col.colliderComponent != null)
                    {
                        //UnityEngine.Debug.Log("collider component: " + col.colliderComponent);
                        RaycastHit hit;
                        // Cast ray from normal back to collision point
                        if (Physics.Raycast((col.intersection + col.normal * 0.1f), col.intersection - (col.intersection + col.normal), out hit, 0.5f, mask)) // Cast a short ray
                        {
                            if (hit.collider != null)
                            {
                                // If we hit MeshCollider paint texture
                                if (hit.collider.GetType() == typeof(MeshCollider))
                                {
                                    if (paintCount < maxTexturePaints && col.velocity.magnitude > 1f)
                                    {
                                        // If performance might be a problem use a queue
                                        if (!GlobalVariables.instance.paintInProgress)
                                        {
                                            StartCoroutine(PaintTextureAsyncTimed(hit));
                                        }
                                        else
                                        {
                                            paintQueue.Enqueue(hit);
                                        }
                                        paintCount++;
                                    }
                                }
                            }
                        }
                    }
                }

                if (bodyPartSplatter)
                {
                    if (col.velocity.magnitude > 1f && splashCount < maxSplashes) // Only spawn blood splash effects when the particle collision has some speed
                    {
                        if (Mathf.Abs(col.velocity.y) > 1f)
                        {
                            ParticleSystem splash = Instantiate(bloodSplash, col.intersection - col.normal * 0.13f, rot);
                            if (bloodSplashHolder != null)
                            {
                                splash.transform.parent = bloodSplashHolder.transform;
                            }
                        }

                        //KostinKoodi

                        GibSoundInstance = FMODUnity.RuntimeManager.CreateInstance(GibSoundPath);
                        GibSoundInstance.setParameterByName("BloodSplatCollisionForce", col.velocity.magnitude);
                        GibSoundInstance.start();
                        GibSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(col.intersection));

                        splashCount++;
                    }
                }
                else
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SX/Blood/Sx_bloodSplatter", col.intersection);
                }
            }
        }
    }
}

