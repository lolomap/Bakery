using UnityEngine;

public class BackgroundCloseHandler : MonoBehaviour
{
    public GameObject[] menusToClose;
    public GameObject background;

    private float lastOpenTime = -999f;
    private const float COOLDOWN = 0.2f; // 200 мс защиты

    void OnEnable()
    {
        lastOpenTime = Time.time;
        Debug.Log("Фон включился в момент: : " + lastOpenTime);
    }

    public void CloseAll()
    {
        // Если прошло меньше COOLDOWN секунд после включения — игнорируем клик
        if (Time.time - lastOpenTime < COOLDOWN)
        {
            Debug.Log("Клик проигнорирован (слишком рано)");
            return;
        }

        Debug.Log("Закрываю всё по клику");
        foreach (GameObject menu in menusToClose)
        {
            menu.SetActive(false);
        }
        background.SetActive(false);
    }
}