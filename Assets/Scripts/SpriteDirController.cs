using UnityEngine;


//Pauli

public class SpriteDirController : MonoBehaviour
{
    public int state;
    enum Directions { SOUTH, SOUTHEAST, EAST, NORTHEAST, NORTH, NORTHWEST, WEST, SOUTHWEST};
    public bool flying;
    public Camera camToFollow;
    GameObject quad;
    public GameObject realLookDir;
    Material material;
    public int spriteDir;

    public bool clampRotation;
    public float rotateUp;
    public float rotateDown;

    // Enemy materials from different perspective clockwise
    public Texture2D[] faces;

    public bool lookAtCamera = true;

    // Start is called before the first frame update
    void Start()
    {

        camToFollow = Camera.main;
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        quad = transform.GetChild(0).gameObject;
        realLookDir = transform.GetChild(1).gameObject;
        realLookDir.transform.LookAt(new Vector3(camToFollow.transform.position.x, realLookDir.transform.position.y, camToFollow.transform.position.z));
        //transform.LookAt(new Vector3(camToFollow.transform.position.x,transform.position.y,camToFollow.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        TurnSprites(faces.Length);

        if(!flying)
        {
            if(clampRotation)
            {
                quad.transform.eulerAngles = new Vector3(Mathf.Clamp(camToFollow.transform.eulerAngles.x,rotateDown,rotateUp), camToFollow.transform.eulerAngles.y, 0);
            }
            else
            {
                quad.transform.eulerAngles = new Vector3(0, camToFollow.transform.eulerAngles.y, 0);
            }
            
        }
        else
        {
            quad.transform.eulerAngles = new Vector3(camToFollow.transform.eulerAngles.x, camToFollow.transform.eulerAngles.y, 0);
        }
        if(lookAtCamera)
            realLookDir.transform.LookAt(camToFollow.transform.position);
    }
    void TurnSprites(int spriteAmount)
    {
        switch(spriteAmount)
        {
            case 8:
                EightSprites();
                break;

            case 4:
                FourSprites();
                break;
        }
        
    }


    void EightSprites()
    {
        if (realLookDir.transform.localEulerAngles.y < 22.5f || realLookDir.transform.localEulerAngles.y > 337.5f)
        {
            spriteDir = (int)Directions.SOUTH;
            //material.mainTexture = faces[0];
            
        }
        else if (realLookDir.transform.localEulerAngles.y < 67.5f)
        {
            spriteDir = (int)Directions.SOUTHEAST;
            //material.mainTexture = faces[1];
        }
        else if (realLookDir.transform.localEulerAngles.y < 112.5f)
        {
            spriteDir = (int)Directions.EAST;
            //material.mainTexture = faces[2];
        }
        else if (realLookDir.transform.localEulerAngles.y < 157.5f)
        {
            spriteDir = (int)Directions.NORTHEAST;
            //material.mainTexture = faces[3];
        }
        else if (realLookDir.transform.localEulerAngles.y < 202.5f)
        {
            spriteDir = (int)Directions.NORTH;
            //material.mainTexture = faces[4];
        }
        else if (realLookDir.transform.localEulerAngles.y < 247.5)
        {
            spriteDir = (int)Directions.NORTHWEST;
            //material.mainTexture = faces[5];
        }
        else if (realLookDir.transform.localEulerAngles.y < 292.5)
        {
            spriteDir = (int)Directions.WEST;
            //material.mainTexture = faces[6];
        }
        else
        {
            spriteDir = (int)Directions.SOUTHWEST;
            //material.mainTexture = faces[7];
        }
    }

    void FourSprites()
    {
        Debug.Log(realLookDir.transform.localEulerAngles.y);
        if (realLookDir.transform.localEulerAngles.y < 45 || realLookDir.transform.localEulerAngles.y > 315)
        {
            material.mainTexture = faces[0];
        }
        else if (realLookDir.transform.localEulerAngles.y < 135)
        {
            material.mainTexture = faces[1];
        }
        else if (realLookDir.transform.localEulerAngles.y < 225)
        {
            material.mainTexture = faces[2];
        }
        else
        {
            material.mainTexture = faces[3];
        }
    }
}
