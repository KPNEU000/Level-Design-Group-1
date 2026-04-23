using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject sadDrawing;
    [SerializeField] private GameObject shatteredSadDrawing;
    private Animator shatteredSadDrawingAnimator;
    public Animator papersAnimator;
    public Material happyMat;


    bool isDead = false;

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
        sadDrawing.SetActive(true);
        shatteredSadDrawing.SetActive(false);
        shatteredSadDrawingAnimator = shatteredSadDrawing.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ShatterImage();
        }
        HandleRegen();
    }

    void ShatterImage()
    {
        sadDrawing.SetActive(false);
        shatteredSadDrawing.SetActive(true);
        StartCoroutine(WaitAndCut());
    }

    IEnumerator WaitAndCut()
    {
        yield return new WaitForSeconds(1f);
        shatteredSadDrawingAnimator.SetBool("startCut", true);
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
        if (!canBeHurt) return;

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
        paper.GetComponent<MeshRenderer>().material = happyMat;
        //paper.SetActive(false);

        Debug.Log(papers.IndexOf(paper) + 1 + "st paper collected");
        ReturnPaper(papers.IndexOf(paper) + 1);

        if (collectedPapers.Count >= papers.Count) AllPiecesCollected?.Invoke();
    }

    public void ReturnPaper(int paperIndex) //it's bad I know
    {
        if (paperIndex == 1)
        {
            papersAnimator.Play("Paper1Return");
        }
        else if (paperIndex == 2)
        {
            papersAnimator.Play("Paper2Return");
        }
        else if (paperIndex == 3)
        {
            papersAnimator.Play("Paper3Return");
        }
        else if (paperIndex == 4)
        {
            papersAnimator.Play("Paper4Return");
        }
        else if (paperIndex == 5)
        {
            papersAnimator.Play("Paper5Return");
        }
        else if (paperIndex == 6)
        {
            papersAnimator.Play("Paper6Return");
        }
    }

    public void AfterRespawn()
    {
        currentHealth = maxHealth;
        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;
        OnHealthChanged?.Invoke();
    }
}
