# Hướng dẫn trình bày code Lab 3.3

## Luồng chạy tổng thể

`Play` → máy bay tự chạy đà → `HasTakenOff = true` → máy bay chạm `Enter Wall` → `EnterGame` gọi `GameManager.ShowPopup()` → popup hiện và game tạm dừng → người dùng chọn `RESTART` hoặc `CONTINUE`.

## Khi cô hỏi “đoạn này nằm ở đâu?”

### Popup được ẩn/hiện ở đâu?

File `Assets/Lab33/Scripts/GameManager.cs`.

- `Start()` ẩn Canvas bằng `gameMenu.enabled = false`.
- `ShowPopup()` bật Canvas và đặt `Time.timeScale = 0` để giữ nguyên khung cảnh lúc va chạm.

### Nội dung UI được nạp bằng code ở đâu?

Trong `GameManager.Start()`:

- `title.text`: tiêu đề popup.
- `message.text`: nội dung thông báo.
- `continueButton` và `restartButton`: tìm TMP_Text con rồi đổi chữ của nút.

Muốn đổi nội dung hiển thị thì sửa các chuỗi tại đây.

### Khi nào popup xuất hiện?

File `Assets/Lab33/Scripts/EnterGame.cs`.

`OnTriggerEnter(Collider other)` kiểm tra:

1. Vật thể chạm trigger có tag `Player`.
2. Máy bay đã cất cánh (`HasTakenOff`).
3. Sau đó gọi `gameManager.ShowPopup()`.

Script này được gắn vào GameObject `Enter Wall`.

### Làm sao biết máy bay đã cất cánh?

File `Assets/Lab33/Scripts/AirplaneTakeoffController.cs`.

Máy bay tự tiến lên. Khi đi đủ quãng đường chạy đà, script đặt `HasTakenOff = true`, nâng độ cao và xoay mũi máy bay.

### Nút Restart hoạt động ở đâu?

`GameManager.RestartScene()` lấy scene hiện tại bằng `SceneManager.GetActiveScene()` rồi tải lại scene đó.

### Nút Continue hoạt động ở đâu?

`GameManager.NextScene()` tải scene `Lab33_Complete`. Nút `CONTINUE` đã được nối với hàm này trong sự kiện `onClick`.

### UI được tạo ở đâu?

File `Assets/Lab33/Editor/Lab33ProjectBuilder.cs`.

- `CreatePopupCanvas()` tạo hierarchy Canvas/Panel/Frame/Text/Button.
- `CreateButton()` tạo Button và TMP_Text con.
- `CreateDemoScene()` gắn GameManager vào Main Camera, gán tham chiếu và nối `onClick`.
- `CreateCompleteScene()` tạo scene thứ hai để chứng minh Continue tải scene kế tiếp.

Đây là Editor script, chỉ dùng để dựng tài nguyên trong Unity Editor; mã gameplay nằm trong thư mục `Scripts`.

## Hierarchy quan trọng trong scene

```text
Main Camera
  └─ GameManager
Enter Wall
  └─ EnterGame
Airplane
  └─ AirplaneTakeoffController
GameMenu (Canvas, mặc định tắt)
  └─ Dimmer
      └─ Panel
          └─ Frame
              ├─ Runway Signal
              ├─ Eyebrow
              ├─ Title
              ├─ Message
              ├─ Divider
              ├─ Actions
              │   ├─ Restart
              │   └─ Continue
              └─ Footnote
EventSystem
```

## Nếu cô yêu cầu sửa nhanh UI

- Đổi chữ động: sửa `GameManager.Start()`.
- Đổi màu/kích thước/bố cục: chọn object trong `GameMenu` rồi sửa `Image`, `TMP_Text` hoặc `RectTransform` ở Inspector.
- Đổi hành vi nút: mở phần `Button > On Click()` hoặc sửa hàm trong `GameManager`.
- Nếu chạy lại menu dựng scene, các thay đổi trực tiếp trên scene có thể bị ghi đè.
