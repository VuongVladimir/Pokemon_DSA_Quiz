using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMusicController : MonoBehaviour
{
    // Hàm này sẽ được gọi khi bắt đầu trận chiến
    public void StartBattle()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBattleMusic();
        }
    }

    // Hàm này sẽ được gọi khi kết thúc trận chiến
    public void EndBattle()
    {
        if (AudioManager.instance != null)
        {
            // Quay lại nhạc gameplay
            AudioManager.instance.PlayGameplayMusic();
        }
    }
} 