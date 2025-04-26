# Hướng dẫn thiết lập Monster trong khu vực cỏ

## Giới thiệu
Hệ thống mới cho phép thiết lập các khu vực cỏ khác nhau với danh sách monster riêng biệt, mỗi monster có tỉ lệ xuất hiện khác nhau.

## Các bước thiết lập

### 1. Tạo GrassEncounterList
1. Trong Unity, chuột phải vào thư mục Assets (hoặc thư mục con) → Create → Monster → Create new grass encounter
2. Đặt tên cho khu vực cỏ (ví dụ: "ForestEncounter", "MountainEncounter", v.v.)
3. Thiết lập các thuộc tính:
   - **Area Name**: Tên của khu vực cỏ
   - **Encounter Items**: Danh sách các monster có thể gặp ở khu vực này
     - Thêm các item bằng cách nhấn nút "+"
     - Đối với mỗi item:
       - **Monster**: Kéo thả một MonsterBase vào ô này
       - **Spawn Chance**: Thiết lập tỉ lệ xuất hiện (0-100%)
       - *Lưu ý: Tổng tỉ lệ xuất hiện nên bằng 100%*

### 2. Thiết lập GrassZoneManager
1. Tạo một GameObject mới trong scene và đặt tên là "GrassZoneManager"
2. Thêm component "GrassZoneManager" (Add Component → Scripts → Grass → GrassZoneManager)
3. Kéo thả một GrassEncounterList vào ô "Default Encounter List"
   - *Đây là danh sách mặc định sẽ được sử dụng nếu ô cỏ không thuộc khu vực nào*

### 3. Tạo các khu vực cỏ (GrassZone)
1. Tạo một GameObject mới (Empty) cho mỗi khu vực cỏ và đặt tên phù hợp (ví dụ: "ForestZone", "MountainZone")
2. Thêm Collider2D (Box Collider 2D, Polygon Collider 2D) và đánh dấu là "Is Trigger"
3. Điều chỉnh kích thước collider để bao quanh toàn bộ khu vực cỏ cần áp dụng danh sách monster
4. Thêm component "GrassZone" (Add Component → Scripts → Grass → GrassZone)
5. Thiết lập các thuộc tính:
   - **Encounter List**: Kéo thả GrassEncounterList tương ứng với khu vực này
   - **Grass Layer**: Chọn layer của cỏ trong game (thường là "Grass" layer)

### 4. Thiết lập Fallback GrassEncounterList (Tùy chọn)
1. Chọn GameObject "Player" trong Scene
2. Tìm component "Movement"
3. Kéo thả một GrassEncounterList vào ô "Default Grass Encounter"
   - *Đây là danh sách mặc định thứ cấp, sẽ được sử dụng nếu không tìm thấy GrassZoneManager*

## Cách hoạt động
1. Khi game bắt đầu, mỗi GrassZone sẽ tự động đăng ký tất cả các ô cỏ nằm trong vùng collider của nó vào GrassZoneManager
2. Khi người chơi đi vào một ô cỏ:
   - Hệ thống sẽ kiểm tra xem ô cỏ đó thuộc khu vực (Zone) nào
   - Sử dụng danh sách monster tương ứng với khu vực đó
   - Chọn một monster ngẫu nhiên dựa trên tỉ lệ xuất hiện

## Ví dụ
- Khu vực rừng:
  - Bulbasaur: 60%
  - Caterpie: 30% 
  - Pikachu: 10%

- Khu vực núi:
  - Geodude: 50%
  - Onix: 30%
  - Machop: 20%

## Lưu ý
- Mỗi khu vực cỏ có thể có nhiều loại monster với tỉ lệ phần trăm xuất hiện khác nhau
- Tỉ lệ gặp monster khi đi vào cỏ hiện được thiết lập cố định là 10% (có thể điều chỉnh trong script Movement.cs)
- Nếu tổng tỉ lệ không bằng 100%, hệ thống sẽ tự động chọn monster đầu tiên trong danh sách khi tỉ lệ vượt quá
- Collider của GrassZone phải được đánh dấu là "Is Trigger"
- Các GrassZone có thể chồng lấn lên nhau, nhưng trong trường hợp đó, GrassZone được đăng ký sau cùng sẽ được ưu tiên 