using System.Collections.Generic;
using System.Linq;
using GameData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UI
{
    [RequireComponent(typeof(Image))]
    public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ItemSO Data;

        public Image Icon { get; private set; }

        private CellUI _targetCell;
        private List<CellUI> _hoveredCells;
        private Camera _camera;
        private GraphicRaycaster _raycaster;
        private RectTransform _rectTransform;

        private Vector3 _beginDragPos;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
            Icon = GetComponent<Image>();
            _camera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _beginDragPos = transform.position;
            if (_targetCell != null)
                _targetCell.Inventory.Clear(_targetCell.GridPosition.x, _targetCell.GridPosition.y, Data);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform,
                eventData.position,
                _camera,
                out Vector3 pos);

            transform.position = pos;

            List<RaycastResult> hits = new();
            _raycaster.Raycast(eventData, hits);

            if (_hoveredCells != null)
            {
                foreach (CellUI cell in _hoveredCells)
                {
                    cell.OnNotHover();
                }
            }
            
            _hoveredCells = hits
                .Select(hit => hit.gameObject.GetComponent<CellUI>())
                .Where(cell => cell != null)
                .ToList();

            _targetCell = null;

            if (_hoveredCells.Count == 0)
                return;

            foreach (CellUI cell in _hoveredCells)
            {
                cell.OnHover(cell.Inventory.CanPlace(cell.GridPosition.x, cell.GridPosition.y, Data));
            }
            

            _targetCell = _hoveredCells
                .OrderByDescending(c => c.transform.position.y)
                .ThenBy(c => c.transform.position.x)
                .First();
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_targetCell == null || _targetCell.InventoryComponent == null || _targetCell.Inventory == null)
            {
                transform.position = _beginDragPos;
                return;
            }

            Vector2Int gridPos = _targetCell.GridPosition;
            
            if (!_targetCell.Inventory.Set(gridPos.x, gridPos.y, Data))
            {
                transform.position = _beginDragPos;
                return;
            }

            Transform parent = _rectTransform.parent;
            _rectTransform.SetParent(_targetCell.transform);
            RectTransform cellRect = _targetCell.GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = Vector2.zero
                + new Vector2((_rectTransform.rect.width - cellRect.rect.width) / 2f, -(_rectTransform.rect.height - cellRect.rect.height) / 2f);
            _rectTransform.SetParent(parent, true);
        }
    }
}
