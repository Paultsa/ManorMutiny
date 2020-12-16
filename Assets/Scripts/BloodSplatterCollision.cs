using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Linq;

// Eelis




public class BloodSplatterCollision : MonoBehaviour
{
    public GameObject bloodSplat;

    GameObject decalHolder; // Holder/parent object for the bloodSplat prefabs
    GameObject bloodSplashHolder; // Holder/parent object for the bloodSplash prefabs

    DecalHandler decalHandler;

    ParticleSystem particle;

    [SerializeField] Sprite[] gibSprites;

    List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();

    [SerializeField] LayerMask mask; // Mask you want the blood splats to collide with. Use "Geometry" layer.

    public bool bodyPartSplatter = false;

    [SerializeField] ParticleSystem bloodSplash;
    //public GameObject paintedDecalsContainer;
    //public Camera texturePainterCamera;
    // public GameObject splatDecalObj;
    public GameObject splatDecalProjector;

    int maxSplats = 70; // Max amount of splats for each particle system, not for single particle
    int maxTexturePaints = 70;
    public int paintCount = 0;
    int localPaintsInProgress = 0;
    int splatCount = 0;
    int splashCount = 0;
    int maxSplashes = 70;
    int collisionCount = 0;
    int maxCollisions = 70; // Max collisions after which particle system stops sending collsion messages and lowers collision quality

    private List<Vector4> customData = new List<Vector4>();

    //public Texture2D blood1_64x64;
    //public Texture2D blood2_64x64;
    //public Texture2D blood3_64x64;

    public Texture2D[] bloodSplatSprites;

    //public Texture2D lowResDecal16;
    //public Texture2D lowResDecal32;
    //public Texture2D lowResDecal64;
    //public Texture2D lowResDecal128;
    //public Texture2D lowResDecal256;
    //public Texture2D lowResDecal512;

    Dictionary<int, Color32> nonTransparentPixels;

    public Dictionary<int, Color32> nonTransparentBlood1Pixels = new Dictionary<int, Color32>();
    public Dictionary<int, Color32> nonTransparentBlood2Pixels = new Dictionary<int, Color32>();
    public Dictionary<int, Color32> nonTransparentBlood3Pixels = new Dictionary<int, Color32>();

    public List<Dictionary<int, Color32>> bloodSplatDictionaries = new List<Dictionary<int, Color32>>();

    List<float> paintTimes = new List<float>();


    bool lowQuality = false;

    //Color32 bloodPixel = new Color32(156, 20, 28, 255);
    //Color32 bloodPixel = new Color32(129, 0, 7, 255);
    //Color32 bloodPixel = new Color32(121, 13, 0, 255);
    Color32 bloodPixel = new Color32(138, 3, 3, 255);

