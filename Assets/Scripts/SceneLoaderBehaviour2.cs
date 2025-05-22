using UnityEngine;

public class SceneLoaderBehaviour2 : MonoBehaviour
{
    public void SceneLoad(string SceneName)
    {
        LoadSceneManager.Instance.ChangeScene(SceneName);
    }

}
