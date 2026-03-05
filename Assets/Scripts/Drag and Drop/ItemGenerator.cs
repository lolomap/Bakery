using System.Collections.Generic;
using DG.Tweening;
using Grid;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ItemGenerator : ItemTetris, IPointerClickHandler
{
    private const float MoveDuration = 0.3f;
    private const float ScaleDuration = 0.2f;
    private const float JumpPower = 50f;
    private const float ClickDelay = 0.3f;
    private const float DepletionShakeStrength = 10f;
    
    [SerializeField] private ItemTetrisSO[] _itemPool;
    [SerializeField] private int _maxGenerations = 5;
    
    private int _generationsCount;
    private float _lastClickTime;
    
    public override void Initialize(Vector2Int origin, Direction direction, GridItemBaseSO gridItemSO, GridItemBase gridItem = null)
    {
        base.Initialize(origin, direction, gridItemSO, gridItem);
        
        // If created from another item, not SO - save its data 
        if (gridItem) _generationsCount = ((ItemGenerator) gridItem)!._generationsCount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanProcessClick(eventData)) return;
        
        _lastClickTime = Time.time;
        TryGenerateItemInAdjacentCell();
    }
    

    private bool CanProcessClick(PointerEventData eventData)
    {
        return Time.time - _lastClickTime >= ClickDelay &&
               eventData.button == PointerEventData.InputButton.Left &&
               inventoryTetris != null &&
               _itemPool != null &&
               _itemPool.Length > 0 &&
               _generationsCount >= 0 && 
               _generationsCount < _maxGenerations;
    }

    private void TryGenerateItemInAdjacentCell()
    {
        var adjacentPositions = GetAdjacentPositions(Origin);
        
        foreach (var position in adjacentPositions)
        {
            if (TryPlaceItemAtPosition(position))
            {
                break;
            }
        }
    }

    private ItemTetrisSO GetRandomItemFromPool()
    {
        int randomIndex = Random.Range(0, _itemPool.Length);
        return _itemPool[randomIndex];
    }

    private bool TryPlaceItemAtPosition(Vector2Int position)
    {
        if (!inventoryTetris.GetGrid().IsValidCell(position)) return false;
        
        var gridObject = inventoryTetris.GetGrid().GetCell(position.x, position.y);
        if (gridObject == null || !gridObject.IsEmpty) return false;

        _canvasGroup.blocksRaycasts = false;
        
        var randomItem = GetRandomItemFromPool();
        CreateItemVisual(randomItem, position);
        return true;
    }

    private void CreateItemVisual(ItemTetrisSO item, Vector2Int targetPosition)
    {
        var visualObject = CreateVisualObject(item);
        var visualRectTransform = visualObject.GetComponent<RectTransform>();
        
        Vector2 generatorLocalPos = GetGeneratorLocalPosition();
        Vector2 targetLocalPos = CalculateTargetLocalPosition(targetPosition, generatorLocalPos);

        AnimateItemVisual(visualRectTransform, generatorLocalPos, targetLocalPos, item, targetPosition);
    }

    private GameObject CreateVisualObject(ItemTetrisSO item)
    {
        var visualObject = new GameObject("ItemVisual");
        var visualRectTransform = visualObject.AddComponent<RectTransform>();
        visualRectTransform.SetParent(inventoryTetris.transform, false);
        
        var visualImage = visualObject.AddComponent<Image>();
        visualImage.sprite = item.itemSprite ?? item.prefab.GetComponentInChildren<Image>().sprite;
        
        return visualObject;
    }

    private Vector2 GetGeneratorLocalPosition()
    {
        Vector2 generatorScreenPos = RectTransformUtility.WorldToScreenPoint(
            Camera.main,
            transform.position
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryTetris.transform as RectTransform,
            generatorScreenPos,
            Camera.main,
            out Vector2 generatorLocalPos
        );

        return generatorLocalPos;
    }

    private Vector2 CalculateTargetLocalPosition(Vector2Int targetPosition, Vector2 generatorLocalPos)
    {
        float cellSize = inventoryTetris.GetGrid().CellSize;
        Vector2Int positionOffset = targetPosition - Origin;
        
        return generatorLocalPos + new Vector2(
            positionOffset.x * cellSize,
            positionOffset.y * cellSize
        );
    }

    private void AnimateItemVisual(
        RectTransform visualTransform, 
        Vector2 startPos, 
        Vector2 targetPos,
        ItemTetrisSO item,
        Vector2Int gridPosition)
    {
        visualTransform.localPosition = startPos;
        visualTransform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence();
        sequence.Append(visualTransform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack));
        sequence.Join(visualTransform.DOLocalJump(targetPos, JumpPower, 1, MoveDuration).SetEase(Ease.OutCubic));
        sequence.AppendCallback(() => OnVisualAnimationComplete(visualTransform.gameObject, item, gridPosition));
    }

    private void OnVisualAnimationComplete(GameObject visualObject, ItemTetrisSO item, Vector2Int position)
    {
        Destroy(visualObject);
        bool valid = inventoryTetris.TryPlaceItem(item, position, Direction.Down);

        if (valid)
        {
            _generationsCount++;
            _canvasGroup.blocksRaycasts = true;

            if (_generationsCount >= _maxGenerations)
            {
                DepleteGenerator();
            }
        }
    }

    private void DepleteGenerator()
    {
        _canvasGroup.blocksRaycasts = false;
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack));
        sequence.Join(transform.DOShakePosition(0.1f, strength: DepletionShakeStrength));
        sequence.OnComplete(() => inventoryTetris.RemoveItemAt(Origin));
    }

    private List<Vector2Int> GetAdjacentPositions(Vector2Int origin)
    {
        return new List<Vector2Int>
        {
            origin + Vector2Int.left,
            origin + Vector2Int.down,
            origin + Vector2Int.right,
            origin + Vector2Int.up
        };
    }
}