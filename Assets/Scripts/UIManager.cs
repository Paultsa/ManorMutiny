using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum HitDir { left, right, back, front };

[System.Serializable]
public class OptionsFile
{
    public float masterValue;
    public float sxValue;
    public float muValue;
    public float voValue;
    public float atValue;
    public float xSensitivity;
    public float ySensitivity;

    public int resoDropdownIndex;
    public bool fullscreen;

    public OptionsFile(float masterVal, float sxVal, float muVal, float voVal, float atVal, int resoIndex, bool fullscrn, float xSens, float ySens)
    {
        masterValue = masterVal;
        sxValue = sxVal;
        muValue = muVal;
        voValue = voVal;
        atValue = atVal;

        resoDropdownIndex = resoIndex;
        fullscreen = fullscrn;

        xSensitivity = xSens;
        ySensitivity = ySens;
    }
}
public class UIManager : MonoBehaviour
{


    public Sprite emptySilhouette;
    public Sprite revolverSilhouette;
    public Sprite chaingunSilhouette;
    public Sprite lgSilhouette;
    public Sprite glSilhouette;
    public Sprite shotgunSilhouette;
    public Sprite rlSilhouette;

    public Sprite emptyCrosshair;
    public Sprite revolverCrosshair;
    public Sprite chaingunCrosshair;
    public Sprite lgCrosshair;
    public Sprite glCrosshair;
    public Sprite shotgunCrosshair;
    public Sprite rlCrosshair;

    public Image crosshair;
    public Image primaryChosen;
    public Image secondaryChosen;

    public int maxHealth;
    public TMP_Text health;
    public TMP_Text primaryAmmo;
    public TMP_Text secondaryAmmo;
    public TMP_Text enemies;
    public TMP_Text lives;
    public GameObject life1;
    public GameObject life2;
    public GameObject life3;
    public TMP_Text respawnCounter;
    public TMP_Text currentRoundText;

    public TMP_Text startCounter;

    public Image primaryWeapon;
    public Image secondaryWeapon;

    public TMP_Text primaryWeaponText;
    public TMP_Text secondaryWeaponText;


    public float lerpTime;

    float hitBackTimer;
    float hitFrontTimer;
    float hitRightTimer;
    float hitLeftTimer;

    public Color hitColor;
    Color emptyColor;
    public Image hitBack;
    public Image hitFront;
    public Image hitRight;
    public Image hitLeft;

    public static bool rocketPack;
    public static float rocketjumpCooldown;
    public static float rocketjumpTimer;



    public static bool grapplingHook;
    public static int grapplingHookCharges;
    public static float grapplingHookCooldown;
    public static float grapplingHookMaxCooldown;
    public static float grapplingHookTimer;

    public GameObject rocketpackUI;
    public GameObject grapplinghookUI;

    public Sprite grapplingInRange;
    public Sprite grapplingOutOfRange;

    public static bool inGrapplingRange;

    public Slider firstCharge;
    public Slider secondCharge;
    public Slider thirdCharge;

    public Slider rocketpackCharge;

    public bool paused = false;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject audioMenu;
    public GameObject gameOverMenu;
    public GameObject controlsMenu;

    public VCAController vcaController = null;

    public Slider masterSlider;
    public Slider sxSlider;
    public Slider muSlider;
    public Slider voSlider;
    public Slider atSlider;

    Resolution[] resolutions;
    public Dropdown resolutionDropdown;

    public Toggle fullscreenToggle;

    float lastMasterVol;
    float lastSxVol;
    float lastMuVol;
    float lastVoVol;
    float lastAtVol;

    public MouseLook mouseLookX;
    public MouseLook mouseLookY;

    public Slider xSlider;
    public Slider ySlider;

    private void Awake()
    {
        rocketPack = false;
        grapplingHook = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        emptyColor = new Color(hitColor.r, hitColor.g, hitColor.b, 0);

        hitBack.color = emptyColor;
        hitFront.color = emptyColor;
        hitRight.color = emptyColor;
        hitLeft.color = emptyColor;



        LoadOptions();

    }



    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (GameManager.gameManager.respawnCounter > 0)
            {
                respawnCounter.transform.parent.gameObject.SetActive(true);
                respawnCounter.text = "Respawning in " + Mathf.CeilToInt(GameManager.gameManager.respawnCounter);
            }
            else
            {
                respawnCounter.transform.parent.gameObject.SetActive(false);
            }
            //lives.text = "Lives: " + GameManager.gameManager.lives;
            switch(GameManager.gameManager.lives)
            {
                case 3:
                    life1.SetActive(true);
                    life2.SetActive(true);
                    life3.SetActive(true);
                    break;
                case 2:
                    life1.SetActive(true);
                    life2.SetActive(true);
                    life3.SetActive(false);
                    break;
                case 1:
                    life1.SetActive(true);
                    life2.SetActive(false);
                    life3.SetActive(false);
                    break;
                case 0:
                    life1.SetActive(false);
                    life2.SetActive(false);
                    life3.SetActive(false);
                    break;

            }
            currentRoundText.text = "Current round: " + (GameManager.gameManager.roundsBeaten+1);

