using System;
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
    public static GameManager Ins => _instance;
    private static GameManager _instance;

    public Action OnHealthChanged;
    public Action OnDeath;


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

    }

    public void RemoveHealth(int amt)
    {
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
        }
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
    }

    public void AfterRespawn()
    {
        currentHealth = maxHealth;

    }
}
