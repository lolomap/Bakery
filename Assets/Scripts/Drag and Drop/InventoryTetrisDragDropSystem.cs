using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using Utils;

public class InventoryTetrisDragDropSystem : MonoBehaviour
{
    public static InventoryTetrisDragDropSystem Instance { get; private set; }

    [SerializeField] private List<InventoryTetrisSetup> inventorySetupsList;

    private InventoryTetris _draggingInventoryTetris;
    private GridItemBase _draggingGridItemBase;
    private Vector2Int _mouseDragGridPositionOffset;
    private Vector2 _mouseDragAnchoredPositionOffset;
    private Vector3 _mouseDragOffset;

    private Direction _direction;
    private readonly List<Cell> _highlightedGridObjects = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (InventoryTetrisSetup inventoryTetrisSetup in inventorySetupsList)
        {
            inventoryTetrisSetup.inventoryTetris.OnObjectPlaced += (_, _) => { };
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _direction = DirectionUtilities.Next(_direction);
        }

        if (_draggingGridItemBase == null) return;


        if (Camera.main != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = _draggingGridItemBase.transform.position.z;

            // Translate world position to local parent coordinates 
            Vector3 targetLocalPosition =
                _draggingGridItemBase.transform.parent.InverseTransformPoint(mouseWorldPosition);

            targetLocalPosition += _mouseDragOffset;

            // Snap to grid 
            float gridSize = 10f;
            targetLocalPosition.x = Mathf.Floor(targetLocalPosition.x / gridSize) * gridSize;
            targetLocalPosition.y = Mathf.Floor(targetLocalPosition.y / gridSize) * gridSize;

            // Smooth move 
            _draggingGridItemBase.transform.localPosition = Vector3.Lerp(
                _draggingGridItemBase.transform.localPosition,
                targetLocalPosition,
                Time.deltaTime * 20f
            );

        }

        _draggingGridItemBase.transform.rotation = Quaternion.Lerp(
            _draggingGridItemBase.transform.rotation,
            Quaternion.Euler(0, 0, -DirectionUtilities.ToDegrees(_direction)),
            Time.deltaTime * 15f
        );

