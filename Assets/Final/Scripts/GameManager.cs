using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    [SerializeField] private List<GameObject> papers = new List<GameObject>();

    private List<GameObject> collectedPapers = new List<GameObject>();

    private int maxHealth = 100;
    private int currentHealth = 100;
    public int damageCooldown;
    public bool canBeHurt = true;
    public static GameManager Ins => _instance;
    private static GameManager _instance;

    public Action OnHealthChanged;
    public Action OnDeath;

    public Action AllPiecesCollected;


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError($"Multiple instances of GameManager in scene, destroying component on {gameObject.name}");
            Destroy(this);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("got here manager");
            AllPiecesCollected?.Invoke();
        }
    }

    public void RemoveHealth(int amt)
    {
        if(canBeHurt) {
        if (amt >= currentHealth)
        {
            currentHealth = 0;
            OnHealthChanged?.Invoke();
            OnDeath?.Invoke();

            if (collectedPapers.Count > 0)
            {
                GameObject lastPaper = collectedPapers[collectedPapers.Count - 1];
                lastPaper.SetActive(true);
                collectedPapers.Remove(lastPaper);
            }
        }
        else
        {
            currentHealth -= amt;
            OnHealthChanged?.Invoke();
            StartCoroutine("DamageCooldown");
        }
        }
        else
        {
            Debug.Log("Damage on Cooldown");
        }
    }

    IEnumerator DamageCooldown()
    {
        canBeHurt = false;
        yield return new WaitForSeconds(damageCooldown);
        canBeHurt = true;
    }

    public void CollectPaper(GameObject paper)
    {
        if (collectedPapers.Contains(paper))
        {
            Debug.Log("rgrgrgrgrg");
            return;
        }

        collectedPapers.Add(paper);
        Debug.Log("rgrgrgrgrg");
        paper.SetActive(false); //stub for a more elegant look later

        if (collectedPapers.Count >= papers.Count)
        {
            AllPiecesCollected?.Invoke();
        }
    }

    public void AfterRespawn()
    {
        currentHealth = maxHealth;

    }
}