            if (lastMasterVol != masterSlider.value ||
            lastSxVol != sxSlider.value ||
            lastMuVol != muSlider.value ||
            lastVoVol != voSlider.value ||
            lastAtVol != atSlider.value)
            {
                SaveOptions();
            }

            if (vcaController != null)
            {
                vcaController.SetMainVolume(masterSlider.value);
                vcaController.SetSxVolume(sxSlider.value);
                vcaController.SetMuVolume(muSlider.value);
                vcaController.SetVoVolume(voSlider.value);
                vcaController.SetAtVolume(atSlider.value);
                //masterSlider.value = vcaController.GetMainVolume();
                //sxSlider.value = vcaController.GetSxVolume();
                //muSlider.value = vcaController.GetMuVolume();
                //voSlider.value = vcaController.GetVoVolume();
                //atSlider.value = vcaController.GetAtVolume();
            }
            lastMasterVol = masterSlider.value;
            lastSxVol = sxSlider.value;
            lastMuVol = muSlider.value;
            lastVoVol = voSlider.value;
            lastAtVol = atSlider.value;

            GameManager.gameManager.gamePaused = paused;
            GameManager.gameManager.startCounter = startCounter;
            GameManager.gameManager.gameOverMenu = gameOverMenu;
            if (grapplingHook)
            {
                grapplinghookUI.SetActive(true);
                rocketpackUI.SetActive(false);

                firstCharge.maxValue = grapplingHookMaxCooldown * (1f / 3f);
                secondCharge.maxValue = grapplingHookMaxCooldown * (2f / 3f);
                secondCharge.minValue = firstCharge.maxValue;
                thirdCharge.maxValue = grapplingHookMaxCooldown;
                thirdCharge.minValue = secondCharge.maxValue;

                firstCharge.value = grapplingHookTimer;
                secondCharge.value = grapplingHookTimer;
                thirdCharge.value = grapplingHookTimer;

                if(inGrapplingRange)
                {
                    firstCharge.targetGraphic.gameObject.GetComponent<Image>().sprite = grapplingInRange;
                    secondCharge.targetGraphic.gameObject.GetComponent<Image>().sprite = grapplingInRange;
                    thirdCharge.targetGraphic.gameObject.GetComponent<Image>().sprite = grapplingInRange;
                }
                else
                {
                    firstCharge.targetGraphic.gameObject.GetComponent<Image>().sprite = grapplingOutOfRange;
                    secondCharge.targetGraphic.gameObject.GetComponent<Image>().sprite = grapplingOutOfRange;
                    thirdCharge.targetGraphic.gameObject.GetComponent<Image>().sprite = grapplingOutOfRange;
                }
            }
            else if (rocketPack)
            {
                grapplinghookUI.SetActive(false);
                rocketpackUI.SetActive(true);

                rocketpackCharge.maxValue = rocketjumpCooldown;

                rocketpackCharge.value = rocketjumpTimer;
            }


            //primaryWeaponText.text = GameManager.gameManager.primaryWeapon.ToString();
            //secondaryWeaponText.text = GameManager.gameManager.secondaryWeapon.ToString();

            if (GameManager.gameManager.enemiesAlive != 0)
            {
                enemies.text = "Enemies alive: " + GameManager.gameManager.enemiesAlive;
                enemies.enabled = true;
            }
            else
            {
                enemies.enabled = false;
            }

