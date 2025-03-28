using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// Scene loader behaviour.
/// SCENE LOADER FOR DEMO PURPOSES 
/// </summary>
public class SceneLoaderBehaviour : MonoBehaviour {

    public void LoadScene()
    {
        SceneManager.LoadScene("CarCrushScene");
    }

    //메인메뉴 씬 로드
    public void LoadMainMenuScene()
	{
		SceneManager.LoadScene ("MainMenuScene");
	}

    //튜토리얼 씬 로드
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    
}
