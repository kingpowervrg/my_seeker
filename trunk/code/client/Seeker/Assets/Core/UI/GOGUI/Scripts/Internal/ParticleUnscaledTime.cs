using System;
using UnityEngine;
using System.Collections;

public class ParticleUnscaledTime : MonoBehaviour
{

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start()
    {
        ReStrart();
    }

    public void ReStrart()
    {
        isPlaying = true;
        particle.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (particle.loop)
        {
            particle.Simulate(Time.unscaledDeltaTime, true, false);
        }
        else if (isPlaying)
        {
            timePlaying += Time.unscaledDeltaTime;
            particle.Simulate(Time.unscaledDeltaTime, true, false);

            if (timePlaying >= particle.duration)
            {
                isPlaying = false;
                timePlaying = 0;
                particle.Simulate(0.0f, true, true); 
            }
        }
    }
    private bool isPlaying = false;
    private float timePlaying = 0;
    private ParticleSystem particle;

}