using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ColorPiece : MonoBehaviour
{
    
    public enum ColorType
    {
        YELLOW,
        BLUE,
        RED,
        GREEN,
        GREENB,
        YELLOWB,
        BLUEB,
        REDB
    };
    [Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    private ColorType color;

    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    public int NumColors
    {
        get { return ColorSprites.Length; }
    }

    private SpriteRenderer sprite;
    
    private Dictionary<ColorType, Sprite> colorSpriteDict;
    
    public ColorSprite[] ColorSprites;
    // Start is called before the first frame update
    void Awake()
    {
        sprite = Transform.FindObjectOfType<GamePiece>().GetComponent<SpriteRenderer>();
            //Resources.Load<GameObject>("Sprite/Temp").GetComponent<SpriteRenderer>();
        
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < ColorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(ColorSprites[i].color))
            {
                colorSpriteDict.Add(ColorSprites[i].color, ColorSprites[i].sprite);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
        {
            
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}
