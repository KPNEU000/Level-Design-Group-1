using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    public SoldierController soldier;
    public int damage;
    void Start()
    {
        //placeholder
        damage = soldier.damage;
    }
    void OnTriggerEnter(Collider other)
    {
        damage = soldier.damage;
        Debug.Log("bullet hit");
        if(other.CompareTag("Player"))
        {
            Debug.Log("bullet hit player");
            GameManager.Ins.RemoveHealth(damage);
        }
    }
}
