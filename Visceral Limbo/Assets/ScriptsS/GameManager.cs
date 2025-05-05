using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScriptsS
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        public void PlayGame()
        {
            SceneManager.LoadScene("SampleScene");
        }
        public void Options(GameObject option)
        {
            option.SetActive(true);
        }
        public void BackMainMenu(GameObject option)
        {
            option.SetActive(false);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
