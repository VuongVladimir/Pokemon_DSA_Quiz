using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class PlayerSaveData
{
    // Vị trí người chơi
    public float positionX;
    public float positionY;
    
    // Thông tin cơ bản của người chơi
    public int level;
    public int exp;
    public int hp;
    public int maxHp;
}

[Serializable]
public class GameSaveData
{
    public PlayerSaveData player;
    public List<string> achievements; // Danh sách id thành tựu đã mở khóa
    public Dictionary<string, bool> answeredQuestions; // Câu hỏi đã trả lời
}

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }
    private string saveFilePath;
    
    public event Action OnGameSaved;
    public event Action OnGameLoaded;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Đường dẫn lưu file
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        
        // Kiểm tra xem file save có tồn tại không, nếu không thì xóa dữ liệu achievement
        CheckSaveFileExistence();
    }
    
    // Phương thức mới để kiểm tra sự tồn tại của file save
    public void CheckSaveFileExistence()
    {
        if (!File.Exists(saveFilePath))
        {
            // Nếu file save không tồn tại, xóa tất cả dữ liệu achievement trong PlayerPrefs
            ResetAchievementData();
            Debug.Log("File save không tồn tại, đã reset dữ liệu achievement.");
        }
    }
    
    // Phương thức mới để xóa dữ liệu thành tựu trong PlayerPrefs
    public void ResetAchievementData()
    {
        // Xóa tất cả thành tựu đã lưu trong PlayerPrefs
        foreach (QuestionType type in Enum.GetValues(typeof(QuestionType)))
        {
            string key = $"Achievement_{type}";
            PlayerPrefs.DeleteKey(key);
        }
        
        // Xóa danh sách câu hỏi đã trả lời
        PlayerPrefs.DeleteKey("AnsweredQuestions");
        PlayerPrefs.Save();
        
        // Nếu AchievementManager đã được khởi tạo, cập nhật lại dữ liệu
        if (AchievementManager.Instance != null)
        {
            // Đặt lại tất cả thành tích về 0
            foreach (var achievement in AchievementManager.Instance.GetAllAchievements())
            {
                achievement.correctAnswers = 0;
            }
            
            // Reset trạng thái đã tải để đảm bảo lần sau sẽ tải mới từ PlayerPrefs
            AchievementManager.Instance.ResetLoadedState();
            
            // Sử dụng phương thức công khai thay vì trực tiếp gọi sự kiện
            AchievementManager.Instance.NotifyAchievementsUpdated();
        }
    }
    
    // Lưu trạng thái game
    public void SaveGame(Movement playerMovement)
    {
        try
        {
            var saveData = new GameSaveData
            {
                player = new PlayerSaveData
                {
                    // Lưu vị trí người chơi
                    positionX = playerMovement.transform.position.x,
                    positionY = playerMovement.transform.position.y,
                    
                    // Lưu thông tin người chơi
                    level = playerMovement.player.Level,
                    exp = playerMovement.player.Exp,
                    hp = playerMovement.player.HP,
                    maxHp = playerMovement.player.MaxHP
                },
                
                // Lưu thành tựu
                achievements = new List<string>()
            };
            
            // Lấy thành tựu từ AchievementManager
            if (AchievementManager.Instance != null)
            {
                saveData.achievements = new List<string>();
                foreach (var achievement in AchievementManager.Instance.GetAllAchievements())
                {
                    for (int i = 0; i < achievement.correctAnswers; i++)
                    {
                        saveData.achievements.Add(achievement.type.ToString());
                    }
                }
            }
            
            // Chuyển đổi dữ liệu thành JSON và lưu vào file
            string jsonData = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, jsonData);
            
            Debug.Log($"Game đã được lưu tại: {saveFilePath}");
            OnGameSaved?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi khi lưu game: {e.Message}");
        }
    }
    
    // Kiểm tra xem có bản lưu nào không
    public bool HasSaveFile()
    {
        bool exists = File.Exists(saveFilePath);
        if (!exists)
        {
            // Nếu file save không tồn tại, xóa dữ liệu achievement
            ResetAchievementData();
        }
        return exists;
    }
    
    // Tải game từ bản lưu
    public GameSaveData LoadGame()
    {
        try
        {
            if (HasSaveFile())
            {
                string jsonData = File.ReadAllText(saveFilePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
                
                OnGameLoaded?.Invoke();
                return saveData;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi khi tải game: {e.Message}");
        }
        
        return null;
    }
    
    // Áp dụng dữ liệu đã tải vào game
    public void ApplySaveData(GameSaveData saveData, Movement playerMovement)
    {
        if (saveData != null && playerMovement != null)
        {
            // Đặt vị trí người chơi
            playerMovement.transform.position = new Vector3(
                saveData.player.positionX,
                saveData.player.positionY,
                playerMovement.transform.position.z
            );
            
            // Cập nhật thông tin nhân vật
            var player = playerMovement.player;
            player.Level = saveData.player.level;
            player.Exp = saveData.player.exp;
            player.HP = saveData.player.hp;
            
            // QUAN TRỌNG: KHÔNG khôi phục thành tựu từ file save
            // Thay vào đó, tải thành tựu từ PlayerPrefs để tránh ghi đè
            if (AchievementManager.Instance != null)
            {
                // Chỉ tải từ PlayerPrefs (achievements đã được lưu vào PlayerPrefs trước đó)
                AchievementManager.Instance.LoadAchievements();
                
                // Thông báo thành tựu đã được cập nhật
                AchievementManager.Instance.NotifyAchievementsUpdated();
                
                Debug.Log("Đã tải thành tựu từ PlayerPrefs thay vì từ file save");
            }
        }
    }
} 