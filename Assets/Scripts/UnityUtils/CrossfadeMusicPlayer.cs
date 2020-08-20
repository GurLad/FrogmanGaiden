﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrossfadeMusicPlayerObject
{
    public string Name;
    public AudioClip AudioClip;
}

public class CrossfadeMusicPlayer : MonoBehaviour
{
    public static CrossfadeMusicPlayer Current;
    public List<CrossfadeMusicPlayerObject> Tracks;
    public float FadeSpeed;
    [Range(0,1)]
    public float Volume = 1;
    public bool PlayOnStart;
    public bool KeepTimestamp;
    public string Playing;
    private AudioSource mainAudioSource;
    private AudioSource seconderyAudioSource;
    private float count;
    private void Awake()
    {
        if (Current != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            Current = this;
        }
        DontDestroyOnLoad(gameObject);
        mainAudioSource = gameObject.AddComponent<AudioSource>();
        seconderyAudioSource = gameObject.AddComponent<AudioSource>();
        mainAudioSource.loop = seconderyAudioSource.loop = true;
        mainAudioSource.volume = Volume;
        seconderyAudioSource.volume = 0;
        if (PlayOnStart)
        {
            mainAudioSource.clip = Tracks[0].AudioClip;
            mainAudioSource.Play();
            Playing = Tracks[0].Name;
        }
    }
    public void Play(string name, bool? keepTimestamp = null)
    {
        CrossfadeMusicPlayerObject target = Tracks.Find(a => a.Name == name);
        if (target == null)
        {
            throw new System.Exception("No matching audio clip! (" + name + ")");
        }
        if (mainAudioSource.clip == target.AudioClip)
        {
            return;
        }
        seconderyAudioSource.clip = target.AudioClip;
        Playing = name;
        mainAudioSource.volume = Volume;
        seconderyAudioSource.volume = 0;
        seconderyAudioSource.Play();
        count = 0;
        if (keepTimestamp ?? KeepTimestamp)
        {
            seconderyAudioSource.time = mainAudioSource.time * (seconderyAudioSource.clip.length / mainAudioSource.clip.length);
        }
        else
        {
            seconderyAudioSource.time = 0;
        }
    }
    public void SwitchBattleMode(bool on)
    {
        if (on)
        {
            Play(Playing + "Battle");
        }
        else
        {
            Play(Playing.Replace("Battle", ""));
        }
    }
    private void Update()
    {
        if (seconderyAudioSource.clip != null)
        {
            count += Time.unscaledDeltaTime * FadeSpeed;
            if (count >= 1)
            {
                AudioSource temp = mainAudioSource;
                mainAudioSource = seconderyAudioSource;
                seconderyAudioSource = temp;
                mainAudioSource.volume = Volume;
                seconderyAudioSource.volume = 0;
                seconderyAudioSource.clip = null;
                seconderyAudioSource.Stop();
                count = 0;
            }
            else
            {
                mainAudioSource.volume = Volume * (1 - count);
                seconderyAudioSource.volume = Volume * count;
            }
        }
    }
}
