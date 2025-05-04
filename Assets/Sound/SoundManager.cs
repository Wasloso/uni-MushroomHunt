

using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundList[] soundList;
        private static SoundManager _instance = null;
        private AudioSource audioSource;

        private void Awake()
        {
            if (_instance) return;
            _instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        

        public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
        {
            AudioClip[] clips = _instance.soundList[(int)sound].sounds;
            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            float finalVolume = _instance.soundList[(int)sound].volume * volume;

            AudioSource targetSource = source ?? _instance.audioSource;

            if (targetSource)
            {
                targetSource.outputAudioMixerGroup = _instance.soundList[(int)sound].mixer;
                targetSource.PlayOneShot(clip, finalVolume);
            }
        }
        
        public static void PlayAmbient()
        {
            if (_instance == null)
            {
                Debug.LogError("SoundManager instance is not initialized.");
                return;
            }
            int amb = (int)SoundType.Ambient;

            AudioClip[] clips = _instance.soundList[amb].sounds;
            if (clips == null || clips.Length == 0)
            {
                Debug.unityLogger.Log("No ambient sounds found");
                return;
            }

            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            _instance.audioSource.clip = clip;
            _instance.audioSource.loop = true;
            _instance.audioSource.outputAudioMixerGroup = _instance.soundList[amb].mixer;
            _instance.audioSource.volume = _instance.soundList[amb].volume;
            _instance.audioSource.Play();
            Debug.Log("Playing ambient");
        }
        
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            string[] names = Enum.GetNames(typeof(SoundType));
            Array.Resize(ref soundList, names.Length);
            for (int i = 0; i < soundList.Length; i++)
            {
                soundList[i].name = names[i];
            }
        }
#endif
    }
  
    

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public AudioClip[] sounds;
    }
}