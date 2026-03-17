using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleEnterPlayer : MonoBehaviour
{

    ParticleSystem ps;
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    void OnParticleTrigger()
    {
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            if (p.remainingLifetime > p.startLifetime / 8f)
                p.remainingLifetime = p.startLifetime / 8f;


            enter[i] = p;
        }
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}
