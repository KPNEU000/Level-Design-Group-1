using UnityEngine;

public class PaperShatterScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnShatterEnd()
    {
        GameManager.Ins.OnShatterEnd();
    }
}
