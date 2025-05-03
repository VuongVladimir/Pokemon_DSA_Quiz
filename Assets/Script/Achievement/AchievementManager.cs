using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum cho loại câu hỏi
public enum QuestionType
{
    Graph,
    List,
    Tree,
    Sort,
    Search,
    StackQueue
}

// Lớp lưu trữ thành tựu cho mỗi loại câu hỏi
[System.Serializable]
public class TypeAchievement
{
    public QuestionType type;
    public int correctAnswers;
    public int totalQuestions;
    
    public TypeAchievement(QuestionType type)
    {
        this.type = type;
        this.correctAnswers = 0;
        this.totalQuestions = 40; // Giá trị mặc định, sẽ được cập nhật nếu tìm thấy câu hỏi
    }
}

// Singleton quản lý thành tựu
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    
    // Danh sách thành tựu theo loại câu hỏi
    public List<TypeAchievement> typeAchievements;
    
    // Event để thông báo khi thành tựu thay đổi
    public event Action OnAchievementsUpdated;
    
    // HashSet lưu trữ ID của các câu hỏi đã trả lời đúng
    private HashSet<string> answeredQuestions = new HashSet<string>();
    
    // Biến để kiểm tra xem dữ liệu đã được tải chưa trong scene hiện tại
    private bool hasLoadedData = false;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Khởi tạo các thành tựu nếu chưa có
        InitializeAchievements();
        
        // Đặt lại trạng thái hasLoadedData mỗi khi AchievementManager được khởi tạo
        hasLoadedData = false;
        
        // Tải thành tựu từ PlayerPrefs nếu có và chưa từng tải trước đó
        if (!hasLoadedData)
        {
            LoadAchievements();
            hasLoadedData = true;
            Debug.Log("Đã tải dữ liệu thành tựu lần đầu tiên.");
        }
        else
        {
            Debug.Log("Bỏ qua việc tải dữ liệu thành tựu vì đã tải trước đó.");
        }
        
        // Đếm số lượng câu hỏi theo từng loại
        CountQuestionsByType();
    }
    
    // Khởi tạo danh sách thành tựu
    private void InitializeAchievements()
    {
        if (typeAchievements == null || typeAchievements.Count == 0)
        {
            typeAchievements = new List<TypeAchievement>
            {
                new TypeAchievement(QuestionType.Graph),
                new TypeAchievement(QuestionType.List),
                new TypeAchievement(QuestionType.Tree),
                new TypeAchievement(QuestionType.Sort),
                new TypeAchievement(QuestionType.Search),
                new TypeAchievement(QuestionType.StackQueue)
            };
        }
    }
    
    // Đếm số lượng câu hỏi theo từng loại
    private void CountQuestionsByType()
    {
        // Tìm tất cả câu hỏi trong Resources
        QuestionBase[] allQuestions = Resources.LoadAll<QuestionBase>("Questions");
        
        if (allQuestions.Length > 0)
        {
            // Đếm câu hỏi theo loại
            Dictionary<MonsterType, int> questionCounts = new Dictionary<MonsterType, int>();
            foreach (var question in allQuestions)
            {
                if (!questionCounts.ContainsKey(question.QuestionType))
                {
                    questionCounts[question.QuestionType] = 0;
                }
                questionCounts[question.QuestionType]++;
            }
            
            // Cập nhật tổng số câu hỏi cho mỗi loại
            foreach (var achievement in typeAchievements)
            {
                MonsterType monsterType = ConvertToMonsterType(achievement.type);
                if (questionCounts.ContainsKey(monsterType))
                {
                    achievement.totalQuestions = questionCounts[monsterType];
                }
            }
        }
    }
    
    // Cập nhật thành tựu khi trả lời đúng câu hỏi
    public void UpdateAchievement(MonsterType questionType, string questionId)
    {
        // Kiểm tra xem câu hỏi đã được trả lời đúng trước đó chưa
        string key = $"{questionType}_{questionId}";
        if (answeredQuestions.Contains(key))
        {
            // Đã trả lời trước đó, không cập nhật
            return;
        }
        
        // Thêm câu hỏi vào danh sách đã trả lời
        answeredQuestions.Add(key);
        
        QuestionType type = ConvertToQuestionType(questionType);
        
        foreach (var achievement in typeAchievements)
        {
            if (achievement.type == type)
            {
                achievement.correctAnswers++;
                
                // Lưu thành tựu vào PlayerPrefs
                SaveAchievements();
                
                // Thông báo thành tựu đã cập nhật
                OnAchievementsUpdated?.Invoke();
                
                return;
            }
        }
    }
    
    // Chuyển đổi từ MonsterType sang QuestionType
    private QuestionType ConvertToQuestionType(MonsterType monsterType)
    {
        switch (monsterType)
        {
            case MonsterType.Graph:
                return QuestionType.Graph;
            case MonsterType.List:
                return QuestionType.List;
            case MonsterType.Tree:
                return QuestionType.Tree;
            case MonsterType.Sort:
                return QuestionType.Sort;
            case MonsterType.Search:
                return QuestionType.Search;
            case MonsterType.StackQueue:
                return QuestionType.StackQueue;
            default:
                return QuestionType.Graph; // Mặc định
        }
    }
    
    // Chuyển đổi từ QuestionType sang MonsterType
    private MonsterType ConvertToMonsterType(QuestionType questionType)
    {
        switch (questionType)
        {
            case QuestionType.Graph:
                return MonsterType.Graph;
            case QuestionType.List:
                return MonsterType.List;
            case QuestionType.Tree:
                return MonsterType.Tree;
            case QuestionType.Sort:
                return MonsterType.Sort;
            case QuestionType.Search:
                return MonsterType.Search;
            case QuestionType.StackQueue:
                return MonsterType.StackQueue;
            default:
                return MonsterType.Graph; // Mặc định
        }
    }
    
    // Lưu thành tựu vào PlayerPrefs
    private void SaveAchievements()
    {
        foreach (var achievement in typeAchievements)
        {
            string key = $"Achievement_{achievement.type}";
            PlayerPrefs.SetInt(key, achievement.correctAnswers);
        }
        
        // Lưu danh sách câu hỏi đã trả lời đúng
        PlayerPrefs.SetString("AnsweredQuestions", string.Join("|", answeredQuestions));
        
        PlayerPrefs.Save();
    }
    
    // Tải thành tựu từ PlayerPrefs
    public void LoadAchievements()
    {
        // Nếu đã tải trong scene này, không tải lại nữa
        if (hasLoadedData)
        {
            Debug.Log("Bỏ qua việc tải thành tựu vì đã tải trước đó trong scene hiện tại.");
            return;
        }
        
        Debug.Log("Đang tải thành tựu từ PlayerPrefs...");
        foreach (var achievement in typeAchievements)
        {
            string key = $"Achievement_{achievement.type}";
            achievement.correctAnswers = PlayerPrefs.GetInt(key, 0);
            Debug.Log($"Loaded achievement {achievement.type}: {achievement.correctAnswers}");
        }
        
        // Tải danh sách câu hỏi đã trả lời đúng
        string answeredQuestionsString = PlayerPrefs.GetString("AnsweredQuestions", "");
        if (!string.IsNullOrEmpty(answeredQuestionsString))
        {
            string[] questions = answeredQuestionsString.Split('|');
            answeredQuestions = new HashSet<string>(questions);
            Debug.Log($"Loaded {answeredQuestions.Count} answered questions");
        }
        
        // Đánh dấu là đã tải trong scene này
        hasLoadedData = true;
    }
    
    // Lấy thành tựu theo loại
    public TypeAchievement GetAchievement(QuestionType type)
    {
        return typeAchievements.Find(a => a.type == type);
    }
    
    // Lấy tất cả thành tựu
    public List<TypeAchievement> GetAllAchievements()
    {
        return typeAchievements;
    }

    // Thêm phương thức này để cho phép các class khác thông báo rằng thành tựu đã được cập nhật
    public void NotifyAchievementsUpdated()
    {
        // Gọi event để thông báo rằng thành tựu đã được cập nhật
        OnAchievementsUpdated?.Invoke();
    }

    // Reset trạng thái đã tải (chỉ được gọi khi reset game)
    public void ResetLoadedState()
    {
        hasLoadedData = false;
        Debug.Log("Đã reset trạng thái tải dữ liệu thành tựu.");
    }
} 