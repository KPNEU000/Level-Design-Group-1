using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    List<Collider> enteredColliders = new List<Collider>();
    public SoldierController soldier;
    public int damage;
    bool isUpdating = false;
    float timer = .1f;
    bool firstTimeUpdate = false;

    bool hasBlocker = false;
    bool hasPlayer = false;
    Collider foundPlayerCollider;
    void Start()
    {
        //placeholder
        damage = soldier.damage;
    }

    void OnEnable()
    {
        List<Collider> enteredColliders = new List<Collider>();
        firstTimeUpdate = false;
        timer = .1f;
    }

    void Update()
    {
        if (!isUpdating)
            return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            isUpdating = false;
            ExecuteOnColliders();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (!firstTimeUpdate)
        {
            isUpdating = true;
            firstTimeUpdate = true;
        }

        damage = soldier.damage;
        if (other.CompareTag("Player"))
        {
            hasPlayer = true;
            foundPlayerCollider = other;
            enteredColliders.Add(other);
        }

        if (other.CompareTag("Blocking"))
        {
            hasBlocker = true;
            enteredColliders.Add(other);
        }
    }

    void ExecuteOnColliders()
    {
        Debug.Log(enteredColliders.Count);

        if (hasPlayer && hasBlocker)
        {
            Debug.Log("has both");
        }


        if (enteredColliders.Count == 0)
        {
            return;
        }

        if (!hasPlayer)
            return;

        if (!hasBlocker)
        {
            Debug.Log("successful shot, no collider detected");
            GameManager.Ins.RemoveHealth(damage);
        }

        float nearestCollider = 999f;

        foreach (Collider col in enteredColliders)
        {
            float distance = Vector3.Distance(soldier.transform.position, col.transform.position);
            if (col.CompareTag("Blocker"))
            {
                if (distance < nearestCollider)
                    nearestCollider = distance;
            }
        }

        if (nearestCollider > Vector3.Distance(soldier.transform.position,
        foundPlayerCollider.transform.position))
        {
            Debug.Log("successful shot; player was in front of collider");
            GameManager.Ins.RemoveHealth(damage);
        }
        else
        {
            Debug.Log("collider blocked damage");
        }
    }
}
