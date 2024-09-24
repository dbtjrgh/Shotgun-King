using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CProjectile : MonoBehaviour
{
    [SerializeField]
    protected float speed = 15f;

    [SerializeField]
    protected float hitOffset = 0f;

    [SerializeField]
    protected bool UseFirePointRotation;

    [SerializeField]
    protected Vector3 rotationOffset = new Vector3(0, 0, 0);

    [SerializeField]
    protected GameObject hit;

    [SerializeField]
    protected ParticleSystem hitPS;

    [SerializeField]
    protected GameObject flash;

    [SerializeField]
    protected Rigidbody rb;

    [SerializeField]
    protected Collider col;

    [SerializeField]
    protected Light lightSourse;

    [SerializeField]
    protected GameObject[] Detached;

    [SerializeField]
    protected ParticleSystem projectilePS;

    private bool startChecker = false;

    [SerializeField]
    protected bool notDestroy = false;

    public float maxDistance; // 총알이 이동할 최대 거리

    private Vector3 startPosition; // 총알의 시작 위치

    private void Start()
    {
        // 시작 위치 저장
        startPosition = transform.position;

        if (!startChecker)
        {
            if (flash != null)
            {
                flash.transform.parent = null;
            }
        }
        startChecker = true;
    }

    private void Update()
    {
        if (speed != 0)
        {
            rb.velocity = transform.forward * speed;
        }

        // 총알이 시작 위치에서 maxDistance 만큼 이동했는지 확인
        float traveledDistance = Vector3.Distance(startPosition, transform.position);
        if (traveledDistance >= maxDistance)
        {
            DestroyProjectile();
        }
    }

    // 충돌 발생 시 호출되는 함수
    protected virtual void OnCollisionEnter(Collision collision)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (lightSourse != null)
            lightSourse.enabled = false;
        col.enabled = false;
        projectilePS.Stop();
        projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            hit.transform.rotation = rot;
            hit.transform.position = pos;
            if (UseFirePointRotation) { hit.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hit.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hit.transform.LookAt(contact.point + contact.normal); }
            hitPS.Play();
        }

        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                ParticleSystem detachedPS = detachedPrefab.GetComponent<ParticleSystem>();
                detachedPS.Stop();
            }
        }
        if (notDestroy)
            StartCoroutine(DisableTimer(hitPS.main.duration));
        else
        {
            if (hitPS != null)
            {
                Destroy(gameObject, hitPS.main.duration);
            }
            else
                Destroy(gameObject, 1);
        }
    }

    // 총알을 제거하는 함수
    private void DestroyProjectile()
    {
        if (notDestroy)
        {
            StartCoroutine(DisableTimer(hitPS.main.duration));
        }
        else
        {
            if (hitPS != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator DisableTimer(float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        yield break;
    }
}
