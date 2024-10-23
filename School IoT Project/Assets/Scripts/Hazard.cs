using System.Collections;
using System.Collections.Generic;
using susy_baka.Shared.Audio;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public Transform spawnPoint;
    AudioSource audioSource;

    private void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.transform.parent.position = spawnPoint.position;
            collider.transform.parent.eulerAngles = Vector3.zero;
            audioSource.Play();
        }
    }
}
