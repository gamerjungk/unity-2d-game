using UnityEngine;
public class SceneLoaderBehaviour : MonoBehaviour
{
  public void SceneLoad(string SceneName)
    {
        LoadSceneManager.Instance.ChangeScene(SceneName);
  }

}
