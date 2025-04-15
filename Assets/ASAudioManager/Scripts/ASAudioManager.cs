using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//音声設定: コア

public static class ASAudioManager
{
    public enum AudioType
    {
        SE,
        BGM
    }

    public delegate void VolumeDelegate(float volume);

    static float masterVolume = 1;
    static float seVolume = 1;
    static float bgmVolume = 1;

    static VolumeDelegate seVolumeDelegate;
    static VolumeDelegate bgmVolumeDelegate;

    public static void GetVolumes(out float master, out float se, out float bgm)
    {
        master = masterVolume;
        se = seVolume;
        bgm = bgmVolume;
    }

    public static void SetVolumes(float master, float se, float bgm)
    {
        bool sechanged = false;
        bool bgmchanged = false;

        if(!masterVolume.Equals(master))
        {
            masterVolume = master;
            sechanged = true;
            bgmchanged = true;
        }
        if(!seVolume.Equals(se))
        {
            seVolume = se;
            sechanged = true;
        }
        if(!bgmVolume.Equals(bgm))
        {
            bgmVolume = bgm;
            bgmchanged = true;
        }

        if(sechanged)
        {
            seVolumeDelegate?.Invoke(seVolume * masterVolume);
        }
        if(bgmchanged)
        {
            bgmVolumeDelegate?.Invoke(bgmVolume * masterVolume);
        }
    }

    public static void Add(AudioType type, VolumeDelegate vol)
    {
        switch(type)
        {
            case AudioType.SE:
                seVolumeDelegate += vol;
                vol(seVolume * masterVolume);
            break;
            case AudioType.BGM:
                bgmVolumeDelegate += vol;
                vol(bgmVolume * masterVolume);
            break;
        }
    }

    public static void Remove(AudioType type, VolumeDelegate vol)
    {
        switch(type)
        {
            case AudioType.SE:
                seVolumeDelegate -= vol;
            break;
            case AudioType.BGM:
                bgmVolumeDelegate -= vol;
            break;
        }
    }
}
