using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text progressText;
    [SerializeField] Image backgroundImage;
    [SerializeField] Color backgroundColor;
    [SerializeField] Color titleTextColor;
    [SerializeField] Color progressTextColor;
    
    private void Awake()
    {
        // Cấu hình layout
        ConfigureLayout();
        
        // Đảm bảo background image tồn tại
        if (backgroundImage == null)
        {
            // Kiểm tra xem đã có component Image trong GameObject chính chưa
            backgroundImage = GetComponent<Image>();
            
            // Nếu chưa có, thêm mới
            if (backgroundImage == null)
            {
                backgroundImage = gameObject.AddComponent<Image>();
            }
        }
        
        // Áp dụng màu sắc nếu có thiết lập
        ApplyColors();
    }
    
    // Áp dụng màu sắc cho các thành phần
    private void ApplyColors()
    {
        // Áp dụng màu nền
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
        
        // Áp dụng màu chữ cho tiêu đề
        if (titleText != null)
        {
            titleText.color = titleTextColor;
        }
        
        // Áp dụng màu chữ cho tiến độ
        if (progressText != null)
        {
            progressText.color = progressTextColor;
        }
    }
    
    // Cấu hình layout cho item
    private void ConfigureLayout()
    {
        // Kiểm tra và xóa các layout group hiện có nếu cần
        HorizontalLayoutGroup existingHGroup = GetComponent<HorizontalLayoutGroup>();
        if (existingHGroup != null)
        {
            Destroy(existingHGroup);
        }
        
        // Đảm bảo titleText có thiết lập đúng
        if (titleText != null)
        {
            titleText.alignment = TextAnchor.MiddleLeft;
            titleText.fontSize = 36; // Tăng kích thước font
            titleText.fontStyle = FontStyle.Bold;
            
            titleText.verticalOverflow = VerticalWrapMode.Overflow;
            // Đảm bảo RectTransform được thiết lập đúng
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            if (titleRect != null)
            {
                titleRect.anchorMin = new Vector2(0, 0.3f);
                titleRect.anchorMax = new Vector2(0.7f, 1);
                titleRect.offsetMin = new Vector2(15, 0); // Thêm padding bên trái
                titleRect.offsetMax = new Vector2(0, 0);
            }
        }
        
        // Đảm bảo progressText có thiết lập đúng
        if (progressText != null)
        {
            progressText.alignment = TextAnchor.MiddleRight;
            progressText.fontSize = 33; // Tăng kích thước font
            progressText.fontStyle = FontStyle.Bold;

            progressText.verticalOverflow = VerticalWrapMode.Overflow;
            
            // Đảm bảo RectTransform được thiết lập đúng
            RectTransform progressRect = progressText.GetComponent<RectTransform>();
            if (progressRect != null)
            {
                progressRect.anchorMin = new Vector2(0.7f, 0.3f);
                progressRect.anchorMax = new Vector2(1, 1);
                progressRect.offsetMin = new Vector2(0, 0);
                progressRect.offsetMax = new Vector2(-15, 0); // Thêm padding bên phải
            }
        }
    }
    
    public void Initialize(TypeAchievement achievement)
    {
        // Hiển thị tiêu đề dựa trên loại câu hỏi
        string typeName = GetTypeName(achievement.type);
        if (titleText != null)
        {
            titleText.text = typeName;
        }
        
        // Hiển thị tiến độ dạng: X/Y câu hỏi
        if (progressText != null)
        {
            progressText.text = $"{achievement.correctAnswers}/{achievement.totalQuestions} câu hỏi";
        }
    }
    
    // Chuyển đổi QuestionType thành tên hiển thị
    private string GetTypeName(QuestionType type)
    {
        switch (type)
        {
            case QuestionType.Graph:
                return "Đồ thị (Graph)";
            case QuestionType.List:
                return "Danh sách (List)";
            case QuestionType.Tree:
                return "Cây (Tree)";
            case QuestionType.Sort:
                return "Sắp xếp (Sort)";
            case QuestionType.Search:
                return "Tìm kiếm (Search)";
            case QuestionType.StackQueue:
                return "Ngăn xếp & Hàng đợi";
            default:
                return "Unknown";
        }
    }
} 