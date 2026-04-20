using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Slider healthBar;
    [SerializeField] private KeyCode healthBarToggleKey = KeyCode.H;

    void Start()
    {
        GameManager.Ins.OnHealthChanged += Refresh;
        healthBar.gameObject.SetActive(false);
        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(healthBarToggleKey))
            healthBar.gameObject.SetActive(!healthBar.gameObject.activeSelf);
    }

    void Refresh()
    {
        healthBar.value = GameManager.Ins.CurrentHealth;
    }
}
