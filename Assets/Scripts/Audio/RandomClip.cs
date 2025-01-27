using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Audio
{
    public class RandomClip : MonoBehaviour
    {
        [SerializeField] private Mode mode;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> _clips = new();

        [HideInInspector] public AudioSource audioSource { get => _audioSource; }
        [HideInInspector] public List<AudioClip> clips { get => _clips; }

        public enum Mode
        {
            RandomNoRepeat,
            Random,
        }

        public void Play()
        {
            AudioClip clip;
            if (mode == Mode.Random) clip = clips.Random();
            else if (mode == Mode.RandomNoRepeat) clip = clips.RandomNoRepeat(0);
            else throw new System.NotImplementedException();
            Debug.Log("Playing " + clip);
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RandomClip))]
    public class RandomClipEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Play")) (target as RandomClip).Play();
        }
    }
#endif
}
