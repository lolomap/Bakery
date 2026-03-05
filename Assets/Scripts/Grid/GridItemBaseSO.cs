using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Grid
{
    public class GridItemBaseSO : ScriptableObject
    {
        public Shape shape;
        public string nameString;
        public GameObject prefab;

        public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction)
        {
            List<Vector2Int> baseGridPositions = direction switch
            {
                Direction.Down => Shapes.GetShapeData(shape).GridPositions0,
                Direction.Left => Shapes.GetShapeData(shape).GridPositions90,
                Direction.Up => Shapes.GetShapeData(shape).GridPositions180,
                Direction.Right => Shapes.GetShapeData(shape).GridPositions270,
                _ => Shapes.GetShapeData(shape).GridPositions0
            };
            
            List<Vector2Int> finalGridPositions = new List<Vector2Int>();
            
            foreach (Vector2Int position in baseGridPositions)
            {
                finalGridPositions.Add(position + offset);
            }

            return finalGridPositions;
        }
        
        public Vector2 GetOriginOffset(Direction direction)
        {
            Vector2 offset = prefab.GetComponent<RectTransform>().rect.size * 0.5f;
            switch (direction)
            {
                default:
                case Direction.Down: return new Vector2(offset.x, offset.y);
                case Direction.Left: return new Vector2(offset.y, offset.x);
                case Direction.Up: return new Vector2(offset.x, offset.y);
                case Direction.Right: return new Vector2(offset.y, offset.x);
            }
        }
        
    }
}