//using FMOD;
using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


//Pauli
public enum _States { IDLE, RUN, HIT, DEATH, SHOOT, STUN, JUMP, SPECIAL };
public class SpriteAnimator : MonoBehaviour
{
    public bool usingCustomShader;
    public bool usingNormals = true;
    public bool usingEmissions = true;
    public bool usingMetallics = true;
    public bool usingOcclusions = true;

    public bool destroyAfterDone;
    public bool doOnce = false;
    public bool pauseCounter = false;
    _States newState;
    public _States currentState;
    int dir;
    public float counter = 0;
    public int frame = 0;
    public bool animDone;

    public float idleTimeBetweenAnimFrames;
    public float movementTimeBetweenAnimFrames;
    public float hitTimeBetweenAnimFrames;
    public float deathTimeBetweenAnimFrames;
    public float shootTimeBetweenAnimFrames;
    public float stunTimeBetweenAnimFrames;
    public float jumpTimeBetweenAnimFrames;
    public float specialTimeBetweenAnimFrames;

    public bool onlyOneTextureSet;

    enum Directions { SOUTH, SOUTHEAST, EAST, NORTHEAST, NORTH, NORTHWEST, WEST, SOUTHWEST };
    public SpriteDirController controller;
    public Material material;

    ////All file paths from Resources folder to sets of textures
    //public string idleTexturePath;
    //public string idleNormalPath;
    //public string idleEmissionPath;
    //public string idleMetallicPath;
    //public string idleOcclusionPath;

    //public string movementTexturePath;
    //public string movementNormalPath;
    //public string movementEmissionPath;
    //public string movementMetallicPath;
    //public string movementOcclusionPath;

    //public string hitTexturePath;
    //public string hitNormalPath;
    //public string hitEmissionPath;
    //public string hitMetallicPath;
    //public string hitOcclusionPath;

    //public string deathTexturePath;
    //public string deathNormalPath;
    //public string deathEmissionPath;
    //public string deathMetallicPath;
    //public string deathOcclusionPath;

    //public string shootTexturePath;
    //public string shootNormalPath;
    //public string shootEmissionPath;
    //public string shootMetallicPath;
    //public string shootOcclusionPath;

    //public string stunTexturePath;
    //public string stunNormalPath;
    //public string stunEmissionPath;
    //public string stunMetallicPath;
    //public string stunOcclusionPath;

    //public string jumpTexturePath;
    //public string jumpNormalPath;
    //public string jumpEmissionPath;
    //public string jumpMetallicPath;
    //public string jumpOcclusionPath;

    //public string specialTexturePath;
    //public string specialNormalPath;
    //public string specialEmissionPath;
    //public string specialMetallicPath;
    //public string specialOcclusionPath;


    //The amount of frames for each direction (e.g 8 directions with 6 frames each -> amount would be 6)
    int amountOfIdleTexturesPerDir;
    int amountOfMovementTexturesPerDir;
    int amountOfHitTexturesPerDir;
    int amountOfDeathTexturesPerDir;
    int amountOfShootTexturesPerDir;
    int amountOfStunTexturesPerDir;
    int amountOfJumpTexturesPerDir;
    int amountOfSpecialTexturesPerDir;

    //lists of all objects' from path
    public List<Object> idleTextures;
    public List<Object> idleNormals;
    public List<Object> idleEmissions;
    public List<Object> idleMetallics;
    public List<Object> idleOcclusions;

    public List<Object> movementTextures;
    public List<Object> movementNormals;
    public List<Object> movementEmissions;
    public List<Object> movementMetallics;
    public List<Object> movementOcclusions;

    public List<Object> hitTextures;
    public List<Object> hitNormals;
    public List<Object> hitEmissions;
    public List<Object> hitMetallics;
    public List<Object> hitOcclusions;

    public List<Object> deathTextures;
    public List<Object> deathNormals;
    public List<Object> deathEmissions;
    public List<Object> deathMetallics;
    public List<Object> deathOcclusions;

    public List<Object> shootTextures;
    public List<Object> shootNormals;
    public List<Object> shootEmissions;
    public List<Object> shootMetallics;
    public List<Object> shootOcclusions;

    public List<Object> stunTextures;
    public List<Object> stunNormals;
    public List<Object> stunEmissions;
    public List<Object> stunMetallics;
    public List<Object> stunOcclusions;

