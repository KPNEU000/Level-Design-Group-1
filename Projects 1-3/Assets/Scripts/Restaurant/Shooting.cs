using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    
    public float range = 100;

    public AudioClip mopShootAudio;
    public AudioSource cameraAudioSource;
    public GameObject mopMuzzleFire;
    public ParticleSystem muzzleFlash;
    public UnityEngine.UI.Image crosshair;
    public int damage = 20;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(JanitorMovement.isAttackMode) {
        RaycastingEffect();
        }
    }

    void RaycastingEffect()
    {
        RaycastHit objectHitByRaycast;
        if (Physics.Raycast(transform.position, transform.forward, out objectHitByRaycast, range))
        {
            if (objectHitByRaycast.collider.CompareTag("Enemy"))
            {
                crosshair.color = new Color32(255,0,0,100);
            }
            else
            {
                crosshair.color = new Color32(0,0,0,0);
            }
            if(Input.GetMouseButtonDown(0))
            {
                cameraAudioSource.PlayOneShot(mopShootAudio);
                StartCoroutine("MopFire");
                if (objectHitByRaycast.collider.CompareTag("Enemy"))
                {
                    GetComponent<Collider>().GetComponent<Health>().TakeDamage(damage);
                }
            }
        }
    }

    IEnumerator MopFire()
    {
        mopMuzzleFire.SetActive(true);
        muzzleFlash.Emit(20);
        yield return new WaitForSeconds(0.2f);
        mopMuzzleFire.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, new Color32(0,0,255,100));
    }
    
}
