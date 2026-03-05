using System.Collections.Generic;
using UnityEngine;

public class InventoryTetrisTesting : MonoBehaviour
{
    [SerializeField] private InventoryTetris inventoryTetris;
    [SerializeField] private InventoryTetris outerInventoryTetris;
    [SerializeField] private List<string> addItemTetrisSaveList;

    private int _addItemTetrisSaveListIndex;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            outerInventoryTetris.Load(addItemTetrisSaveList[_addItemTetrisSaveListIndex]);
            _addItemTetrisSaveListIndex = (_addItemTetrisSaveListIndex + 1) % addItemTetrisSaveList.Count;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(inventoryTetris.Save());
        }
    }
}