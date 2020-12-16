using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System.Linq;

// Eelis

public static class OperatorFunctions
{
    public static bool ColorEq(this Color32 rhs, Color32 lhs)
    {
        if (rhs.r == lhs.r && rhs.g == lhs.g && rhs.b == lhs.b && rhs.a == lhs.a)
        {
            return true;
        }
        return false;
    }
}

public enum BloodBurst { Cone, ConeTrails, Sphere, SphereTrails, ConeTrailsLong, SphereTrailsLong }

public enum DecalType { DecalType1, DecalType2, DecalType3, DecalType4 } // Change to something more descriptive later
public enum DecalEffectType { NoDecalEfect, DecalEffectType1, DecalEffectType2, DecalEffectType3, DecalEffectType4 } // Change to something more descriptive later
public class DecalHandler : MonoBehaviour
{

    public GameObject decalCross;

    [SerializeField] LayerMask mask; // Mask you want the blood splats to collide with. Use "Geometry" layer.

    [SerializeField] Sprite[] decalSprites;

    public Texture2D[] bloodSplatSprites;
    public Texture2D[] bulletHoleSprites;
    public Texture2D[] rocketBlastSprites;
    public Texture2D[] railGunBlastSprites;


    [SerializeField] ParticleSystem[] decalEffects;

    [SerializeField] DecalEffectType testEffect = DecalEffectType.NoDecalEfect;

    [SerializeField] DecalType testDecal = DecalType.DecalType1;

    public ParticleSystem bodyPartSplatter;
    public ParticleSystem bloodSplash;

    GameObject decalHolder; // Holder/Parent object for decal objects(must be named "DecalHolder")

    //public int decalCount;

    public bool testControls = false; // Set to true to enable test controls
    public bool bloodAlphaFalloff = false;

    //public int maxDecalCount = 5000;

    public int wallDripsInProgress = 0;
    public int maxWallDripsInProgress = 100;

    //public Texture2D blood1_64x64;
    //public Texture2D blood2_64x64;
    //public Texture2D blood3_64x64;



    //public Texture2D bloodDecal16;
    //public Texture2D bloodDecal32;
    //public Texture2D bloodDecal64;
    //public Texture2D bloodDecal128;
    //public Texture2D bloodDecal256;
    //public Texture2D bloodDecal512;

    //public Texture2D bulletHoleDecal16;

    public ParticleSystem bulletImpactPS;
    //public ParticleSystem bulletImpactSmallPS;
    public ParticleSystem bulletImpactSplashPS;
    public ParticleSystem bulletImpactSplashSparklesPS;
    public ParticleSystem bulletImpactResiduePS;
    public ParticleSystem bulletImpactReflectSparklesPS;
    public ParticleSystem bulletImpactReflectPS;
    public ParticleSystem bulletImpactDustPS;

    public ParticleSystem explosionSmallPS;
    public ParticleSystem explosionMediumPS;
    public ParticleSystem explosionLargePS;

    public ParticleSystem explosionSmallSlowPS;
    public ParticleSystem explosionMediumSlowPS;
    public ParticleSystem explosionLargeSlowPS;

    public ParticleSystem lightningHitEffectPS;

    public ParticleSystem dustCloudMediumPS;
    public ParticleSystem dustCloudLargePS;

    public ParticleSystem bulletHitBloodBurstPS;
    public ParticleSystem bulletHitBloodMistPS;

    public ParticleSystem[] enemyDeathBloodBursts;
    public ParticleSystem enemyGibPS;
    //public ParticleSystem bloodBurstConeShortPS;
    //public ParticleSystem bloodBurstConeLongPS;
    //public ParticleSystem bloodBurstSphereShortPS;
    //public ParticleSystem bloodBurstSphereLongPS;
    //public ParticleSystem bloodExplosionPS;
    //public ParticleSystem bloodExplosionWTrailsPS;
    //public ParticleSystem bloodExplosion3DPS;
    //public ParticleSystem bloodExplosionWTrails3DPS;

    //[SerializeField]
    //public Texture2D[] bulletHoles;

    public bool flipBlood = false;
    public bool flipBulletHoles = false;

    float frames = 0;
    float frameTime = 0;

    public Dictionary<int, Color32> nonTransparentBloodPixels = new Dictionary<int, Color32>();
    public Dictionary<int, Color32> nonTransparentBlood1Pixels = new Dictionary<int, Color32>();
    public Dictionary<int, Color32> nonTransparentBlood2Pixels = new Dictionary<int, Color32>();
    public Dictionary<int, Color32> nonTransparentBlood3Pixels = new Dictionary<int, Color32>();

    public List<Dictionary<int, Color32>> bulletHoleDictionaries = new List<Dictionary<int, Color32>>();
    public List<Dictionary<int, Color32>> bloodSplatDictionaries = new List<Dictionary<int, Color32>>();
    public List<Dictionary<int, Color32>> rocketBlastDictionaries = new List<Dictionary<int, Color32>>();

    public Dictionary<int, Color32> nonTransparentBulletHolePixels = new Dictionary<int, Color32>();
    public Dictionary<int, Color32> nonTransparentRocketBlastPixels = new Dictionary<int, Color32>();


    //Dictionary<int, Color32> bloodDecalColorData = new Dictionary<int, Color32>();
    //Dictionary<int, Color32> bulletHoleDecalColorData = new Dictionary<int, Color32>();

    List<float> paintTimes = new List<float>();
    //bool paintInProgress = false;
    Queue<RaycastHit> paintQueue = new Queue<RaycastHit>();

    public struct wallDripStruct
    {
        public Texture2D tex;
        public Vector2 uv;
        public int bloodType;
    }

    public Queue<wallDripStruct> wallDripQueue = new Queue<wallDripStruct>();

    

    //Color32 bloodPixel = new Color32(156, 20, 28, 255);
    //Color32 bloodPixel = new Color32(129, 0, 7, 255);
    //Color32 bloodPixel = new Color32(121, 13, 0, 255);
    Color32 bloodPixel = new Color32(138, 3, 3, 255);

    Color32 bulletholeColor1 = new Color32(11, 11, 11, 255);
    Color32 bulletholeColor2 = new Color32(30, 28, 21, 255);
    Color32 bulletholeColor3 = new Color32(19, 19, 19, 255);
    Color32 bulletholeColor4 = new Color32(52, 47, 32, 255);
    Color32 bulletholeColor5 = new Color32(27, 25, 20, 255);
    Color32 bulletholeColor6 = new Color32(27, 25, 19, 255);

    Dictionary<int, List<Vector2>> dripStartSpotListsDictionary = new Dictionary<int, List<Vector2>>();
    //List<List<Vector2>> dripStartSpotList = new List<List<Vector2>>();