            //Primary and secondary weapons
            if (GameManager.gameManager.playersFound)
            {
                if (GameManager.gameManager.localPlayer.CompareTag("PlayerOne"))
                {
                    ShowPrimaryWeapon(GameManager.gameManager.players[0].primaryWeapon);
                    ShowSecondaryWeapon(GameManager.gameManager.players[0].secondaryWeapon);
                    ShowCrosshair(GameManager.gameManager.players[0].chosenWeapon, 0);
                    AmmosHandler(0);
                    health.text = "HP: " + GameManager.gameManager.players[0].health.ToString();
                }
                else
                {
                    ShowPrimaryWeapon(GameManager.gameManager.players[1].primaryWeapon);
                    ShowSecondaryWeapon(GameManager.gameManager.players[1].secondaryWeapon);
                    ShowCrosshair(GameManager.gameManager.players[1].chosenWeapon, 1);
                    AmmosHandler(1);
                    health.text = "HP: " + GameManager.gameManager.players[1].health.ToString();
                }
            }



            //Directions for testing, delete later
            /*
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GotHit(HitDir.left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GotHit(HitDir.right);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GotHit(HitDir.front);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GotHit(HitDir.back);
            }
            */

            hitLeftTimer = Mathf.Clamp(hitLeftTimer -= Time.deltaTime, 0, 1);
            hitRightTimer = Mathf.Clamp(hitRightTimer -= Time.deltaTime, 0, 1);
            hitFrontTimer = Mathf.Clamp(hitFrontTimer -= Time.deltaTime, 0, 1);
            hitBackTimer = Mathf.Clamp(hitBackTimer -= Time.deltaTime, 0, 1);

