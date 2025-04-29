using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour
{
    public float movementSpeed;
    [SerializeField] private float runSpeedMultiplier = 2f; // Hệ số tăng tốc khi chạy
    private Boolean isMoving;
    private Boolean isRunning; // Biến theo dõi trạng thái chạy
    private Boolean wasRunning; // Biến để theo dõi thay đổi trạng thái chạy
    private Vector2 input;
    private Animator animator;
    public LayerMask solidObjLayer;
    public LayerMask grassLayer;
    public LayerMask pokemonObjLayer;
    public LayerMask InteractableLayer;

    [SerializeField] GrassEncounterList defaultGrassEncounter;
    [SerializeField] public MonsterBase playerBase;
    public Monster player;
    
    [SerializeField] bool debugMode = true;
    [SerializeField] private bool showRunningIndicator = true; // Hiển thị chỉ báo chạy trên màn hình

    public event Action<MonsterBase, Monster, Collider2D> onEncountered;

    private void Awake()
    {
        player = new Monster(playerBase, 5);
        //Take the animation in animator
        animator = GetComponent<Animator>();
        
        if (defaultGrassEncounter == null)
        {
            Debug.LogError("Movement: DefaultGrassEncounter chưa được thiết lập! Vui lòng gán một GrassEncounterList mặc định.");
        }
    }

    public void HandleUpdate()
    {
        // Lưu trạng thái chạy cũ
        wasRunning = isRunning;
        
        // Cập nhật trạng thái chạy
        isRunning = Input.GetButton("Fire3"); // Fire3 là nút Shift
        
        // Hiển thị thông báo khi trạng thái chạy thay đổi
        if (isRunning != wasRunning && debugMode)
        {
            if (isRunning)
                Debug.Log("Đang chạy với tốc độ x" + runSpeedMultiplier);
            else
                Debug.Log("Đã chuyển về tốc độ đi bộ bình thường");
        }
        
        if (!isMoving)
        {
            //Take input from player
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Removing diagonal movement
            if (input.x != 0) input.y = 0;

            //Check does player input something
            if(input != Vector2.zero)
            {
                //Set for both parameter to let the animation know which direction
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsWalkAble(targetPosition)) {
                    StartCoroutine(Move(targetPosition));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact(){
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var InteractPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, InteractPos, Color.green, 0.5f);
        var collider = Physics2D.OverlapCircle(InteractPos , 0.3f, InteractableLayer);
        if (collider != null){
            collider.GetComponent<Interactable>()?.Interact();
        }
    }
    
    //Function to move a player one tiles
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        // Sử dụng biến isRunning đã được cập nhật trong HandleUpdate
        float currentSpeed = isRunning ? movementSpeed * runSpeedMultiplier : movementSpeed;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
        checkForEncounter(targetPos);
    }


    //Check collision with something that can't not pass
    private bool IsWalkAble(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjLayer | InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void checkForEncounter(Vector3 targetPos)
    {
        // Kiểm tra xem có đang đứng trên cỏ hay không
        var grassCollider = Physics2D.OverlapCircle(targetPos, 0.1f, grassLayer);
        if (grassCollider != null)
        {
            // Tỉ lệ 10% gặp monster khi đi vào cỏ
            if(Random.Range(1, 101) <= 10)
            {
                Debug.Log("Đang đứng trên cỏ và kích hoạt gặp monster");
                animator.SetBool("isMoving", false);
                
                // Lấy vị trí hiện tại
                if (debugMode) Debug.Log($"Đang đứng tại vị trí: {targetPos}");
                
                // Lấy danh sách encounter từ GrassZoneManager dựa trên vị trí
                GrassEncounterList encounterList;
                
                if (GrassZoneManager.Instance != null)
                {
                    // Sử dụng GrassZoneManager để lấy danh sách encounter cho vị trí này
                    encounterList = GrassZoneManager.Instance.GetEncounterListForPosition(targetPos);
                    if (debugMode) Debug.Log($"Lấy danh sách encounter từ GrassZoneManager cho vị trí {targetPos}");
                }
                else
                {
                    // Fallback nếu không có GrassZoneManager
                    encounterList = defaultGrassEncounter;
                    if (debugMode) Debug.LogWarning($"Không tìm thấy GrassZoneManager, sử dụng danh sách mặc định từ Movement");
                }
                
                // Nếu không tìm thấy danh sách encounter nào, sử dụng mặc định
                if (encounterList == null)
                {
                    encounterList = defaultGrassEncounter;
                    if (debugMode) Debug.LogWarning($"Không tìm thấy danh sách encounter, sử dụng mặc định từ Movement");
                }
                
                // Lấy một monster ngẫu nhiên từ danh sách
                MonsterBase randomMonster = encounterList.GetRandomMonster();
                if (debugMode) Debug.Log($"Gặp monster ngẫu nhiên: {randomMonster.Name}");
                
                // Tạo monster với level tương ứng
                Monster wildMonster = new Monster(randomMonster, player.Level <= 5 ? player.Level + Random.Range(0, 3) : (Random.Range(0, 2) == 0 ? player.Level + Random.Range(0, 3) : player.Level - Random.Range(0, 3)));
                
                // Hiển thị thông tin cơ bản của monster
                Debug.Log($"===== THÔNG TIN MONSTER =====");
                Debug.Log($"Tên: {wildMonster.Base.Name} | Level: {wildMonster.Level} | Type: {wildMonster.Base.Type}");
                Debug.Log($"HP: {wildMonster.HP}/{wildMonster.MaxHP} | Attack: {wildMonster.Attack} | Defense: {wildMonster.Defense}");
                Debug.Log($"SpAttack: {wildMonster.SpAttack} | SpDefense: {wildMonster.SpDefense} | Speed: {wildMonster.Speed}");
                Debug.Log($"=============================");
                
                onEncountered(randomMonster, player, null);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Monster"){
            ListMonsters monsters = collision.GetComponent<ListMonsters>();
            
            // Tạo monster với level tương ứng
            Monster wildMonster = new Monster(monsters.Monster, player.Level <= 5 ? player.Level + Random.Range(0, 3) : (Random.Range(0, 2) == 0 ? player.Level + Random.Range(0, 3) : player.Level - Random.Range(0, 3)));
            
            // Hiển thị thông tin cơ bản của monster
            Debug.Log($"===== THÔNG TIN MONSTER =====");
            Debug.Log($"Tên: {wildMonster.Base.Name} | Level: {wildMonster.Level} | Type: {wildMonster.Base.Type}");
            Debug.Log($"HP: {wildMonster.HP}/{wildMonster.MaxHP} | Attack: {wildMonster.Attack} | Defense: {wildMonster.Defense}");
            Debug.Log($"SpAttack: {wildMonster.SpAttack} | SpDefense: {wildMonster.SpDefense} | Speed: {wildMonster.Speed}");
            Debug.Log($"=============================");
            
            onEncountered(monsters.Monster, player, collision);
        }
    }

    // Hiển thị thông báo khi đang chạy
    private void OnGUI()
    {
        if (showRunningIndicator && isRunning)
        {
            // Tạo một hộp thông báo ở góc màn hình
            GUI.Box(new Rect(10, 10, 150, 25), "Đang chạy (x" + runSpeedMultiplier + ")");
        }
    }
}
