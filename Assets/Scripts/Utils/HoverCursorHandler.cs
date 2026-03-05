using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class HoverCursorHandler : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
    {
            [SerializeField] private Texture2D hoverCursor;
            [SerializeField] private Texture2D restoreCursor;
            [SerializeField] private Vector2 hotSpot = Vector2.zero; 

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (hoverCursor != null)
                    Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                Cursor.SetCursor(restoreCursor, Vector2.zero, CursorMode.Auto); 
            }
        }
}
