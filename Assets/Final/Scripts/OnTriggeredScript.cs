using System;
using Unity.VisualScripting;
using UnityEngine;

public class OnTriggeredScript : MonoBehaviour
{
    public L1LevelManager lm;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lm.OnTrigger(this.name);
        }
    }
}
