using UnityEngine;
using System.Collections.Generic;

public class RainSplashSpawner : MonoBehaviour
{
    public ParticleSystem splashEffect;

    private List<ParticleCollisionEvent> collisionEvents;
    ParticleSystem.EmissionModule emission;

    void Awake()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        emission = splashEffect.emission;
        emission.enabled = false;
        GameManager.Ins.OnGameProperStart += StartRain;
    }

    void OnDisable()
    {
        GameManager.Ins.OnGameProperStart -= StartRain;
    }

    void StartRain()
    {
        emission.enabled = true;
    }

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem rain = GetComponent<ParticleSystem>();

        int count = rain.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < count; i++)
        {
            Destroy(
                Instantiate(splashEffect, collisionEvents[i].intersection, Quaternion.identity).gameObject,
                2f
            );
        }
    }
}