using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    private AudioSource audioSource;
    
    private Vector3 initialPosition;
    private int currentTick = 0;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialPosition = transform.position;
    }
    
    public void Tick()
    {
        audioSource.Play();
        currentTick++;
        float offset = currentTick % 4;
        transform.position = initialPosition + new Vector3(0, 0, offset);
    }

    public void StartMetronome(float currentBpm, float delay)
    {
        float tickTime = 60f / currentBpm;
        InvokeRepeating("Tick", delay, tickTime);
    }
}
