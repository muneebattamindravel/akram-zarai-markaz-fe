using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MRSound
{
    public List<AudioClip> clips;
    public SoundType type;
    [Range(0, 1)]
    public float volume = 1;
    [Range(0, 3)]
    public float pitch = 1;
}

[System.Serializable]
public enum SoundType
{
    BUTTON_CLICK = 1,
};
