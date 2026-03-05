using System;
using UnityEngine;

namespace Grid
{
    public class Grid<TGridObject>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        private readonly TGridObject[,] _gridArray;

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public Vector3 Origin => _originPosition;
        public Grid(int width, int height, float cellSize, Vector3 originPosition,
            Func<int, int, TGridObject> createCell)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new TGridObject[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createCell(x, y);
                }
            }
        }
    
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * _cellSize + _originPosition;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
        }

        public TGridObject GetCell(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _gridArray[x, y];
            }

            return default;
        }

        public bool IsValidCell(Vector2Int gridPosition)
        {
            int x = gridPosition.x;
            int y = gridPosition.y;

            return x >= 0 && y >= 0 && x < _width && y < _height;
        }
    }
}