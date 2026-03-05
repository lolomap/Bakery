using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ItemTetrisVisual : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Color selectColor;
    public Color hoverColor;
    public Color errorColor;
    [SerializeField] private ItemTetrisSO itemData;

    public ItemTetrisSO Data => itemData;
    private ItemTetris _item;
    private bool _isHighlighted;
    private bool _isHovered;
    private bool _isError;
    private static readonly int OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");
    private static readonly int ShakeUvSpeed = Shader.PropertyToID("_ShakeUvSpeed");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    void Awake()
    {
        image.material = new Material(image.material);
        _item = GetComponent<ItemTetris>();
    }
    
    public void SetHighlight(bool isHighlighted)
    {
        _isHighlighted = isHighlighted;
        UpdateVisual();
    }

    public void SetError(bool isError)
    {
        _isError = isError;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (image.material == null) return;
        
        if (_isHovered)
            image.material.SetColor(OutlineColor, hoverColor);
        if (_isError)
            image.material.SetColor(OutlineColor, errorColor);
        if (_isHighlighted)
            image.material.SetColor(OutlineColor, selectColor);
        
        image.material.SetFloat(OutlineAlpha, _isHighlighted | _isHovered ? 1f : 0f);
        image.material.SetFloat(ShakeUvSpeed, _isHighlighted ? 3f : 0f);
    }

    public void Blink(Color color, float delay)
    {
        image.material.SetColor(OutlineColor, color);
        image.material.SetFloat(OutlineAlpha, 1f);
        image.material.SetFloat(ShakeUvSpeed, _isHighlighted ? 3f : 0f);
        
        DOTween.Sequence()
            .SetDelay(delay)
            .AppendCallback(UpdateVisual);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                break;
            case PointerEventData.InputButton.Right:
                SetHighlight(!_isHighlighted);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        UpdateVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        UpdateVisual();
    }
}