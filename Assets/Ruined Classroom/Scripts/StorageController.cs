using UnityEngine;
using TMPro;

public class StorageController : MonoBehaviour
{
    public TextMeshProUGUI storageText;
    public int curStorage = 0;
    public int maxStorage = 9;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void addStorage()
    {
        curStorage++;
        storageText.text = $"STORAGE: {curStorage}/{maxStorage}";
    }
}
