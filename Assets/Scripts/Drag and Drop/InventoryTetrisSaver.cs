using System.Collections.Generic;
using Grid;
using UnityEngine;
using Utils;

[System.Serializable]
public struct AddItemTetris
{
    public string itemTetrisSoName;
    public Vector2Int gridPosition;
    public Direction direction;
    public int cost;
    public bool isLocked;
}

[System.Serializable]
public struct ListAddItemTetris
{
    public List<AddItemTetris> addItemTetrisList;
}

public class InventoryTetrisSaver
{
    private readonly InventoryTetris _inventory;

    public InventoryTetrisSaver(InventoryTetris inventory)
    {
        _inventory = inventory;
    }

    public string Save()
    {
        var placedObjects = GetUniquePlacedObjects();
        var saveData = ConvertToSaveFormat(placedObjects);
        return JsonUtility.ToJson(new ListAddItemTetris { addItemTetrisList = saveData });
    }

    public void Load(string loadString)
    {
        var loadedData = JsonUtility.FromJson<ListAddItemTetris>(loadString);
        Load(loadedData);
    }
    public void Load(ListAddItemTetris loadedData)
    {
        RestoreInventory(loadedData.addItemTetrisList);
    }

    private List<GridItemBase> GetUniquePlacedObjects()
    {
        var placedObjects = new HashSet<GridItemBase>();
        var grid = _inventory.GetGrid();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (!grid.GetCell(x, y).IsEmpty)
                {
                    placedObjects.Add(grid.GetCell(x, y).GridItemBase);
                }
            }
        }

        return new List<GridItemBase>(placedObjects);
    }

    private List<AddItemTetris> ConvertToSaveFormat(List<GridItemBase> placedObjects)
    {
        var saveData = new List<AddItemTetris>();

        foreach (var placedObject in placedObjects)
        {
            saveData.Add(new AddItemTetris
            {
                direction = placedObject.Direction,
                gridPosition = placedObject.Origin,
                itemTetrisSoName = (placedObject.BaseSO as ItemTetrisSO)?.name,
                isLocked = placedObject.GetComponent<ItemTetris>()?.IsLocked ?? false,
            });
        }

        return saveData;
    }

    private void RestoreInventory(List<AddItemTetris> itemsToLoad)
    {
        foreach (var itemData in itemsToLoad)
        {
            var itemSo = InventoryTetrisAssets.Instance.GetItemTetrisSoFromName(itemData.itemTetrisSoName);
            bool isPlaced = _inventory.TryPlaceItem(itemSo, itemData.gridPosition, itemData.direction,
                out GridItemBase placed, null, itemData.isLocked);
        }
    }
}