using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassZoneManager : MonoBehaviour
{
    // Singleton pattern
    public static GrassZoneManager Instance { get; private set; }
    
    // Danh sách các khu vực trong game
    private List<GrassZone> grassZones = new List<GrassZone>();
    
    // Danh sách encounter mặc định
    [SerializeField] GrassEncounterList defaultEncounterList;
    
    // Thêm debug mode
    [SerializeField] bool debugMode = true;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (defaultEncounterList == null)
        {
            Debug.LogError("GrassZoneManager: DefaultEncounterList chưa được thiết lập!");
        }
    }
    
    // Đăng ký một zone mới vào hệ thống
    public void RegisterZone(GrassZone zone)
    {
        if (zone == null) return;
        
        if (!grassZones.Contains(zone))
        {
            grassZones.Add(zone);
            if (debugMode) Debug.Log($"[GrassZoneManager] Đăng ký khu vực mới: {zone.gameObject.name}");
        }
    }
    
    // Lấy danh sách encounter cho một vị trí
    public GrassEncounterList GetEncounterListForPosition(Vector3 position)
    {
        // Tìm zone chứa vị trí này
        foreach (var zone in grassZones)
        {
            if (zone.IsPositionInZone(position))
            {
                GrassEncounterList zoneList = zone.GetEncounterList();
                if (zoneList != null)
                {
                    if (debugMode) Debug.Log($"[GrassZoneManager] Sử dụng danh sách từ khu vực {zone.gameObject.name} cho vị trí {position}");
                    return zoneList;
                }
            }
        }
        
        if (debugMode) Debug.Log($"[GrassZoneManager] Vị trí {position} không thuộc khu vực nào, sử dụng danh sách mặc định");
        return defaultEncounterList;
    }
    
    // Thêm phương thức debug để kiểm tra số lượng khu vực
    public void PrintDebugInfo()
    {
        Debug.Log($"[GrassZoneManager] Tổng số khu vực đã đăng ký: {grassZones.Count}");
        foreach (var zone in grassZones)
        {
            Debug.Log($"- {zone.gameObject.name}");
        }
    }
    
    // Thêm nút debug trong Inspector
    [ContextMenu("Print Debug Info")]
    private void DebugInfo()
    {
        PrintDebugInfo();
    }
    
    // Thêm phương thức xóa tất cả đăng ký hiện tại (hữu ích cho testing)
    [ContextMenu("Clear All Registrations")]
    private void ClearRegistrations()
    {
        grassZones.Clear();
        Debug.Log("[GrassZoneManager] Đã xóa tất cả khu vực.");
    }
} 