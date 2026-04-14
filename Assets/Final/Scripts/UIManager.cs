using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Slider healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Ins.OnHealthChanged += Refresh;
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Refresh()
    {
        healthBar.value = GameManager.Ins.CurrentHealth;
    }
}
