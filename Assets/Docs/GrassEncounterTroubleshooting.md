# Hướng dẫn khắc phục lỗi với hệ thống khu vực cỏ

## Giải pháp mới: Quản lý dựa trên vị trí

Trong phiên bản mới, hệ thống đã được cải tiến để xử lý trường hợp tất cả các ô cỏ đều sử dụng cùng một GameObject. Thay vì đăng ký từng ô cỏ, chúng ta đăng ký các vùng không gian và xác định monster dựa trên vị trí của người chơi.

### Ưu điểm:
- Hoạt động với mọi loại thiết lập cỏ (nhiều GameObject hoặc một GameObject duy nhất)
- Quản lý khu vực dễ dàng hơn với collider trực quan
- Không cần đánh dấu từng ô cỏ

## Các vấn đề thường gặp

### Tất cả các ô cỏ đều dùng cùng một danh sách monster

Nếu tất cả các ô cỏ đều sử dụng cùng một danh sách monster (ví dụ: grass1), dù bạn đã thiết lập các khu vực khác nhau, hãy kiểm tra các nguyên nhân sau:

1. **GrassZoneManager chưa được thêm vào scene**
   - Đảm bảo bạn đã tạo GameObject với component GrassZoneManager
   - Kiểm tra log lỗi liên quan đến GrassZoneManager trong Console

2. **Collider của GrassZone không đúng**
   - Đảm bảo mỗi GrassZone có Collider2D (Box hoặc Polygon) đã được đánh dấu "Is Trigger"
   - Kiểm tra xem collider có bao quanh vùng cỏ muốn áp dụng không
   - Sử dụng Scene view để xem Gizmo màu xanh lá thể hiện khu vực

3. **Thứ tự đăng ký của các khu vực**
   - Khu vực được đăng ký sau có thể ghi đè lên khu vực trước nếu chúng chồng lấn
   - Đảm bảo các khu vực không chồng lấn nếu không cần thiết
   
## Các bước chẩn đoán

1. **Kiểm tra đăng ký khu vực**
   - Trong khi chạy game, chọn GameObject "GrassZoneManager" trong Hierarchy
   - Chuột phải vào component GrassZoneManager trong Inspector
   - Chọn "Print Debug Info" để xem danh sách các khu vực đã được đăng ký

2. **Kiểm tra ranh giới khu vực**
   - Trong Scene view, xem các Gizmo màu xanh lá hiển thị ranh giới của các GrassZone
   - Đảm bảo khu vực bao quanh đúng các ô cỏ cần áp dụng danh sách monster

3. **Kiểm tra log khi gặp monster**
   - Đảm bảo debugMode = true trong Movement và GrassZoneManager
   - Di chuyển vào cỏ và theo dõi Console để xem:
     - Vị trí hiện tại 
     - Khu vực nào được áp dụng
     - Danh sách monster nào được sử dụng

## Cách thiết lập

1. **Tạo GrassZoneManager**
   - Tạo một GameObject mới trong scene và đặt tên là "GrassZoneManager"
   - Thêm component "GrassZoneManager"
   - Gán defaultEncounterList (đây là danh sách mặc định)

2. **Tạo các khu vực (GrassZone)**
   - Tạo các GameObject mới (Empty) và đặt tên phù hợp (ví dụ: "ForestZone", "MountainZone")
   - Thêm Box Collider 2D hoặc Polygon Collider 2D và đánh dấu "Is Trigger"
   - Điều chỉnh kích thước collider để bao quanh khu vực cỏ cần áp dụng danh sách monster
   - Thêm component "GrassZone"
   - Gán encounterList tương ứng với khu vực

3. **Cách hoạt động**
   - Khi game bắt đầu, mỗi GrassZone đăng ký với GrassZoneManager
   - Khi người chơi đi vào cỏ, hệ thống kiểm tra vị trí hiện tại
   - GrassZoneManager xác định khu vực nào chứa vị trí đó
   - Hệ thống sử dụng danh sách monster của khu vực tương ứng

## Một số mẹo khắc phục lỗi phổ biến

1. **Nếu không có khu vực nào được phát hiện**
   - Kiểm tra xem collider của GrassZone có đủ lớn không
   - Đảm bảo collider được đánh dấu "Is Trigger"
   - Kiểm tra log để xem vị trí hiện tại có match với bất kỳ khu vực nào không

2. **Nếu dùng sai danh sách monster**
   - Kiểm tra xem GrassZone đã được đăng ký thành công chưa
   - Kiểm tra thứ tự đăng ký nếu có các vùng chồng lấn
   - Đảm bảo encounterList đã được gán cho GrassZone

3. **Nếu tất cả các khu vực đều dùng danh sách mặc định**
   - Kiểm tra xem GrassZone có Collider2D không 
   - Đảm bảo collider bao quanh được vùng cỏ
   - Kiểm tra log vị trí để xem có nằm trong khu vực nào không 