    public List<Object> jumpTextures;
    public List<Object> jumpNormals;
    public List<Object> jumpEmissions;
    public List<Object> jumpMetallics;
    public List<Object> jumpOcclusions;

    public List<Object> specialTextures;
    public List<Object> specialNormals;
    public List<Object> specialEmissions;
    public List<Object> specialMetallics;
    public List<Object> specialOcclusions;

    //Used as ambiguous variables for all the above
    //[HideInInspector]
    int amountOfTexturesPerDir;
    List<Object> allTextures;
    List<Object> allNormals;
    List<Object> allEmissions;
    List<Object> allMetallics;
    List<Object> allOcclusions;
    [HideInInspector]
    public float timeBetweenAnimFrames;

    
    [SerializeField]
    EnemyAi enemyAi;
    [SerializeField]
    PlayerMovement playerMovement;
    [SerializeField]
    GunController gunController;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<EnemyAi>() != null) enemyAi = GetComponent<EnemyAi>();
        if(transform.parent != null)
        {
            if (transform.parent.GetComponent<PlayerMovement>() != null) playerMovement = transform.parent.GetComponent<PlayerMovement>();
            if (transform.parent.GetChild(0).GetChild(0).GetComponent<GunController>() != null) gunController = transform.parent.GetChild(0).GetChild(0).GetComponent<GunController>();
        }
        
        LoadTextures();



        //transform.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_NORMALMAP");
        //transform.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_METALLICGLOSSMAP");
        if (!onlyOneTextureSet)
        {
            controller = GetComponent<SpriteDirController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerMovement != null)
        {
            if (playerMovement.grappling && currentState != _States.SPECIAL)
            {
                ChangeState(_States.SPECIAL);
            }
            if (!playerMovement.grappling && currentState == _States.SPECIAL)
            {
                if(transform.parent.GetChild(0).GetComponent<RocketPack>().enabled)
                {
                    if(transform.parent.GetChild(0).GetComponent<RocketPack>().crashing)
                    {
                        ChangeState(_States.HIT);
                    }
                }
                else
                {
                    ChangeState(_States.IDLE);
                }
            }
            if (transform.parent.GetChild(0).GetComponent<RocketPack>().enabled && currentState == _States.HIT)
            {
                if (!transform.parent.GetChild(0).GetComponent<RocketPack>().crashing)
                {
                    ChangeState(_States.IDLE);
                }       
            }
            if (!playerMovement.grounded && currentState != _States.JUMP && !playerMovement.grappling)
            {
                ChangeState(_States.JUMP);
                
            }
            if (playerMovement.grounded && currentState == _States.JUMP)
            {
                ChangeState(_States.IDLE);
            }
            if(playerMovement.grounded && Mathf.Lerp(0, 1, Mathf.Clamp(new Vector3(playerMovement.playerRB.velocity.x, 0, playerMovement.playerRB.velocity.z).magnitude, 0, 1)) > 0.01f && currentState != _States.RUN)
            {
                ChangeState(_States.RUN);
            }
            if(Mathf.Lerp(0, 1, Mathf.Clamp(new Vector3(playerMovement.playerRB.velocity.x, 0, playerMovement.playerRB.velocity.z).magnitude, 0, 1)) <= 0.01f && currentState == _States.RUN)
            {
                ChangeState(_States.IDLE);
            }
            if(playerMovement.dodging && currentState != _States.DEATH)
            {
                ChangeState(_States.DEATH);
            }
            if(!playerMovement.dodging && currentState == _States.DEATH)
            {
                ChangeState(_States.IDLE);
            }
            if(gunController.shot && currentState != _States.SHOOT)
            {
                ChangeState(_States.SHOOT);
            }
            if(!gunController.shot && currentState == _States.SHOOT)
            {
                ChangeState(_States.IDLE);
            }
        }
        if(enemyAi != null)
        {
            if (enemyAi.specialAttackStart && currentState != _States.SPECIAL && currentState != _States.SHOOT && !enemyAi.specialRecovering)
            {
                ChangeState(_States.SPECIAL);
            }
            if (currentState == _States.SPECIAL && animDone)
            {
                ChangeState(_States.SHOOT);
            }
            if(!enemyAi.specialRecovering && currentState == _States.SHOOT)
            {
                ChangeState(_States.IDLE);
            }
            if (enemyAi.meleeAttackStart && currentState != _States.HIT && currentState != _States.RUN && !enemyAi.MeleeRecovering)
            {
                ChangeState(_States.HIT);
            }
            if (currentState == _States.HIT && animDone)
            {
                ChangeState(_States.RUN);
            }
            if (!enemyAi.MeleeRecovering && currentState == _States.RUN)
            {
                ChangeState(_States.IDLE);
            }
            if (!enemyAi.grounded && currentState != _States.JUMP)
            {
                ChangeState(_States.JUMP);
            }
            if(enemyAi.grounded && currentState == _States.JUMP)
            {
                ChangeState(_States.IDLE);
            }
        }
        
        //Dir being the direction the sprite looks at
        if (!onlyOneTextureSet)
        {
            dir = controller.spriteDir;
        }


        //Counter for constantly changing the sprite
        counter += Time.deltaTime;
        animDone = false;
        if (counter > timeBetweenAnimFrames)
        {
            if (amountOfTexturesPerDir <= 1 && !doOnce)
            {
                counter = 0;
                //Debug.Log("Frame with many textures " + frame);
                NextFrame(frame);
                frame++;
                doOnce = true;
            }
            else if (amountOfTexturesPerDir > 1)
            {
                counter = 0;
                //Debug.Log("Frame with one " + frame);
                NextFrame(frame);
                frame++;
                doOnce = false;
            }


            if (frame >= amountOfTexturesPerDir)
            {
                animDone = true;
                if (destroyAfterDone)
                {
                    Destroy(this.gameObject);
                }
                frame = 0;
            }
        }

    }

    //Changes the sprite depending on the rotation
    void NextFrame(int currentFrame)
    {
        if(!pauseCounter)
        {
            if(currentFrame >= allTextures.Count)
            {
                LoadTextures();
            }
            if(allTextures.Count == 0)
            {
                LoadTextures();
            }
            if (onlyOneTextureSet)
            {
                //Debug.Log(currentFrame + " = currentFrame");
                material.mainTexture = (Texture2D)allTextures[currentFrame];
                if (usingNormals) material.SetTexture("_BumpMap", (Texture2D)allNormals[currentFrame]);
                if (usingEmissions) material.SetTexture("_EmissionMap", (Texture2D)allEmissions[currentFrame]);
                if (usingMetallics) material.SetTexture("_MetallicGlossMap", (Texture2D)allMetallics[currentFrame]);
                if (usingOcclusions) material.SetTexture("_OcclusionMap", (Texture2D)allOcclusions[currentFrame]);

            }

            else
            {
                material.mainTexture = (Texture2D)allTextures[currentFrame + amountOfTexturesPerDir * dir];
                if (usingNormals) material.SetTexture("_BumpMap", (Texture2D)allNormals[currentFrame + amountOfTexturesPerDir * dir]);
                if (usingEmissions)  material.SetTexture("_EmissionMap", (Texture2D)allEmissions[currentFrame + amountOfTexturesPerDir * dir]);
                if (usingMetallics)  material.SetTexture("_MetallicGlossMap", (Texture2D)allMetallics[currentFrame + amountOfTexturesPerDir * dir]);
                if (usingOcclusions) material.SetTexture("_OcclusionMap", (Texture2D)allOcclusions[currentFrame + amountOfTexturesPerDir * dir]);
            }
        }

        



    }

    //Changes the objects state (0=idle, 1=move, 2=attack)
    public void ChangeState(_States state)
    {
        switch (state)
        {
            case _States.IDLE:
                currentState = _States.IDLE;
                amountOfTexturesPerDir = amountOfIdleTexturesPerDir;
                allTextures = idleTextures;
                allNormals = idleNormals;
                allEmissions = idleEmissions;
                allMetallics = idleMetallics;
                allOcclusions = idleOcclusions;
                timeBetweenAnimFrames = idleTimeBetweenAnimFrames;
                break;
            case _States.RUN:
                currentState = _States.RUN;
                amountOfTexturesPerDir = amountOfMovementTexturesPerDir;
                allTextures = movementTextures;
                allNormals = movementNormals;
                allEmissions = movementEmissions;
                allMetallics = movementMetallics;
                allOcclusions = movementOcclusions;
                timeBetweenAnimFrames = movementTimeBetweenAnimFrames;
                break;
            case _States.HIT:
                currentState = _States.HIT;
                amountOfTexturesPerDir = amountOfHitTexturesPerDir;
                allTextures = hitTextures;
                allNormals = hitNormals;
                allEmissions = hitEmissions;
                allMetallics = hitMetallics;
                allOcclusions = hitOcclusions;
                timeBetweenAnimFrames = hitTimeBetweenAnimFrames;
                break;
            case _States.DEATH:
                currentState = _States.DEATH;
                amountOfTexturesPerDir = amountOfDeathTexturesPerDir;
                allTextures = deathTextures;
                allNormals = deathNormals;
                allEmissions = deathEmissions;
                allMetallics = deathMetallics;
                allOcclusions = deathOcclusions;
                timeBetweenAnimFrames = deathTimeBetweenAnimFrames;
                break;
            case _States.SHOOT:
                currentState = _States.SHOOT;
                amountOfTexturesPerDir = amountOfShootTexturesPerDir;
                allTextures = shootTextures;
                allNormals = shootNormals;
                allEmissions = shootEmissions;
                allMetallics = shootMetallics;
                allOcclusions = shootOcclusions;
                timeBetweenAnimFrames = shootTimeBetweenAnimFrames;
                break;
            case _States.STUN:
                currentState = _States.STUN;
                amountOfTexturesPerDir = amountOfStunTexturesPerDir;
                allTextures = stunTextures;
                allNormals = stunNormals;
                allEmissions = stunEmissions;
                allMetallics = stunMetallics;
                allOcclusions = stunOcclusions;
                timeBetweenAnimFrames = stunTimeBetweenAnimFrames;
                break;
            case _States.JUMP:
                currentState = _States.JUMP;
                amountOfTexturesPerDir = amountOfJumpTexturesPerDir;
                allTextures = jumpTextures;
                allNormals = jumpNormals;
                allEmissions = jumpEmissions;
                allMetallics = jumpMetallics;
                allOcclusions = jumpOcclusions;
                timeBetweenAnimFrames = jumpTimeBetweenAnimFrames;
                break;
            case _States.SPECIAL:
                currentState = _States.SPECIAL;
                amountOfTexturesPerDir = amountOfSpecialTexturesPerDir;
                allTextures = specialTextures;
                allNormals = specialNormals;
                allEmissions = specialEmissions;
                allMetallics = specialMetallics;
                allOcclusions = specialOcclusions;
                timeBetweenAnimFrames = specialTimeBetweenAnimFrames;
                break;
        }
        frame = 0;
        counter = 0;
        animDone = false;
    }

    public void LoadTextures()
    {
        /*
        //Loads all the resources
        idleTextures = Resources.LoadAll(idleTexturePath, typeof(Texture2D)).ToList();
        idleNormals = Resources.LoadAll(idleNormalPath, typeof(Texture2D)).ToList();
        idleEmissions = Resources.LoadAll(idleEmissionPath, typeof(Texture2D)).ToList();
        idleMetallics = Resources.LoadAll(idleMetallicPath, typeof(Texture2D)).ToList();
        idleOcclusions = Resources.LoadAll(idleOcclusionPath, typeof(Texture2D)).ToList();

        movementTextures = Resources.LoadAll(movementTexturePath, typeof(Texture2D)).ToList();
        movementNormals = Resources.LoadAll(movementNormalPath, typeof(Texture2D)).ToList();
        movementEmissions = Resources.LoadAll(movementEmissionPath, typeof(Texture2D)).ToList();
        movementMetallics = Resources.LoadAll(movementMetallicPath, typeof(Texture2D)).ToList();
        movementOcclusions = Resources.LoadAll(movementOcclusionPath, typeof(Texture2D)).ToList();

        hitTextures = Resources.LoadAll(hitTexturePath, typeof(Texture2D)).ToList();
        hitNormals = Resources.LoadAll(hitNormalPath, typeof(Texture2D)).ToList();
        hitEmissions = Resources.LoadAll(hitEmissionPath, typeof(Texture2D)).ToList();
        hitMetallics = Resources.LoadAll(hitMetallicPath, typeof(Texture2D)).ToList();
        hitOcclusions = Resources.LoadAll(hitOcclusionPath, typeof(Texture2D)).ToList();

        deathTextures = Resources.LoadAll(deathTexturePath, typeof(Texture2D)).ToList();
        deathNormals = Resources.LoadAll(deathNormalPath, typeof(Texture2D)).ToList();
        deathEmissions = Resources.LoadAll(deathEmissionPath, typeof(Texture2D)).ToList();
        deathMetallics = Resources.LoadAll(deathMetallicPath, typeof(Texture2D)).ToList();
        deathOcclusions = Resources.LoadAll(deathOcclusionPath, typeof(Texture2D)).ToList();

        shootTextures = Resources.LoadAll(shootTexturePath, typeof(Texture2D)).ToList();
        shootNormals = Resources.LoadAll(shootNormalPath, typeof(Texture2D)).ToList();
        shootEmissions = Resources.LoadAll(shootEmissionPath, typeof(Texture2D)).ToList();
        shootMetallics = Resources.LoadAll(shootMetallicPath, typeof(Texture2D)).ToList();
        shootOcclusions = Resources.LoadAll(shootOcclusionPath, typeof(Texture2D)).ToList();

        stunTextures = Resources.LoadAll(stunTexturePath, typeof(Texture2D)).ToList();
        stunNormals = Resources.LoadAll(stunNormalPath, typeof(Texture2D)).ToList();
        stunEmissions = Resources.LoadAll(stunEmissionPath, typeof(Texture2D)).ToList();
        stunMetallics = Resources.LoadAll(stunMetallicPath, typeof(Texture2D)).ToList();
        stunOcclusions = Resources.LoadAll(stunOcclusionPath, typeof(Texture2D)).ToList();

        jumpTextures = Resources.LoadAll(jumpTexturePath, typeof(Texture2D)).ToList();
        jumpNormals = Resources.LoadAll(jumpNormalPath, typeof(Texture2D)).ToList();
        jumpEmissions = Resources.LoadAll(jumpEmissionPath, typeof(Texture2D)).ToList();
        jumpMetallics = Resources.LoadAll(jumpMetallicPath, typeof(Texture2D)).ToList();
        jumpOcclusions = Resources.LoadAll(jumpOcclusionPath, typeof(Texture2D)).ToList();

        specialTextures = Resources.LoadAll(specialTexturePath, typeof(Texture2D)).ToList();
        specialNormals = Resources.LoadAll(specialNormalPath, typeof(Texture2D)).ToList();
        specialEmissions = Resources.LoadAll(specialEmissionPath, typeof(Texture2D)).ToList();
        specialMetallics = Resources.LoadAll(specialMetallicPath, typeof(Texture2D)).ToList();
        specialOcclusions = Resources.LoadAll(specialOcclusionPath, typeof(Texture2D)).ToList();

        */


        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;

        transform.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_NORMALMAP");
        transform.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_METALLICGLOSSMAP");

        //Counts the textures per direction
        amountOfIdleTexturesPerDir = idleTextures.Count / 8;
        amountOfMovementTexturesPerDir = movementTextures.Count / 8;
        amountOfHitTexturesPerDir = hitTextures.Count / 8;
        amountOfDeathTexturesPerDir = deathTextures.Count / 8;
        amountOfShootTexturesPerDir = shootTextures.Count / 8;
        amountOfStunTexturesPerDir = stunTextures.Count / 8;
        amountOfJumpTexturesPerDir = jumpTextures.Count / 8;
        amountOfSpecialTexturesPerDir = specialTextures.Count / 8;




        if (onlyOneTextureSet)
        {
            amountOfIdleTexturesPerDir = idleTextures.Count;
            amountOfMovementTexturesPerDir = movementTextures.Count;
            amountOfHitTexturesPerDir = hitTextures.Count;
            amountOfDeathTexturesPerDir = deathTextures.Count;
            amountOfShootTexturesPerDir = shootTextures.Count;
            amountOfStunTexturesPerDir = stunTextures.Count;
            amountOfJumpTexturesPerDir = jumpTextures.Count;
            amountOfSpecialTexturesPerDir = specialTextures.Count;
        }
        amountOfTexturesPerDir = amountOfIdleTexturesPerDir;
        allTextures = idleTextures;
        allNormals = idleNormals;
        allEmissions = idleEmissions;
        allMetallics = idleMetallics;
        allOcclusions = idleOcclusions;
        timeBetweenAnimFrames = idleTimeBetweenAnimFrames;
        /*
        material.mainTexture = (Texture2D)idleTextures[0];
        material.SetTexture("_BumpMap", (Texture2D)idleNormals[0]);
        material.SetTexture("_EmissionMap", (Texture2D)idleEmissions[0]);
        material.SetTexture("_MetallicGlossMap", (Texture2D)idleMetallics[0]);
        material.SetTexture("_OcclusionMap", (Texture2D)idleOcclusions[0]);
        */
        animDone = true;
        frame = 0;
        counter = 0;
        doOnce = false;
        pauseCounter = false;

    }
}
