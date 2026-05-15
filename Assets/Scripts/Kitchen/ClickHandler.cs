using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public GameObject menuToOpen;
    public GameObject globalBackground;

    void OnMouseDown()
    {
        if (menuToOpen != null) 
        {  
            menuToOpen.SetActive(true);
            Debug.Log("Open the menu for " + gameObject.name);

            if (globalBackground != null) 
            {
                globalBackground.SetActive(true);
            }
        }
        
    }

}
