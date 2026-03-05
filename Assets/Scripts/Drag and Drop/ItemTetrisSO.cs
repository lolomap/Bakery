using Grid;
using UnityEngine;
using UnityEngine.UI;
using Utils;


[CreateAssetMenu(menuName = "TetrisInventory/ItemTetris")]
public class ItemTetrisSO : GridItemBaseSO
{
    public int id;
    public string nameRu;
    public string nameEn;
    public ItemType type;
    public int rarity;
    public Sprite itemSprite;

    private void OnValidate()
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[ItemTetrisSo] ({name}): Prefab is not assigned!", this);
            return;
        }

        if (itemSprite != null) return;
        var image = prefab.GetComponentInChildren<Image>();
        if (image != null)
        {
            itemSprite = image.sprite;
        }
        else
        {
            Debug.LogWarning($"[ItemTetrisSo] ({name}): There is no Image component in the prefab!", this);
        }
    }
}