using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Image_Changer : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public int z = 0;

    public void Image_Change()
    {
    spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.sprite = sprites[z];
    z += 1;
    if (z == 4)
            z = 0;
    }
 
}