        HighlightGridPositions();
    }

    private void HighlightGridPositions()
    {
        // Clear previous highlights
        foreach (var gridObject in _highlightedGridObjects)
        {
            gridObject.ClearHighlight();
        }

        _highlightedGridObjects.Clear();

        InventoryTetris toInventoryTetris = null;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (InventoryTetrisSetup inventoryTetrisSetup in inventorySetupsList)
        {
            // Convert mouse world position to local position in the inventory's coordinate system
            Vector3 localPoint = inventoryTetrisSetup.transform.InverseTransformPoint(mouseWorldPosition);
            Vector2 anchoredPosition = new Vector2(localPoint.x, localPoint.y);

            Vector2 offset = _draggingGridItemBase.BaseSO.GetOriginOffset(_direction);
            Vector2Int placedObjectOrigin =
                inventoryTetrisSetup.inventoryTetris.SnapToNearestCell(anchoredPosition -
                                                                       new Vector2(offset.x, offset.y));

            if (inventoryTetrisSetup.inventoryTetris.IsValidGridPosition(placedObjectOrigin))
            {
                toInventoryTetris = inventoryTetrisSetup.inventoryTetris;
                break;
            }
        }

        // Check if it's on top of an InventoryTetris
        if (toInventoryTetris != null)
        {
            // Convert mouse world position to local position in the inventory's coordinate system
            Vector3 localPoint = toInventoryTetris.transform.InverseTransformPoint(mouseWorldPosition);
            Vector2 anchoredPosition = new Vector2(localPoint.x, localPoint.y);

            Vector2 offset = _draggingGridItemBase.BaseSO.GetOriginOffset(_direction);
            Vector2Int placedObjectOrigin =
                toInventoryTetris.SnapToNearestCell(anchoredPosition - new Vector2(offset.x, offset.y));
            List<Vector2Int> gridPositionList = _draggingGridItemBase.BaseSO.GetGridPositionList(placedObjectOrigin, _direction);

            bool canPlace = gridPositionList.All(gridPosition =>
            {
                // Case 0: Check if grid position is valid
                if (!toInventoryTetris.IsValidGridPosition(gridPosition)) return false;

                var gridObject = toInventoryTetris.GetGrid().GetCell(gridPosition.x, gridPosition.y);
                ItemType draggingItemType = ((ItemTetrisSO) _draggingGridItemBase.BaseSO).type;

                // Case 1: Cell is occupied by another object (not our dragged object)
                if (!gridObject.IsEmpty && gridObject.GridItemBase != _draggingGridItemBase)
                {
                    return false;
                }

                // Case 2: Cell has special type that doesn't match our item type

                if (gridObject.Type != ItemType.Default && gridObject.Type != draggingItemType)
                {
                    return false;
                }

                // All checks passed - this position is valid
                return true;
            });

            var highlightColor = canPlace ? Colors.AvailableCellBackgroundColor : Colors.UnavailableCellBackgroundColor;

            foreach (var gridPosition in gridPositionList)
            {
                if (!toInventoryTetris.IsValidGridPosition(gridPosition)) continue;
                var gridObject = toInventoryTetris.GetGrid().GetCell(gridPosition.x, gridPosition.y);
                gridObject.Highlight(highlightColor);
                _highlightedGridObjects.Add(gridObject);
            }
        }
    }

    public void StartedDragging(InventoryTetris inventoryTetris, GridItemBase gridItemBase)
    {
        // Started Dragging
        _draggingInventoryTetris = inventoryTetris;
        _draggingGridItemBase = gridItemBase;

        // Save initial direction when started dragging
        _direction = gridItemBase.Direction;

        // Calculate and store the offset between the mouse position and the object's position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = gridItemBase.transform.position.z;
        _mouseDragOffset = gridItemBase.transform.position - mouseWorldPosition;
    }

    public void StoppedDragging(InventoryTetris fromInventory, GridItemBase draggedItem)
    {
        fromInventory.RemoveItemAt(draggedItem.GetGridPositionList()[0]);

        Vector3 mouseWorldPos = GetMouseWorldPosition(fromInventory.transform.position.z);
        InventoryTetris toInventory = GetInventoryUnderMouse(mouseWorldPos, draggedItem);

        bool wasPlaced = TryPlaceItemInTargetInventory(toInventory, mouseWorldPos, draggedItem);
        
        // Restore if item can't be placed
        if (!wasPlaced)
        {
            draggedItem.CancelPlacement();
            fromInventory.TryPlaceItem(draggedItem.BaseSO as ItemTetrisSO, draggedItem.Origin, draggedItem.Direction);
        }
        
        draggedItem.ConfirmPlacement();
        ClearHighlights();
        ResetDraggingState();
    }

    private Vector3 GetMouseWorldPosition(float zPlane)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = zPlane;
        return position;
    }

    private InventoryTetris GetInventoryUnderMouse(Vector3 mouseWorldPos, GridItemBase gridItemBase)
    {
        foreach (InventoryTetrisSetup setup in inventorySetupsList)
        {
            Vector2 anchoredPos = GetAnchoredPosition(setup.transform, mouseWorldPos);
            Vector2 offset = gridItemBase.BaseSO.GetOriginOffset(_direction);
            Vector2Int origin = setup.inventoryTetris.SnapToNearestCell(anchoredPos - offset);

            if (setup.inventoryTetris.IsValidGridPosition(origin))
                return setup.inventoryTetris;
        }

        return null;
    }

    private Vector2 GetAnchoredPosition(Transform transform, Vector3 worldPosition)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPosition);
        return new Vector2(localPoint.x, localPoint.y);
    }

    private Vector2Int GetPlacementOrigin(InventoryTetris inventory, Vector3 mouseWorldPos, GridItemBase gridItemBase)
    {
        Vector2 anchoredPos = GetAnchoredPosition(inventory.transform, mouseWorldPos);
        Vector2 offset = gridItemBase.BaseSO.GetOriginOffset(_direction);
        return inventory.SnapToNearestCell(anchoredPos - offset);
    }

    private bool TryPlaceItemInTargetInventory(InventoryTetris targetInventory, Vector3 mouseWorldPos, GridItemBase item) 
    {
        if (targetInventory == null)
            return false;

        Vector2Int origin = GetPlacementOrigin(targetInventory, mouseWorldPos, item);
        
        return targetInventory.TryPlaceItem(item.BaseSO as ItemTetrisSO, origin, _direction, item);
    }
    
    private void ClearHighlights()
    {
        foreach (var gridObject in _highlightedGridObjects)
        {
            gridObject.ClearHighlight();
        }

        _highlightedGridObjects.Clear();
    }

    private void ResetDraggingState()
    {
        _draggingInventoryTetris = null;
        _draggingGridItemBase = null;
        _mouseDragOffset = Vector3.zero;
    }
    
    private void ClearAllMergeHighlights()
    {
        foreach (var inventory in inventorySetupsList)
        {
            foreach (ItemTetrisVisual item in inventory.inventoryTetris.GetItemCollection())
            {
                item.SetHighlight(false);
            }
        }
    }
}