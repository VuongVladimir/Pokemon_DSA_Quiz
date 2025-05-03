using UnityEngine;

public class GameSaveManagerInitializer : MonoBehaviour
{
    [SerializeField] GameObject gameSaveManagerPrefab;
    
    private void Awake()
    {
        // Kiểm tra xem GameSaveManager đã tồn tại chưa
        if (GameSaveManager.Instance == null && gameSaveManagerPrefab != null)
        {
            // Tạo GameSaveManager từ prefab
            Instantiate(gameSaveManagerPrefab);
            Debug.Log("GameSaveManager được khởi tạo");
        }
    }
} 