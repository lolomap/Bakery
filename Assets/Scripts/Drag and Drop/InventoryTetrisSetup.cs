using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;


[System.Serializable]
public struct CellTypeOverride
{
    public Vector2Int position;
    public ItemType type;
}
public class InventoryTetrisSetup : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int gridWidth = 3;
    public int gridHeight = 3;
    public float cellSize = 75f;
    public GameObject cellTemplate;
    public ListAddItemTetris contentSetup;

    [Header("Internal References")]
    [HideInInspector] public InventoryTetris inventoryTetris;
    [HideInInspector] public InventoryTetrisBackground background;

    private void Start()
    {
        inventoryTetris.Load(contentSetup);
    }

    public void SetupInventory()
    {
        inventoryTetris = GetComponentInChildren<InventoryTetris>();
        background = GetComponentInChildren<InventoryTetrisBackground>();

        inventoryTetris.gridWidth = gridWidth;
        inventoryTetris.gridHeight = gridHeight;
        inventoryTetris.cellSize = cellSize;

        GridLayoutGroup gridLayout = background.gameObject.GetComponent<GridLayoutGroup>();

        gridLayout.cellSize = Vector2.one * cellSize;
        gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
        gridLayout.childAlignment = TextAnchor.LowerLeft;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridWidth;

        background.inventoryTetris = inventoryTetris;
        background.cellTemplate = cellTemplate;


        if (Application.isPlaying) return;
        
        inventoryTetris.InitializeGrid(gridWidth, gridHeight);
        background.InitializeGrid();
    }
}