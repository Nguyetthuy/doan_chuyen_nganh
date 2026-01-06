# Requirements Document

## Introduction

Hệ thống tạo và duyệt CV cho phép người dùng tạo ra hồ sơ CV chuyên nghiệp từ thông tin cá nhân đã nhập, sau đó gửi cho admin duyệt và hiển thị trên trang chủ khi được phê duyệt.

## Glossary

- **CV_System**: Hệ thống tạo và quản lý CV
- **User_Profile**: Thông tin cá nhân của người dùng bao gồm kinh nghiệm, học vấn, kỹ năng
- **CV_Template**: Mẫu CV được định dạng sẵn như hình mẫu
- **Admin_Panel**: Giao diện quản trị để duyệt CV
- **Homepage**: Trang chủ hiển thị các CV đã được duyệt
- **Approval_Status**: Trạng thái duyệt CV (pending, approved, rejected)

## Requirements

### Requirement 1

**User Story:** Là một người dùng, tôi muốn tạo CV từ thông tin cá nhân đã nhập, để có thể tạo ra một hồ sơ chuyên nghiệp.

#### Acceptance Criteria

1. WHEN người dùng đã hoàn thành đầy đủ thông tin cá nhân THEN CV_System SHALL cho phép tạo CV
2. WHEN người dùng bấm nút tạo CV THEN CV_System SHALL tạo CV theo mẫu với thông tin đã nhập
3. WHEN CV được tạo thành công THEN CV_System SHALL lưu CV với trạng thái pending
4. WHEN CV được tạo THEN CV_System SHALL hiển thị thông báo tạo thành công cho người dùng
5. WHEN thông tin cá nhân chưa đầy đủ THEN CV_System SHALL hiển thị thông báo yêu cầu hoàn thiện thông tin

### Requirement 2

**User Story:** Là một admin, tôi muốn duyệt các CV được gửi lên, để kiểm soát chất lượng nội dung hiển thị trên trang chủ.

#### Acceptance Criteria

1. WHEN admin truy cập trang duyệt CV THEN CV_System SHALL hiển thị danh sách CV đang chờ duyệt
2. WHEN admin xem chi tiết CV THEN CV_System SHALL hiển thị CV theo định dạng mẫu
3. WHEN admin duyệt CV THEN CV_System SHALL cập nhật trạng thái thành approved
4. WHEN admin từ chối CV THEN CV_System SHALL cập nhật trạng thái thành rejected
5. WHEN admin thực hiện hành động duyệt THEN CV_System SHALL ghi lại thời gian và người duyệt

### Requirement 3

**User Story:** Là một khách truy cập, tôi muốn xem các CV đã được duyệt trên trang chủ, để tìm kiếm ứng viên phù hợp.

#### Acceptance Criteria

1. WHEN khách truy cập vào trang chủ THEN CV_System SHALL hiển thị các CV đã được duyệt
2. WHEN CV được admin duyệt THEN CV_System SHALL tự động cập nhật hiển thị trên trang chủ
3. WHEN hiển thị CV trên trang chủ THEN CV_System SHALL sử dụng định dạng mẫu đã thiết kế
4. WHEN có nhiều CV được duyệt THEN CV_System SHALL sắp xếp theo thời gian duyệt mới nhất
5. WHEN khách click vào CV THEN CV_System SHALL hiển thị chi tiết CV đầy đủ

### Requirement 4

**User Story:** Là một người dùng, tôi muốn theo dõi trạng thái duyệt CV của mình, để biết khi nào CV được phê duyệt.

#### Acceptance Criteria

1. WHEN người dùng truy cập trang quản lý CV THEN CV_System SHALL hiển thị trạng thái hiện tại của CV
2. WHEN CV được admin duyệt hoặc từ chối THEN CV_System SHALL thông báo cho người dùng
3. WHEN CV bị từ chối THEN CV_System SHALL hiển thị lý do từ chối nếu có
4. WHEN người dùng có CV bị từ chối THEN CV_System SHALL cho phép chỉnh sửa và gửi lại
5. WHEN người dùng gửi lại CV THEN CV_System SHALL đặt lại trạng thái về pending

### Requirement 5

**User Story:** Là hệ thống, tôi cần đảm bảo CV được tạo theo đúng mẫu thiết kế, để duy trì tính nhất quán và chuyên nghiệp.

#### Acceptance Criteria

1. WHEN tạo CV THEN CV_System SHALL sử dụng template cố định như mẫu thiết kế
2. WHEN hiển thị thông tin THEN CV_System SHALL định dạng đúng các trường thông tin
3. WHEN thiếu thông tin bắt buộc THEN CV_System SHALL hiển thị placeholder hoặc ẩn section đó
4. WHEN có ảnh đại diện THEN CV_System SHALL hiển thị ảnh với kích thước chuẩn
5. WHEN không có ảnh đại diện THEN CV_System SHALL hiển thị ảnh mặc định