using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassZone : MonoBehaviour
{
    [SerializeField] GrassEncounterList encounterList;
    [SerializeField] bool debugMode = true;
    
    private Collider2D zoneCollider;
    
    private void Start()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider == null)
        {
            Debug.LogError($"GrassZone '{gameObject.name}' cần có Collider2D. Vui lòng thêm Box Collider 2D hoặc Polygon Collider 2D.");
            return;
        }
        
        // Đăng ký zone này với GrassZoneManager
        if (GrassZoneManager.Instance != null)
        {
            GrassZoneManager.Instance.RegisterZone(this);
            if (debugMode) Debug.Log($"GrassZone '{gameObject.name}' đã đăng ký với GrassZoneManager.");
        }
        else
        {
            Debug.LogError("GrassZoneManager không tồn tại trong scene!");
        }
    }
    
    // Kiểm tra xem một vị trí có nằm trong zone này không
    public bool IsPositionInZone(Vector3 position)
    {
        if (zoneCollider == null) return false;
        
        // Chuyển đổi từ Vector3 sang Vector2 cho việc kiểm tra 2D
        Vector2 position2D = new Vector2(position.x, position.y);
        
        // Kiểm tra xem vị trí có nằm trong collider không
        bool isInside = zoneCollider.OverlapPoint(position2D);
        
        if (debugMode && isInside)
        {
            Debug.Log($"Vị trí {position} nằm trong khu vực '{gameObject.name}'");
        }
        
        return isInside;
    }
    
    public GrassEncounterList GetEncounterList()
    {
        return encounterList;
    }
    
    private void OnDrawGizmos()
    {
        // Vẽ viền khu vực trong Scene view để dễ nhìn
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f); // Màu xanh lá với độ trong suốt
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.8f); // Màu xanh lá đậm hơn cho viền
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
} 