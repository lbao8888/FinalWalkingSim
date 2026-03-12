using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}