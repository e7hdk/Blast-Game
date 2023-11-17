using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GamePiece: MonoBehaviour
    {
        private int x;
        private int y;

        public Button button;
        public int X
        {
            get { return x; }
            set
            {
                if (IsMovable())
                {
                    x = value;
                }
            }
        }

        public int Y
        {
            get { return y; }
            set
            {
                if (IsMovable())
                {
                    y = value;
                }
            }
        }

        private Grid.PieceType type;

        public Grid.PieceType Type
        {
            get { return type; }
        }

        private Grid grid;

        public Grid GridRef
        {
            get { return grid; }
        }

        private MovablePiece movableComponent;

        public MovablePiece MovableComponent
        {
            get { return movableComponent; }
        }
        
        private ColorPiece colorComponent;

        public ColorPiece ColorComponent
        {
            get { return colorComponent; }
        }

        private ClearablePiece clearablePiece;

        public ClearablePiece ClearablePiece
        {
            get { return clearablePiece; }
        }

        private void Awake()
        {
            movableComponent = GetComponent<MovablePiece>();
            colorComponent = GetComponent<ColorPiece>();
            clearablePiece = GetComponent<ClearablePiece>();

        }

        private void Start()
        {
            
        }

        public void Init(int _x, int _y, Grid _grid, Grid.PieceType _type)
        {
            x = _x;
            y = _y;
            grid = _grid;
            type = _type;
        }

        public bool IsMovable()
        {
            return movableComponent != null;
        }

        public bool IsColored()
        {
            return colorComponent != null;
        }

        public bool IsClearable()
        {
            return clearablePiece != null;
        }
        private void OnMouseDown()
        {
            
            grid.PressPiece(this);
            
        }
        public GamePiece Left => x > 0 ? grid.pieces[x - 1, y] : null;
        public GamePiece Top => y > 0 ? grid.pieces[x, y - 1] : null;
        public GamePiece Right => x < grid.xDim - 1 ? grid.pieces[x + 1, y] : null;
        public GamePiece Bottom => y < grid.yDim - 1 ? grid.pieces[x, y + 1] : null;
        
        public GamePiece[] Neighbours => new[]
        {
            Left,
            Top,
            Right,
            Bottom,
        };
        public List<GamePiece> getConnectedTiles(List<GamePiece> exclude = null)
        {
            var result = new List<GamePiece> { this, };
            //var obstacleResult = new List<Tile>();

            if(exclude == null)
            {
                exclude = new List<GamePiece> { this, };
            }
            else
            {
                exclude.Add(this);
            }

            foreach(var neighbour in Neighbours)
            {
                
                if (neighbour == null || exclude.Contains(neighbour)) continue;
                
                if (IsColored() && (neighbour.Type == Grid.PieceType.BOX ||
                                    neighbour.Type == Grid.PieceType.VASE || neighbour.Type == Grid.PieceType.VASE2))
                {   
                    result.Add(neighbour);
                }
            
                if (IsColored() && neighbour.IsColored()) {
                    if((colorComponent.Color == ColorPiece.ColorType.BLUE || colorComponent.Color == ColorPiece.ColorType.BLUEB) && 
                       (neighbour.ColorComponent.Color == ColorPiece.ColorType.BLUE 
                        ||neighbour.ColorComponent.Color == ColorPiece.ColorType.BLUEB) )
                        result.AddRange(neighbour.getConnectedTiles(exclude));
                    if((colorComponent.Color == ColorPiece.ColorType.RED || colorComponent.Color == ColorPiece.ColorType.REDB) && 
                       (neighbour.ColorComponent.Color == ColorPiece.ColorType.RED 
                        ||neighbour.ColorComponent.Color == ColorPiece.ColorType.REDB) )
                        result.AddRange(neighbour.getConnectedTiles(exclude));
                    if((colorComponent.Color == ColorPiece.ColorType.YELLOW || colorComponent.Color == ColorPiece.ColorType.YELLOWB)&& 
                       (neighbour.ColorComponent.Color == ColorPiece.ColorType.YELLOW 
                        ||neighbour.ColorComponent.Color == ColorPiece.ColorType.YELLOWB) )
                        result.AddRange(neighbour.getConnectedTiles(exclude));
                    if((colorComponent.Color == ColorPiece.ColorType.GREEN || colorComponent.Color == ColorPiece.ColorType.GREENB) && 
                       (neighbour.ColorComponent.Color == ColorPiece.ColorType.GREEN 
                        ||neighbour.ColorComponent.Color == ColorPiece.ColorType.GREENB) )
                        result.AddRange(neighbour.getConnectedTiles(exclude));
                    //neighbour.ColorComponent.Color == ColorComponent.Color;
                    //result.AddRange(neighbour.getConnectedTiles(exclude));
                }


            }
            return result;

        }
    }
}