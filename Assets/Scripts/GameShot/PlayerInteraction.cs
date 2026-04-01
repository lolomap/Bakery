using UnityEngine;
using UnityEngine.InputSystem; // Добавляем этот неймспейс

public class PlayerInteraction : MonoBehaviour
{
    void Update()
    {
        // Проверяем нажатие левой кнопки мыши через новый Input System
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 mousePos = Pointer.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Showcase sc = hit.collider.GetComponentInParent<Showcase>();
                if (sc != null)
                {
                    BakeryUIManager.Instance.OpenShowcase(sc);
                }
            }
        }
    }
}