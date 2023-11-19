using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BOX,
        STONE,
        VASE,
        VASE2,
        TNT
    }
    [Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }

    public Congrat congrat;
    public GameObject congratPanel;
    public Animator crossFade;
    public GameOver gameOver;
    public GameObject stoneTick;
    public GameObject vaseTick;
    public GameObject boxTick;
    public GameObject gridBackground;
    public GameObject boxImage;
    public GameObject vaseImage;
    public GameObject stoneImage;
    public TextMeshPro vaseText;
    public TextMeshPro boxText;
    public TextMeshPro stoneText;
    public TextMeshPro moveText;
    private bool hasVase = false;
    private bool hasStone = false;
    private bool hasBox = false;
    public int moveCount;
    public static string _filePath;
    private bool _isFinished = false;
    
    
    public const float TweenDuration = 0.25f;

    public float fillTime;
    public int xDim;
    private Dictionary<PieceType, GameObject> piecePrefabDict;
    public int yDim;
    private int vaseNum;
    private int boxNum;
    private int stoneNum;

    public PiecePrefab[] piecePrefabs;

   
    private GamePiece pressedPiece;

    public GamePiece[,] pieces;
    
    

    void Start()
    {

        var level = PlayerPrefs.GetInt("Level", 1);
            
        _filePath = $"Assets/Levels/level_0{level}.json";
        if (level == 10)
        {
            _filePath = $"Assets/Levels/level_{level}.json";
        }
        
        string jsonContent = File.ReadAllText(_filePath);
        
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonContent);
        
        xDim = levelData.grid_width; 
        yDim = levelData.grid_height;
        moveCount = levelData.move_count;
        string[] Grids = levelData.grid;
        
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (var i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        
        pieces = new GamePiece[xDim, yDim];
        
        for (var y = 0; y < yDim; y++)
        {
            for (var x = 0; x < xDim; x++)
            {
                GamePiece tiled;
                switch (Grids.GetValue((yDim - y - 1) * xDim + x))
                { 
                        
                    case "g":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.GREEN);
                        break;
                    case "r":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.RED);
                        break;
                    case "y":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.YELLOW);
                        break;
                    case "b":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.BLUE);
                        break;
                    case "rand":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, 4));
                        break;
                    case "bo":
                        SpawnNewPieces(x, y, PieceType.BOX);
                        boxNum++;
                        hasBox = true;
                        break;
                    case "s":
                        SpawnNewPieces(x, y, PieceType.STONE);
                        stoneNum++;
                        hasStone = true;
                        
                        break;
                    case "v":
                        SpawnNewPieces(x, y, PieceType.VASE);
                        vaseNum++;
                        hasVase = true;
                        
                        break;
                    case "t":
                        SpawnNewPieces(x, y, PieceType.TNT);
                        break;
                    
                }
                
            }
        }
        UpdateGridBackground();
        UpdateGoalImages();
        CheckTnt();
        UpdateGoals();
        congratPanel.gameObject.SetActive(false);
        
    }

    private void UpdateGridBackground()
    {
        
        var xScale = (xDim / 6.000f) * 0.575f;
        var yScale = (yDim / 6.000f) * 0.525f;
        
        gridBackground.transform.localScale = new Vector3(xScale, yScale, 0.86f);
    }
    private void UpdateGoalImages()
    {
        boxTick.SetActive(false);
        vaseTick.SetActive(false);
        stoneTick.SetActive(false);
        if(!hasStone) stoneImage.SetActive(false);
        if(!hasVase) vaseImage.SetActive(false);
        if(!hasBox) boxImage.SetActive(false);
    }
    private void UpdateGoals()
    {
        
        vaseNum = 0;
        stoneNum = 0;
        boxNum = 0;
        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {
                if (pieces[i, j].Type == PieceType.VASE || pieces[i, j].Type == PieceType.VASE2) vaseNum++;
                if (pieces[i, j].Type == PieceType.BOX) boxNum++;
                if (pieces[i, j].Type == PieceType.STONE) stoneNum++;
                
            }
        }

        if (vaseNum == 0 && hasVase)
        {
            vaseText.gameObject.SetActive(false);
            vaseTick.SetActive(true);
        }

        if (boxNum == 0 && hasBox)
        {
            boxText.gameObject.SetActive(false);
            boxTick.SetActive(true);
        }

        if (stoneNum == 0 && hasStone)
        {
            stoneText.gameObject.SetActive(false);
            stoneTick.SetActive(true);
        }
        vaseText.text = vaseNum.ToString();
        stoneText.text = stoneNum.ToString();
        boxText.text = boxNum.ToString();
        moveText.text = moveCount.ToString();
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
    }

    private IEnumerator Fill()
    {
        while (FillStep())
        {
            
            yield return new WaitForSeconds(fillTime);
        }
    }

    private bool FillStep()
    {
        
        bool movedPiece = false;
        for (var y = yDim - 2; y >= 0; y--)
        {
            for (var xloop = 0; xloop < xDim; xloop++)
            {
                var x = xloop;
                
                GamePiece piece = pieces[x, y];
                if (!piece.IsMovable()) continue;
                GamePiece pieceBelow = pieces[x, y + 1];
                if (pieceBelow.Type == PieceType.EMPTY)
                {
                    Destroy(pieceBelow.gameObject);
                    piece.MovableComponent.Move(x, y + 1, fillTime);
                    pieces[x, y + 1] = piece;
                    SpawnNewPieces(x, y, PieceType.EMPTY);
                    movedPiece = true;
                }
                else if (pieceBelow.Type == PieceType.BOX || pieceBelow.Type == PieceType.STONE)
                {
                        
                    for (var t = yDim - 1; t > y + 1; t--)
                    {
                        GamePiece piecen = pieces[x, t];
                        if (piecen.Type == PieceType.EMPTY)
                        {
                            Destroy(piecen.gameObject);
                            piece.MovableComponent.Move(x, t, fillTime);
                            pieces[x, t] = piece;
                            SpawnNewPieces(x, y, PieceType.EMPTY);
                                
                            //movedPiece = true;
                            break;
                        }
                    }
                }
            }
        }

        for (var x = 0; x < xDim; x++)
        {
            
            GamePiece pieceBelow = pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL],
                    GetWorldPosition(x, -1), quaternion.identity);
                newPiece.transform.parent = transform;
                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, 4));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    private GamePiece SpawnNewPieces(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y),
            quaternion.identity);
        newPiece.transform.parent = transform;
        
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);
        
        

        return pieces[x, y];
    }

    

    private List<GamePiece> GetMatch(GamePiece piece)
    {
        List<GamePiece> matching = piece.getConnectedTiles(null);
        if (matching.Where(a => a.Type == PieceType.NORMAL).Count() >= 2)
        {
            if (matching.Where(a => a.Type == PieceType.NORMAL).Count() >= 5)
            {
                piece.transform.DOScale(Vector3.zero, TweenDuration);
                Destroy(piece.gameObject);
                SpawnNewPieces(piece.X, piece.Y, PieceType.TNT);
                return matching.Except(new GamePiece[]{piece}).ToList();
            }
            return matching;
        }

        return null;
    }

    private List<GamePiece> GetTntParticles(GamePiece tile, List<GamePiece> exclude = null, bool combo = false)
    {
        var result = new List<GamePiece> { tile, };
        

        if(exclude == null) exclude = new List<GamePiece>{ tile, };
        else exclude.Add(tile);
        
        var tntScale = combo ? 3 : 2;
        
        for (var y = tile.Y - tntScale; y <= tile.Y + tntScale; y++)
        {
            for (var x = tile.X - tntScale; x <= tile.X + tntScale; x++)
            {
                if (x < xDim && x >= 0 && y < yDim && y >= 0)
                {
                    var connTile = pieces[x, y];
                    if (!exclude.Contains(connTile))
                    {
                        if (connTile.Type == PieceType.TNT) result.AddRange(GetTntParticles(connTile, exclude));
                        else result.Add(connTile);
                        
                    }
                    
                }
            }
        }
        
        return result;
    }
    public IEnumerator LoadScene(string sceneName)
    {
        crossFade.SetTrigger("Start");
        if (_isFinished)
        {
            
            //congratPanel.SetActive(true);
            //SceneManager.LoadScene(sceneName);
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(sceneName);
        }
        
    }

    private void CheckTnt()
    {
        for (var y = 0; y < yDim; y++)
        {
            for (var x = 0; x < xDim; x++)
            {
                var piece = pieces[x, y];
                if (piece.IsColored() && piece.getConnectedTiles().Where(a => a.IsColored()).Count() >= 5)
                {
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.BLUE) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.BLUEB);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.RED) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.REDB);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.GREEN) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.GREENB);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.YELLOW) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.YELLOWB);
                }

                if (piece.IsColored() && piece.getConnectedTiles().Where(a => a.IsColored()).Count() < 5)
                {
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.BLUEB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.BLUE);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.REDB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.RED);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.GREENB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.GREEN);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.YELLOWB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.YELLOW);
                }
            }
        }
    }
    private bool IsTntCombo(GamePiece piece)
    {
        
        if ( piece.Neighbours.Length != 0 
             && piece.Neighbours.Any(a => a != null && a.Type == PieceType.TNT)) return true;
        return false;
    }
    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
        var cleaningTiles = GetMatch(piece);
        if (pressedPiece.Type == PieceType.TNT)
        {
            cleaningTiles = IsTntCombo(piece) ? GetTntParticles(piece, null, true) 
                : GetTntParticles(piece, null);
        }

        if (cleaningTiles == null) return;
        moveCount -= 1;
        foreach (var tile in cleaningTiles)
        {
            ClearPiece(tile.X, tile.Y);
                
        }
        UpdateGoals();
        StartCoroutine(Fill());
            
        if (IsGameFinished())
        {
            DOTween.Clear();
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
            _isFinished = true;
            //StartCoroutine(WaitforMenu());
            //SceneManager.LoadSceneAsync("MainScreen");
            congratPanel.SetActive(true);
            congrat.gameObject.SetActive(true);
            congrat.ShowCongratScreen();
            //StartCoroutine(LoadScene("MainScreen"));
        }
        else if (moveCount == 0 && !IsGameFinished())
        {
            gameOver.ShowLoseScreen();
                
        }

    }

    public void ReplayLevel()
    {
        var currLevel = PlayerPrefs.GetInt("Level");
        SceneManager.LoadSceneAsync($"SampleScene{currLevel}");
    }
    IEnumerator WaitforMenu()
    {
        
        yield return new WaitForSeconds(3);
        //panel.SetActive(true);
    }

    private bool ClearPiece(int x, int y)
    {
        if (pieces[x, y].IsClearable())
        {   
            //Debug.Log("sadfvadfv");
            pieces[x, y].ClearablePiece.Clear();
            if (pieces[x, y].Type == PieceType.VASE)
            {
                SpawnNewPieces(x, y, PieceType.VASE2);
            }
            else SpawnNewPieces(x, y, PieceType.EMPTY);
            return true;
        }

        return false;
    }

    private bool IsGameFinished()
    {
        for (var y = 0; y < yDim; y++)
        {
            for (var x = 0; x < xDim; x++)
            {
                if (pieces[x, y].Type == PieceType.BOX || pieces[x, y].Type == PieceType.VASE
                    || pieces[x, y].Type == PieceType.VASE2 || pieces[x, y].Type == PieceType.STONE ) return false;
            }
        }
        return true;
    }

    

   
    void Update()
    {
        CheckTnt();
    }
    public class LevelData
    {
        public int grid_width;
        public int grid_height;
        public int move_count;
        public string[] grid;
    }
}
