using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Goals : MonoBehaviour
{
    public int level;

    private int x;
    private int y;
    private string filePath;
    private int vaseNum;
    private int boxNum;
    private int stoneNum;

    public TextMeshPro vaseText;
    public TextMeshPro boxText;
    public TextMeshPro stoneText;
    
    
    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("Level");
        filePath = $"Assets/Levels/level_0{level}.json";
        string jsonContent = File.ReadAllText(filePath);
        Grid.LevelData levelData = JsonUtility.FromJson<Grid.LevelData>(jsonContent);
        
        x = levelData.grid_width;
        y = levelData.grid_height;
        for (var i = 0; i < x * y; i++)
        {
            if (levelData.grid[i] == "s") stoneNum++;
            if (levelData.grid[i] == "v") vaseNum++;
            if (levelData.grid[i] == "bo") boxNum++;
        }

        vaseText.text = vaseNum.ToString();
        stoneText.text = stoneNum.ToString();
        boxText.text = boxNum.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
