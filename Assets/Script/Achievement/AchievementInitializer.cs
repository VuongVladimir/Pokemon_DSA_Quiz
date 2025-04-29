using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lớp này tạo ra GameObject AchievementManager nếu nó chưa tồn tại
public class AchievementInitializer : MonoBehaviour
{
    [SerializeField] GameObject achievementManagerPrefab;
    
    private void Awake()
    {
        Debug.Log("AchievementInitializer Awake called");
        
        // Kiểm tra prefab
        if (achievementManagerPrefab == null)
        {
            Debug.LogError("achievementManagerPrefab is null - cannot create AchievementManager");
            return;
        }
        
        // Kiểm tra xem AchievementManager đã được tạo chưa
        if (AchievementManager.Instance == null)
        {
            Debug.Log("AchievementManager.Instance is null, creating new instance from prefab");
            
            // Tạo AchievementManager từ prefab
            Instantiate(achievementManagerPrefab);
            
            // Kiểm tra lại sau khi tạo
            if (AchievementManager.Instance != null)
            {
                Debug.Log("AchievementManager created successfully");
            }
            else
            {
                Debug.LogError("Failed to create AchievementManager - Instance still null after Instantiate");
            }
        }
        else
        {
            Debug.Log("AchievementManager.Instance already exists");
        }
    }
} 