using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoUI : MonoBehaviour
{
    [SerializeField] BattleUnit monsterUnit;
    [SerializeField] Text monsterNameText;
    [SerializeField] Text monsterLevelText;
    [SerializeField] Text monsterHPText;
    [SerializeField] Text monsterAttackText;
    [SerializeField] Text monsterDefenseText;
    [SerializeField] Text monsterSpeedText;
    [SerializeField] Text monsterSpAttackText;
    [SerializeField] Text monsterSpDefenseText;
    [SerializeField] GameObject infoPanel;

    private bool isInfoPanelActive = false;

    void Start()
    {
        // Ẩn panel ban đầu
        if (infoPanel != null)
            infoPanel.SetActive(false);
        else
            Debug.LogError("infoPanel chưa được gán trong Inspector!");

        if (monsterUnit == null)
            Debug.LogError("monsterUnit chưa được gán trong Inspector!");

        // Kiểm tra các Text component
        if (monsterNameText == null || monsterLevelText == null || monsterHPText == null)
            Debug.LogWarning("Một số Text component chưa được gán trong Inspector!");

        Debug.Log("MonsterInfoUI đã được khởi tạo");
    }

    public void ToggleInfoPanel()
    {
        isInfoPanelActive = !isInfoPanelActive;
        if (infoPanel != null)
        {
            infoPanel.SetActive(isInfoPanelActive);
            
            if (isInfoPanelActive)
            {
                Debug.Log("Hiển thị bảng thông tin monster");
                UpdateMonsterInfo();
            }
            else
            {
                Debug.Log("Ẩn bảng thông tin monster");
            }
        }
        else
        {
            Debug.LogError("Không thể bật/tắt infoPanel vì nó chưa được gán!");
        }
    }

    public void UpdateMonsterInfo()
    {
        if (monsterUnit == null || monsterUnit.Monster == null)
        {
            Debug.LogWarning("monsterUnit hoặc monster không tồn tại");
            return;
        }

        Monster monster = monsterUnit.Monster;

        if (monsterNameText != null)
            monsterNameText.text = monster.Base.Name;

        if (monsterLevelText != null)
            monsterLevelText.text = $"{monster.Level}";

        if (monsterHPText != null)
            monsterHPText.text = $"{monster.HP}/{monster.MaxHP}";

        if (monsterAttackText != null)
            monsterAttackText.text = $"{monster.Attack}";

        if (monsterDefenseText != null)
            monsterDefenseText.text = $"{monster.Defense}";

        if (monsterSpeedText != null)
            monsterSpeedText.text = $"{monster.Speed}";

        if (monsterSpAttackText != null)
            monsterSpAttackText.text = $"{monster.SpAttack}";

        if (monsterSpDefenseText != null)
            monsterSpDefenseText.text = $"{monster.SpDefense}";

        Debug.Log($"Cập nhật thông tin monster: Level={monster.Level}, HP={monster.HP}/{monster.MaxHP}, " +
                 $"ATK={monster.Attack}, DEF={monster.Defense}, SPD={monster.Speed}, SP.ATK={monster.SpAttack}, SP.DEF={monster.SpDefense}");
    }
} 