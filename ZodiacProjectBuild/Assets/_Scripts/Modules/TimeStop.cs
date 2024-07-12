using System.Collections;
using Cinemachine;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
    [SerializeField] GameObject ImpactEffect;

    CinemachineImpulseSource impulseSource;
    Animator animator;

    float speed;
    bool restoreTime;

    private void Start()
    {
        restoreTime = false;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * speed;
            }
            else
            {
                Time.timeScale = 1f;
                restoreTime = false;
                //animator.SetBool("Damaged", false);
            }
        }    
    }

    public void StopTime
        (
            float changeTime,
            int restoreSpeed,
            float delay
        )
    {
        speed = restoreSpeed;

        if (delay > 0)
        {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }
        else
            restoreTime = true;

        CameraShakeManager.instance.CameraShake(impulseSource);
        Instantiate(ImpactEffect, transform.position, Quaternion.identity);
        //animator.SetBool("Damaged", true);
        
        Time.timeScale = changeTime;
    }

    IEnumerator StartTimeAgain(float amt)
    {
        restoreTime = true;
        yield return new WaitForSecondsRealtime(amt);
    }
}
