using UnityEngine;
using UnityEngine.Audio;

namespace ScriptsS
{
    public class M_Options : MonoBehaviour
    {
        [SerializeField] AudioMixer audioMixer;
        public void FullScreenOption(bool FullScreen)
        {
            Screen.fullScreen = FullScreen;
        }
        public void ChangeVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }
        public void Quality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }

        public void SetSensibility(float value)
        {
            //mouseSensitivity = value;
        }
    }
}
