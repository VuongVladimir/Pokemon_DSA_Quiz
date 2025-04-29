using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] AchievementUI achievementUI;
    
    private void Start()
    {
        Debug.Log("MainMenu Start called");
        
        // Kiểm tra tham chiếu tới AchievementUI
        if (achievementUI == null)
        {
            Debug.LogError("achievementUI is null in MainMenu - achievement button won't work");
        }
        else
        {
            Debug.Log("achievementUI reference is valid");
        }
        
        // Kiểm tra AchievementManager
        if (AchievementManager.Instance == null)
        {
            Debug.LogError("AchievementManager.Instance is null in MainMenu Start");
        }
        else
        {
            Debug.Log("AchievementManager.Instance is valid in MainMenu Start");
        }
    }
    
    public void PlayGame(){
        Debug.Log("PlayGame called");
        SceneManager.LoadSceneAsync(1);
    }
    
    public void ExitGame(){
        Debug.Log("ExitGame called");
        Application.Quit();
    }
    
    public void ShowAchievements(){
        Debug.Log("ShowAchievements called");
        if (achievementUI != null)
        {
            Debug.Log("Calling achievementUI.ShowPanel()");
            achievementUI.ShowPanel();
        }
        else
        {
            Debug.LogError("Cannot show achievements - achievementUI is null");
        }
    }
}
