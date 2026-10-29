using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lab33
{
    public sealed class CompletionScreen : MonoBehaviour
    {
        public void PlayAgain()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Lab33_UI_Popup");
        }
    }
}
