using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jimmy
//Kostin koodimuistiinpanot: kuolema

public class PlayerHealth : MonoBehaviour, IHealth
{
    [Header("Health settings")]
    [Tooltip("The amount of health the character has")]
    public int MaxHealth = 100;
    [Tooltip("The amount of health the character actually has behind the scenes. This needs to be the same max as the max in the health curve")]
    public int realMaxHealth = 150;
    [Tooltip("how wide is the angle getting hit in this area is")]
    public int frontHitAngle = 90;
    [Tooltip("how wide is the angle getting hit in this area is")]
    public int leftHitAngle = 90;
    [Tooltip("how wide is the angle getting hit in this area is")]
    public int backHitAngle = 90;
    [Tooltip("how wide is the angle getting hit in this area is")]
    public int rightHitAngle = 90;
    public UIManager uiManager;
    public AnimationCurve healthCurve = AnimationCurve.Linear(0, 0, 150, 100);
    public AnimationCurve inverseHealthCurve;
    public GunController gunCon;

    int minFrontAngle;
    int maxFrontAngle;

    int minLeftAngle;
    int maxLeftAngle;

    int minBackAngle;
    int maxBackAngle;

    int minRightAngle;
    int maxRightAngle;

    bool hasUi;

    int currentHealth;
    int realCurrentHealth;

    //[HideInInspector]
    public bool alive = true;
    public bool diedOnce = false;
    public bool respawnTimerActive = false;

    void Start()
    {
        if (uiManager)
        {
            hasUi = true;
        }
        else
        {
            hasUi = false;
        }
        realCurrentHealth = realMaxHealth;
        currentHealth = (int)healthCurve.Evaluate(realCurrentHealth);

        createInverseCurve();

        minFrontAngle = -frontHitAngle / 2;
        maxFrontAngle = frontHitAngle / 2;

        maxLeftAngle = -90 + (leftHitAngle / 2);
        minLeftAngle = -90 - (leftHitAngle / 2);

        minRightAngle = 90 - (rightHitAngle / 2);
        maxRightAngle = 90 + (rightHitAngle / 2);

        minBackAngle = -180 + (backHitAngle / 2);
        maxBackAngle = 180 - (backHitAngle / 2);



        //Debug.Log("ANGLES " + maxFrontAngle + " " + minFrontAngle + " " + maxLeftAngle + " " + minLeftAngle + " " + minBackAngle + " " + maxBackAngle + " " + maxRightAngle + " " + minRightAngle);
    }
    void Update()
    {
        if (GameManager.gameManager != null)
        {
            if(GameManager.gameManager.playersFound)
            {
                if (transform.CompareTag("PlayerOne"))
                {
                    GameManager.gameManager.players[0].health = currentHealth;
                }
                else
                {
                    GameManager.gameManager.players[1].health = currentHealth;
                }
            }
        }
        if (!alive && !diedOnce)
        {
            diedOnce = true;
            Die();
        }

    }
    void createInverseCurve()
    {

        //create inverse health curve with same amount of keys. Low detail
        /*inverseHealthCurve = new AnimationCurve();
        for (int i = 0; i < healthCurve.length; i++)
        {
            Keyframe inverseKey = new Keyframe(healthCurve.keys[i].value, healthCurve.keys[i].time);
            inverseHealthCurve.AddKey(inverseKey);
        }*/

        //create inverse health curve. High detail
        inverseHealthCurve = new AnimationCurve();

        float totalTime = healthCurve.keys[healthCurve.length - 1].time;
        float sampleX = 0; //The "sample-point"
        float deltaX = 0.01f; //The "sample-delta"
        float lastY = healthCurve.Evaluate(sampleX);
        while (sampleX < totalTime)
        {
            float y = healthCurve.Evaluate(sampleX); //The "value"
            float deltaY = y - lastY; //The "value-delta"
            float tangent = deltaX / deltaY;
            Keyframe invertedKey = new Keyframe(y, sampleX, tangent, tangent);
            inverseHealthCurve.AddKey(invertedKey);

            sampleX += deltaX;
            lastY = y;
        }
        for (int i = 0; i < inverseHealthCurve.length; i++)
        {
            inverseHealthCurve.SmoothTangents(i, 0.1f);
        }
        //Debug.Log("REAL " + realCurrentHealth + " OTHER " + currentHealth);
        //int test = 35;
        //Debug.Log(healthCurve.Evaluate(test) + " " + inverseHealthCurve.Evaluate(healthCurve.Evaluate(test)));
    }


