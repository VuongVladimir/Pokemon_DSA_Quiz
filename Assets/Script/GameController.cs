using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { FreeRoam, Battle, Dialog}
public class GameController : MonoBehaviour
{
    GameState state;
    [SerializeField] Movement playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject DeathScene;

    private void Start()
    {
        playerController.onEncountered += StartBattle;
        battleSystem.onBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
        
        // Sử dụng phương thức mới để xử lý khi game bắt đầu
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.HandleGameStart(playerController);
        }
        else
        {
            Debug.LogError("GameSaveManager.Instance là null trong GameController.Start()");
        }
    }
    
    // Phương thức mới để đảm bảo các thành tựu đồng bộ với file save
    private void EnsureAchievementSyncWithSaveFile()
    {
        if (GameSaveManager.Instance != null)
        {
            // Gọi trực tiếp phương thức kiểm tra
            GameSaveManager.Instance.CheckSaveFileExistence();
        }
    }
    
    // NÊN XÓA, không sử dụng nữa vì đã được thay thế bởi HandleGameStart
    private void LoadGameIfSaveExists()
    {
        // Phương thức này không được sử dụng nữa
        // Giữ lại để tránh lỗi nếu có mã khác tham chiếu đến nó
        Debug.Log("LoadGameIfSaveExists đã không còn được sử dụng, thay vào đó dùng HandleGameStart");
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        
        // Đảm bảo cập nhật HP đầy đủ cho Player sau trận đấu nếu thắng
        if (won)
        {
            // HP đã được cập nhật trong BattleSystem, nhưng chúng ta đảm bảo nó cũng được đồng bộ ở đây
            playerController.player.HealFull();
        }
        
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    void StartBattle(MonsterBase Enemy, Monster Player, Collider2D Collision)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        battleSystem.StartBattle(Enemy, Player, Collision);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            if (playerController.player.HP == 0)
            {
                DeathScene.SetActive(true);
                
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    // Tải lại game từ bản lưu nếu có, ngược lại reset scene
                    if (GameSaveManager.Instance != null && GameSaveManager.Instance.HasSaveFile())
                    {
                        DeathScene.SetActive(false);
                        var saveData = GameSaveManager.Instance.LoadGame();
                        if (saveData != null)
                        {
                            GameSaveManager.Instance.ApplySaveData(saveData, playerController);
                            Debug.Log("Đã tải lại game từ bản lưu sau khi die");
                        }
                    }
                    else
                    {
                        // Không có bản lưu, reset toàn bộ game
                        if (playerController.player != null)
                        {
                            // Reset tất cả bonus stats về 0 trước khi tải lại scene
                            playerController.player.ResetBonusStats();
                            Debug.Log("Reset tất cả bonus stats về 0 trước khi tải lại scene");
                        }
                        
                        SceneManager.LoadScene("SampleScene");
                    }
                }
            }
            else
            {
                playerController.HandleUpdate();
            }
        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
