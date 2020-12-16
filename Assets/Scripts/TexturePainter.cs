using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter : MonoBehaviour
{
    public GameObject paintedDecalsContainer;
    public Material baseGeometryMaterial;
    public RenderTexture rTexture;

    void PaintDecalsToTexture()
    {
        RenderTexture.active = rTexture;
        Texture2D texture = new Texture2D(rTexture.width, rTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, rTexture.width, rTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        baseGeometryMaterial.mainTexture = texture; // Set the painted texture as the base texture for the TextureCanvas
        foreach (Transform child in paintedDecalsContainer.transform)
        {
            //Destroy decal prefabs
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If container reaches more than 10 decals paint them to base texture and delete decal prefabs
        if(paintedDecalsContainer.transform.childCount > 0)
        {
            Debug.Log("Paint decals to texture");
            PaintDecalsToTexture();
        }
    }
}
