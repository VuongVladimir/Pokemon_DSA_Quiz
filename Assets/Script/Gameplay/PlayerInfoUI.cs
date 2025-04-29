using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] Movement playerMovement;
    //[SerializeField] Text playerNameText;
    [SerializeField] Text playerLevelText;
    [SerializeField] Text playerExpText;
    [SerializeField] Text playerHPText;
    [SerializeField] Text playerAttackText;
    [SerializeField] Text playerDefenseText;
    [SerializeField] Text playerSpeedText;
    [SerializeField] Text playerSpAttackText;
    [SerializeField] Text playerSpDefenseText;
    [SerializeField] GameObject infoPanel;

    private bool isInfoPanelActive = false;

    void Start()
    {
        // Ẩn panel ban đầu
        if (infoPanel != null)
            infoPanel.SetActive(false);
        else
            Debug.LogError("infoPanel chưa được gán trong Inspector!");

        if (playerMovement == null)
            Debug.LogError("playerMovement chưa được gán trong Inspector!");

        // Kiểm tra các Text component
        if (playerLevelText == null || playerExpText == null || playerHPText == null)
            Debug.LogWarning("Một số Text component chưa được gán trong Inspector!");

        Debug.Log("PlayerInfoUI đã được khởi tạo");
    }

    void Update()
    {
        // Cập nhật thông tin nếu panel đang hiển thị
        UpdatePlayerInfo();
    }

    public void ToggleInfoPanel()
    {
        isInfoPanelActive = !isInfoPanelActive;
        if (infoPanel != null)
        {
            infoPanel.SetActive(isInfoPanelActive);
            
            if (isInfoPanelActive)
            {
                Debug.Log("Hiển thị bảng thông tin người chơi");
                UpdatePlayerInfo();
            }
            else
            {
                Debug.Log("Ẩn bảng thông tin người chơi");
            }
        }
        else
        {
            Debug.LogError("Không thể bật/tắt infoPanel vì nó chưa được gán!");
        }
    }

    void UpdatePlayerInfo()
    {
        if (playerMovement == null || playerMovement.player == null)
        {
            Debug.LogWarning("playerMovement hoặc player không tồn tại");
            return;
        }

        Monster player = playerMovement.player;

        // Bỏ hiển thị tên player vì chưa cần hiện tại
        // if (playerNameText != null)
        //    playerNameText.text = player.Base.Name;

        if (playerLevelText != null)
            playerLevelText.text = $"{player.Level}";

        if (playerExpText != null)
            playerExpText.text = $"{player.Exp}/{player.ExpToNextLevel}";

        if (playerHPText != null)
            playerHPText.text = $"{player.HP}/{player.MaxHP}";

        if (playerAttackText != null)
            playerAttackText.text = $"{player.Attack}";

        if (playerDefenseText != null)
            playerDefenseText.text = $"{player.Defense}";

        if (playerSpeedText != null)
            playerSpeedText.text = $"{player.Speed}";

        if (playerSpAttackText != null)
            playerSpAttackText.text = $"{player.SpAttack}";

        if (playerSpDefenseText != null)
            playerSpDefenseText.text = $"{player.SpDefense}";

        Debug.Log($"Cập nhật thông tin: Level={player.Level}, EXP={player.Exp}/{player.ExpToNextLevel}, HP={player.HP}/{player.MaxHP}, " +
                 $"ATK={player.Attack}, DEF={player.Defense}, SPD={player.Speed}, SP.ATK={player.SpAttack}, SP.DEF={player.SpDefense}");
    }
}