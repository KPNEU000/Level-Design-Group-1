using UnityEngine;
using System.Collections.Generic;

public class RainSplashSpawner : MonoBehaviour
{
    public ParticleSystem splashEffect;

    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem rain = GetComponent<ParticleSystem>();

        int count = rain.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < count; i++)
        {
            Instantiate(splashEffect, collisionEvents[i].intersection, Quaternion.identity);
        }
    }
}