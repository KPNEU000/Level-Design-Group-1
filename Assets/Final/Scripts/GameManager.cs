using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { Intro, Playing, End }
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

    [Header("Transition Info")]
    [SerializeField] private GameObject bed;
    [SerializeField] private Transform bedEndingTransform;
    [SerializeField] private Transform bedStartingTransform;

    [SerializeField] private GameObject frame;
    [SerializeField] private Transform frameEndingTransform;
    [SerializeField] private Transform frameStartingTransform;
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject ghostlyApparitions;
    [SerializeField] private GameObject variousThings;
    [SerializeField] private GameObject startingArea;
    [SerializeField] private GameObject startingAreaColliders;

    private Collider terrainCol;


    public GameObject initialFog;

    public Action OnPapersGrounded;
    public Action OnGameProperStart;
    public Action RightBeforeGameProperStart;


    bool isDead = false;

    private readonly int maxHealth = 100;
    private int currentHealth = 100;
    private readonly List<GameObject> collectedPapers = new();
    float timeSinceLastDamage;
    float regenAccumulator;

    bool frameNeedsToShatter = false;

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

        bed.transform.position = bedStartingTransform.position;
        frame.transform.position = frameEndingTransform.position;
        terrainCol = terrain.GetComponent<Collider>();
        terrainCol.enabled = false;
        terrain.enabled = false;
        ghostlyApparitions.SetActive(false);
        variousThings.SetActive(false);
    }

    void Start()
    {
        Debug.Log($"terrain enabled: {terrain.enabled}, ghostly active: {ghostlyApparitions.activeSelf}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ShatterImage();
            initialFog.transform.position = Vector3.Lerp(initialFog.transform.position, Vector3.down * 10, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            foreach (GameObject paper in papers)
            {
                if (!collectedPapers.Contains(paper))
                    CollectPaper(paper);
            }
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
            return;


        collectedPapers.Add(paper);
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

    public void OnShatterEnd()
    {
        OnPapersGrounded?.Invoke();
    }

    public void Trigger1()
    {
        StartCoroutine(MoveOverTime(frame, frameStartingTransform.position, 3f));
    }

    IEnumerator MoveOverTime(GameObject obj, Vector3 targetPos, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = obj.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float smoothT = t * t * (3f - 2f * t);
            obj.transform.position = Vector3.Lerp(startingPos, targetPos, smoothT);
            yield return null;
        }
        obj.transform.position = targetPos;
        if (frameNeedsToShatter)
        {
            frameNeedsToShatter = false;
            ShatterImage();
        }
    }

    public void Trigger2()
    {
        sadDrawing.SetActive(false);
        shatteredSadDrawing.SetActive(true);
        RightBeforeGameProperStart?.Invoke();
        variousThings.SetActive(true);
        terrain.enabled = true;
        terrainCol.enabled = true;
        StartCoroutine(WaitAndMove());
    }
    IEnumerator WaitAndMove()
    {
        frameNeedsToShatter = true;
        startingAreaColliders.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MoveOverTime(frame, frameEndingTransform.position, 3f));
        StartCoroutine(MoveOverTime(bed, bedEndingTransform.position, 3f));
        ghostlyApparitions.SetActive(true);
        yield return new WaitForSeconds(2f);
        startingArea.SetActive(false);
        OnGameProperStart?.Invoke();
    }
}
