using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager 
{
    // bgm용 + effct용 사운드
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // MP3 Player       -> AudioSource
    // MP3 음원         -> AudioClip
    // 관객(귀)         -> AudioLisenear

    public void Init()
    {
        // 빈 게임오브젝트를 생성한다.
        GameObject root = GameObject.Find("@Sound");

        // 해당 게임오브젝트가 만들어지지 않았다면
        if(root == null)
        {
            // 해당 게임오브젝트를 만든다.
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            // C#리플렉션을 이용하여 사운드에 있는 정보를 추출한다.
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            
            for(int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };    // 새로운 게임오브젝트를 만든다.
                _audioSources[i] = go.AddComponent<AudioSource>();  // AudioSource컴포넌트를 추가한다.
                go.transform.parent = root.transform;   // root밑에다가 새로 생성한 트렌스폼을 추가한다.
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    // 기존의 오디오클립들을 없앤다. 
    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }

        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect,  float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)  // 오디오클립이 없다면 종료한다.
            return;

        // bgm일 떄
        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];

            // 다른 곡을 틀고 있다면
            if (audioSource.isPlaying)
                audioSource.Stop(); // 이전 곡을 멈춘다.

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {   // effect일 때
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        // bgm일 떄
        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {   // effect일 때

            // 기존 오디오클립에 해당하는 오디오클립이 없다면
            if (_audioClips.TryGetValue(path, out audioClip) == false)  // 있다면 out으로 인하여 audioClip변수에 저장된다.
            {
                audioClip = Managers.Resource.Load<AudioClip>(path); // 새로운 오디오클립을 받아온다.
                _audioClips.Add(path, audioClip);   // 기존의 오디오클립에 추가한다.
            }
        }
        if (audioClip == null)
        {
            Debug.Log($"AudioClip Missing ! {path}");
        }


        return audioClip;
    }
}