    /*float timer = 0.5f;
    bool suunta = true;
    void Update()
    {
        
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0.5f;
            if (suunta)
            {
                TakeDamage(5);
                if (currentHealth == 0)
                    suunta = false;
            }
            else
            {
                HealDamage(5);
                if (currentHealth == MaxHealth)
                    suunta = true;
            }
            Debug.Log("REAL " + realCurrentHealth + " OTHER " + currentHealth);
        }

    }*/

    public bool HealDamage(int heal)
    {
        if (currentHealth < MaxHealth)
        {
            currentHealth += heal;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            realCurrentHealth = (int)inverseHealthCurve.Evaluate(currentHealth);
            return true;
        }
        else
            return false;
    }

    public void TakeDamage(int damage)
    {
        if (realCurrentHealth > 0 && realCurrentHealth > damage)
        {
            realCurrentHealth -= damage;
            currentHealth = (int)healthCurve.Evaluate(realCurrentHealth);
            //Debug.Log("REAL " + realCurrentHealth + " FOR HUD " + currentHealth);
            //currentHealth = realCurrentHealth;

            

            if (GameManager.gameManager != null)
            {
                if (transform.CompareTag("PlayerOne"))
                {
                    GameManager.gameManager.players[0].health = currentHealth;
                }
                else
                {
                    GameManager.gameManager.players[1].health = currentHealth;
                }
                
            }
        }
        else
        {
           
            Die();
        }
    }

    public void TakeDamage(int damage, Vector3 damageDealer)
    {
        if (realCurrentHealth > 0 && realCurrentHealth > damage)
        {

            //KostinKoodi
            FMODUnity.RuntimeManager.PlayOneShot("event:/VO/Player/Small/Player_takeDamage");
            FMODUnity.RuntimeManager.PlayOneShot("event:/SX/Monsters/Melee01/Sx_meleeMonster_attack_hit");

            realCurrentHealth -= damage;
            currentHealth = (int)healthCurve.Evaluate(realCurrentHealth);
            CalculateDamageDirection(damageDealer);
            Debug.Log("REAL " + realCurrentHealth + " FOR HUD " + currentHealth);
            //currentHealth = realCurrentHealth;
            if (GameManager.gameManager != null)
            {
                if (transform.CompareTag("PlayerOne"))
                {
                    GameManager.gameManager.players[0].health = currentHealth;
                }
                else
                {
                    GameManager.gameManager.players[1].health = currentHealth;
                }

            }
        }
        else
        {
            Die();
            //KostinKoodi
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SX/Monsters/Melee01/Sx_meleeMonster_takeDamage", this.gameObject);
        }

    }

    void CalculateDamageDirection(Vector3 damageDealer)
    {
        Vector3 damageDealerLevelled = new Vector3(damageDealer.x, transform.position.y, damageDealer.z);
        float damageAngle = Vector3.SignedAngle(transform.forward, damageDealerLevelled - transform.position, transform.up);

        if (hasUi)
        {
            if (damageAngle <= maxFrontAngle && damageAngle >= minFrontAngle)        //Front
            {
                //Debug.Log("FRONT");
                uiManager.GotHit(HitDir.front);
            }
            if (damageAngle <= maxLeftAngle && damageAngle >= minLeftAngle)     //Left
            {
                //Debug.Log("LEFT");
                uiManager.GotHit(HitDir.left);
            }
            if (damageAngle <= maxRightAngle && damageAngle >= minRightAngle)       //Right
            {
                //Debug.Log("RIGHT");
                uiManager.GotHit(HitDir.right);
            }
            if (damageAngle <= minBackAngle || damageAngle >= maxBackAngle)     //Back
            {
                //Debug.Log("BACK");
                uiManager.GotHit(HitDir.back);
            }
        }
        //Debug.Log("DAMAGE " + damageAngle);
    }

