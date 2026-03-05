using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Grid
{
    public abstract class GridItemBase : MonoBehaviour
    {
        private GridItemBaseSO _baseSo;
        private Vector2Int _origin;
        private Direction _direction;

        public GridItemBaseSO BaseSO => _baseSo;
        public Direction Direction => _direction;
        public Vector2Int Origin => _origin;
        
        public event Action OnCanceled;
        public event Action OnPlaced;

        public virtual void Initialize(Vector2Int origin, Direction direction, GridItemBaseSO gridItemSO, GridItemBase gridItem = null)
        {
            _baseSo = gridItemSO;
            _origin = origin;
            _direction = direction;
        }

        public List<Vector2Int> GetGridPositionList()
        {
            return _baseSo.GetGridPositionList(_origin, _direction);
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public override string ToString()
        {
            return _baseSo.nameString;
        }
        
        public void CancelPlacement()
        {
            OnCanceled?.Invoke();
            OnCanceled = null;
            OnPlaced = null;
        }
        
        public void ConfirmPlacement()
        {
            OnPlaced?.Invoke();
        }
    }
}