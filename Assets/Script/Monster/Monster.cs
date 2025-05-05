using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Monster
{ 
    public MonsterBase Base{get; set;}
    public int Level{get; set;}

    public int HP { get; set; }
    public List<Move> Moves { get; set; } 
    public List<Question> Questions { get; set; }
    
    // Thêm biến kinh nghiệm
    public int Exp { get; set; }
    public int ExpToNextLevel { get { return Level * Level * 5; } } // Công thức tính kinh nghiệm cần để lên cấp (tuỳ chỉnh)

    // Thêm stat bonus
    public int MaxHPBonus { get; set; }
    public int AttackBonus { get; set; }
    public int DefenseBonus { get; set; }
    public int SpeedBonus { get; set; }
    public int SpAttackBonus { get; set; }
    public int SpDefenseBonus { get; set; }

    public Monster(MonsterBase pBase, int pLevel)
    {
        this.Base = pBase;
        this.Level = pLevel;
        HP = MaxHP;
        Exp = 0;

        // Khởi tạo các bonus stat là 0
        MaxHPBonus = 0;
        AttackBonus = 0;
        DefenseBonus = 0;
        SpeedBonus = 0;
        SpAttackBonus = 0;
        SpDefenseBonus = 0;

        //Add moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnAbleMoves)
        {
            if(move.Level <= this.Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if(Moves.Count >= 4)
            {
                break;
            }
        }

        Questions = new List<Question>();
        foreach (var question in Base.LearnAbleQuestions)
        {
            if(question.Level <= this.Level)
            {
                Questions.Add(new Question(question.Base));
            }
        }
    }

    // Cập nhật các thuộc tính để thêm bonus stats
    public int Attack {get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 + AttackBonus; } }
    public int Defense { get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5 + DefenseBonus; } }
    public int MaxHP { get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + MaxHPBonus; } }
    public int SpAttack { get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5 + SpAttackBonus; } }
    public int SpDefense { get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5 + SpDefenseBonus; } }
    public int Speed { get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5 + SpeedBonus; } }

    // Phương thức tăng chỉ số ngẫu nhiên khi đạt thành tựu
    public string IncreaseRandomStat()
    {
        try
        {
            int statChoice = UnityEngine.Random.Range(0, 6);
            int increaseAmount = 1;
            string statName = "";
            
            switch (statChoice)
            {
                case 0: // HP
                    MaxHPBonus += increaseAmount;
                    statName = "HP";
                    break;
                case 1: // Attack
                    AttackBonus += increaseAmount;
                    statName = "Attack";
                    break;
                case 2: // Defense
                    DefenseBonus += increaseAmount;
                    statName = "Defense";
                    break;
                case 3: // Speed
                    SpeedBonus += increaseAmount;
                    statName = "Speed";
                    break;
                case 4: // Sp. Attack
                    SpAttackBonus += increaseAmount;
                    statName = "Special Attack";
                    break;
                case 5: // Sp. Defense
                    SpDefenseBonus += increaseAmount;
                    statName = "Special Defense";
                    break;
                default:
                    // Trường hợp mặc định
                    MaxHPBonus += increaseAmount;
                    statName = "HP";
                    break;
            }

            return statName;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi khi tăng chỉ số ngẫu nhiên: {e.Message}");
            return "Chỉ số";
        }
    }
    
    // Reset tất cả bonus stats về 0
    public void ResetBonusStats()
    {
        MaxHPBonus = 0;
        AttackBonus = 0;
        DefenseBonus = 0;
        SpeedBonus = 0;
        SpAttackBonus = 0;
        SpDefenseBonus = 0;
    }

    public bool TakeDamage(Move move, Monster attacker, bool correct, float bonusDmg)
    {
        // Hệ số ngẫu nhiên từ 0.85 đến 1.0
        float modifiers = Random.Range(0.85f, 1f);
        // Hệ số cấp độ có tác động ít hơn
        float levelFactor = (1 + attacker.Level / 50f);
        // Tỷ lệ tấn công/phòng thủ được điều chỉnh để không quá mạnh
        float attackDefenseRatio = Mathf.Sqrt((float)attacker.Attack / Defense);
        // Giới hạn hệ số bonusDmg
        float maxBonusDmg = Mathf.Min(bonusDmg, 1.5f); 
        // Công thức tính sát thương cơ bản
        float baseDamage = (move.Base.Power / 5f) * levelFactor * attackDefenseRatio;
        // Thêm giá trị tối thiểu để tránh sát thương quá thấp
        float finalDamage = Mathf.Max(1, baseDamage * modifiers);
        // Chỉ gây sát thương khi trả lời đúng, với hệ số bonus từ thời gian
        int damage = correct ? Mathf.FloorToInt(finalDamage * (maxBonusDmg > 1 ? maxBonusDmg : 1)) : 0;
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        return false;
    }

    public Move GetRandomMove()
    {
        // Lọc danh sách các move sẵn sàng (không trong cooldown)
        List<Move> availableMoves = Moves.FindAll(m => m.IsReady());
        
        // Nếu không có move nào sẵn sàng, trả về move đầu tiên
        if (availableMoves.Count == 0)
        {
            Debug.Log("Không có move nào sẵn sàng, sử dụng move đầu tiên");
            return Moves[0];
        }
        
        // Chọn ngẫu nhiên từ các move sẵn sàng
        int r = Random.Range(0, availableMoves.Count);
        Debug.Log($"Sử dụng {availableMoves[r].Base.Name} từ {availableMoves.Count} move sẵn sàng");
        return availableMoves[r];
    }
    
    // Thêm hàm nhận EXP và kiểm tra level up
    public bool GainExp(int amount)
    {
        Exp += amount;
        bool leveledUp = false;
        
        // Kiểm tra nếu đủ exp để lên cấp
        while (Exp >= ExpToNextLevel)
        {
            // Trừ exp để lên cấp
            Exp -= ExpToNextLevel;
            // Tăng level
            Level++;
            // Kiểm tra xem có học được chiêu mới không
            CheckForNewMoves();
            // Kiểm tra xem có học được câu hỏi mới không
            CheckForNewQuestions();
            
            leveledUp = true;
        }
        
        return leveledUp;
    }
    
    // Kiểm tra xem có học được chiêu mới không khi lên cấp
    private void CheckForNewMoves()
    {
        foreach (var move in Base.LearnAbleMoves)
        {
            if (move.Level == Level) // Chỉ thêm move mới ở cấp hiện tại
            {
                if (Moves.Count < 4) // Nếu chưa đủ 4 chiêu thì thêm vào
                {
                    Moves.Add(new Move(move.Base));
                }
                // Nếu đã đủ 4 chiêu, có thể thêm logic để thay thế chiêu ở đây
            }
        }
    }
    
    // Kiểm tra xem có học được câu hỏi mới không khi lên cấp
    private void CheckForNewQuestions()
    {
        foreach (var question in Base.LearnAbleQuestions)
        {
            if (question.Level == Level) // Chỉ thêm câu hỏi mới ở cấp hiện tại
            {
                Questions.Add(new Question(question.Base));
            }
        }
    }
    
    // Hàm hồi máu
    public void HealFull()
    {
        HP = MaxHP;
    }
}
