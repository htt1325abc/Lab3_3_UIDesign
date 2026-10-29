# Lab 3.3 — User Interface Design

Project Unity riêng cho yêu cầu từ slide 26 đến hết của bài Lab 3.3.

## Chạy bài

1. Mở project `Lab3_3_UIDesign` bằng Unity Hub (Unity 6.0/6000.5.10f1).
2. Mở scene `Assets/Lab33/Scenes/Lab33_UI_Popup.unity`.
3. Bấm Play. Máy bay tự chạy đà và cất cánh.
4. Khi máy bay đã cất cánh và đi qua `Enter Wall`, popup xuất hiện.
5. `RESTART` tải lại scene hiện tại; `CONTINUE` mở scene hoàn thành.

## Đối chiếu yêu cầu slide 26–34

- Popup được dựng bằng Canvas, Panel, Image, TextMeshPro và hai Button-TMP.
- Nội dung tiêu đề, thông báo và chữ trên nút được nạp từ `GameManager.Start()`.
- Canvas popup bị tắt khi scene bắt đầu.
- `EnterGame` chỉ gọi popup sau khi máy bay đã cất cánh và chạm trigger `Enter Wall`.
- `RestartScene()` tải lại scene hiện tại.
- `NextScene()` tải scene kế tiếp là `Lab33_Complete`.
- Hai sự kiện `Button.onClick` được nối sẵn với GameManager.
- GameManager nằm trên `Main Camera`; toàn bộ tham chiếu UI đã được gán.

## Cấu trúc chính

- `Assets/Lab33/Scripts`: mã chạy trong game.
- `Assets/Lab33/Editor/Lab33ProjectBuilder.cs`: mã dựng lại toàn bộ scene/material/UI.
- `Assets/Lab33/Scenes`: hai scene của bài.
- `Assets/Lab33/Materials`: vật liệu được tạo tự động.
- `Assets/Lab33/Fonts`: TMP Font Asset được tạo tự động.

Nếu cần dựng lại toàn bộ bài, chọn menu `Lab 3.3 > Build Complete UI Lab` trong Unity.
