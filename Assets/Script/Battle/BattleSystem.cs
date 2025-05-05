using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, QuestionAnswer};

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit player;
    [SerializeField] BattleUnit enemy;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] MonsterQuestion monsterQuestion;
    [SerializeField] MonsterInfoUI monsterInfoUI;
    [SerializeField] GameObject victoryPanel;

    [SerializeField] Movement playerMovement;
    
    // Tham chiếu tới BattleMusicController
    private BattleMusicController battleMusicController;

    BattleState state;
    int currAction;
    int currMove;
    int currAnswer;
    int randomQuestion;
    bool correct;
    public event Action<bool> onBattleOver;
    int escapeAttempts;

    Collider2D Collision;
    
    private void Awake()
    {
        // Lấy tham chiếu tới BattleMusicController đã gắn vào GameObject này
        battleMusicController = GetComponent<BattleMusicController>();
    }

    public void StartBattle(MonsterBase Enemy, Monster Player, Collider2D collision){
        Collision = collision;
        enemy._base = Enemy;
        player.Monster = Player;
        
        // Bắt đầu phát nhạc chiến đấu
        if (battleMusicController != null)
        {
            battleMusicController.StartBattle();
        }
        int monsterLevel;
        if(Enemy.Name == "Boss")
        {
            monsterLevel = 50;
            if(Player.Level >= 50)
            {
                monsterLevel = Player.Level + Random.Range(0, 3);
            }
        }
        else {
            monsterLevel = Player.Level <= 5 ? Player.Level + Random.Range(0, 3): (Random.Range(0, 2) == 0 ? Player.Level + Random.Range(0, 5): Player.Level - Random.Range(0, 3));
        }
        StartCoroutine(SetupBattle(new Monster(Enemy, monsterLevel), Player));
    }

    public IEnumerator SetupBattle(Monster Enemy, Monster Player){
        player.Setup(Player);
        playerHUD.SetData(player.Monster);
        enemy.Setup(Enemy);
        enemyHUD.SetData(enemy.Monster);

        // Cập nhật MonsterInfoUI nếu có
        if (monsterInfoUI != null)
        {
            monsterInfoUI.UpdateMonsterInfo();
        }

        Debug.Log(player.Monster.HP);

        dialogBox.SetMoveNames(player.Monster.Moves);
        yield return dialogBox.TypeDialog($"A monster {enemy.Monster.Base.Name} appear");
        yield return new WaitForSeconds(1f);

        escapeAttempts = 0;

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableAnswerSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    void QuestionAnswer()
    {
        state = BattleState.QuestionAnswer;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(false);
        monsterQuestion.gameObject.SetActive(true);
        monsterQuestion.EnableMonsterQuestion(true);
        dialogBox.EnableAnswerSelector(true);
        randomQuestion = Random.Range(0, enemy.Monster.Questions.Count);
        monsterQuestion.setUpMonsterQuestion(enemy.Monster.Questions[randomQuestion].Base.Question);
        dialogBox.SetAnswerDialog(enemy.Monster.Questions[randomQuestion].Base.Answers);
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            handlePlayerAction();
        }
        else if (state == BattleState.PlayerMove)
        {
            handlePlayerMove();
        }
        else if (state == BattleState.QuestionAnswer) {
            handlePlayerQuestionAnswer();
        }
    }

    IEnumerator PerformPlayerMove(bool correct, float bonusDmg)
    {
        state = BattleState.Busy;
        var move = player.Monster.Moves[currMove];
        
        // Kiểm tra xem skill có đang trong thời gian hồi chiêu không
        if (!move.IsReady())
        {
            yield return dialogBox.TypeDialog($"{move.Base.Name} đang hồi chiêu. Còn {move.CurrentCooldown} lượt");
            PlayerAction();
            yield break;
        }
        
        // Đặt cooldown cho skill
        move.UseMove();
        
        if (correct)
        {
            yield return dialogBox.TypeDialog($"{player.Monster.Base.Name} used {move.Base.Name} with full damage");
        }
        else
        {
            yield return dialogBox.TypeDialog($"{player.Monster.Base.Name} used {move.Base.Name}, but you answered wrong so no damage");
        }
        player.PlayerAttackAnimation();
        if (correct) enemy.PlayHitAnimation();
        yield return new WaitForSeconds(1f);

        bool isFainted = enemy.Monster.TakeDamage(move, player.Monster, correct, bonusDmg);
        StartCoroutine(enemyHUD.UpdateHP(enemy.Monster));
        
        // Giảm thời gian hồi chiêu cho các chiêu thức khác (không bao gồm chiêu vừa sử dụng)
        foreach (var playerMove in player.Monster.Moves)
        {
            if (playerMove != move) // Không giảm cooldown cho move vừa sử dụng
                playerMove.DecreaseCooldown();
        }
        
        if (isFainted) {
            yield return dialogBox.TypeDialog($"{enemy.Monster.Base.Name} is fainted");
            enemy.PlayFaintAnimation();
            if(Collision != null){
                Collision.gameObject.SetActive(false);
            }
            
            // Tính exp cho người chơi (dựa vào level của monster và các yếu tố khác)
            int expGain = CalculateExpGain(enemy.Monster);
            yield return dialogBox.TypeDialog($"You gained {expGain} EXP!");
            
            // Tăng exp và kiểm tra xem có lên cấp không
            bool leveledUp = player.Monster.GainExp(expGain);
            
            // Hồi máu sau trận đấu
            player.Monster.HealFull();
            StartCoroutine(playerHUD.UpdateHP(player.Monster));
            
            // Nếu lên cấp thì hiển thị thông báo
            if (leveledUp) {
                yield return dialogBox.TypeDialog($"{player.Monster.Base.Name} leveled up to level {player.Monster.Level}!");
                yield return dialogBox.TypeDialog($"Your stats increased!");
            }
            
            // Kết thúc nhạc chiến đấu khi đã thắng
            if (battleMusicController != null)
            {
                battleMusicController.EndBattle();
            }
            
            yield return new WaitForSeconds(2f);

            // Kiểm tra xem có phải đánh bại Boss không
            if (enemy.Monster.Base.Name == "Boss")
            {
                // Hiển thị victory panel
                if (victoryPanel != null)
                {
                    victoryPanel.SetActive(true);
                    
                    // Bắt đầu coroutine đợi người chơi nhấn phím để restart game
                    StartCoroutine(WaitForRestartInput());
                }
                else
                {
                    Debug.LogError("Victory Panel is not assigned!");
                    onBattleOver(true);
                }
            }
            else
            {
                onBattleOver(true);
            }
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    // Hàm tính exp nhận được khi đánh bại một monster
    private int CalculateExpGain(Monster defeatedMonster)
    {
        // Công thức: exp = (level của monster bị đánh bại * hệ số cơ bản)
        // Hệ số cơ bản có thể điều chỉnh để cân bằng game
        int baseExpYield = 10; // Hệ số cơ bản
        if(defeatedMonster.Base.Name == "Zapdos" || defeatedMonster.Base.Name == "Articuno" || defeatedMonster.Base.Name == "Moltres")
        {
            baseExpYield = 50; // Hệ số cơ bản cho Pkm huyền thoại
        }
        return defeatedMonster.Level * baseExpYield;
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemy.Monster.GetRandomMove();
        
        // Đặt cooldown cho move của enemy
        move.UseMove();
        
        yield return dialogBox.TypeDialog($"{enemy.Monster.Base.Name} is using {move.Base.Name}");
        yield return new WaitForSeconds(1f);

        bool lucky = Random.Range(0, 100) <= 70;

        bool isFainted = player.Monster.TakeDamage(move, enemy.Monster, lucky, 1);
        yield return dialogBox.TypeDialog(lucky ? $"{enemy.Monster.Base.Name} hit you !!!": $"{enemy.Monster.Base.Name} is miss.");
        yield return new WaitForSeconds(1f);
        enemy.PlayerAttackAnimation();
        if (lucky) player.PlayHitAnimation();
        yield return new WaitForSeconds(1f);
        StartCoroutine(playerHUD.UpdateHP(player.Monster));
        
        // Log để debug cooldown
        Debug.Log("Cooldown values after enemy move:");
        foreach (var playerMove in player.Monster.Moves)
        {
            Debug.Log($"{playerMove.Base.Name}: Cooldown = {playerMove.CurrentCooldown}");
        }
        
        // Giảm thời gian hồi chiêu cho tất cả các chiêu thức của người chơi
        foreach (var playerMove in player.Monster.Moves)
        {
            playerMove.DecreaseCooldown();
        }
        
        // Giảm thời gian hồi chiêu cho tất cả các chiêu thức của enemy trừ move vừa sử dụng
        foreach (var enemyMove in enemy.Monster.Moves)
        {
            if (enemyMove != move) // Không giảm cooldown cho move vừa sử dụng
                enemyMove.DecreaseCooldown();
        }
        
        // Log để check sau khi giảm cooldown
        Debug.Log("Cooldown values after decreasing:");
        foreach (var playerMove in player.Monster.Moves)
        {
            Debug.Log($"{playerMove.Base.Name}: Cooldown = {playerMove.CurrentCooldown}");
        }
        
        if (isFainted) {
            yield return dialogBox.TypeDialog($"You are dead. BYE BYE !!!");
            player.PlayFaintAnimation();
            
            // Kết thúc nhạc chiến đấu khi đã thua
            if (battleMusicController != null)
            {
                battleMusicController.EndBattle();
            }
            
            yield return new WaitForSeconds(2f);
            onBattleOver(false);
        }
        else
        {
            PlayerAction();
        }
    }

    void handlePlayerQuestionAnswer()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currAnswer < enemy.Monster.Questions[randomQuestion].Base.Answers.Count - 2)
            {
                currAnswer += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currAnswer > 1)
            {
                currAnswer -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currAnswer < enemy.Monster.Questions[randomQuestion].Base.Answers.Count - 1)
            {
                ++currAnswer;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currAnswer > 0)
            {
                --currAnswer;
            }
        }

        dialogBox.UpdateAnswerSelection(currAnswer);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            float multiplyDame = monsterQuestion.timer.timerValue;
            correct = enemy.Monster.Questions[randomQuestion].Base.Answers[currAnswer].correctAnswer;
            
            // Cập nhật thành tựu nếu trả lời đúng
            if (correct && AchievementManager.Instance != null)
            {
                // Lấy loại câu hỏi từ câu hỏi hiện tại và cập nhật thành tựu
                MonsterType questionType = enemy.Monster.Questions[randomQuestion].Base.QuestionType;
                string questionId = enemy.Monster.Questions[randomQuestion].Base.name; // Sử dụng tên của QuestionBase làm ID
                AchievementManager.Instance.UpdateAchievement(questionType, questionId);
            }
            
            monsterQuestion.EnableMonsterQuestion(false);
            monsterQuestion.gameObject.SetActive(false);
            dialogBox.EnableAnswerSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove(correct, multiplyDame));
        }
    }

    void handlePlayerMove()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currMove < player.Monster.Moves.Count - 2)
            {
                currMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currMove > 1)
            {
                currMove -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(currMove < player.Monster.Moves.Count - 1)
            {
                ++currMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currMove > 0)
            {
                --currMove;
            }
        }

        dialogBox.UpdateMoveSelection(currMove, player.Monster.Moves[currMove]);
        
        // Debug log
        Debug.Log($"Selected move: {player.Monster.Moves[currMove].Base.Name}, Cooldown: {player.Monster.Moves[currMove].CurrentCooldown}");
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Nếu kỹ năng đang trong thời gian hồi chiêu thì không thể sử dụng
            if (!player.Monster.Moves[currMove].IsReady())
            {
                StartCoroutine(ShowCooldownMessage());
                return;
            }
            
            QuestionAnswer();
        }
    }

    IEnumerator ShowCooldownMessage()
    {
        state = BattleState.Busy; // Đặt trạng thái busy để ngăn chặn các input khác
        dialogBox.EnableMoveSelector(false); // Ẩn bảng chọn skill
        dialogBox.EnableDialogText(true); // Hiển thị dialog
        
        yield return dialogBox.TypeDialog($"{player.Monster.Moves[currMove].Base.Name} đang hồi chiêu. Còn {player.Monster.Moves[currMove].CurrentCooldown} lượt");
        yield return new WaitForSeconds(2f); // Đợi 2 giây
        
        // Quay lại màn hình chọn skill
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
        state = BattleState.PlayerMove; // Đặt lại trạng thái PlayerMove
    }

    void handlePlayerAction()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currAction < 1)
            {
                ++currAction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currAction > 0)
            {
                --currAction;
            }
        }
        dialogBox.UpdateActionSelection(currAction);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(currAction == 0)
            {
                //Fight
                PlayerMove();
            }
            else if( currAction == 1)
            {
                //Run
                StartCoroutine(TryToEscape());
            }
        }
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        int playerSpeed = player.Monster.Speed;
        int enemySpeed = enemy.Monster.Speed;
        ++escapeAttempts;
        if(enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            
            // Kết thúc nhạc chiến đấu khi thoát thành công
            if (battleMusicController != null)
            {
                battleMusicController.EndBattle();
            }
            
            onBattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                dialogBox.EnableActionSelector(false);
                
                // Kết thúc nhạc chiến đấu khi thoát thành công
                if (battleMusicController != null)
                {
                    battleMusicController.EndBattle();
                }
                
                onBattleOver(true);
                playerMovement.transform.Translate(Vector3.up*0.5f);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape");
                state = BattleState.PlayerAction;
                PlayerAction();
            }
        }
    }

    // Coroutine mới để đợi người chơi nhấn phím để restart game
    private IEnumerator WaitForRestartInput()
    {
        // Đợi người chơi nhấn phím Z để restart game
        while (!Input.GetKeyDown(KeyCode.Z))
        {
            yield return null;
        }
        
        // Xóa bản save trước khi restart game
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.DeleteSaveFile();
            Debug.Log("Đã xóa file save khi restart game sau khi đánh bại Boss");
        }
        
        // Restart game bằng cách tải lại scene hiện tại
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
