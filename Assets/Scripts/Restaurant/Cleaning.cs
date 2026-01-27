using UnityEngine;
using TMPro;
using System.Collections;

public class Cleaning : MonoBehaviour
{
    public float range = 10;

    public Animator mopAnimator;
    public AudioClip mopAudio;
    public AudioSource cameraAudioSource;

    [Header("HUD")]
    public TMP_Text clean;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        RaycastingEffect();
    }

    void RaycastingEffect()
    {
        RaycastHit objectHitByRaycast;
        if (Physics.Raycast(transform.position, transform.forward, out objectHitByRaycast, range))
        {
            if (objectHitByRaycast.collider.CompareTag("Mess"))
            {
                clean.text = "LMB to Clean";
                if(Input.GetMouseButtonDown(0))
                {
                    if(mopAnimator)
                    {
                        mopAnimator.Play("Mop");
                    }
                    if (mopAudio) {
                        cameraAudioSource.PlayOneShot(mopAudio);
                    }
                    StartCoroutine("CleanMess", objectHitByRaycast.collider.gameObject);
                }
            }
            else
            {
                clean.text = "";
            }
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }

    IEnumerator CleanMess(GameObject mess)
    {
        Debug.Log("cleaning" + mess.name);
        yield return new WaitForSeconds(1);
        Destroy(mess);
    }
}

