using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public float range = 100;

    public AudioClip mopShootAudio;
    public AudioSource cameraAudioSource;
    public GameObject mopMuzzleFire;
    public Image crosshair;
    public int damage = 20;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
            if (objectHitByRaycast.collider.CompareTag("Enemy"))
            {
                crosshair.Color = Color.Red;
            }
            else
            {
                crosshair.Color = Color.Black;
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
        yield return new WaitForSeconds(0.2f);
        mopMuzzleFire.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.Blue);
    }
}
