using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] GameObject achievementPanel;
    [SerializeField] GameObject achievementItemPrefab;
    [SerializeField] Transform achievementList;
    [SerializeField] Button closeButton;
    [SerializeField] float itemHeight = 250f; // Chiều cao cho mỗi item
    [SerializeField] float spacing = 50f; // Đặt lại spacing ban đầu
    
    private void Start()
    {
        // Đăng ký sự kiện cho nút đóng
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
        
        // Ẩn panel khi bắt đầu
        if (achievementPanel != null)
        {
            achievementPanel.SetActive(false);
        }
        
        // Đăng ký sự kiện khi thành tựu được cập nhật
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementsUpdated += UpdateUI;
        }
    }
    
    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi thành tựu được cập nhật
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementsUpdated -= UpdateUI;
        }
        
        // Hủy đăng ký sự kiện cho nút đóng
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePanel);
        }
    }
    
    // Hiển thị panel thành tựu
    public void ShowPanel()
    {
        if (achievementPanel != null)
        {
            achievementPanel.SetActive(true);
            UpdateUI();
        }
    }
    
    // Ẩn panel thành tựu
    public void ClosePanel()
    {
        if (achievementPanel != null)
        {
            achievementPanel.SetActive(false);
        }
    }
    
    // Cập nhật UI thành tựu
    private void UpdateUI()
    {
        if (AchievementManager.Instance == null || achievementList == null || achievementItemPrefab == null)
            return;
        
        // Xóa các item thành tựu cũ
        foreach (Transform child in achievementList)
        {
            Destroy(child.gameObject);
        }
        
        // Xóa các component không cần thiết có thể gây lỗi
        RemoveUnwantedComponents();
        
        // Lấy danh sách thành tựu
        var achievements = AchievementManager.Instance.GetAllAchievements();
        
        // Thiết lập kích thước content trước
        RectTransform contentRect = achievementList.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            float contentHeight = achievements.Count * (itemHeight + spacing);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentHeight);
        }
        
        // Tạo các item thành tựu mới với vị trí không chồng lên nhau
        for (int i = 0; i < achievements.Count; i++)
        {
            // Tính toán vị trí Y cho mỗi item, bắt đầu từ trên xuống
            float yPosition = -(itemHeight / 2) - (i * (itemHeight + spacing));
            
            // Tạo và đặt vị trí cho item
            GameObject itemObj = Instantiate(achievementItemPrefab, achievementList);
            RectTransform itemRect = itemObj.GetComponent<RectTransform>();
            
            if (itemRect != null)
            {
                // Thiết lập RectTransform cho item
                itemRect.anchorMin = new Vector2(0, 1); // Anchor ở góc trên bên trái
                itemRect.anchorMax = new Vector2(1, 1); // Anchor ở góc trên bên phải
                itemRect.pivot = new Vector2(0.5f, 0.5f); // Pivot ở giữa
                itemRect.anchoredPosition = new Vector2(0, yPosition);
                itemRect.sizeDelta = new Vector2(0, itemHeight);
                
                // Đảm bảo item sẽ hiển thị đúng
                Image bgImage = itemObj.GetComponent<Image>();
                if (bgImage != null)
                {
                    bgImage.raycastTarget = true;
                    Color bgColor = bgImage.color;
                    bgColor.a = 1f;
                    bgImage.color = bgColor;
                }
            }
            
            // Đảm bảo mọi text có alpha = 1
            foreach (Text text in itemObj.GetComponentsInChildren<Text>())
            {
                Color color = text.color;
                color.a = 1f;
                text.color = color;
            }
            
            // Khởi tạo item
            AchievementItem item = itemObj.GetComponent<AchievementItem>();
            if (item != null)
            {
                item.Initialize(achievements[i]);
            }
        }
        
        // Đặt lại vị trí scroll view
        ScrollRect scrollRect = achievementList.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
            // Buộc ScrollRect cập nhật
            StartCoroutine(DelayedScrollUpdate(scrollRect));
        }
    }
    
    // Coroutine để đảm bảo ScrollRect cập nhật sau 1 frame
    private IEnumerator DelayedScrollUpdate(ScrollRect scrollRect)
    {
        yield return null; // Đợi 1 frame
        scrollRect.normalizedPosition = new Vector2(0, 1);
        Canvas.ForceUpdateCanvases();
    }
    
    // Xóa bỏ các component không cần thiết có thể gây lỗi
    private void RemoveUnwantedComponents()
    {
        RectTransform contentRect = achievementList.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            // Xóa VerticalLayoutGroup nếu có
            VerticalLayoutGroup vlg = contentRect.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                Destroy(vlg);
            }
            
            // Xóa ContentSizeFitter nếu có
            ContentSizeFitter csf = contentRect.GetComponent<ContentSizeFitter>();
            if (csf != null)
            {
                Destroy(csf);
            }
            
            // Thiết lập lại thuộc tính RectTransform ban đầu
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 0.5f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
        }
    }
} 