            hitBack.color = Color.Lerp(emptyColor, hitColor, hitBackTimer);
            hitFront.color = Color.Lerp(emptyColor, hitColor, hitFrontTimer);
            hitLeft.color = Color.Lerp(emptyColor, hitColor, hitLeftTimer);
            hitRight.color = Color.Lerp(emptyColor, hitColor, hitRightTimer);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!paused)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }

        }
    }
        

    public void GotHit(HitDir hitDirection)
    {
        switch (hitDirection)
        {
            case HitDir.left:
                hitLeftTimer = lerpTime;
                hitLeft.color = hitColor;
                break;
            case HitDir.right:
                hitRightTimer = lerpTime;
                hitRight.color = hitColor;
                break;
            case HitDir.front:
                hitFrontTimer = lerpTime;
                hitFront.color = hitColor;
                break;
            case HitDir.back:
                hitBackTimer = lerpTime;
                hitBack.color = hitColor;
                break;

        }
    }

    public void ShowPrimaryWeapon(weapon primary)
    {
        switch (primary)
        {
            case weapon.empty:
            case weapon.meleeFist:
                primaryWeapon.sprite = emptySilhouette;
                break;
            case weapon.revolver:
                primaryWeapon.sprite = revolverSilhouette;
                break;
            case weapon.chaingun:
                primaryWeapon.sprite = chaingunSilhouette;
                break;
            case weapon.grenadeLauncher:
                primaryWeapon.sprite = glSilhouette;
                break;
            case weapon.lightningGun:
                primaryWeapon.sprite = lgSilhouette;
                break;
            case weapon.rocketLauncher:
                primaryWeapon.sprite = rlSilhouette;
                break;
            case weapon.shotgun:
                primaryWeapon.sprite = shotgunSilhouette;
                break;
            default:
                primaryWeapon.sprite = emptySilhouette;
                break;
        }
    }

    void ShowSecondaryWeapon(weapon secondary)
    {
        switch (secondary)
        {
            case weapon.empty:
            case weapon.meleeFist:
                secondaryWeapon.sprite = emptySilhouette;
                break;
            case weapon.revolver:
                secondaryWeapon.sprite = revolverSilhouette;
                break;
            case weapon.chaingun:
                secondaryWeapon.sprite = chaingunSilhouette;
                break;
            case weapon.grenadeLauncher:
                secondaryWeapon.sprite = glSilhouette;
                break;
            case weapon.lightningGun:
                secondaryWeapon.sprite = lgSilhouette;
                break;
            case weapon.rocketLauncher:
                secondaryWeapon.sprite = rlSilhouette;
                break;
            case weapon.shotgun:
                secondaryWeapon.sprite = shotgunSilhouette;
                break;
            default:
                secondaryWeapon.sprite = emptySilhouette;
                break;
        }
    }

    void ShowCrosshair(int chosen, int playerIndex)
    {
        weapon currentWeapon = weapon.empty;
        switch (chosen)
        {
            case 0:
                currentWeapon = GameManager.gameManager.players[playerIndex].primaryWeapon;
                break;
            case 1:
                currentWeapon = GameManager.gameManager.players[playerIndex].secondaryWeapon;
                break;
        }
        switch (currentWeapon)
        {
            case weapon.empty:
                crosshair.sprite = emptyCrosshair;
                break;
            case weapon.revolver:
                crosshair.sprite = revolverCrosshair;
                break;
            case weapon.chaingun:
                crosshair.sprite = chaingunCrosshair;
                break;
            case weapon.grenadeLauncher:
                crosshair.sprite = glCrosshair;
                break;
            case weapon.lightningGun:
                crosshair.sprite = lgCrosshair;
                break;
            case weapon.rocketLauncher:
                crosshair.sprite = rlCrosshair;
                break;
            case weapon.shotgun:
                crosshair.sprite = shotgunCrosshair;
                break;
            default:
                crosshair.sprite = emptyCrosshair;
                break;
        }
    }



    //Pause Menu functions

    public void PauseGame()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        audioMenu.SetActive(false);
        controlsMenu.SetActive(false);
        paused = false;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        Destroy(GameManager.gameManager);
        Destroy(GameObject.Find("Music"));
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        GameManager.gameManager.QuitGame();
    }

    public void ChangeResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveOptions();
    }

    public void SetFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        SaveOptions();
    }

    public void AmmosHandler(int playerIndex)
    {
        switch (GameManager.gameManager.players[playerIndex].chosenWeapon)
        {
            case 0:
                primaryChosen.enabled = true;
                secondaryChosen.enabled = false;
                break;
            case 1:
                primaryChosen.enabled = false;
                secondaryChosen.enabled = true;
                break;
        }
        if (GameManager.gameManager.players[playerIndex].ammos[0] == -1 || GameManager.gameManager.players[playerIndex].primaryWeapon == weapon.meleeFist)
        {
            primaryAmmo.text = "";
        }
        else
        {
            primaryAmmo.text = /*"Ammo: " +*/ GameManager.gameManager.players[playerIndex].ammos[0].ToString();
        }
        if (GameManager.gameManager.players[playerIndex].ammos[1] == -1 || GameManager.gameManager.players[playerIndex].secondaryWeapon == weapon.meleeFist)
        {
            secondaryAmmo.text = "";
        }
        else
        {
            secondaryAmmo.text = /*"Ammo: " +*/ GameManager.gameManager.players[playerIndex].ammos[1].ToString();
        }
    }

    public void SaveOptions()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/options3.opts";
        FileStream stream = new FileStream(path, FileMode.Create);

        OptionsFile opts = new OptionsFile(masterSlider.value, sxSlider.value, muSlider.value, voSlider.value, atSlider.value, resolutionDropdown.value, fullscreenToggle.isOn, mouseLookX.sensitivityX, mouseLookY.sensitivityY);

        formatter.Serialize(stream, opts);
        stream.Close();


    }
    public void LoadOptions()
    {
        resolutions = Screen.resolutions;
        List<string> resolutionList = new List<string>();
        int currentResolutionIndex = 0;
        foreach (Resolution r in resolutions)
        {
            resolutionList.Add(r.width + " x " + r.height);

            if (r.width == Screen.currentResolution.width &&
                r.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = resolutionList.Count - 1;
            }
        }

        string path = Application.persistentDataPath + "/options3.opts";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            OptionsFile opts = formatter.Deserialize(stream) as OptionsFile;
            stream.Close();

            resolutionDropdown.ClearOptions();
            fullscreenToggle.isOn = opts.fullscreen;
            Screen.fullScreen = opts.fullscreen;
            currentResolutionIndex = opts.resoDropdownIndex;

            masterSlider.value = opts.masterValue;
            sxSlider.value = opts.sxValue;
            muSlider.value = opts.muValue;
            voSlider.value = opts.voValue;
            atSlider.value = opts.atValue;

            mouseLookX.sensitivityX = opts.xSensitivity;
            mouseLookY.sensitivityY = opts.ySensitivity;
            xSlider.value = opts.xSensitivity;
            ySlider.value = opts.ySensitivity;

            if (vcaController != null)
            {
                vcaController.SetMainVolume(opts.masterValue);
                vcaController.SetSxVolume(opts.sxValue);
                vcaController.SetMuVolume(opts.muValue);
                vcaController.SetVoVolume(opts.voValue);
                vcaController.SetAtVolume(opts.atValue);
                
            }
        }
        else
        {
            Debug.LogError("Options file not found");
        }



        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        ChangeResolution();
    }

    public void SetHorizontalSensitivity(float sensitivity)
    {
        mouseLookX.sensitivityX = sensitivity;
    }
    public void SetVerticalSensitivity(float sensitivity)
    {
        mouseLookY.sensitivityY = sensitivity;
    }
}
