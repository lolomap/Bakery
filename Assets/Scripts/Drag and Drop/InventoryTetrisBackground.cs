using System;
using System.Collections.Generic;
using Grid;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class InventoryTetrisBackground : MonoBehaviour
{
    [HideInInspector] public InventoryTetris inventoryTetris;
    [HideInInspector] public GameObject cellTemplate;
    public List<CellTypeOverride> cellTypeOverrides = new();
    private GridLayoutGroup _gridLayoutGroup;
    private GameObject[,] _cellObjects;

    private void Start()
    {
        InitializeGrid();
    }
    public Dictionary<ItemType, Sprite> LoadSpriteMapAndApplyOverrides()
    {
        Dictionary<ItemType, Sprite> spriteMap = new Dictionary<ItemType, Sprite>();

        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            if (itemType == ItemType.Default)
                continue;

            Sprite sprite = Resources.Load<Sprite>($"InventoryCellTypes/{itemType}");
            if (sprite)
            {
                spriteMap[itemType] = sprite;
            }

        }

        if (cellTypeOverrides == null) return spriteMap;
        foreach (var overrideEntry in cellTypeOverrides)
        {
            if (!inventoryTetris.GetGrid().IsValidCell(overrideEntry.position)) 
                continue;
                    
            var cell = inventoryTetris.GetGrid().GetCell(overrideEntry.position.x, overrideEntry.position.y);
            cell.Type = overrideEntry.type;
        }

        return spriteMap;
    }
    private void ClearExistingGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
        
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        _cellObjects = new GameObject[0,0];

     
#if UNITY_EDITOR
        if (!Application.isPlaying && transform.childCount > 0)
        {
            ClearExistingGrid(); 
        }
#endif
    }

    public void InitializeGrid(Dictionary<ItemType, Sprite> spriteMap = null)
    {
        ClearExistingGrid();
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();

        int gridWidth = inventoryTetris.GetGrid().Width;
        int gridHeight = inventoryTetris.GetGrid().Height;
        float cellSize = inventoryTetris.GetGrid().CellSize;

        _gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        _gridLayoutGroup.constraintCount = gridWidth;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);
        rt.anchoredPosition = inventoryTetris.GetComponent<RectTransform>().anchoredPosition;

        _cellObjects = new GameObject[gridWidth, gridHeight];
        
        if (cellTypeOverrides.Count != 0)
        {
            spriteMap = LoadSpriteMapAndApplyOverrides();
        }
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cellInstance = Instantiate(cellTemplate, transform);
                Image image = cellInstance.GetComponent<Image>();
                cellInstance.SetActive(true);
                _cellObjects[x, y] = cellInstance;
                
                Cell cell = inventoryTetris.GetGrid().GetCell(x, y);
                if (spriteMap != null && spriteMap.TryGetValue(cell.Type, out var sprite))
                {
                    image.sprite = sprite;
                }
                cell.SetBackgroundImage(image);
            }
        }
    }
}