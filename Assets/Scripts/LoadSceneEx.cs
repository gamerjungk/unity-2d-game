using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneEx : MonoBehaviour
{
    public void LoadScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }
}
