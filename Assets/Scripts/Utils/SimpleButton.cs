using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class SimpleButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent onClick;
        private void OnMouseUp()
        { 
            onClick.Invoke();
        } 
    }
}