    void StoreNonTransparentPixelsBlood(Texture2D tex, Dictionary<int, Color32> dict)
    {
        for (int j = 0; j < tex.height; j++)
        {
            for (int i = 0; i < tex.width; i++)
            {
                Color32 pixel = tex.GetPixel(i, j);
                int neighbors = 0;

                // Get the amount of neighboring colored pixels
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        if (tex.GetPixel(i + x, j + y).a > 0)
                        {
                            neighbors++;
                        }
                    }
                }

                if (pixel.a == 255) // Default was > 0
                {
                    if (bloodAlphaFalloff)
                    {
                        // Alpha is used for normal and smoothness
                        pixel.a = (byte)(((float)neighbors / 8f) * 255f);
                    }
                    else
                    {
                        // Alpha is used for normal and smoothness
                        pixel.a = 50;
                    }
                    //(byte)Random.Range(0, 256); // For noisy result
                    if (flipBlood)
                    {
                        dict.Add(((tex.height - 1) - j) * tex.width + i, pixel);
                    }
                    else
                    {
                        dict.Add(j * tex.width + i, pixel);
                    }
                }
            }
        }
    }
    void StoreNonTransparentPixelsBulletHoles(Texture2D tex, Dictionary<int, Color32> dict)
    {
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color32 pixel = tex.GetPixel(x, y);
                //int neighbors = 0;

                // Get the amount of neighboring colored pixels
                //for (int y = -1; y < 2; y++)
                //{
                //    for (int x = -1; x < 2; x++)
                //    {
                //        if (tex.GetPixel(i + x, j + y).a > 0)
                //        {
                //            neighbors++;
                //        }
                //    }
                //}

                if (pixel.a == 255) // Default was > 0
                {
                    //if (bloodAlphaFalloff)
                    //{
                    //    // Alpha is used for normal and smoothness
                    //    pixel.a = (byte)(((float)neighbors / 8f) * 255f);
                    //}
                    //else
                    //{
                    //    // Alpha is used for normal and smoothness
                    //    pixel.a = 50;
                    //}
                    //(byte)Random.Range(0, 256); // For noisy result
                    if(flipBulletHoles)
                    {
                        dict.Add(((tex.height - 1) - y) * tex.width + x, pixel);
                    }
                    else
                    {
                        dict.Add(y * tex.width + x, pixel);
                    }
                }
            }
        }
    }

    void GenerateDripSpotLists()
    {
        UnityEngine.Debug.Log("Generating drip spot lists");
        for (int i = 0; i < bloodSplatSprites.Length; i++)
        {
            List<Vector2> dripSpotList = new List<Vector2>();
            for (int x = 0; x < bloodSplatSprites[i].width; x++)
            {
                for (int y = 0; y < bloodSplatSprites[i].height; y++)
                {
                    if (x - 1 >= 0 && x + 1 < bloodSplatSprites[i].width)
                    {
                        if (bloodSplatSprites[i].GetPixel(x - 1, y).a == 1 && bloodSplatSprites[i].GetPixel(x, y).a == 1 && bloodSplatSprites[i].GetPixel(x + 1, y).a == 1) // Color uses scale 0-1 Color32 uses scale 0-255
                        {
                            if(flipBlood)
                            {
                                dripSpotList.Add(new Vector2(x, bloodSplatSprites[i].height - y));
                            }
                            else
                            {
                                dripSpotList.Add(new Vector2(x, y));
                            }
                        }
                    }
                }
            }
            dripStartSpotListsDictionary.Add(i, dripSpotList);
        }
    }

    public void BulletWallHit(RaycastHit wallHit, Vector3 rayDir)
    {
        Color32 surfaceColor = GetPixelColor(wallHit);
        Color32 metallicColor = GetPixelMetallicColor(wallHit);
        //UnityEngine.Debug.Log("Surface color: " + surfaceColor);
        //UnityEngine.Debug.Log("Metallic color: " + metallicColor);

        if (metallicColor.r > 0)
        {
            ParticleSystem impactSplashSparkles = Instantiate(bulletImpactSplashSparklesPS);
            ParticleSystem impactReflectSparkles = Instantiate(bulletImpactReflectSparklesPS);

            impactSplashSparkles.gameObject.SetActive(true);
            impactSplashSparkles.transform.position = wallHit.point + wallHit.normal * 0.01f;
            impactSplashSparkles.transform.forward = wallHit.normal;


            impactReflectSparkles.gameObject.SetActive(true);
            impactReflectSparkles.transform.position = wallHit.point + wallHit.normal * 0.01f;
            Vector3 sparklesReflectionVector = Vector3.Reflect(rayDir, wallHit.normal);
            impactReflectSparkles.transform.forward = sparklesReflectionVector;//hit.normal;

            //FMODUnity.RuntimeManager.PlayOneShot("event:/SX/Weapons/Hand Mortar/Sx_handMortar_projectile_Bounce", wallHit.point);
        }
        else
        {
            ParticleSystem bulletImpact = Instantiate(bulletImpactPS);
            ParticleSystem impactSplash = Instantiate(bulletImpactSplashPS);
            ParticleSystem impactReflect = Instantiate(bulletImpactReflectPS);
            ParticleSystem impactResidue = Instantiate(bulletImpactResiduePS);
            ParticleSystem dustEffect = Instantiate(bulletImpactDustPS);


            if (surfaceColor.r > 0 || surfaceColor.g > 0 || surfaceColor.b > 0 && surfaceColor.a > 0)
            {
                surfaceColor.a = 255;
                ParticleSystem.MainModule impactMain = bulletImpact.main;
                ParticleSystem.MainModule splashMain = impactSplash.main;
                ParticleSystem.MainModule reflectMain = impactReflect.main;
                ParticleSystem.MainModule residueMain = impactResidue.main;

                impactMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                splashMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                reflectMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
                residueMain.startColor = new ParticleSystem.MinMaxGradient(surfaceColor, surfaceColor - new Color(30f / 255f, 30f / 255f, 30f / 255f, 0));
            }


            bulletImpact.gameObject.SetActive(true);
            bulletImpact.transform.position = wallHit.point + wallHit.normal * 0.01f;
            bulletImpact.transform.forward = wallHit.normal;


            impactSplash.gameObject.SetActive(true);
            impactSplash.transform.position = wallHit.point + wallHit.normal * 0.01f;
            impactSplash.transform.forward = wallHit.normal;


            impactReflect.gameObject.SetActive(true);
            impactReflect.transform.position = wallHit.point + wallHit.normal * 0.01f;
            Vector3 reflectionVector = Vector3.Reflect(rayDir, wallHit.normal);
            impactReflect.transform.forward = reflectionVector;//hit.normal;


            impactResidue.gameObject.SetActive(true);
            impactResidue.transform.position = wallHit.point + wallHit.normal * 0.01f;
            impactResidue.transform.forward = wallHit.normal;


            dustEffect.gameObject.SetActive(true);
            dustEffect.transform.position = wallHit.point + wallHit.normal * 0.13f;// Bring outwards from the surface to avoid clipping
            dustEffect.transform.position += new Vector3(0f, 0.2f, 0f);
            dustEffect.transform.forward = wallHit.normal;

            FMODUnity.RuntimeManager.PlayOneShot("event:/SX/Weapons/Hand Mortar/Sx_handMortar_projectile_Bounce", wallHit.point);
        }
        
        PaintBulletHole(wallHit);
       
    }

    //public void RocketWallHit(RaycastHit wallHit)
    //{
    //    ParticleSystem hitExplosion = Instantiate(explosionMediumSlowPS);

    //    hitExplosion.gameObject.SetActive(true);
    //    hitExplosion.transform.position = wallHit.point + wallHit.normal * 0.13f;// Bring outwards from the surface to avoid clipping
    //    hitExplosion.transform.position += new Vector3(0f, 0.2f, 0f);
    //    hitExplosion.transform.forward = wallHit.normal;

    //    PaintRocketBlastDecalTransparent(wallHit);
    //}

    public void ExplosionWallHit(Transform explosionTransform)
    {
        //UnityEngine.Debug.Log(wallContact.otherCollider.gameObject);

        //UnityEngine.Debug.Log("hit");
        ParticleSystem hitExplosion = Instantiate(explosionMediumSlowPS);

        hitExplosion.gameObject.SetActive(true);
        hitExplosion.transform.position = explosionTransform.position;// + explosionTransform.normal * 0.2f;// Bring outwards from the surface to avoid clipping
        //hitExplosion.transform.position += new Vector3(0f, 0.3f, 0f);
        //hitExplosion.transform.forward = explosionTransform.normal;

        RaycastHit wallHit;
        float hitDist = 0.7f;
        UnityEngine.Debug.DrawRay(explosionTransform.position, Vector3.forward * hitDist, Color.blue, 5f);
        if (Physics.Raycast(explosionTransform.position, Vector3.forward, out wallHit, hitDist, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay(explosionTransform.position, -Vector3.forward * hitDist, Color.blue, 5f);
        if (Physics.Raycast(explosionTransform.position, -Vector3.forward, out wallHit, hitDist, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay(explosionTransform.position, Vector3.right * hitDist, Color.red, 5f);
        if (Physics.Raycast(explosionTransform.position, Vector3.right, out wallHit, hitDist, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay(explosionTransform.position, -Vector3.right * hitDist, Color.red, 5f);
        if (Physics.Raycast(explosionTransform.position, -Vector3.right, out wallHit, hitDist, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay(explosionTransform.position, Vector3.up * hitDist, Color.green, 5f);
        if (Physics.Raycast(explosionTransform.position, Vector3.up, out wallHit, hitDist, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay(explosionTransform.position, -Vector3.up * hitDist, Color.green, 5f);
        if (Physics.Raycast(explosionTransform.position, -Vector3.up, out wallHit, hitDist, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        //// Blast ricochet

        //UnityEngine.Debug.DrawRay((explosionTransform.point + explosionTransform.normal * 0.1f), explosionTransform.otherCollider.transform.forward * 0.5f, Color.red, 5f);
        //if (Physics.Raycast((explosionTransform.point + explosionTransform.normal * 0.1f), explosionTransform.otherCollider.transform.forward, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        //{
        //    PaintRocketBlastDecalTransparent(wallHit);
        //}

        ////UnityEngine.Debug.DrawRay((wallContact.point + wallContact.normal * 0.1f), -wallContact.otherCollider.transform.forward * 0.5f, Color.red, 5f);
        //if (Physics.Raycast((explosionTransform.point + explosionTransform.normal * 0.1f), -explosionTransform.otherCollider.transform.forward, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        //{
        //    PaintRocketBlastDecalTransparent(wallHit);
        //}

        //UnityEngine.Debug.DrawRay((explosionTransform.point + explosionTransform.normal * 0.1f), explosionTransform.otherCollider.transform.right * 0.5f, Color.red, 5f);
        //if (Physics.Raycast((explosionTransform.point + explosionTransform.normal * 0.1f), explosionTransform.otherCollider.transform.right, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        //{
        //    PaintRocketBlastDecalTransparent(wallHit);
        //}

        //UnityEngine.Debug.DrawRay((explosionTransform.point + explosionTransform.normal * 0.1f), -explosionTransform.otherCollider.transform.right * 0.5f, Color.red, 5f);
        //if (Physics.Raycast((explosionTransform.point + explosionTransform.normal * 0.1f), -explosionTransform.otherCollider.transform.right, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        //{
        //    PaintRocketBlastDecalTransparent(wallHit);
        //}

        //// Outwards from hit point
        //UnityEngine.Debug.DrawRay((explosionTransform.point + explosionTransform.normal * 0.1f), explosionTransform.otherCollider.transform.up * 0.5f, Color.red, 5f);
        //if (Physics.Raycast((explosionTransform.point + explosionTransform.normal * 0.1f), explosionTransform.otherCollider.transform.up, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        //{
        //    PaintRocketBlastDecalTransparent(wallHit);
        //}
    }

    public void RocketWallHit(ContactPoint wallContact)
    {
        //UnityEngine.Debug.Log(wallContact.otherCollider.gameObject);

        //UnityEngine.Debug.Log("hit");
        ParticleSystem hitExplosion = Instantiate(explosionMediumSlowPS);

        hitExplosion.gameObject.SetActive(true);
        hitExplosion.transform.position = wallContact.point + wallContact.normal * 0.2f;// Bring outwards from the surface to avoid clipping
        hitExplosion.transform.position += new Vector3(0f, 0.3f, 0f);
        hitExplosion.transform.forward = wallContact.normal;

        RaycastHit wallHit;
        if(Physics.Raycast((wallContact.point + wallContact.normal * 0.1f), wallContact.point - (wallContact.point + wallContact.normal), out wallHit, 1f, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        // Blast ricochet

        UnityEngine.Debug.DrawRay((wallContact.point + wallContact.normal * 0.1f), wallContact.otherCollider.transform.forward * 0.5f, Color.red, 5f);
        if (Physics.Raycast((wallContact.point + wallContact.normal * 0.1f), wallContact.otherCollider.transform.forward, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        {
          PaintRocketBlastDecalTransparent(wallHit);
        }

        //UnityEngine.Debug.DrawRay((wallContact.point + wallContact.normal * 0.1f), -wallContact.otherCollider.transform.forward * 0.5f, Color.red, 5f);
        if (Physics.Raycast((wallContact.point + wallContact.normal * 0.1f), -wallContact.otherCollider.transform.forward, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay((wallContact.point + wallContact.normal * 0.1f), wallContact.otherCollider.transform.right * 0.5f, Color.red, 5f);
        if (Physics.Raycast((wallContact.point + wallContact.normal * 0.1f), wallContact.otherCollider.transform.right, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        UnityEngine.Debug.DrawRay((wallContact.point + wallContact.normal * 0.1f), -wallContact.otherCollider.transform.right * 0.5f, Color.red, 5f);
        if (Physics.Raycast((wallContact.point + wallContact.normal * 0.1f), -wallContact.otherCollider.transform.right, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }

        // Outwards from hit point
        UnityEngine.Debug.DrawRay((wallContact.point + wallContact.normal * 0.1f), wallContact.otherCollider.transform.up * 0.5f, Color.red, 5f);
        if (Physics.Raycast((wallContact.point + wallContact.normal * 0.1f), wallContact.otherCollider.transform.up, out wallHit, 0.5f, LayerMask.GetMask("Geometry")))
        {
            PaintRocketBlastDecalTransparent(wallHit);
        }
    }

    public void RocketEnemyHit(RaycastHit enemyHit)
    {
        ParticleSystem hitExplosion = Instantiate(explosionMediumSlowPS);
        ParticleSystem bloodBurst = Instantiate(bulletHitBloodBurstPS);

        hitExplosion.gameObject.SetActive(true);
        hitExplosion.transform.position = enemyHit.point + enemyHit.normal * 0.13f; // Bring outwards from the surface to avoid clipping
        //hitExplosion.transform.position += new Vector3(0f, 0.2f, 0f);
        hitExplosion.transform.forward = enemyHit.normal;
    }

    public void RocketEnemyHit(ContactPoint enemyContact)
    {
        ParticleSystem hitExplosion = Instantiate(explosionMediumSlowPS);
        ParticleSystem bloodBurst = Instantiate(bulletHitBloodBurstPS);

        hitExplosion.gameObject.SetActive(true);
        hitExplosion.transform.position = enemyContact.point + enemyContact.normal * 0.2f; // Bring outwards from the surface to avoid clipping
        hitExplosion.transform.position += new Vector3(0f, 0.3f, 0f);
        hitExplosion.transform.forward = enemyContact.normal;

        bloodBurst.gameObject.SetActive(true);
        bloodBurst.transform.position = enemyContact.point + enemyContact.normal * 0.01f;
        bloodBurst.transform.forward = enemyContact.normal;
    }

    public void BulletEnemyHit(RaycastHit enemyHit)
    {
        ParticleSystem bloodBurst = Instantiate(bulletHitBloodBurstPS);
        ParticleSystem bloodMist = Instantiate(bulletHitBloodMistPS);

        bloodBurst.gameObject.SetActive(true);
        bloodBurst.transform.position = enemyHit.point + enemyHit.normal * 0.01f;
        bloodBurst.transform.forward = enemyHit.normal;

        bloodMist.gameObject.SetActive(true);
        bloodMist.transform.position = enemyHit.point + enemyHit.normal * 0.01f;
        bloodMist.transform.forward = enemyHit.normal;
    }

    public void EnemyLightningHit(ContactPoint enemyContact)
    {
        ParticleSystem railGunEffect = Instantiate(lightningHitEffectPS);

        railGunEffect.gameObject.SetActive(true);
        railGunEffect.transform.position = enemyContact.point + enemyContact.normal * 0.13f; // Bring outwards from the surface to avoid clipping
        //hitExplosion.transform.position += new Vector3(0f, 0.2f, 0f);
        railGunEffect.transform.forward = enemyContact.normal;
    }

    public void EnemyLightningHit(RaycastHit[] enemyHits)
    {
        foreach(RaycastHit enemyHit in enemyHits)
        {
            ParticleSystem railGunEffect = Instantiate(lightningHitEffectPS);

            railGunEffect.gameObject.SetActive(true);
            railGunEffect.transform.position = enemyHit.point + enemyHit.normal * 0.13f; // Bring outwards from the surface to avoid clipping
            railGunEffect.transform.forward = enemyHit.normal;
        }
    }


    public void WallLightningHit(RaycastHit[] wallHits, Vector3 shootPos, Vector3 rayDir, float range)
    {
        int paints = 0;
        foreach (RaycastHit wallHit in wallHits)
        {
            ParticleSystem railGunEffect = Instantiate(lightningHitEffectPS);

            railGunEffect.gameObject.SetActive(true);
            railGunEffect.transform.position = wallHit.point + wallHit.normal * 0.13f; // Bring outwards from the surface to avoid clipping
            railGunEffect.transform.forward = wallHit.normal;

            // Limit paints to 4 for each direction
            if (paints < 4)
            {
                paints++;
                PaintRailGunBlastDecal(wallHit);
            }
        }

        Vector3 endPos = (shootPos + rayDir * range);

        //UnityEngine.Debug.DrawRay(endPos, shootPos - endPos, Color.blue, 5f);
        RaycastHit[] backCasts = Physics.RaycastAll(endPos, shootPos - endPos, range, LayerMask.GetMask("Geometry"));

        foreach (RaycastHit backHit in backCasts)
        {
            if (paints < 8)
            {
                paints++;
                PaintRailGunBlastDecal(backHit);
            }
        }
    }

    public void EnemyGib(Transform enemyPos, BloodBurst burstType)
    {
        //UnityEngine.Debug.Log("EnemyGib");
        ParticleSystem gib = Instantiate(enemyGibPS);
        ParticleSystem bloodBurst;

        if ((int)burstType >= 0 && (int)burstType < enemyDeathBloodBursts.Length)
        {
            bloodBurst = Instantiate(enemyDeathBloodBursts[(int)burstType]);//Random.Range(0, enemyDeathBloodBursts.Length)]);
        }
        else
        {
            bloodBurst = Instantiate(enemyDeathBloodBursts[(int)BloodBurst.ConeTrails]);
        }
        

        gib.transform.position = enemyPos.position;
        bloodBurst.transform.position = enemyPos.position;
    }

    //void fixLowResDecal(Texture2D decalTex)
    //{
    //    Color32[] decalColors = decalTex.GetPixels32();
    //    for(int i = 0; i < decalColors.Length; i++)
    //    {
    //        decalColors[i].a = 255;
    //    }
    //    decalTex.SetPixels32(decalColors);
    //}


    //void loadTexture(Texture2D tex)
    //{
    //    int textureSize = tex.width * tex.height;
    //    Color32[] colorData = tex.GetPixels32();
    //    //Debug.Log(colorData.Length);
    //    for (int i = 0; i < textureSize; i++)
    //    {
    //        decalColorData.Add(i, colorData[i]);
    //    }
    //    //Debug.Log(decalColorData.Count);
    //}

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

    public Color32 GetPixelMetallicColor(RaycastHit hit)
    {
        Color32 pixelColor = new Color32(0, 0, 0, 0);
        Renderer rend = hit.transform.GetComponent<Renderer>();
        if (rend != null)
        {
            Texture2D groundTex = (Texture2D)hit.transform.GetComponent<Renderer>().material.GetTexture("_MetallicGlossMap");

            if (groundTex != null && groundTex.isReadable)
            {
                pixelColor = (Color32)groundTex.GetPixel((int)(hit.textureCoord.x * groundTex.width), (int)(hit.textureCoord.y * groundTex.height));
                //UnityEngine.Debug.Log(pixelColor);
                return pixelColor;
            }
        }
        //Debug.Log("Couldnt get texture pixel");
        return pixelColor;
    }


    public void StartWallDripCoroutine(Texture2D tex, Vector2 v, int bloodType)
    {
        StartCoroutine(WallDrip(tex, v, bloodType));
    }

    public IEnumerator WallDrip(Texture2D tex, Vector2 brushCenter, int bloodType)
    {
        //UnityEngine.Debug.Log("bloodType: " + bloodType);
        wallDripsInProgress++;
        List<Vector2> dripSpots = new List<Vector2>();


        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            dripSpots.Add((brushCenter - new Vector2(bloodSplatSprites[bloodType].width / 2, bloodSplatSprites[bloodType].height / 2)) + dripStartSpotListsDictionary[bloodType][Random.Range(0, dripStartSpotListsDictionary[bloodType].Count)]);
        }


        int dripDist = 0;

        List<int> endDists = new List<int>();
        for (int i = 0; i < dripSpots.Count; i++)
        {
            endDists.Add((Random.Range(40, 100)));
        }


        //endDists.Add((Random.Range(40, 100)));
        //endDists.Add((Random.Range(40, 100)));
        //endDists.Add((Random.Range(40, 100)));
        //endDists.Add((Random.Range(40, 100)));
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
                    if(flipBlood)
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
                            //UnityEngine.Debug.Log("drip dist: " + dripDist);
                            //UnityEngine.Debug.Log((int)dripSpots[i].x - 1 + " " + ((int)dripSpots[i].y + dripDist));

                            tex.SetPixel((int)dripSpots[i].x - 1, (int)dripSpots[i].y + dripDist, bloodPixel);
                            tex.SetPixel((int)dripSpots[i].x + 1, (int)dripSpots[i].y + dripDist, bloodPixel);
                            tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y + dripDist, bloodPixel);
                            endDrip = false;
                        }
                    }
                    else
                    {
                        if ((float)endDists[i] / (float)dripDist < 1.3f && dripSpots[i].y - dripDist >= 0)
                        {
                            tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y - dripDist, bloodPixel);
                            endDrip = false;
                        }
                        else if ((float)endDists[i] / (float)dripDist < 2 && dripSpots[i].y - dripDist >= 0)
                        {
                            tex.SetPixel((int)dripSpots[i].x + 1, (int)dripSpots[i].y - dripDist, bloodPixel);
                            tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y - dripDist, bloodPixel);
                            endDrip = false;
                        }
                        else if (dripSpots[i].y - dripDist >= 0)
                        {
                            //UnityEngine.Debug.Log("drip dist: " + dripDist);
                            //UnityEngine.Debug.Log((int)dripSpots[i].x - 1 + " " + ((int)dripSpots[i].y + dripDist));

                            tex.SetPixel((int)dripSpots[i].x - 1, (int)dripSpots[i].y - dripDist, bloodPixel);
                            tex.SetPixel((int)dripSpots[i].x + 1, (int)dripSpots[i].y - dripDist, bloodPixel);
                            tex.SetPixel((int)dripSpots[i].x, (int)dripSpots[i].y - dripDist, bloodPixel);
                            endDrip = false;
                        }
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
        wallDripsInProgress--;

        //UnityEngine.Debug.Log("Wall drip stopped");
    }

    void GenerateTextureInstances()
    {
        UnityEngine.Debug.Log("Generating texture instances");
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        //UnityEngine.Debug.Log("renderers found: " + renderers.Length);
        List<Renderer> geometryRenderers = new List<Renderer>();

        foreach (Renderer rend in renderers)
        {
            //UnityEngine.Debug.Log("layer: " + rend.gameObject.layer);

            if (mask == (mask | (1 << rend.gameObject.layer)))// && rend.gameObject.name.Contains("valeseina"))
            {
                //UnityEngine.Debug.Log("Geometry renderer found");
                geometryRenderers.Add(rend);
            }
        }

        int count = 0;
        foreach (Renderer geoRend in geometryRenderers)
        {
            //UnityEngine.Debug.Log("Generating texture instance for object: " + geoRend.transform.gameObject);
            Texture2D originalTex = (Texture2D)geoRend.material.GetTexture("_BaseMap");
            if (originalTex != null && originalTex.isReadable)
            {
                //UnityEngine.Debug.Log(originalTex.format.ToString());
                Texture2D newTex = new Texture2D(originalTex.width, originalTex.height, TextureFormat.RGBA32, false);
                newTex.filterMode = FilterMode.Point; // Change to point for pixel perfect, Bilinear for blurry
                newTex.SetPixels32(originalTex.GetPixels32());
                newTex.IncrementUpdateCount(); // Make sure that update count is not 0. Change if need accurate updatecount
                newTex.Apply();
                geoRend.material.SetTexture("_BaseMap", newTex);
                count++;
            }
        }
        UnityEngine.Debug.Log(count + " texture instances generated");
    }

    public void PaintBulletHole(RaycastHit hit) // Maybe should use Enum or something for multiple different bullet hole types
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();

        if (rend != null)
        {
            Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");
            //UnityEngine.Debug.Log("Original texture format: " + originalTex.format);

            if (originalTex != null && originalTex.isReadable)
            {
                // Select random bullet hole type
                int bulletHoleType = Random.Range(0, bulletHoleSprites.Length);
                //Texture2D brush = bulletHoleDecal16;
                Texture2D brush = bulletHoleSprites[bulletHoleType];
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
                //UnityEngine.Debug.Log("x: " + uvx + " y: " + uvy);

                int paintStartX = uvx - (brushWidth / 2);
                int paintStartY = uvy - (brushHeight / 2);

                //UnityEngine.Debug.Log("PaintStartX:" + paintStartX);
                //UnityEngine.Debug.Log("PaintStartY:" + paintStartY);
                //UnityEngine.Debug.Log("PaintStart: " + paintStartX + ", " + paintStartY);



                Texture2D modifiedTex;
                bool createNewTex = false;
                // If texture has not been edited/copied yet, create a new one
                if (originalTex.updateCount == 0)
                {
                    createNewTex = true;
                    modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
                    modifiedTex.filterMode = FilterMode.Point;
                    Color32[] newColors = originalTex.GetPixels32();

                    modifiedTex.SetPixels32(newColors);
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
                    for (int j = 0; j < brushHeight; j++)
                    {
                        //int pos = j * decalWidth + i;
                        //Color32 color = decalColorData[pos];
                        if (paintStartX + i >= 0 && paintStartX + i < orgTexWidth && paintStartY + j >= 0 && paintStartY + j < orgTexHeight)
                        {
                            Color32 c;
                            if (bulletHoleDictionaries[bulletHoleType].TryGetValue(j * brushWidth + i, out c))
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
            }
        }
    }
    public void PaintRocketBlastDecal(RaycastHit hit) // Maybe should use Enum or something for multiple different bullet hole types
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();

        if (rend != null)
        {
            Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");
            //UnityEngine.Debug.Log("Original texture format: " + originalTex.format);

            if (originalTex != null && originalTex.isReadable)
            {
                // Select random bullet hole type
                int rocketBlastDecalType = Random.Range(0, rocketBlastSprites.Length);
                //Texture2D brush = bulletHoleDecal16;
                Texture2D brush = rocketBlastSprites[rocketBlastDecalType];
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
                //UnityEngine.Debug.Log("x: " + uvx + " y: " + uvy);

                int paintStartX = uvx - (brushWidth / 2);
                int paintStartY = uvy - (brushHeight / 2);

                //UnityEngine.Debug.Log("PaintStartX:" + paintStartX);
                //UnityEngine.Debug.Log("PaintStartY:" + paintStartY);



                Texture2D modifiedTex;
                bool createNewTex = false;
                // If texture has not been edited/copied yet, create a new one
                if (originalTex.updateCount == 0)
                {
                    createNewTex = true;
                    modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
                    modifiedTex.filterMode = FilterMode.Point;
                    Color32[] newColors = originalTex.GetPixels32();

                    modifiedTex.SetPixels32(newColors);
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
                    for (int j = 0; j < brushHeight; j++)
                    {
                        //int pos = j * decalWidth + i;
                        //Color32 color = decalColorData[pos];
                        if (paintStartX + i >= 0 && paintStartX + i < orgTexWidth && paintStartY + j >= 0 && paintStartY + j < orgTexHeight)
                        {
                            Color32 c;
                            if (rocketBlastDictionaries[rocketBlastDecalType].TryGetValue(j * brushWidth + i, out c))
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
            }
        }
    }
    public void PaintRocketBlastDecalTransparent(RaycastHit hit) // Maybe should use Enum or something for multiple different bullet hole types
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();

        if (rend != null)
        {
            Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");
            //UnityEngine.Debug.Log("Original texture format: " + originalTex.format);

            if (originalTex != null && originalTex.isReadable)
            {
                // Select random bullet hole type
                int rocketBlastDecalType = Random.Range(0, rocketBlastSprites.Length);
                //Texture2D brush = bulletHoleDecal16;
                Texture2D brush = rocketBlastSprites[rocketBlastDecalType];
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
                //UnityEngine.Debug.Log("x: " + uvx + " y: " + uvy);

                int paintStartX = uvx - (brushWidth / 2);
                int paintStartY = uvy - (brushHeight / 2);

                //UnityEngine.Debug.Log("PaintStartX:" + paintStartX);
                //UnityEngine.Debug.Log("PaintStartY:" + paintStartY);



                Texture2D modifiedTex;
                bool createNewTex = false;
                // If texture has not been edited/copied yet, create a new one
                if (originalTex.updateCount == 0)
                {
                    createNewTex = true;
                    modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
                    modifiedTex.filterMode = FilterMode.Point;
                    Color32[] newColors = originalTex.GetPixels32();

                    modifiedTex.SetPixels32(newColors);
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
                    for (int j = 0; j < brushHeight; j++)
                    {
                        //int pos = j * decalWidth + i;
                        //Color32 color = decalColorData[pos];
                        if (paintStartX + i >= 0 && paintStartX + i < orgTexWidth && paintStartY + j >= 0 && paintStartY + j < orgTexHeight)
                        {
                            Color32 brushPixel = brush.GetPixel(i, j);

                            Color32 c = Color32.Lerp((Color32)modifiedTex.GetPixel(paintStartX + i, paintStartY + j), (Color32)brushPixel, (float)brushPixel.a / 255f);
                            modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c);
                        }
                    }
                }

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
            }
        }
    }

    public void PaintRailGunBlastDecal(RaycastHit hit) // Maybe should use Enum or something for multiple different bullet hole types
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();

        if (rend != null)
        {
            Texture2D originalTex = (Texture2D)rend.material.GetTexture("_BaseMap"); //HDRP/Lit version: GetTexture("_BaseColorMap");
            //UnityEngine.Debug.Log("Original texture format: " + originalTex.format);

            if (originalTex != null && originalTex.isReadable)
            {
                // Select random bullet hole type
                int railGunBlastDecalType = Random.Range(0, railGunBlastSprites.Length);
                //Texture2D brush = bulletHoleDecal16;
                Texture2D brush = railGunBlastSprites[railGunBlastDecalType];
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
                //UnityEngine.Debug.Log("x: " + uvx + " y: " + uvy);

                int paintStartX = uvx - (brushWidth / 2);
                int paintStartY = uvy - (brushHeight / 2);

                //UnityEngine.Debug.Log("PaintStartX:" + paintStartX);
                //UnityEngine.Debug.Log("PaintStartY:" + paintStartY);



                Texture2D modifiedTex;
                bool createNewTex = false;
                // If texture has not been edited/copied yet, create a new one
                if (originalTex.updateCount == 0)
                {
                    createNewTex = true;
                    modifiedTex = new Texture2D(orgTexWidth, orgTexHeight, TextureFormat.RGBA32, false);// Create copy of the original texture
                    modifiedTex.filterMode = FilterMode.Point;
                    Color32[] newColors = originalTex.GetPixels32();

                    modifiedTex.SetPixels32(newColors);
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
                    for (int j = 0; j < brushHeight; j++)
                    {
                        //int pos = j * decalWidth + i;
                        //Color32 color = decalColorData[pos];
                        if (paintStartX + i >= 0 && paintStartX + i < orgTexWidth && paintStartY + j >= 0 && paintStartY + j < orgTexHeight)
                        {
                            Color32 brushPixel = brush.GetPixel(i, j);

                            Color32 c = Color32.Lerp((Color32)modifiedTex.GetPixel(paintStartX + i, paintStartY + j), (Color32)brushPixel, (float)brushPixel.a / 255f);
                            modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c);
                        }
                    }
                }

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
            }
        }
    }


    // Under 2ms on 512x512 textures, ~7ms with 1024x1024 textures (Deep Profiler mode), much faster in build (under 1ms 512x512 textures)
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
    //                    //int pos = j * decalWidth + i;
    //                    //Color32 color = decalColorData[pos];
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
    //                StartCoroutine(WallDrip(modifiedTex, new Vector2(uvx, uvy), bloodType));
    //            }
    //        }
    //    }
    //    //UnityEngine.Debug.LogError("PaintTime: " + (float)startToEndStopwatch.ElapsedTicks / (float)10000f + "ms");
    //    paintTimes.Add((float)startToEndStopwatch.ElapsedTicks / (float)10000f);
    //    GlobalVariables.instance.paintInProgress = false;
    //}

    // Takes 6(new texture) or 5(editing existing) frames to complete to reduce load on single frame
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
    //                StartCoroutine(WallDrip(modifiedTex, new Vector2(uvx, uvy), bloodType));
    //            }
    //        }
    //    }
    //    paintTimes.Add((float)startToEndStopwatch.ElapsedTicks / (float)10000f);
    //    //UnityEngine.Debug.LogError("PaintTime(Async): " + (float)startToEndStopwatch.ElapsedTicks / (float)10000f + "ms");
    //    //yield return null;
    //    GlobalVariables.instance.paintInProgress = false;
    //    //yield return null;
    //}

    // Execution time of this function is limited to maxvalue per frame(some single functions inside this function can still take longer)
    IEnumerator PaintTextureAsyncTimed(RaycastHit hit) // paintInprogress causes problems
    {
        GlobalVariables.instance.paintInProgress = true;
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

                Texture2D brush = bloodSplatSprites[bloodType];//blood1_64x64;

                //if (bloodType == 1)
                //{
                //    brush = blood1_64x64;
                //}
                //else if(bloodType == 2)
                //{
                //    brush = blood2_64x64;
                //}
                //else if (bloodType == 3)
                //{
                //    brush = blood3_64x64;
                //}
                //brush = bloodDecal64;
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
                                //UnityEngine.Debug.Log(c);
                                modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
                            }


                            //if(bloodType == 1)
                            //{
                            //    Color32 c;
                            //    if (nonTransparentBlood1Pixels.TryGetValue(j * brushWidth + i, out c))
                            //    {
                            //        modifiedTex.SetPixel(paintStartX + i, paintStartY + j, c); //brush.GetPixel(i, j)); // Alternative: Store pixel colors to Dictionary instead of using GetPixel from Texture2D? (almost the same speed, causes calls to color contructor)
                            //    }
                            //}
                            //else if(bloodType == 2)
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

                    wallDripQueue.Enqueue(wds);
                }
            }
        }
        paintTimes.Add((float)startToEndStopwatch.ElapsedTicks / (float)10000f);
        UnityEngine.Debug.LogError("PaintTime(TimedAsync): " + (float)startToEndStopwatch.ElapsedTicks / (float)10000f + "ms");

        //if (timeInInstance > maxTimePerInstance)
        //{
        //    yield return null;
        //}
        GlobalVariables.instance.paintInProgress = false; // Sometimes this object gets destroyed before the coroutine reaches this point
    }

    public void PlaceDecal(RaycastHit hit, DecalType type, DecalEffectType effectType = DecalEffectType.NoDecalEfect, float ttl = 10f) // If DecalEffectType is not given as an argument no effect is played
    {

        GameObject decal = Instantiate(decalCross);

        decal.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
        decal.transform.forward = hit.normal * -1;
        decal.transform.parent = decalHolder.transform;


        SpriteRenderer spirteRenderer = decal.GetComponent<SpriteRenderer>();

        spirteRenderer.sprite = decalSprites[(int)type];

        if (effectType != DecalEffectType.NoDecalEfect)
        {
            ParticleSystem effect = decalEffects[(int)effectType - 1];
            Instantiate(effect, decal.transform);
        }

        /*
        // Choose which decal sprite is placed
        switch (type)
        {
            case DecalType.DecalType1: 
                spirteRenderer.sprite = decalSprites[(int)DecalType.DecalType1];
                break;
            case DecalType.DecalType2:
                spirteRenderer.sprite = decalSprites[(int)DecalType.DecalType2];
                break;
            case DecalType.DecalType3:
                spirteRenderer.sprite = decalSprites[(int)DecalType.DecalType3];
                break;
            case DecalType.DecalType4:
                spirteRenderer.sprite = decalSprites[(int)DecalType.DecalType4];
                break;
            default: spirteRenderer.sprite = decalSprites[(int)DecalType.DecalType1];
                break;
        }

        // Choose effect type
        switch (effectType)
        {
            case DecalEffectType.NoDecalEfect: 
                break;
            case DecalEffectType.DecalEffectType1: 
                Instantiate(decalEffects[(int)DecalEffectType.DecalEffectType1 - 1], decal.transform);
                break;
            case DecalEffectType.DecalEffectType2:
                Instantiate(decalEffects[(int)DecalEffectType.DecalEffectType2 - 1], decal.transform);
                break;
            case DecalEffectType.DecalEffectType3:
                Instantiate(decalEffects[(int)DecalEffectType.DecalEffectType3 - 1], decal.transform);
                break;
            case DecalEffectType.DecalEffectType4:
                Instantiate(decalEffects[(int)DecalEffectType.DecalEffectType4 - 1], decal.transform);
                break;
            default: break;
        }
        */
    }


    public void SpawnBulletImpactPS(RaycastHit hit, Vector3 rayDir)
    {
        ParticleSystem impactReflect = Instantiate(bulletImpactPS);
        ParticleSystem impactSplash = Instantiate(bulletImpactSplashPS);
        ParticleSystem impactResidue = Instantiate(bulletImpactResiduePS);
        //ParticleSystem impactNormal = Instantiate(bulletImpactPS);
        //impactNormal.startColor = Color.red;

        impactReflect.gameObject.SetActive(true);
        impactReflect.transform.position = hit.point + hit.normal * 0.01f;
        Vector3 reflectionVector = Vector3.Reflect(rayDir, hit.normal);
        impactReflect.transform.forward = reflectionVector;//hit.normal;
        impactReflect.transform.parent = decalHolder.transform;


        impactSplash.gameObject.SetActive(true);
        impactSplash.transform.position = hit.point + hit.normal * 0.01f;
        impactSplash.transform.forward = hit.normal;
        impactSplash.transform.parent = decalHolder.transform;

        impactResidue.gameObject.SetActive(true);
        impactResidue.transform.position = hit.point + hit.normal * 0.01f;
        impactResidue.transform.forward = hit.normal;
        impactResidue.transform.parent = decalHolder.transform;

        Color32 surfaceColor = GetPixelColor(hit);
        if (surfaceColor.r > 0 || surfaceColor.g > 0 || surfaceColor.b > 0 && surfaceColor.a > 0)
        {
            impactSplash.startColor = surfaceColor;

            ParticleSystem.TrailModule trails = impactSplash.trails;
            ParticleSystem.MinMaxGradient trailColorModule = impactSplash.trails.colorOverLifetime;
            trailColorModule.color = surfaceColor;
            trails.colorOverLifetime = trailColorModule;
            //UnityEngine.Debug.Log("particle color: " + impactSplash.startColor);
            //UnityEngine.Debug.Log("surface color: " + surfaceColor);
            //UnityEngine.Debug.Log("trail color: " + impactSplash.trails.colorOverLifetime.color);
        }


        //impactNormal.gameObject.SetActive(true);
        //impactNormal.transform.position = hit.point + hit.normal * 0.01f;
        //float hitAngle =  Vector3.Angle(rayDir, hit.normal) - 90;
        //impactNormal.transform.forward = hit.normal;
        //impactNormal.transform.parent = decalHolder.transform;
    }
    // BloodSplash
    public void SpawnBloodSplash(RaycastHit hit)
    {
        ParticleSystem particles = Instantiate(bloodSplash);//, hit.point, Quaternion.identity);// + hit.normal * 0.3f, Quaternion.identity); // Problem when instantiating too close to the surface
        particles.gameObject.SetActive(true);
        //particles.transform.position = hit.point + hit.normal * 0.1f;
        particles.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
        //particles.transform.forward = hit.normal * -1;
        particles.transform.parent = decalHolder.transform;
    }

    private void Awake()
    {
        //GenerateTextureInstances(); // WARNING!!! SLOW! (seems like not necessary to generate instances of all textures at start,
        //can be done at runtime, for better overall performance(Performance will degrade overtime as new textures get created)) 
        
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
        for (int i = 0; i < bulletHoleSprites.Length; i++)
        {
            Dictionary<int, Color32> bulletHoleDictionary = new Dictionary<int, Color32>();
            StoreNonTransparentPixelsBulletHoles(bulletHoleSprites[i], bulletHoleDictionary);
            bulletHoleDictionaries.Add(bulletHoleDictionary);
        }

        for (int i = 0; i < rocketBlastSprites.Length; i++)
        {
            Dictionary<int, Color32> rocketBlastDictionary = new Dictionary<int, Color32>();
            StoreNonTransparentPixelsBulletHoles(rocketBlastSprites[i], rocketBlastDictionary);
            rocketBlastDictionaries.Add(rocketBlastDictionary);
        }

        for (int i = 0; i < bloodSplatSprites.Length; i++)
        {
            Dictionary<int, Color32> bloodSplatDictionary = new Dictionary<int, Color32>();
            StoreNonTransparentPixelsBlood(bloodSplatSprites[i], bloodSplatDictionary);
            bloodSplatDictionaries.Add(bloodSplatDictionary);
        }

        //for (int i = 0; i < bulletHoles.Length; i++)
        //{
        //    Dictionary<int, Color32> bulletHoleDictionary = new Dictionary<int, Color32>();
        //    StoreNonTransparentPixels(bulletHoles[i], bulletHoleDictionary);
        //    bulletHoleDictionaries.Add(bulletHoleDictionary);
        //}

        //for (int i = 0; i < bloodSplats.Length; i++)
        //{
        //    Dictionary<int, Color32> bloodSplatDictionary = new Dictionary<int, Color32>();
        //    StoreNonTransparentPixels(bloodSplats[i], bloodSplatDictionary);
        //    bloodSplatDictionaries.Add(bloodSplatDictionary);
        //}

        GenerateDripSpotLists();

        //Application.targetFrameRate = 300;
        //fixLowResDecal(lowResDecal64);
        decalHolder = GameObject.Find("DecalHolder");
        //StoreNonTransparentPixels(bloodDecal64, nonTransparentBloodPixels); // Make sure to use the same sprite and dictionary as in BloodSplatterCollision
        //storeNonTransparentPixels(bulletHoles[0], nonTransparentBulletHolePixels);
        //StoreNonTransparentPixels(blood1_64x64, nonTransparentBlood1Pixels);
        //StoreNonTransparentPixels(blood2_64x64, nonTransparentBlood2Pixels);
        //StoreNonTransparentPixels(blood3_64x64, nonTransparentBlood3Pixels);
        //loadTexture(lowResDecal);
        //StartCoroutine(spawnSplatter());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime > 0.02)
        {
            //UnityEngine.Debug.LogError("High frametime: " + Time.deltaTime * 1000 + "ms");
        }

        frames++;
        frameTime += Time.deltaTime;

        if (wallDripQueue.Count > 0 && wallDripsInProgress < maxWallDripsInProgress)
        {
            wallDripStruct w = new wallDripStruct();
            w = wallDripQueue.Dequeue();
            StartCoroutine(WallDrip(w.tex, w.uv, w.bloodType));
        }

        if (!GlobalVariables.instance.paintInProgress && paintQueue.Count > 0)
        {
            StartCoroutine(PaintTextureAsyncTimed(paintQueue.Dequeue()));
        }

        //decalCount = decalHolder.transform.childCount;

        // Start deleting SplatDecalProjectors when maxDecal count is reached // Possibly causes framedrops FIX!
        //if (decalCount > maxDecalCount)
        //{
        //    for (int i = 0; i < decalCount - maxDecalCount; i++)
        //    {
        //        if (decalHolder.transform.GetChild(i).name == "SplatDecalProjector(Clone)" || decalHolder.transform.GetChild(i).name == "BloodSplat(Clone)")
        //        {
        //            if (!decalHolder.transform.GetChild(i).GetComponent<Decal>().destroy) // Unnecessary if?
        //            {
        //                decalHolder.transform.GetChild(i).GetComponent<Decal>().destroy = true;
        //            }
        //        }
        //    }
        //}

        #region Testing controls
        if (testControls)
        {
            // Instantiate body part splatter effect
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                    {
                        if (bloodSplash != null)
                        {
                            //ParticleSystem particles = Instantiate(bloodSplash);
                            //particles.gameObject.SetActive(true);
                            //particles.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
                            //particles.transform.forward = hit.normal * -1;
                            //particles.transform.parent = decalHolder.transform;
                            //SpawnBloodSplash(hit);
                            //particles.Play();
                        }
                    }
                }
                else if (Input.GetKey(KeyCode.V))
                {
                    if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                    {
                        //if (bloodBurstConeShortPS != null)
                        //{
                        //    Vector3 rayDir = (Camera.main.ScreenPointToRay(Input.mousePosition).direction);
                        //    //ParticleSystem particles = Instantiate(bloodSplash);
                        //    //particles.gameObject.SetActive(true);
                        //    //particles.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
                        //    //particles.transform.forward = hit.normal * -1;
                        //    //particles.transform.parent = decalHolder.transform;
                        //    //SpawnBloodBurstSphereShort(hit, rayDir);
                        //    SpawnBloodExplosion(hit, rayDir);
                        //    //particles.Play();
                        //}
                    }
                }

                if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                {
                    /*
                    //Debug.Log("Ray hit: " + hit.transform.gameObject.name);
                    if (bodyPartSplatter != null)
                    {
                        ParticleSystem particles = Instantiate(bodyPartSplatter);
                        particles.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
                        particles.transform.forward = hit.normal * -1;
                        particles.transform.parent = decalHolder.transform;
                        //particles.Play();
                    }
                    */
                    //PaintTexture(hit);
                }

            }

            // Delete all decals
            //if (Input.GetKeyDown(KeyCode.Backspace))
            //{
            //    if (decalHolder != null)
            //    {
            //        foreach (Transform decal in decalHolder.transform)
            //        {
            //            Destroy(decal.gameObject);
            //        }
            //    }
            //}

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                    {
                        //UnityEngine.Debug.Log("Ray hit: " + hit.transform.gameObject.name);
                        if (bodyPartSplatter != null)
                        {
                            //StartCoroutine(PaintTextureAsyncTimed(hit));
                            //ParticleSystem particles = Instantiate(bodyPartSplatter);
                            //particles.gameObject.SetActive(true);
                            //particles.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
                            //particles.transform.forward = hit.normal * -1;
                            //particles.transform.parent = decalHolder.transform;
                            //SpawnBodyPartSplatter(hit.point);
                            //UnityEngine.Debug.LogError("Avg frametime: " + frameTime * 1000  / frames + "ms");
                            frameTime = 0;
                            frames = 0;
                            //particles.Play();
                        }
                    }
                }
                else if (Input.GetKey(KeyCode.V))
                {
                    if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                    {
                        
                            Vector3 rayDir = (Camera.main.ScreenPointToRay(Input.mousePosition).direction);
                            //ParticleSystem particles = Instantiate(bloodSplash);
                            //particles.gameObject.SetActive(true);
                            //particles.transform.position = hit.point + hit.normal * 0.01f; // Offset decal by 0.01 units outwards from the surface
                            //particles.transform.forward = hit.normal * -1;
                            //particles.transform.parent = decalHolder.transform;
                            //SpawnBloodBurstConeLong(hit, rayDir);
                            //SpawnBloodBurstConeLong(hit, rayDir, true);
                            //SpawnBloodExplosion3D(hit, rayDir);
                            //BulletWallHit(hit, rayDir);
                            //particles.Play();
                        
                    }
                }
                else
                {
                    //if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask)) // Change max dist 
                    //{
                    //    if (hit.collider != null)
                    //    {
                    //        // If we hit MeshCollider paint texture
                    //        //if (hit.collider.GetType() == typeof(MeshCollider))
                    //       // {
                    //            //UnityEngine.Debug.Log("Ray hit: " + hit.transform.gameObject.name);
                    //            //int decalType = Random.Range(0, decalSprites.Length);
                    //            //PlaceDecal(hit, (DecalType)decalType, (DecalEffectType)decalType + 1);
                    //            //PlaceDecal(hit, testDecal, testEffect);
                    //            if (!GlobalVariables.instance.paintInProgress)
                    //            {
                    //                //PaintTexture(hit);
                    //                StartCoroutine(PaintTextureAsyncTimed(hit)); // Async version
                    //            }
                    //            else
                    //            {
                    //                paintQueue.Enqueue(hit);
                    //            }
                    //       // }
                    //    }
                }

                //Debug.Log("Fired");
                if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask)) // Change max dist 
                {
                    //BulletEnemyHit(hit);
                    //EnemyGib(hit.collider.transform, Random.Range(0, enemyDeathBloodBursts.Length));
                    //RocketWallHit(hit);

                    Vector3 rayDir = (Camera.main.ScreenPointToRay(Input.mousePosition).direction);
                    //UnityEngine.Debug.Log("Collider type1: " + hit.collider.GetType());
                    int boxColliderHits = 0;
                    int maxColliderHits = 3;
                    List<BoxCollider> boxCollidersHit = new List<BoxCollider>();

                    // While we hit BoxColliders and boxColliderHits is less than maxColliderHits, disable boxColliders add them to list and cast more rays
                    while (hit.collider != null && hit.collider.GetType() == typeof(BoxCollider) && boxColliderHits < maxColliderHits)
                    {
                        hit.collider.enabled = false;
                        boxCollidersHit.Add((BoxCollider)hit.collider);
                        if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask)) // Change max dist 
                        {
                            //UnityEngine.Debug.Log("Collider type" + boxColliderHits + ": " + hit.collider.GetType());
                        }
                        boxColliderHits++;
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
                            //UnityEngine.Debug.Log("Ray hit: " + hit.transform.gameObject.name);
                            //int decalType = Random.Range(0, decalSprites.Length);
                            //PlaceDecal(hit, (DecalType)decalType, (DecalEffectType)decalType + 1);
                            //PlaceDecal(hit, testDecal, testEffect);
                            if (!GlobalVariables.instance.paintInProgress)
                            {
                                //PaintTexture(hit);
                                StartCoroutine(PaintTextureAsyncTimed(hit)); // Async version
                                //StartCoroutine(PaintTextureAsync(hit)); // Async version
                                //PaintBulletHole(hit);
                                //SpawnBulletImpactPS(hit, rayDir);
                                //PaintRocketBlastDecal(hit);
                                //PaintRocketBlastDecalTransparent(hit);
                                //BulletWallHit(hit, rayDir);
                                //BulletEnemyHit(hit);
                            }
                            else
                            {
                                paintQueue.Enqueue(hit);
                            }
                        }
                    }


                    //if (hit.collider.GetType() == typeof(BoxCollider))
                    //{
                    //    hit.transform.GetComponent<BoxCollider>().enabled = false;
                    //    if (Physics.Raycast((Camera.main.ScreenPointToRay(Input.mousePosition)), out hit, Mathf.Infinity, mask))
                    //    {
                    //        UnityEngine.Debug.Log("Collider type2: " + hit.collider.GetType());
                    //        if (hit.collider.GetType() == typeof(MeshCollider))
                    //        {
                    //            //Debug.Log("Ray hit: " + hit.transform.gameObject.name);
                    //            //int decalType = Random.Range(0, decalSprites.Length);
                    //            //PlaceDecal(hit, (DecalType)decalType, (DecalEffectType)decalType + 1);
                    //            //PlaceDecal(hit, testDecal, testEffect);
                    //            if (!paintInProgress)
                    //            {
                    //                PaintTexture(hit);
                    //                //StartCoroutine(PaintTextureAsyncTimed(hit)); // Async version
                    //            }
                    //            else
                    //            {
                    //                //paintQueue.Enqueue(hit);
                    //            }
                    //        }
                    //    }
                    //    hit.transform.GetComponent<BoxCollider>().enabled = true;
                    //}
                    //else
                    //{
                    //    UnityEngine.Debug.Log("Collider type: " + hit.collider.GetType());
                    //    if (hit.collider.GetType() == typeof(MeshCollider))
                    //    {
                    //        //Debug.Log("Ray hit: " + hit.transform.gameObject.name);
                    //        //int decalType = Random.Range(0, decalSprites.Length);
                    //        //PlaceDecal(hit, (DecalType)decalType, (DecalEffectType)decalType + 1);
                    //        //PlaceDecal(hit, testDecal, testEffect);
                    //        if (!paintInProgress)
                    //        {
                    //            PaintTexture(hit);
                    //            //StartCoroutine(PaintTextureAsyncTimed(hit)); // Async version
                    //        }
                    //        else
                    //        {
                    //            //paintQueue.Enqueue(hit);
                    //        }
                    //    }
                    //}

                    //}
                    //Debug.Log("Decal count: " + decalCount);
                }
            }
        }
        #endregion
    }
}
