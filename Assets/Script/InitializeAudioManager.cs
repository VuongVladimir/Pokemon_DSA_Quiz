using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeAudioManager : MonoBehaviour
{
    public GameObject audioManagerPrefab;

    void Awake()
    {
        // Kiểm tra xem đã có AudioManager chưa
        if (AudioManager.instance == null)
        {
            // Nếu chưa có, tạo mới từ prefab
            if (audioManagerPrefab != null)
            {
                Instantiate(audioManagerPrefab);
            }
            else
            {
                Debug.LogError("AudioManager prefab không được gán! Vui lòng gán prefab vào Inspector.");
            }
        }
    }
} 