using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lớp này quản lý sự khởi tạo của game và các hệ thống chung giữa các scene
/// Đặt lớp này ở scene đầu tiên (Main Menu)
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameObject achievementManagerPrefab;
    
    private static bool isInitialized = false;
    
    private void Awake()
    {
        if (isInitialized)
        {
            // Đã khởi tạo trong session trước đó, không cần tạo lại
            Destroy(gameObject);
            return;
        }
        
        // Đánh dấu là đã khởi tạo
        isInitialized = true;
        
        // Giữ đối tượng này khi chuyển scene
        DontDestroyOnLoad(gameObject);
        
        // Đăng ký sự kiện khi chuyển scene
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Khởi tạo AchievementManager nếu chưa có
        InitializeAchievementManager();
        
        Debug.Log("GameInitializer: Khởi tạo hệ thống game thành công");
    }
    
    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi đối tượng bị hủy
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void InitializeAchievementManager()
    {
        // Kiểm tra xem AchievementManager đã tồn tại chưa
        if (AchievementManager.Instance == null && achievementManagerPrefab != null)
        {
            // Tạo mới nếu chưa có
            Instantiate(achievementManagerPrefab);
            Debug.Log("GameInitializer: Đã tạo AchievementManager mới");
        }
        else
        {
            Debug.Log("GameInitializer: AchievementManager đã tồn tại hoặc prefab chưa được thiết lập");
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameInitializer: Đã chuyển đến scene {scene.name}");
        
        // Đảm bảo các hệ thống quan trọng đã được khởi tạo
        EnsureSystemsInitialized();
    }
    
    private void EnsureSystemsInitialized()
    {
        // Kiểm tra AchievementManager
        if (AchievementManager.Instance == null)
        {
            Debug.LogWarning("GameInitializer: AchievementManager chưa được khởi tạo sau khi chuyển scene");
            InitializeAchievementManager();
        }
        else
        {
            // AchievementManager đã tồn tại
            // Reset trạng thái đã tải để đảm bảo đọc từ PlayerPrefs khi cần
            AchievementManager.Instance.ResetLoadedState();
            Debug.Log("GameInitializer: Đã reset trạng thái tải của AchievementManager sau khi chuyển scene");
        }
    }
    
    // Gọi phương thức này để reset toàn bộ game
    public void ResetGame()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.ResetAchievementData();
        }
        
        if (AchievementManager.Instance != null)
        {
            // Reset trạng thái load để đảm bảo tải lại từ đầu
            AchievementManager.Instance.ResetLoadedState();
            // Tải lại thành tựu từ PlayerPrefs (bây giờ đã trống)
            AchievementManager.Instance.LoadAchievements();
        }
        
        Debug.Log("GameInitializer: Đã reset game");
    }
} 