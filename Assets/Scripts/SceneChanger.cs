using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    public string sceneName;

    public void OnClickChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneLoader.Instance.LoadSceneWithFade(sceneName);
        }
    }
}