    Dictionary<int, Color32> decalColorData = new Dictionary<int, Color32>();

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
        /*
        // Only allow debug messages in editor
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
    Debug.unityLogger.logEnabled = false;
#endif
        */
    }




    /*
    void loadTexture(Texture2D tex)
    {
        int textureSize = tex.width * tex.height;
        Color32[] colorData = tex.GetPixels32();
        for (int i = 0; i < textureSize; i++)
        {
            decalColorData.Add(i, colorData[i]);
        }
    }
    */

    public IEnumerator WallDrip(Texture2D tex, Vector2 brushCenter)
    {
        //wallDripsInProgress++;
        List<Vector2> dripSpots = new List<Vector2>();
        // Pos1: x: -2: +10
        // Pos2: x: +20 y: +16
        // Pos3: x: -17 y: - 2
        Vector2 spot1 = new Vector2(brushCenter.x - 2, brushCenter.y + 10); // Center
        Vector2 spot2 = new Vector2(brushCenter.x + 24, brushCenter.y + 1); //Left
        Vector2 spot3 = new Vector2(brushCenter.x - 18, brushCenter.y - 2); // Right
        dripSpots.Add(spot1);
        dripSpots.Add(spot2);
        dripSpots.Add(spot3);

        int dripDist = 0;

        List<int> endDists = new List<int>();
        endDists.Add((Random.Range(30, 75)));
        endDists.Add((Random.Range(30, 75)));
        endDists.Add((Random.Range(30, 75)));
        //int endDist = Random.Range(32, 64);

        bool endDrip = false;



        //for (int i = 0; i < 5; i++)
        //{
        //    int x = Random.Range(Mathf.RoundToInt(brushCenter.x - 64 / 2), (Mathf.RoundToInt(brushCenter.x + 64 / 2)));
        //    int y = Random.Range(Mathf.RoundToInt(brushCenter.y - 64 / 2), (Mathf.RoundToInt(brushCenter.y + 64 / 2)));
        //    dripSpots.Add(new Vector2(x, y));
        //}

        while (!endDrip)
        {
            dripDist++;
            //UnityEngine.Debug.Log("Drip dist: " + dripDist);
            for (int i = 0; i < dripSpots.Count; i++)
            {
                if (dripDist < endDists[i])
                {
                    if ((float)endDists[i] / (float)dripDist < 1.3f && dripSpots[i].y + dripDist < tex.height)
                    {
                        tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y + dripDist, bloodPixel);
                        endDrip = false;
                    }
                    else if ((float)endDists[i] / (float)dripDist < 2 && dripSpots[i].y + dripDist < tex.height)
                    {
                        tex.SetPixel((int)dripSpots[i].x + 1, (int)dripSpots[i].y + dripDist, bloodPixel);
                        tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y + dripDist, bloodPixel);
                        endDrip = false;
                    }
                    else if (dripSpots[i].y + dripDist < tex.height)
                    {
                        tex.SetPixel((int)dripSpots[i].x - 1, (int)dripSpots[i].y + dripDist, bloodPixel);
                        tex.SetPixel((int)dripSpots[i].x + 1, (int)dripSpots[i].y + dripDist, bloodPixel);
                        tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y + dripDist, bloodPixel);
                        endDrip = false;
                    }
                }
                else
                {
                    endDrip = true;
                }


                //if ((float)endDist / (float)dripDist < 1.3f && spot.y + dripDist < tex.height)
                //    {
                //        tex.SetPixel((int)spot.x, (int)spot.y + dripDist, bloodPixel);
                //    }
                //else if ((float)endDist / (float)dripDist < 2 && spot.y + dripDist < tex.height)
                //{
                //    tex.SetPixel((int)spot.x + 1, (int)spot.y + dripDist, bloodPixel);
                //    tex.SetPixel((int)spot.x, (int)spot.y + dripDist, bloodPixel);
                //}
                //else if (spot.y + dripDist < tex.height)
                //{
                //    tex.SetPixel((int)spot.x - 1, (int)spot.y + dripDist, bloodPixel);
                //    tex.SetPixel((int)spot.x + 1, (int)spot.y + dripDist, bloodPixel);
                //    tex.SetPixel((int)spot.x, (int)spot.y + dripDist, bloodPixel);
                //}
            }
            tex.Apply();
            yield return new WaitForSeconds(0.1f);
            //tex.GetPixel((int)spot.x, (int)spot.y);
        }
        //wallDripsInProgress--;
        UnityEngine.Debug.Log("Drip finished");
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
        //nonTransparentPixels = decalHandler.nonTransparentBloodPixels;
        //nonTransparentBlood1Pixels = decalHandler.nonTransparentBlood1Pixels;
        //nonTransparentBlood2Pixels = decalHandler.nonTransparentBlood2Pixels;
        //nonTransparentBlood3Pixels = decalHandler.nonTransparentBlood3Pixels;

        

        if (bodyPartSplatter)
        {
            //particle.textureSheetAnimation.SetSprite(0, gibSprites[Random.Range(0, gibSprites.Length - 1)]);
        }
    }

 

    private void Update()
    {
        timeInInstance = 0;

        if (!GlobalVariables.instance.paintInProgress && paintQueue.Count > 0)
        {
            //UnityEngine.Debug.LogError("Paint dequeued!");
            StartCoroutine(PaintTextureAsyncTimed(paintQueue.Dequeue())); // Make sure to use the same function for dequeue
        }
    }

    // Under 2ms on 512x512 textures, ~7ms with 1024x1024 textures (Deep Profiler mode), much faster in build (under 1ms)
    //void PaintTexture(RaycastHit hit)
    //{
    //    //UnityEngine.Debug.Log("PaintTexture");
    //    GlobalVariables.instance.paintInProgress = true;
    //    Renderer rend = hit.transform.GetComponent<Renderer>();
    //    Stopwatch startToEndStopwatch = new Stopwatch();
    //    startToEndStopwatch.Start();

    //    int bloodType = Random.Range(1, 4);

    //    if (rend != null)
    //    {
    //        Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");

    //        if (originalTex != null && originalTex.isReadable)
    //        {
    //            Texture2D brush = blood1_64x64;

    //            if (bloodType == 1)
    //            {
    //                brush = blood1_64x64;
    //            }
    //            else if (bloodType == 2)
    //            {
    //                brush = blood2_64x64;
    //            }
    //            else if (bloodType == 3)
    //            {
    //                brush = blood3_64x64;
    //            }
    //            int orgTexWidth = originalTex.width;
    //            int orgTexHeight = originalTex.height;

    //            // Scaling
    //            /*
    //            if (orgWidth > 1024)
    //            {
    //                brush = lowResDecal64;
    //            }
    //            else if (orgWidth > 512)
    //            {
    //                brush = lowResDecal32;
    //            }
    //            else
    //            {
    //                brush = lowResDecal16;
    //            }
    //            */

    //            int brushWidth = brush.width;
    //            int brushHeight = brush.height;

    //            //Debug.Log("brush width: " + brushWidth);

    //            int uvx = (int)(hit.textureCoord.x * orgTexWidth);
    //            int uvy = (int)(hit.textureCoord.y * orgTexHeight);

    //            //UnityEngine.Debug.LogError("texCords: " + hit.textureCoord);
    //            //UnityEngine.Debug.LogError("x: " + uvx +  " y: " + uvy);
    //            //UnityEngine.Debug.Log("tex cord: " + hit.textureCoord);
    //            //Debug.Log("x: " + uvx + " y: " + uvy);


    //            int paintStartX = uvx - (brushWidth / 2);
    //            int paintStartY = uvy - (brushHeight / 2);

    //            bool createNewTex = false;

    //            Texture2D modifiedTex;
    //            // If texture has not been edited/copied yet, create a new one
    //            if (originalTex.updateCount == 0)
    //            {
    //                createNewTex = true;
    //                modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
    //                modifiedTex.filterMode = FilterMode.Point;
    //                modifiedTex.SetPixels32(originalTex.GetPixels32());
    //            }
    //            // If texture has been already edited/copied, edit existing texture rather than creating a new one
    //            else
    //            {
    //                modifiedTex = originalTex;

    //                //UnityEngine.Debug.Log("Edit existing texture");
    //                //UnityEngine.Debug.LogError("Edit existing texture");

    //                //UnityEngine.Debug.Log("Existing texture update count : " + modifiedTex.updateCount);
    //                //UnityEngine.Debug.LogError("Existing texture update count : " + modifiedTex.updateCount);
    //            }





    //            for (int i = 0; i < brushWidth; i++)
    //            {
    //                for (int j = 0; j < brushHeight; j++)
    //                {
    //                    if (bloodType == 1)
    //                    {
    //                        Color32 c;
    //                        if (nonTransparentBlood1Pixels.TryGetValue(j * brushWidth + i, out c))
    //                        {
    //                            modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
    //                        }
    //                    }
    //                    else if (bloodType == 2)
    //                    {
    //                        Color32 c;
    //                        if (nonTransparentBlood2Pixels.TryGetValue(j * brushWidth + i, out c))
    //                        {
    //                            modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
    //                        }
    //                    }
    //                    else if (bloodType == 3)
    //                    {
    //                        Color32 c;
    //                        if (nonTransparentBlood3Pixels.TryGetValue(j * brushWidth + i, out c))
    //                        {
    //                            modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
    //                        }
    //                    }
    //                }
    //            }

    //            modifiedTex.Apply();
    //            if (createNewTex)
    //            {
    //                rend.material.SetTexture("_BaseMap", modifiedTex);
    //            }
    //            else
    //            {

    //            }

    //            if (Mathf.Abs(rend.transform.rotation.eulerAngles.x) > 75)
    //            {
    //                StartCoroutine(decalHandler.WallDrip(modifiedTex, new Vector2(uvx, uvy), bloodType));
    //            }
    //        }
    //    }
    //    //UnityEngine.Debug.LogError("PaintTime: " + (float)startToEndStopwatch.ElapsedTicks / (float)10000f + "ms");
    //    paintTimes.Add((float)startToEndStopwatch.ElapsedTicks / (float)10000f);
    //    GlobalVariables.instance.paintInProgress = false;
    //}

    // Takes 6(new texture) or 5(editing existing texture) frames to complete to reduce load on single frame
    //IEnumerator PaintTextureAsync(RaycastHit hit)
    //{
    //    GlobalVariables.instance.paintInProgress = true;
    //    Stopwatch startToEndStopwatch = new Stopwatch();
    //    startToEndStopwatch.Start();

    //    Renderer rend = hit.transform.GetComponent<Renderer>();

    //    int bloodType = Random.Range(1, 4);

    //    if (rend != null)
    //    {
    //        Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");

    //        if (originalTex != null && originalTex.isReadable)
    //        {
    //            Texture2D brush = blood1_64x64;

    //            if (bloodType == 1)
    //            {
    //                brush = blood1_64x64;
    //            }
    //            else if (bloodType == 2)
    //            {
    //                brush = blood2_64x64;
    //            }
    //            else if (bloodType == 3)
    //            {
    //                brush = blood3_64x64;
    //            }

    //            int orgTexWidth = originalTex.width;
    //            int orgTexHeight = originalTex.height;

    //            // Scaling
    //            /*
    //            if (orgWidth > 1024)
    //            {
    //                brush = lowResDecal64;
    //            }
    //            else if (orgWidth > 512)
    //            {
    //                brush = lowResDecal32;
    //            }
    //            else
    //            {
    //                brush = lowResDecal16;
    //            }
    //            */

    //            int brushWidth = brush.width;
    //            int brushHeight = brush.height;

    //            int uvx = (int)(hit.textureCoord.x * orgTexWidth);
    //            int uvy = (int)(hit.textureCoord.y * orgTexHeight);

    //            //UnityEngine.Debug.Log("tex cord: " + hit.textureCoord);
    //            //Debug.Log("x: " + uvx + " y: " + uvy);


    //            int paintStartX = uvx - (brushWidth / 2);
    //            int paintStartY = uvy - (brushHeight / 2);

    //            Texture2D modifiedTex;
    //            bool createNewTex = false;

    //            // If texture has been already edited/copied, edit existing texture rather than creating a new one
    //            if (originalTex.updateCount == 0)
    //            {
    //                createNewTex = true;
    //                modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
    //                modifiedTex.filterMode = FilterMode.Point;

    //                //UnityEngine.Debug.Log("Create new texture");
    //                //UnityEngine.Debug.LogError("Create new texture");

    //                modifiedTex.SetPixels32(originalTex.GetPixels32());
    //                //UnityEngine.Debug.Log("original texture update count : " + modifiedTex.updateCount);
    //                //UnityEngine.Debug.LogError("original texture update count : " + modifiedTex.updateCount);

    //                yield return null;
    //            }
    //            // If texture has not been edited/copied yet, create a new one
    //            else
    //            {
    //                modifiedTex = originalTex;
    //                //UnityEngine.Debug.Log("Edit existing texture");
    //                //UnityEngine.Debug.LogError("Edit existing texture");

    //                //UnityEngine.Debug.Log("Existing texture update count : " + modifiedTex.updateCount);
    //                //UnityEngine.Debug.LogError("Existing texture update count : " + modifiedTex.updateCount);
    //            }

    //            bool loop1 = false;
    //            bool loop2 = false;
    //            bool loop3 = false;

    //            for (int i = 0; i < brushWidth; i++)
    //            {
    //                // Chop loop to 4 frames
    //                float complete = (float)i / (float)brushWidth;
    //                if (complete > 0.25f && !loop1)
    //                {
    //                    loop1 = true;
    //                    yield return null;
    //                }
    //                else if (complete > 0.50f && !loop2)
    //                {
    //                    loop2 = true;
    //                    yield return null;
    //                }
    //                else if (complete > 0.75f && !loop3)
    //                {
    //                    loop3 = true;
    //                    yield return null;
    //                }
    //                for (int j = 0; j < brushHeight; j++)
    //                {
    //                    //int pos = j * decalWidth + i;
    //                    //Color32 color = decalColorData[pos];
    //                    if (paintStartX + i >= 0 && paintStartX + i < orgTexWidth && paintStartY + j >= 0 && paintStartY + j < orgTexHeight)
    //                    {
    //                        if (bloodType == 1)
    //                        {
    //                            Color32 c;
    //                            if (nonTransparentBlood1Pixels.TryGetValue(j * brushWidth + i, out c))
    //                            {
    //                                modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
    //                            }
    //                        }
    //                        else if (bloodType == 2)
    //                        {
    //                            Color32 c;
    //                            if (nonTransparentBlood2Pixels.TryGetValue(j * brushWidth + i, out c))
    //                            {
    //                                modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
    //                            }
    //                        }
    //                        else if (bloodType == 3)
    //                        {
    //                            Color32 c;
    //                            if (nonTransparentBlood3Pixels.TryGetValue(j * brushWidth + i, out c))
    //                            {
    //                                modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            yield return null;


    //            modifiedTex.Apply();

    //            if (createNewTex)
    //            {
    //                rend.material.SetTexture("_BaseMap", modifiedTex);
    //            }
    //            else
    //            {
    //                //UnityEngine.Debug.Log("Existing texture update count after: " + modifiedTex.updateCount);
    //                //UnityEngine.Debug.LogError("Existing texture update count after: " + modifiedTex.updateCount);
    //            }

    //            if (Mathf.Abs(rend.transform.rotation.eulerAngles.x) > 75)
    //            {
    //                StartCoroutine(decalHandler.WallDrip(modifiedTex, new Vector2(uvx, uvy), bloodType));
    //            }
    //        }
    //    }
    //    paintTimes.Add((float)startToEndStopwatch.ElapsedTicks / (float)10000f);
    //    //UnityEngine.Debug.LogError("PaintTime(Async): " + (float)startToEndStopwatch.ElapsedTicks / (float)10000f + "ms");
    //    //yield return null;
    //    GlobalVariables.instance.paintInProgress = false;
    //} // Causes problems with paintInProgress

    // Execution time of this function is limited to maxvalue per frame(some single functions inside this function can still take longer)
    IEnumerator PaintTextureAsyncTimed(RaycastHit hit) // paintInprogress causes problems
    {
        GlobalVariables.instance.paintInProgress = true;
        GlobalVariables.instance.globalPaintsInProgress++;
        localPaintsInProgress++;
        int frames = 0;
        float maxTimePerFrame = 4f;
        Stopwatch stopWatch = new Stopwatch();
        Stopwatch startToEndStopwatch = new Stopwatch();
        //UnityEngine.Debug.Log("frq: " + Stopwatch.Frequency + "ishr: " + Stopwatch.IsHighResolution);
        startToEndStopwatch.Start();
        stopWatch.Start();
        Renderer rend = hit.transform.GetComponent<Renderer>();

        int bloodType = Random.Range(0, bloodSplatSprites.Length);

        if (rend != null)
        {
            Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");
            //UnityEngine.Debug.Log("Original texture format: " + originalTex.format);

            if (originalTex != null && originalTex.isReadable)
            {
                float elapsedMs = 0;

                Texture2D brush = bloodSplatSprites[bloodType]; //blood1_64x64;

                //if (bloodType == 1)
                //{
                //    brush = blood1_64x64;
                //}
                //else if (bloodType == 2)
                //{
                //    brush = blood2_64x64;
                //}
                //else if (bloodType == 3)
                //{
                //    brush = blood3_64x64;
                //}

                int orgTexWidth = originalTex.width;
                int orgTexHeight = originalTex.height;
                // Scaling
                /*
                if (orgWidth > 1024)
                {
                    brush = lowResDecal64;
                }
                else if (orgWidth > 512)
                {
                    brush = lowResDecal32;
                }
                else
                {
                    brush = lowResDecal16;
                }
                */

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

                    //UnityEngine.Debug.Log("Edit existing texture");
                    //UnityEngine.Debug.LogError("Edit existing texture");

                    //UnityEngine.Debug.Log("Existing texture update count : " + modifiedTex.updateCount);
                    //UnityEngine.Debug.LogError("Existing texture update count : " + modifiedTex.updateCount);
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
                            //if (bloodType == 1)
                            //{
                            //    Color32 c;
                            //    if (nonTransparentBlood1Pixels.TryGetValue(j * brushWidth + i, out c))
                            //    {
                            //        modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
                            //    }
                            //}
                            //else if (bloodType == 2)
                            //{
                            //    Color32 c;
                            //    if (nonTransparentBlood2Pixels.TryGetValue(j * brushWidth + i, out c))
                            //    {
                            //        modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
                            //    }
                            //}
                            //else if (bloodType == 3)
                            //{
                            //    Color32 c;
                            //    if (nonTransparentBlood3Pixels.TryGetValue(j * brushWidth + i, out c))
                            //    {
                            //        modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
                            //    }
                            //}
                        }
                    }
                }

                // Apply() is approx 0.7ms for 512x512 texture so this last yield might not be needed

                //elapsedMs = (float)stopWatch.ElapsedTicks / (float)10000f; // Tick is 100ns. 10K ticks = 1ms
                //if (elapsedMs > maxTimePerFrame / 1.5) // For larger textures it might be benefitical to have smaller than max value here so that long loop portion doesnt get chained with Apply()
                //{
                //    frames++;
                //    stopWatch.Reset();
                //    yield return null;
                //    stopWatch.Start();
                //}

                modifiedTex.Apply();

                if (createNewTex)
                {
                    rend.material.SetTexture("_BaseMap", modifiedTex);

                    //UnityEngine.Debug.Log("new texture update count after: " + modifiedTex.updateCount);
                    //UnityEngine.Debug.LogError("new texture update count after: " + modifiedTex.updateCount);

                    //UnityEngine.Debug.Log("new: " + modifiedTex.updateCount);
                    //UnityEngine.Debug.Log("orgAfter: " + modifiedTex.updateCount);
                }
                else
                {
                    //UnityEngine.Debug.Log("Existing texture update count after: " + modifiedTex.updateCount);
                    //UnityEngine.Debug.LogError("Existing texture update count after: " + modifiedTex.updateCount);
                }
                //UnityEngine.Debug.Log(Mathf.Abs(rend.transform.rotation.eulerAngles.x));
                if (Mathf.Abs(rend.transform.rotation.eulerAngles.x) > 75)
                {
                    //StartCoroutine(WallDrip(modifiedTex, new Vector2(uvx, uvy)));
                    //StartCoroutine(decalHandler.WallDrip(modifiedTex, new Vector2(uvx, uvy))); // Problem
                    //StartWallDripCoroutine(modifiedTex, new Vector2(uvx, uvy));
                    DecalHandler.wallDripStruct wds = new DecalHandler.wallDripStruct();
                    wds.tex = modifiedTex;
                    wds.uv = new Vector2(uvx, uvy);
                    wds.bloodType = bloodType;

                    decalHandler.wallDripQueue.Enqueue(wds);
                }
            }
        }
        timeInInstance += (float)startToEndStopwatch.ElapsedTicks / (float)10000f;
        paintTimes.Add((float)startToEndStopwatch.ElapsedTicks / (float)10000f);
        //UnityEngine.Debug.LogError("PaintTime(TimedAsync): " + (float)startToEndStopwatch.ElapsedTicks / (float)10000f + "ms");

        if (timeInInstance > maxTimePerInstance)
        {
            yield return null;
        }
        GlobalVariables.instance.paintInProgress = false; // Sometimes this object gets destroyed before the coroutne reaches this point
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
        //float averagePaintTime = paintTimes.Sum() / paintTimes.Count();
        //UnityEngine.Debug.LogError("Average paint time: " + averagePaintTime + "ms");
    }

    private void OnParticleCollision(GameObject other)
    {
        collisionCount++;
        UnityEngine.Debug.Log("Col count: " + collisionCount);
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
            //particle.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

            //Debug.Log("customData[0]: " + customData[0]);
            foreach (ParticleCollisionEvent col in collisions) // Add velocity check right here?
            {
                
                //UnityEngine.Debug.Log("Particle collision");
                //UnityEngine.Debug.Log("Blood particle collided with: " + col.colliderComponent.gameObject);
                // Sprites

                Quaternion rot = new Quaternion();
                Quaternion projRot = new Quaternion();

                if (col.normal != Vector3.zero)
                {
                    rot = Quaternion.LookRotation(col.normal);
                    projRot = Quaternion.LookRotation(-col.normal);
                }


                rot.eulerAngles += new Vector3(0f, 0f, Random.Range(0f, 360f)); // Randomize rotation around z axis for more uniqueness between splats
                projRot.eulerAngles += new Vector3(0f, 0f, Random.Range(0f, 360f)); // Randomize rotation around z axis for more uniqueness between splats

                //Debug.Log("Collision velocity: " + col.velocity.magnitude);
                //Debug.Log("Collision velocity: " + col.velocity);


                //if (col.velocity.magnitude > 0.9f && splatCount < maxSplats)// && customData[0].x < 4)  // Only spawn blood splat sprites when the particle collision has some speed and maxSplats has not been reached
                //{
                //    Because particle collisions are inaccurate use raycast
                //    RaycastHit hit;
                //    if (Physics.Raycast((col.intersection + col.normal * 0.1f), col.intersection - (col.intersection + col.normal), out hit, Mathf.Infinity, mask))
                //    {
                //        //UnityEngine.Debug.Log(hit.transform.gameObject);
                //        GameObject splat = Instantiate(bloodSplat, hit.point + hit.normal * 0.01f, rot);//col.intersection - col.normal * 0.13f, rot);// * 0.13f, rot);
                //                                                                                        //GameObject splat = Instantiate(splatDecalProjector, col.intersection, projRot);
                //        splat.GetComponent<Decal>().ttl = 1200f;
                //        splat.GetComponent<Decal>().bloodSplat = true;

                //        if (decalHolder != null)
                //        {
                //            splat.transform.parent = decalHolder.transform;
                //        }

                //        //customData[0] = customData[0] + new Vector4(1, 0, 0, 0); // Increase customData[0] by 1 on each collision
                //        //particle.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

                //        splatCount++;
                //    }
                //}
                




                // Texture painting
#if true
                if (col.colliderComponent != null)
                {
                    //UnityEngine.Debug.Log("collider component: " + col.colliderComponent);
                    RaycastHit hit;
                    // Cast ray from normal back to collision point
                    if (Physics.Raycast((col.intersection + col.normal * 0.1f), col.intersection - (col.intersection + col.normal), out hit, 0.5f, mask)) // Cast a short ray
                    {
                        //UnityEngine.Debug.Log("Collider type1: " + hit.collider.GetType());
                        int boxColliderHits = 0;
                        int maxColliderHits = 3;
                        int loops = 0;
                        List<BoxCollider> boxCollidersHit = new List<BoxCollider>();

                        // While we hit BoxColliders and boxColliderHits is less than maxColliderHits, disable boxColliders add them to list and cast more rays
                        while (hit.collider != null && hit.collider.GetType() == typeof(BoxCollider) && boxColliderHits < maxColliderHits && loops < 5)
                        {
                            hit.collider.enabled = false;
                            boxCollidersHit.Add((BoxCollider)hit.collider);
                            if (Physics.Raycast((col.intersection + col.normal * 0.1f), col.intersection - (col.intersection + col.normal), out hit, 0.5f, mask)) // Cast a short ray
                            {
                                //UnityEngine.Debug.Log("Collider type" + boxColliderHits + ": " + hit.collider.GetType());
                            }
                            boxColliderHits++;
                            loops++;
                        }

                        // Re-enable disabled BoxColliders
                        foreach (BoxCollider box in boxCollidersHit)
                        {
                            box.enabled = true;
                        }

                        if (hit.collider != null)
                        {
                            // If we hit MeshCollider paint texture
                            if (hit.collider.GetType() == typeof(MeshCollider))
                            {
                                //UnityEngine.Debug.Log("Collision velocity: " + col.velocity.magnitude);
                                //UnityEngine.Debug.Log("Splat count: " + splatCount);
                                if (paintCount < maxTexturePaints && col.velocity.magnitude > 1f)
                                {
                                    // If performance might be a problem use a queue
                                    if(!GlobalVariables.instance.paintInProgress)
                                    {
                                        //UnityEngine.Debug.Log("Paint started");
                                        StartCoroutine(PaintTextureAsyncTimed(hit));
                                        //PaintTexture(hit);
                                    }
                                    else
                                    {
                                        //UnityEngine.Debug.LogError("Paint Enqueued!");
                                        paintQueue.Enqueue(hit);
                                    }
                                    paintCount++;
                                }
                            }
                        }

                        //if (hit.collider.GetType() == typeof(MeshCollider))
                        //{
                        //    // Only paint max amount of bounces
                        //    if (splatCount < maxTexturePaints)
                        //    {
                        //        splatCount++;

                        //        if (!paintInProgress)
                        //        {
                        //            StartCoroutine(PaintTextureAsyncTimed(hit));
                        //        }
                        //        else
                        //        {
                        //            paintQueue.Enqueue(hit);
                        //        }
                        //        // For async use this check
                        //        //if(!paintInProgress)
                        //        //{
                        //        //    splatCount++;
                        //        //    StartCoroutine(PaintTextureAsync(hit));
                        //        //    //PaintTexture(hit);
                        //        //}

                        //    }
                        //}

                    }
                }
#endif

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
