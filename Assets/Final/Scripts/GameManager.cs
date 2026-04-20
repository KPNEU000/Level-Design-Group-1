using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Ins { get; private set; }

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public Action OnHealthChanged;
    public Action OnDamaged;
    public Action OnDeath;
    public Action AllPiecesCollected;

    [Header("Damage")]
    public int damageCooldown;
    public bool canBeHurt = true;

    [Header("Health Regen")]
    [SerializeField] float regenDelay = 5f;
    [SerializeField] float regenRate = 5f;

    [Header("Papers")]
    [SerializeField] private List<GameObject> papers = new();

    private readonly int maxHealth = 100;
    private int currentHealth = 100;
    private readonly List<GameObject> collectedPapers = new();
    float timeSinceLastDamage;
    float regenAccumulator;

    void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Debug.LogError($"Multiple instances of GameManager in scene, destroying component on {gameObject.name}");
            Destroy(this);
            return;
        }
        Ins = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("got here manager");
            AllPiecesCollected?.Invoke();
        }
        HandleRegen();
    }

    void HandleRegen()
    {
        if (currentHealth <= 0 || currentHealth >= maxHealth) return;

        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage < regenDelay) return;

        regenAccumulator += regenRate * Time.deltaTime;
        int toRegen = Mathf.FloorToInt(regenAccumulator);

        if (toRegen < 1) return;

        currentHealth = Mathf.Min(currentHealth + toRegen, maxHealth);
        regenAccumulator -= toRegen;
        OnHealthChanged?.Invoke();
    }

    public void RemoveHealth(int amt)
    {
        if (!canBeHurt)
        {
            Debug.Log("Damage on Cooldown");
            return;
        }

        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;

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
            OnDamaged?.Invoke();
            StartCoroutine(nameof(DamageCooldown));
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
        paper.SetActive(false);

        if (collectedPapers.Count >= papers.Count) AllPiecesCollected?.Invoke();
    }

    public void AfterRespawn()
    {
        currentHealth = maxHealth;
        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;
        OnHealthChanged?.Invoke();
    }
}
