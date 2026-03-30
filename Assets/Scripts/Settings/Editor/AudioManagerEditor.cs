using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioManager audioManager = (AudioManager)target;

        if (GUILayout.Button("Play Music"))
        {
            audioManager.TestMusicClip();
        }
        if (GUILayout.Button("Play SFX"))
        {
            audioManager.TestSFXClip();
        }
        if (GUILayout.Button("Play Voice"))
        {
            audioManager.TestVoiceClip();
        }
        if (GUILayout.Button("Stop All"))
        {
            audioManager.TestStopClips();
        }
    }
}