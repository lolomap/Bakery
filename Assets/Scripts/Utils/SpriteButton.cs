using UnityEngine;
using UnityEngine.Events;

public class SpriteButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick; 
    [SerializeField] private Color hoverColor = Color.gray; 
    [SerializeField] private Color clickColor = Color.white;
    [SerializeField] private float hoverScale = 1.1f;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Vector3 _originalScale;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
            return;
        }

        _originalColor = _spriteRenderer.color;
        _originalScale = transform.localScale;

        // Автоматически создаем PolygonCollider2D, если его нет
        if (GetComponent<PolygonCollider2D>() == null)
        {
            gameObject.AddComponent<PolygonCollider2D>();
        }
    }

    private void OnMouseEnter()
    {
        _spriteRenderer.color = hoverColor;
        transform.localScale = _originalScale * hoverScale;
    }

    private void OnMouseExit()
    {
        _spriteRenderer.color = _originalColor;
        transform.localScale = _originalScale;
    }

    private void OnMouseDown()
    {
        _spriteRenderer.color = clickColor;
    }

    private void OnMouseUp()
    {
 
        onClick.Invoke();
        _spriteRenderer.color = hoverColor;
    }
} 