# 📘 TÀI LIỆU THUẬT TOÁN FSRS (Flexible Spaced Repetition Scheduler)

---

## I. Định nghĩa & Lý do sử dụng thuật toán FSRS

### 1. Khái niệm
**FSRS (Flexible Spaced Repetition Scheduler)** là một thuật toán lập lịch ôn tập dựa trên nguyên lý **“spaced repetition” (lặp lại ngắt quãng)** nhằm tối ưu hiệu quả ghi nhớ của người học.  
Thuật toán này sử dụng các chỉ số cá nhân hóa như **Stability**, **Difficulty** và **Retention** để điều chỉnh khoảng cách giữa các lần ôn tập, giúp người học dành nhiều thời gian hơn cho các thẻ khó và tiết kiệm thời gian với thẻ đã nắm vững.

### 2. Lý do sử dụng
- ✅ Cá nhân hóa quá trình học tập cho từng thẻ và từng người dùng.  
- ✅ Tối ưu hóa lịch ôn tập dựa trên phản hồi thực tế.  
- ✅ Đảm bảo hiệu quả ghi nhớ lâu dài, tránh học dàn trải hoặc quên lãng.  

---

## II. Công thức toán học & Định nghĩa các biến

### 1. Stability (S) – Độ ổn định trí nhớ

**Công thức:**  
```math
S_new = S_old × (1 + α × (rating - 2))
```
**Giải thích:**  
- *S_old*: Stability trước lần ôn tập.  
- *S_new*: Stability sau khi cập nhật.  
- *α (alpha)*: Hệ số điều chỉnh độ nhạy (0.2–0.4).  
- *rating*: Đánh giá của người học (0 = quên, 1 = hơi nhớ, 2 = nhớ, 3 = nhớ xuất sắc).  

**Mục đích:**  
Tăng Stability khi người học nhớ tốt, giảm khi quên. Giúp hệ thống giãn cách hợp lý giữa các lần ôn tập, tùy theo năng lực ghi nhớ từng thẻ.

---

### 2. Difficulty (D) – Độ khó của thẻ

**Công thức:**  
```math
D_new = D_old + β × (expected_rating - rating)
```
**Giải thích:**  
- *D_old*: Difficulty trước khi ôn tập.  
- *D_new*: Difficulty sau khi cập nhật.  
- *β (beta)*: Hệ số điều chỉnh (0.1–0.2).  
- *expected_rating*: Giá trị kỳ vọng (thường là 2).  
- *rating*: Đánh giá của người học.  

**Mục đích:**  
Nếu người học đánh giá thấp hơn kỳ vọng, Difficulty tăng (thẻ khó hơn); nếu đánh giá cao hơn, Difficulty giảm (thẻ dễ hơn). Giúp hệ thống xác định thẻ nào cần ôn lại nhiều hơn.

---

### 3. Retention (R) – Xác suất nhớ thẻ

**Công thức:**  
```math
R = exp(-Δt / S)
```
**Giải thích:**  
- *Δt*: Số ngày từ lần ôn tập trước (thời gian trôi qua).  
- *S*: Stability hiện tại của thẻ.  

**Mục đích:**  
Retention giảm theo thời gian, giúp hệ thống xác định nguy cơ quên thẻ và ưu tiên lên lịch ôn lại.

---

### 4. Định nghĩa & Lý do các biến

| Biến | Định nghĩa | Lý do sử dụng |
|------|-------------|----------------|
| α (alpha) | Độ nhạy tăng/giảm Stability | Cá nhân hóa tốc độ giãn cách, phù hợp từng nhóm người học |
| β (beta) | Độ nhạy tăng/giảm Difficulty | Cá nhân hóa khả năng điều chỉnh độ khó của thẻ |
| expected_rating | Giá trị kỳ vọng của lần ôn tập | Là mốc chuẩn để so sánh với đánh giá thực tế |
| rating | Đánh giá của người học (0–3) | Đầu vào cho mọi thuật toán cập nhật |
| S_old / S_new | Stability trước và sau khi ôn tập | Quản lý khoảng cách đề xuất cho lần ôn tập tiếp theo |
| D_old / D_new | Difficulty trước và sau khi ôn tập | Quản lý độ khó, cá nhân hóa lịch ôn tập |
| Δt | Số ngày từ lần ôn tập trước | Đầu vào để tính Retention |
| R | Xác suất nhớ thẻ | Giúp ưu tiên sắp xếp thẻ cần ôn tập |

---

## III. Luồng học tập và các trường hợp xử lý

### 1. Luồng trạng thái thẻ

- **New (Mới):** Thẻ vừa tạo, chưa được ôn tập lần nào.  
- **Learning (Đang học):** Người dùng bắt đầu học thẻ, nhập rating.  
- **Review (Ôn lại):** Thẻ đã học trước đó, được lên lịch ôn lại dựa trên Stability.  
- **Relearning (Ôn lại sau khi quên):** Nếu đánh giá là quên hoặc Retention thấp, thẻ chuyển sang trạng thái “học lại”.  

---

### 2. Các trường hợp trong luồng học tập

| Trường hợp | Mô tả | Xử lý |
|-------------|-------|--------|
| Thẻ mới được tạo | Chưa có dữ liệu, khởi tạo Stability & Difficulty mặc định | Chuyển sang Learning, nhập rating đầu tiên |
| Người học đánh giá tốt | Rating ≥ expected_rating | Stability tăng, Difficulty giảm, giãn cách ôn tập |
| Người học đánh giá kém | Rating < expected_rating | Stability giảm, Difficulty tăng, rút ngắn khoảng ôn tập |
| Retention thấp (< threshold) | Xác suất nhớ thấp, nguy cơ quên thẻ | Chuyển sang Relearning, tăng số lần ôn lại |
| Lặp lại ôn tập nhiều lần | Theo dõi các lần đánh giá, cập nhật trạng thái | Cá nhân hóa tham số, tối ưu lịch ôn tập |

---

### 3. Luồng dữ liệu

1. Người dùng ôn tập thẻ → nhập **rating**  
2. Hệ thống tính toán **Stability** và **Difficulty** mới  
3. Cập nhật **CardState**  
4. Tính **Retention**, xác định lịch ôn tập tiếp theo  
5. Nếu cần, chuyển trạng thái thẻ (**Review / Relearning**)  
6. Lưu thông tin vào **ReviewHistory**  

---

## IV. Nguồn tài liệu tham khảo

- 🔗 **FSRS - Flexible Spaced Repetition Scheduler**  
  [https://github.com/open-spaced-repetition/fsrs](https://github.com/open-spaced-repetition/fsrs)  
  [https://github.com/open-spaced-repetition/fsrs-rs](https://github.com/open-spaced-repetition/fsrs-rs)  
  [https://zhuanlan.zhihu.com/p/595345033](https://zhuanlan.zhihu.com/p/595345033)  

- 📘 **Spaced Repetition & SM2 Algorithm:**  
  [https://www.supermemo.com/english/ol/sm2.htm](https://www.supermemo.com/english/ol/sm2.htm)  
  [https://en.wikipedia.org/wiki/Spaced_repetition](https://en.wikipedia.org/wiki/Spaced_repetition)  

- 🧠 **Ứng dụng thực tế (Anki):**  
  [https://docs.ankiweb.net/#/deck-options?id=custom-scheduling](https://docs.ankiweb.net/#/deck-options?id=custom-scheduling)

---

**PrivateGPT Project © 2025 – FSRS Algorithm Implementation (C# / DDD / ML Integration)**  
