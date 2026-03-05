using System.Linq;
using UnityEngine;

public class InventoryTetrisAssets : MonoBehaviour
{
    public static InventoryTetrisAssets Instance { get; private set; }
    public ItemTetrisSO[] itemTetrisSoArray;
    public Transform gridVisual;

    private void Awake()
    {
        Instance = this;
    }
    
    public ItemTetrisSO GetItemTetrisSoFromName(string itemTetrisSoName)
    {
        return itemTetrisSoArray.FirstOrDefault(itemTetrisSo => itemTetrisSo.name == itemTetrisSoName);
    }
    
    public ItemTetrisSO GetItemTetrisSoFromId(int itemTetrisSoId)
    {
        return itemTetrisSoArray.FirstOrDefault(itemTetrisSo => itemTetrisSo.id == itemTetrisSoId);
    }
}