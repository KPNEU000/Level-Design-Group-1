using TMPro;
using UnityEngine;

public class OxygenController : MonoBehaviour
{
    public TextMeshProUGUI oxygenText;
    public float oxygen = 100f;
    float accumulator = 0f;
    float updateInterval = 5f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        accumulator += Time.deltaTime;

        if (accumulator >= updateInterval)
        {
            ChangeOxygen(-1);
            accumulator -= updateInterval;
        }
    }

    public void ChangeOxygen(int amount)
    {
        oxygen += amount;
        oxygen = Mathf.Clamp(oxygen, 0, 100);
        UpdateUI();
    }

    void UpdateUI()
    {
        oxygenText.text = $"OXYGEN: {oxygen}%";
    }
}
