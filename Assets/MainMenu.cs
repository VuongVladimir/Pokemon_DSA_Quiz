using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] AchievementUI achievementUI;
    [SerializeField] GameObject saveConfirmationPanel;
    [SerializeField] GameObject savedMessagePanel;
    
    private void Start()
    {
        Debug.Log("MainMenu Start called");
        
        // Ẩn các panel thông báo khi bắt đầu
        if (saveConfirmationPanel != null)
            saveConfirmationPanel.SetActive(false);
        
        if (savedMessagePanel != null)
            savedMessagePanel.SetActive(false);
        
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
    
    // Hiển thị hộp thoại xác nhận lưu game
    public void ShowSaveConfirmation()
    {
        Debug.Log("ShowSaveConfirmation called");
        if (saveConfirmationPanel != null)
        {
            saveConfirmationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("saveConfirmationPanel is null, cannot show save confirmation");
        }
    }
    
    // Xử lý khi người chơi nhấn nút YES để lưu game
    public void ConfirmSave()
    {
        Debug.Log("ConfirmSave called");
        
        // Ẩn panel xác nhận
        if (saveConfirmationPanel != null)
            saveConfirmationPanel.SetActive(false);
        
        // Tìm đối tượng người chơi
        var playerMovement = FindObjectOfType<Movement>();
        
        if (playerMovement != null && GameSaveManager.Instance != null)
        {
            // Lưu game
            GameSaveManager.Instance.SaveGame(playerMovement);
            
            // Hiển thị thông báo đã lưu
            StartCoroutine(ShowSavedMessage());
        }
        else
        {
            Debug.LogError("Cannot save game: playerMovement or GameSaveManager.Instance is null");
        }
    }
    
    // Xử lý khi người chơi nhấn nút NO để hủy lưu game
    public void CancelSave()
    {
        Debug.Log("CancelSave called");
        if (saveConfirmationPanel != null)
            saveConfirmationPanel.SetActive(false);
    }
    
    // Hiển thị thông báo đã lưu trong vài giây
    private IEnumerator ShowSavedMessage()
    {
        if (savedMessagePanel != null)
        {
            savedMessagePanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            savedMessagePanel.SetActive(false);
        }
    }
}
