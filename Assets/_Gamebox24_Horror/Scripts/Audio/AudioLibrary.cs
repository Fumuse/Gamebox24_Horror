using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AudioLibrary : Singleton<AudioLibrary>
{
    [SerializeField] private List<AudioClipEntry> audioClips;

    /// <summary>
    /// Получить звук из библиотеки
    /// </summary>
    /// <param name="clipName"></param>
    /// <returns></returns>
    [CanBeNull]
    public AudioClip GetAudioClip(string clipName)
    {
        foreach (AudioClipEntry entry in audioClips)
        {
            if (entry.name == clipName)
            {
                return entry.clip;
            }
        }
        
        return null;
    }
}