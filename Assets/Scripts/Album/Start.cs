using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    public void launchGame()
    {
        SceneManager.LoadScene("Main");
    }
}