    void Die()
    {
        Debug.LogError("DIE");
        realCurrentHealth = 0;
        currentHealth = (int)healthCurve.Evaluate(realCurrentHealth);
        
        if (alive && GameManager.gameManager)
        {
            alive = false;

            Debug.LogError("You died");
            GameManager.gameManager.deaths++;
            GameManager.gameManager.lives--;
            if (GameManager.gameManager.lives <= 0)
            {
             

                GameManager.gameManager.GameOver();
                return;
            }

            GameObject camera = transform.GetChild(0).gameObject;
            if (GameManager.gameManager.playerCount == 2)
            {

                if (transform.CompareTag("PlayerOne"))
                {
                    GameManager.gameManager.GameOver();
                    return;
                }
                alive = false;
                //Player is ded
                if (GameManager.gameManager.playerCount == 2)
                {

                    if (transform.CompareTag("PlayerOne"))
                    {
                        camera.transform.SetParent(GameObject.FindWithTag("PlayerTwo").transform);
                    }
                    else
                    {
                        camera.transform.SetParent(GameObject.FindWithTag("PlayerOne").transform);
                    }
                    camera.transform.SetAsFirstSibling();
                    camera.transform.localPosition = camera.transform.parent.GetChild(0).localPosition;
                    camera.transform.localEulerAngles = camera.transform.parent.GetChild(0).localEulerAngles;
                    camera.transform.rotation = camera.transform.parent.GetChild(0).rotation;
                    camera.transform.GetChild(0).gameObject.SetActive(false);
                    transform.position = new Vector3(1000, 1000, 1000);
                    camera.GetComponent<GrapplingHook>().enabled = false;
                    camera.GetComponent<RocketPack>().enabled = false;


                }
            }
            else
            {
                GameManager.gameManager.respawnCounter = GameManager.gameManager.respawnTime;
                camera.transform.GetChild(0).gameObject.SetActive(false);
                camera.GetComponent<GrapplingHook>().enabled = false;
                camera.GetComponent<RocketPack>().enabled = false;
            }

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            if (!respawnTimerActive)
            {
                Debug.LogError("RESPAWNING");
                StartCoroutine(RespawnTimer(camera));
            }
            if (GameManager.gameManager.playerCount == 2)
            {
                EnemyAi[] enemies = FindObjectsOfType<EnemyAi>() as EnemyAi[];
                foreach (EnemyAi enemy in enemies)
                {
                    enemy.ReAggro();
                }
            }


            //Reset Guns
            gunCon.weapons[0] = weapon.empty;
            gunCon.ammos[0] = -1;
            gunCon.weapons[1] = weapon.empty;
            gunCon.ammos[1] = -1;
            gunCon.current_weapon = gunCon.inventory_size;
            gunCon.gunPaths.currentWeapon = weapon.meleeFist;
        }

        
        //Player is ded

        //FMODUnity.RuntimeManager.GetBus("bus:/=sum").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        
    }
    public IEnumerator RespawnTimer(GameObject camera)
    {
        respawnTimerActive = true;
        while (GameManager.gameManager.respawnCounter > 0)
        {
            yield return new WaitForSeconds(1);
            GameManager.gameManager.respawnCounter--;
        }
        camera.transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GameManager.gameManager.RespawnPlayers();
        respawnTimerActive = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
