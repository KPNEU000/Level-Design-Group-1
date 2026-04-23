using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    public SoldierController soldier;
    public int damage;

    float timer = 0.1f;
    bool isActive = false;
    Collider playerCollider = null;

    void OnEnable()
    {
        timer = 0.1f;
        isActive = false;
        playerCollider = null;  // ← this is the critical one
        damage = soldier != null ? soldier.damage : damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCollider = other;
            isActive = true;
        }
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            isActive = false;
            TryDealDamage();
        }
    }

    void TryDealDamage()
    {
        if (playerCollider == null) return;

        bool isDaughter = soldier.name.Contains("Daughter");

        Vector3 origin = soldier.transform.position;
        Vector3 target = playerCollider.transform.position;
        Vector3 direction = target - origin;
        float distanceToPlayer = direction.magnitude;

        LayerMask mask = LayerMask.GetMask("Blocker");

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distanceToPlayer, mask))
        {
            if (isDaughter) Debug.Log($"[DaughterTrench] BLOCKED by {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            return;
        }

        if (isDaughter) Debug.Log($"[DaughterTrench] HIT LANDED - calling RemoveHealth({damage})");
        GameManager.Ins.RemoveHealth(damage);
    }
}