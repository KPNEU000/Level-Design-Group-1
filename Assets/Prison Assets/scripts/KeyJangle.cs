using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class KeyJangle : MonoBehaviour
{
    [SerializeField]
    public List<AudioClip> keyJangles = new List<AudioClip>();
    AudioSource AudioSource;
    NavMeshAgent agent;
    public bool readyToPlay = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("PlayAudio", 0.001f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.isStopped)
        {
            AudioSource.mute = true;
        }
        else
        {
            AudioSource.mute = false;
        }
    }

    void PlayAudio()
    {
        AudioSource.PlayOneShot(keyJangles[Random.Range(0, keyJangles.Count - 1)]);
    }
}
