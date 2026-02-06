using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float hp = 100;
    public MeshRenderer mopBristles;
    public bool isPlayer;
    public bool isMonster;
    public bool isFurniture;
    public Animator monsterAnim;
    public GameObject goop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mopBristles)
        {
            mopBristles.material.SetColor("_BaseColor", Color.white);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        hp = hp - damage;
        if (mopBristles)
        {
            if(hp <= 75 && hp > 50)
            {
                mopBristles.material.SetColor("_BaseColor", Color.yellow);
            }
            else if (hp <= 50 && hp > 25)
            {
                mopBristles.material.SetColor("_BaseColor", Color.green);
            }
            else if (hp <= 25 && hp > 0)
            {
                mopBristles.material.SetColor("_BaseColor", Color.black);
            }
        }
        if (hp == 0) //Too many if statements, but that's okay
        {
            if (isPlayer)
            {
                PlayerDeath();
            }
            else if (isMonster)
            {
                if (monsterAnim) {
                monsterAnim.SetInteger("Anim State", 2);
                }
                MonsterDeath();
            }
            else if (isFurniture)
            {
                FurnitureDestruction();
            }
        }
    }

    private void FurnitureDestruction()
    {
        Destroy(gameObject);
        //gameObject.GetComponent<MeshFilter>().mesh = rubbleMesh;
    }

    void MonsterDeath()
    {
        Instantiate(goop, transform.position - transform.forward, Quaternion.identity);
        Destroy(gameObject);
    }

    void PlayerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
