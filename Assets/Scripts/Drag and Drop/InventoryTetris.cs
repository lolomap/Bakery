using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using Utils;
using Zenject;

public class InventoryTetris : MonoBehaviour
{
    public event EventHandler<GridItemBase> OnObjectPlaced;
    public event EventHandler<GridItemBase> OnObjectRemoved;
    
    private Grid<Cell> _grid;
    private HashSet<Vector2Int> _freeGridPositions;
    private RectTransform _itemContainer;
    private readonly List<ItemTetrisVisual> _items = new();
    public IReadOnlyList<ItemTetrisVisual> GetItemCollection() => _items;

    private InventoryTetrisSaver _saver;

    [HideInInspector] public int gridWidth = 3;
    [HideInInspector] public int gridHeight = 3;
    [HideInInspector] public float cellSize = 75f;

    private void Awake()
    {
        _itemContainer = GetComponent<RectTransform>();
        _saver = new InventoryTetrisSaver(this);
        InitializeGrid(gridWidth, gridHeight);
    }

    public void InitializeGrid(int width, int height)
    {
        _grid = new Grid<Cell>(width, height, cellSize,
            new Vector3(-width * cellSize / 2, -height * cellSize / 2, 0),
            (x, y) => new Cell(x, y));
        
        _freeGridPositions = new HashSet<Vector2Int>();
        for (int y = 0; y < gridHeight; y++)
        for (int x = 0; x < gridWidth; x++)
            _freeGridPositions.Add(new Vector2Int(x, y));
    }

    public Grid<Cell> GetGrid() => _grid;
    public bool IsValidGridPosition(Vector2Int gridPosition) => _grid.IsValidCell(gridPosition);

    public Vector2Int SnapToNearestCell(Vector3 worldPosition)
    {
        Vector2 localPos = (worldPosition - _grid.Origin) / _grid.CellSize;

        int snappedX = Mathf.RoundToInt(localPos.x);
        int snappedY = Mathf.RoundToInt(localPos.y);

        return new Vector2Int(snappedX, snappedY);
    }
    
    public Vector2Int GetFreeOrigin()
    {
        return _freeGridPositions.Count > 0 ? _freeGridPositions.First() : -Vector2Int.one;
    }

    public bool TrySpawnItem(ItemTetrisSO itemTetrisSo, GridItemBase itemTetris = null)
    {
        var freeOrigin = GetFreeOrigin();
        return freeOrigin != -Vector2Int.one && TryPlaceItem(itemTetrisSo, freeOrigin, Direction.Down, itemTetris);
    }

    public bool TryPlaceItem(ItemTetrisSO itemTetrisSo, Vector2Int origin, Direction direction, GridItemBase itemTetris = null, bool isLocked = false)
    {
        if (!CanPlaceItem(itemTetrisSo, origin, direction))
        {
            return false;
        }

        GridItemBase gridItemBase = PlaceItem(origin, itemTetrisSo, direction, itemTetris, isLocked);
        OnObjectPlaced?.Invoke(this, gridItemBase);
        return true;
    }
    public bool TryPlaceItem(ItemTetrisSO itemTetrisSo, Vector2Int origin, Direction direction, out GridItemBase placedItem,
        GridItemBase itemTetris = null, bool isLocked = false)
    {
        if (!CanPlaceItem(itemTetrisSo, origin, direction))
        {
            placedItem = null;
            return false;
        }

        GridItemBase gridItemBase = PlaceItem(origin, itemTetrisSo, direction, itemTetris, isLocked);
        placedItem = gridItemBase;
        OnObjectPlaced?.Invoke(this, gridItemBase);
        return true;
    }

    private bool CanPlaceItem(ItemTetrisSO item, Vector2Int origin, Direction dir)
    {
        foreach (var pos in item.GetGridPositionList(origin, dir))
        {
            // Case 0: Check if grid position is valid and empty
            if (!_grid.IsValidCell(pos) || !_grid.GetCell(pos.x, pos.y).IsEmpty) {
                return false;
            }
            
            // Case 1: Check if grid position has special type that doesn't match our item type
            if (_grid.GetCell(pos.x, pos.y).Type != ItemType.Default && _grid.GetCell(pos.x, pos.y).Type != item.type)
            {
                return false;
            }
        }
        
        // All checks passed - this position is valid
        return true;
    }

    public GridItemBase PlaceItem(Vector2Int origin, ItemTetrisSO itemSO, Direction direction, GridItemBase item = null,
        bool isLocked = false)
    {
        List<Vector2Int> gridPositionList = itemSO.GetGridPositionList(origin, direction);
        Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(origin.x, origin.y) +
                                            (Vector3) itemSO.GetOriginOffset(direction);

        var gridItemBase = GridItemFactory.Create(_itemContainer, placedObjectWorldPosition,
            origin, direction, itemSO, item);

        gridItemBase.transform.rotation = Quaternion.Euler(0, 0, -DirectionUtilities.ToDegrees(direction));

        SetupDragDropComponent(gridItemBase, isLocked, itemSO.type != ItemType.Skill);
        OccupyGridCells(gridItemBase, gridPositionList);
        _items.Add(gridItemBase.GetComponent<ItemTetrisVisual>());
        _freeGridPositions.Remove(origin);

        return gridItemBase;
    }

    private void SetupDragDropComponent(GridItemBase gridItemBase, bool isLocked, bool isDraggable)
    {
        var dragDrop = gridItemBase.GetComponent<ItemTetris>();
        dragDrop.Setup(this);
        dragDrop.SetLocked(isLocked);
        dragDrop.IsDraggable = isDraggable;
    }

    private void OccupyGridCells(GridItemBase gridItemBase, List<Vector2Int> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
            _grid.GetCell(gridPosition.x, gridPosition.y).GridItemBase = gridItemBase;
    }

    public void RemoveItemAt(Vector2Int gridPosition)
    {
        var placedObject = _grid.GetCell(gridPosition.x, gridPosition.y).GridItemBase;
        if (placedObject == null) return;
        
        RemoveItem(placedObject);
        _freeGridPositions.Add(gridPosition);
    }

    private void RemoveItem(GridItemBase gridItemBase)
    {
        gridItemBase.DestroySelf();
        ClearGridCells(gridItemBase);
        _items.Remove(gridItemBase.GetComponent<ItemTetrisVisual>());
        OnObjectRemoved?.Invoke(this, gridItemBase);
    }

    private void ClearGridCells(GridItemBase gridItemBase)
    {
        foreach (var gridPosition in gridItemBase.GetGridPositionList())
            _grid.GetCell(gridPosition.x, gridPosition.y).ClearPlacedObject();
    }
    
    public string Save() => _saver.Save();

    public void Load(string loadString) => _saver.Load(loadString);
    public void Load(ListAddItemTetris loadData) => _saver.Load(loadData);
}