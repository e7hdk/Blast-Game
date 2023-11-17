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
        TNT,
        COUNT
    };
    [Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
    public TextMeshPro vaseText;
    public TextMeshPro boxText;
    public TextMeshPro stoneText;
    public TextMeshPro movetext;
    public int Width;
    public int Height;
    public int moveCount;
    static string filePath;
    //public GameObject panel;
    
    private bool inverse = false;
    public const float TweenDuration = 0.25f;

    public float fillTime;
    public int xDim;
    private Dictionary<PieceType, GameObject> piecePrefabDict;
    public int yDim;
    private int vaseNum;
    private int boxNum;
    private int stoneNum;

    public PiecePrefab[] piecePrefabs;

    public GameObject backgroundPrefab;
    private GamePiece pressedPiece;

    public GamePiece[,] pieces;
    // Start is called before the first frame update
    

    void Start()
    {
        //panel.SetActive(false);
        var level = 5;
            //PlayerPrefs.GetInt("Level", 1);
            
        filePath = $"Assets/Levels/level_0{level}.json";
        if (level == 10)
        {
            filePath = $"Assets/Levels/level_{level}.json";
        }
        
        string jsonContent = File.ReadAllText(filePath);
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

        for (var x = 0; x < xDim; x++)
        {
            for (var y = 0; y < yDim; y++)
            {
                //GameObject background = (GameObject)Instantiate(backgroundPrefab, new Vector3(x, y, 0), quaternion.identity);
                //background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, yDim];
        /*for (var x = 0; x < xDim; x++)
        {
            for (var y = 0; y < yDim; y++)
            {
                SpawnNewPieces(x, y, PieceType.EMPTY);
            }
        }*/
        
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
                        //tiled.transform.parent = transform;
                        break;
                    case "r":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.RED);
                        //tiled.transform.parent = transform;
                        break;
                    case "y":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.YELLOW);
                        
                        //tiled.transform.parent = transform;
                        break;
                    case "b":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.BLUE);
                        //tiled.transform.parent = transform;
                        break;
                    case "rand":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, 4));
                        //tiled.transform.parent = transform;
                        break;
                    case "bo":
                        tiled = SpawnNewPieces(x, y, PieceType.BOX);
                        boxNum++;
                        //tiled.transform.parent = transform;
                        break;
                    case "s":
                        tiled = SpawnNewPieces(x, y, PieceType.STONE);
                        stoneNum++;
                        //tiled.transform.parent = transform;
                        break;
                    case "v":
                        tiled = SpawnNewPieces(x, y, PieceType.VASE);
                        vaseNum++;
                        //tiled.transform.parent = transform;
                        break;
                    case "t":
                        tiled = SpawnNewPieces(x, y, PieceType.NORMAL);
                        tiled.ColorComponent.SetColor(ColorPiece.ColorType.RED);
                        //tiled.transform.parent = transform;
                        break;
                    
                }
                
            }
        }
        CheckTnt();
        UpdateGoals();
        //StartCoroutine(Fill());
    }

    public void UpdateGoals()
    {
        if (vaseNum == 0) ;
        if (boxNum == 0) ;
        if (stoneNum == 0) ;
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
        vaseText.text = vaseNum.ToString();
        stoneText.text = stoneNum.ToString();
        boxText.text = boxNum.ToString();
        movetext.text = moveCount.ToString();
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
    }

    public IEnumerator Fill()
    {
        while (FillStep())
        {
            //inverse = !inverse;
            yield return new WaitForSeconds(fillTime);
        }
    }

    public bool FillStep()
    {
        
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int xloop = 0; xloop < xDim; xloop++)
            {
                int x = xloop;
                
                GamePiece piece = pieces[x, y];
                if (piece.IsMovable())
                {
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
        }

        for (int x = 0; x < xDim; x++)
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

    public GamePiece SpawnNewPieces(int x, int y, PieceType type)
    {
        GameObject newPiece;
        if (type == PieceType.TNT)
        {
            
            newPiece =
                (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y),
                    quaternion.identity);
            newPiece.transform.parent = transform;
        }
        else
        {
            newPiece =
                (GameObject)Instantiate(piecePrefabDict[type]);
            newPiece.transform.parent = transform;
            newPiece.transform.DOScale(new Vector3(0.7f,0.7f,0.7f), TweenDuration);
            newPiece.transform.DOMove(new Vector3(GetWorldPosition(x, y).x, GetWorldPosition(x, y).y,0f), TweenDuration);
        }
        
                //GetWorldPosition(x, y), quaternion.identity);
        

        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);
        
        

        return pieces[x, y];
    }

    public bool isAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Math.Abs(piece1.Y - piece2.Y) == 1) ||
               (piece1.Y == piece2.Y && (int)Math.Abs(piece1.X - piece2.X) == 1);
    }

    public List<GamePiece> GetMatch(GamePiece piece)
    {
        List<GamePiece> matching = piece.getConnectedTiles(null);
        if (matching.Where(a => a.Type == PieceType.NORMAL).Count() >= 2)
        {
            if (matching.Where(a => a.Type == PieceType.NORMAL).Count() >= 4)
            {
                piece.transform.DOScale(Vector3.zero, TweenDuration);
                Destroy(piece.gameObject);
                SpawnNewPieces(piece.X, piece.Y, PieceType.TNT);
                return matching.Except(new GamePiece[]{piece}).ToList();
            }
                return matching;
        };

        return null;
    }

    public List<GamePiece> getTntParticles(GamePiece tile, List<GamePiece> exclude = null, bool combo = false)
    {
        var result = new List<GamePiece> { tile, };
        

        if(exclude == null)
        {
            exclude = new List<GamePiece>{ tile, };
        }
        else
        {
            exclude.Add(tile);
        }
        
        //var tntList = new List<Tile>();
        //var tempTntTiles = new List<Tile>();
        var tntScale = combo ? 3 : 2;
        //var tntHeight = combo ? 4: 2;
        for (var y = tile.Y - tntScale; y <= tile.Y + tntScale; y++)
        {
            for (var x = tile.X - tntScale; x <= tile.X + tntScale; x++)
            {
                if (x < xDim && x >= 0 && y < yDim && y >= 0)
                {
                    var connTile = pieces[x, y];
                    if (!exclude.Contains(connTile))
                    {
                        if (connTile.Type == PieceType.TNT)
                        {
                            
                            //tntList.Add(Tiles[x,y]);
                            result.AddRange(getTntParticles(connTile, exclude));
                            
                        }
                        else result.Add(connTile);
                        
                    }
                    
                }
            }
        }
        
        return result;
    }

    public void CheckTnt()
    {
        for (var y = 0; y < yDim; y++)
        {
            for (var x = 0; x < xDim; x++)
            {
                var piece = pieces[x, y];
                if (piece.IsColored() && piece.getConnectedTiles().Where(a => a.IsColored()).Count() >= 4)
                {
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.BLUE) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.BLUEB);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.RED) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.REDB);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.GREEN) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.GREENB);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.YELLOW) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.YELLOWB);
                }

                if (piece.IsColored() && piece.getConnectedTiles().Where(a => a.IsColored()).Count() < 4)
                {
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.BLUEB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.BLUE);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.REDB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.RED);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.GREENB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.GREEN);
                    if(piece.ColorComponent.Color == ColorPiece.ColorType.YELLOWB) pieces[x, y].ColorComponent.SetColor(ColorPiece.ColorType.YELLOW);
                }
            }
        }
    }
    public bool IsTntCombo(GamePiece piece)
    {
        Debug.Log(piece.Neighbours.Length);
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
            if (IsTntCombo(piece)) cleaningTiles = getTntParticles(piece, null, true);
            else cleaningTiles = getTntParticles(piece, null);
        }
        
        if (cleaningTiles != null)
        {
            moveCount -= 1;
            foreach (var tile in cleaningTiles)
            {
                ClearPiece(tile.X, tile.Y);
                
            }
            UpdateGoals();
            StartCoroutine(Fill());
            
            if (isGameFinished())
            {
                DOTween.Clear();
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
                
                StartCoroutine(WaitforMenu());
                SceneManager.LoadSceneAsync("MainScreen");
            }
            else if (moveCount == 0)
            {
                SceneManager.LoadSceneAsync("MainScreen");
            }

        }
        
    }

    IEnumerator WaitforMenu()
    {
        
        yield return new WaitForSeconds(3);
        //panel.SetActive(true);
    }

    public bool ClearPiece(int x, int y)
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

    public bool isGameFinished()
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

    // Update is called once per frame
    void Update()
    {
        CheckTnt();
    }
    public class LevelData
    {
        public  int grid_width;
        public  int grid_height;
        public  int move_count;
        public  string[] grid;
    }
}
