using Grid;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class ItemTetris : GridItemBase, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform _rectTransform;
    protected CanvasGroup _canvasGroup;
    public InventoryTetris inventoryTetris;
    //private TooltipManager _tooltipManager;
    private Canvas rootCanvas;
    private Transform _originalParent;

    private bool _isDragging;
    private bool _isLocked;
    private Image _lockIcon;
    private RectTransform _raycastMaskTransform;

    public bool IsLocked => _isLocked;
    public bool IsDraggable;

    [SerializeField] private bool isSpawner;
    [SerializeField] private ItemTetrisSO spawnerItemSo;
    public ItemTetrisSO SpawnerItemSo => spawnerItemSo;
    
    private GridItemBase _draggedObject;
    
    private void Awake()
    {
        //_tooltipManager = FindFirstObjectByType<TooltipManager>();
        /*if (_tooltipManager == null)
        {
            Debug.LogError("TooltipManager not found in the scene.");
        }*/
        rootCanvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        FindRaycastMask();

        if (_lockIcon == null)
        {
            CreateLockIcon();
        }

        UpdateLockStatus();
    }

    private void FindRaycastMask()
    {
        Transform visualTransform = transform.Find("Visual");
        if (visualTransform != null)
        {
            foreach (Transform child in visualTransform)
            {
                if (child.name != "RaycastMask") continue;
                _raycastMaskTransform = child as RectTransform;
                break;
            }
        }

        if (_raycastMaskTransform == null)
        {
            _raycastMaskTransform = _rectTransform;
        }
    }

    private void CreateLockIcon()
    {
        GameObject lockObject = new GameObject("LockIcon");
        _lockIcon = lockObject.AddComponent<Image>();

        _lockIcon.sprite = Resources.Load<Sprite>("LockIcon");

        RectTransform lockRectTransform = _lockIcon.GetComponent<RectTransform>();

        lockObject.transform.SetParent(_raycastMaskTransform, false);

        lockRectTransform.anchorMin = new Vector2(1, 1);
        lockRectTransform.anchorMax = new Vector2(1, 1);
        lockRectTransform.anchoredPosition = new Vector2(-5, -5);

        UpdateLockIconSize(lockRectTransform);
    }

    private void UpdateLockIconSize(RectTransform lockRectTransform)
    {
        // Calculate the size of the lock icon relative to RaycastMask
        Vector2 raycastMaskSize = _raycastMaskTransform.rect.size;
        float iconSize = Mathf.Min(raycastMaskSize.x, raycastMaskSize.y) * 0.4f; // 40% of the smaller side
        lockRectTransform.sizeDelta = new Vector2(iconSize, iconSize);
    }

    public void Setup(InventoryTetris inventoryTetris)
    {
        this.inventoryTetris = inventoryTetris;
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        UpdateLockStatus();
    }

    private void UpdateLockStatus()
    {
        if (!_canvasGroup) return;

        if (_isLocked)
        {
            _canvasGroup.alpha = 0.7f;
            _lockIcon.gameObject.SetActive(true);
        }
        else
        {
            _canvasGroup.alpha = 1f;
            _lockIcon.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || _isLocked || !IsDraggable) return;

        _isDragging = true;
        Cursor.visible = false;

        _draggedObject = isSpawner ? inventoryTetris.PlaceItem(Origin, spawnerItemSo, Direction.Down) : this;

        if (isSpawner)
        {
            SetLocked(true);
            _draggedObject.OnCanceled += HandleSpawnedItemCanceled;
        } 
        
        CanvasGroup draggedObjectCanvasGroup = _draggedObject.GetComponent<CanvasGroup>();
        draggedObjectCanvasGroup.alpha = .7f;
        draggedObjectCanvasGroup.blocksRaycasts = false;
        
        _originalParent = _draggedObject.transform.parent;
        _draggedObject.transform.SetParent(rootCanvas.transform, true);

        CreateVisualGrid(
            _draggedObject.transform.GetChild(0),
            inventoryTetris.GetGrid().CellSize,
            _draggedObject.BaseSO.shape
        );
    
        //_tooltipManager.DisableTooltipTemporarily();
        InventoryTetrisDragDropSystem.Instance.StartedDragging(inventoryTetris, _draggedObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        RectTransform draggedRectTransform = _draggedObject.GetComponent<RectTransform>();
        draggedRectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        
        _canvasGroup.alpha = _isLocked ? 0.7f : 1f;
        _canvasGroup.blocksRaycasts = true;
            
        if (_originalParent != null)
        {
            _draggedObject.transform.SetParent(_originalParent, true);
        }
            
        InventoryTetrisDragDropSystem.Instance.StoppedDragging(inventoryTetris, _draggedObject);
        _isDragging = false;
        Cursor.visible = true;
            
        // Now tooltip displayable again
        //_tooltipManager.EnableTooltip();
    }
    
    private void HandleSpawnedItemCanceled()
    {
        SetLocked(false);
    }

    public void CreateVisualGrid(Transform visualParentTransform, float cellSize, Shape shape)
    {
        Transform visualTransform = Instantiate(InventoryTetrisAssets.Instance.gridVisual, visualParentTransform);

        // Create background
        Transform template1 = visualTransform.Find("Template");
        template1.gameObject.SetActive(false);

        Transform template2 = visualTransform.Find("Template2");
        template2.gameObject.SetActive(false);


        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (Vector2Int gridPosition in Shapes.GetShapeData(shape).GridPositions0)
        {
            minX = Mathf.Min(minX, gridPosition.x);
            minY = Mathf.Min(minY, gridPosition.y);
            maxX = Mathf.Max(maxX, gridPosition.x);
            maxY = Mathf.Max(maxY, gridPosition.y);
        }

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (Shapes.GetShapeData(shape).GridPositions0.Contains(new Vector2Int(x, y)))
                {
                    Transform backgroundSingleTransform = Instantiate(template1, visualTransform);
                    backgroundSingleTransform.gameObject.SetActive(true);
                }
                else
                {
                    Transform backgroundSingleTransform = Instantiate(template2, visualTransform);
                    backgroundSingleTransform.gameObject.SetActive(true);
                }
            }
        }

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;
        visualTransform.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerLeft;
        visualTransform.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Vertical;
        visualTransform.GetComponent<RectTransform>().sizeDelta =
            new Vector2(maxX - minX + 1, maxY - minY + 1) * cellSize;
        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        visualTransform.SetAsFirstSibling();
    }
}