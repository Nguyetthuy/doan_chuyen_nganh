# Design Document

## Overview

Hệ thống tạo và duyệt CV sẽ mở rộng từ hệ thống quản lý hồ sơ hiện tại để cho phép người dùng tạo CV theo mẫu chuyên nghiệp, gửi cho admin duyệt và hiển thị trên trang chủ khi được phê duyệt. Hệ thống sử dụng ASP.NET Core MVC với Entity Framework và SQL Server.

## Architecture

### Kiến trúc tổng thể
- **Presentation Layer**: ASP.NET Core MVC Controllers và Razor Views
- **Business Logic Layer**: Service classes xử lý logic nghiệp vụ
- **Data Access Layer**: Entity Framework Core với Repository pattern
- **Database Layer**: SQL Server với các bảng mở rộng từ schema hiện tại

### Luồng dữ liệu chính
1. User tạo CV từ UserProfile đã có
2. CV được lưu với trạng thái "Pending"
3. Admin duyệt CV thông qua Admin Panel
4. CV được duyệt sẽ hiển thị trên Homepage
5. User có thể theo dõi trạng thái CV

## Components and Interfaces

### 1. Models mở rộng

#### CV Model (mới)
```csharp
public class CV
{
    public int CVId { get; set; }
    public int ProfileId { get; set; }
    public string CVTitle { get; set; }
    public string TemplateType { get; set; } = "Professional"; // Loại template
    public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? RejectionReason { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; } = false; // Hiển thị trên trang chủ
    
    // Navigation properties
    public virtual UserProfile Profile { get; set; }
    public virtual User? ApprovedByUser { get; set; }
}
```

#### Mở rộng UserProfile Model
```csharp
// Thêm vào UserProfile hiện tại
public virtual ICollection<CV> CVs { get; set; } = new List<CV>();
```

### 2. Controllers

#### CVController (mới)
- `Index()`: Hiển thị danh sách CV của user
- `Create(int profileId)`: Tạo CV từ profile
- `Preview(int cvId)`: Xem trước CV
- `Edit(int cvId)`: Chỉnh sửa CV
- `Delete(int cvId)`: Xóa CV
- `Resubmit(int cvId)`: Gửi lại CV bị từ chối

#### Mở rộng Admin/ManagementController
- `CVApproval()`: Trang duyệt CV
- `ApproveCV(int cvId)`: Duyệt CV
- `RejectCV(int cvId, string reason)`: Từ chối CV
- `CVDetails(int cvId)`: Xem chi tiết CV

#### Mở rộng HomeController
- Cập nhật `Index()`: Hiển thị CV đã duyệt
- `CVDetail(int cvId)`: Xem chi tiết CV công khai

### 3. Services

#### CVService (mới)
```csharp
public interface ICVService
{
    Task<CV> CreateCVFromProfileAsync(int profileId, string title);
    Task<CV> GetCVByIdAsync(int cvId);
    Task<List<CV>> GetUserCVsAsync(int userId);
    Task<List<CV>> GetPendingCVsAsync();
    Task<List<CV>> GetApprovedCVsAsync();
    Task<bool> ApproveCVAsync(int cvId, int approvedBy);
    Task<bool> RejectCVAsync(int cvId, int rejectedBy, string reason);
    Task<string> GenerateCVHtmlAsync(int cvId);
}
```

#### CVTemplateService (mới)
```csharp
public interface ICVTemplateService
{
    Task<string> RenderCVAsync(CV cv, string templateType = "Professional");
    Task<byte[]> GenerateCVPdfAsync(int cvId);
    List<string> GetAvailableTemplates();
}
```

### 4. Views

#### CV Views (mới)
- `Views/CV/Index.cshtml`: Danh sách CV của user
- `Views/CV/Create.cshtml`: Tạo CV mới
- `Views/CV/Preview.cshtml`: Xem trước CV
- `Views/CV/Edit.cshtml`: Chỉnh sửa CV

#### Admin Views (mở rộng)
- `Areas/Admin/Views/Management/CVApproval.cshtml`: Duyệt CV
- `Areas/Admin/Views/Management/CVDetails.cshtml`: Chi tiết CV

#### Partial Views (mới)
- `Views/Shared/_CVTemplate.cshtml`: Template CV chuyên nghiệp
- `Views/Shared/_CVCard.cshtml`: Card hiển thị CV trên trang chủ

## Data Models

### Database Schema Changes

#### Bảng CVs (mới)
```sql
CREATE TABLE CVs (
    CVId INT IDENTITY(1,1) PRIMARY KEY,
    ProfileId INT NOT NULL,
    CVTitle NVARCHAR(200) NOT NULL,
    TemplateType NVARCHAR(50) NOT NULL DEFAULT 'Professional',
    ApprovalStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    RejectionReason NVARCHAR(500) NULL,
    ApprovedBy INT NULL,
    ApprovedDate DATETIME2 NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsPublic BIT NOT NULL DEFAULT 0,
    
    FOREIGN KEY (ProfileId) REFERENCES UserProfiles(ProfileId),
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserId)
);
```

#### Indexes
```sql
CREATE INDEX IX_CVs_ProfileId ON CVs(ProfileId);
CREATE INDEX IX_CVs_ApprovalStatus ON CVs(ApprovalStatus);
CREATE INDEX IX_CVs_IsPublic ON CVs(IsPublic);
CREATE INDEX IX_CVs_ApprovedDate ON CVs(ApprovedDate);
```

### Entity Relationships
- CV -> UserProfile (Many-to-One)
- CV -> User (Many-to-One, ApprovedBy)
- UserProfile -> CV (One-to-Many)

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property Reflection

Trước khi định nghĩa các thuộc tính, tôi cần phân tích các tiêu chí chấp nhận từ requirements:

**Acceptance Criteria Testing Prework:**

1.1 WHEN người dùng đã hoàn thành đầy đủ thông tin cá nhân THEN CV_System SHALL cho phép tạo CV
Thoughts: Đây là một quy tắc áp dụng cho tất cả người dùng. Chúng ta có thể tạo profile ngẫu nhiên với thông tin đầy đủ và kiểm tra xem hệ thống có cho phép tạo CV không.
Testable: yes - property

1.2 WHEN người dùng bấm nút tạo CV THEN CV_System SHALL tạo CV theo mẫu với thông tin đã nhập
Thoughts: Đây là quy tắc về việc tạo CV từ thông tin profile. Chúng ta có thể tạo profile ngẫu nhiên và kiểm tra CV được tạo có chứa thông tin từ profile không.
Testable: yes - property

1.3 WHEN CV được tạo thành công THEN CV_System SHALL lưu CV với trạng thái pending
Thoughts: Đây là quy tắc về trạng thái mặc định của CV mới tạo. Áp dụng cho tất cả CV.
Testable: yes - property

1.4 WHEN CV được tạo THEN CV_System SHALL hiển thị thông báo tạo thành công cho người dùng
Thoughts: Đây là về UI feedback, có thể test được bằng cách kiểm tra response hoặc UI state.
Testable: yes - property

1.5 WHEN thông tin cá nhân chưa đầy đủ THEN CV_System SHALL hiển thị thông báo yêu cầu hoàn thiện thông tin
Thoughts: Đây là về validation, có thể test bằng cách tạo profile thiếu thông tin và kiểm tra thông báo lỗi.
Testable: yes - property

2.1 WHEN admin truy cập trang duyệt CV THEN CV_System SHALL hiển thị danh sách CV đang chờ duyệt
Thoughts: Đây là về hiển thị danh sách, có thể test bằng cách tạo CV pending và kiểm tra chúng có xuất hiện trong danh sách không.
Testable: yes - property

2.2 WHEN admin xem chi tiết CV THEN CV_System SHALL hiển thị CV theo định dạng mẫu
Thoughts: Đây là về rendering CV, có thể test bằng cách kiểm tra output HTML chứa các thành phần cần thiết.
Testable: yes - property

2.3 WHEN admin duyệt CV THEN CV_System SHALL cập nhật trạng thái thành approved
Thoughts: Đây là quy tắc về state transition, áp dụng cho tất cả CV.
Testable: yes - property

2.4 WHEN admin từ chối CV THEN CV_System SHALL cập nhật trạng thái thành rejected
Thoughts: Đây là quy tắc về state transition, áp dụng cho tất cả CV.
Testable: yes - property

2.5 WHEN admin thực hiện hành động duyệt THEN CV_System SHALL ghi lại thời gian và người duyệt
Thoughts: Đây là về audit trail, áp dụng cho tất cả hành động duyệt.
Testable: yes - property

3.1 WHEN khách truy cập vào trang chủ THEN CV_System SHALL hiển thị các CV đã được duyệt
Thoughts: Đây là về filtering và hiển thị, có thể test bằng cách tạo CV approved và kiểm tra chúng xuất hiện trên trang chủ.
Testable: yes - property

3.2 WHEN CV được admin duyệt THEN CV_System SHALL tự động cập nhật hiển thị trên trang chủ
Thoughts: Đây là về real-time update hoặc consistency, có thể test bằng cách duyệt CV và kiểm tra trang chủ.
Testable: yes - property

3.3 WHEN hiển thị CV trên trang chủ THEN CV_System SHALL sử dụng định dạng mẫu đã thiết kế
Thoughts: Đây là về template consistency, có thể test bằng cách kiểm tra HTML output.
Testable: yes - property

3.4 WHEN có nhiều CV được duyệt THEN CV_System SHALL sắp xếp theo thời gian duyệt mới nhất
Thoughts: Đây là về sorting logic, có thể test bằng cách tạo nhiều CV với thời gian duyệt khác nhau.
Testable: yes - property

3.5 WHEN khách click vào CV THEN CV_System SHALL hiển thị chi tiết CV đầy đủ
Thoughts: Đây là về navigation và hiển thị detail, có thể test bằng cách kiểm tra detail view chứa đầy đủ thông tin.
Testable: yes - property

4.1 WHEN người dùng truy cập trang quản lý CV THEN CV_System SHALL hiển thị trạng thái hiện tại của CV
Thoughts: Đây là về status display, có thể test bằng cách tạo CV với các trạng thái khác nhau.
Testable: yes - property

4.2 WHEN CV được admin duyệt hoặc từ chối THEN CV_System SHALL thông báo cho người dùng
Thoughts: Đây là về notification system, có thể test bằng cách kiểm tra notification được tạo.
Testable: yes - property

4.3 WHEN CV bị từ chối THEN CV_System SHALL hiển thị lý do từ chối nếu có
Thoughts: Đây là về displaying rejection reason, có thể test bằng cách từ chối CV với lý do và kiểm tra hiển thị.
Testable: yes - property

4.4 WHEN người dùng có CV bị từ chối THEN CV_System SHALL cho phép chỉnh sửa và gửi lại
Thoughts: Đây là về workflow state, có thể test bằng cách kiểm tra UI cho phép resubmit.
Testable: yes - property

4.5 WHEN người dùng gửi lại CV THEN CV_System SHALL đặt lại trạng thái về pending
Thoughts: Đây là về state reset, có thể test bằng cách resubmit và kiểm tra trạng thái.
Testable: yes - property

5.1 WHEN tạo CV THEN CV_System SHALL sử dụng template cố định như mẫu thiết kế
Thoughts: Đây là về template consistency, có thể test bằng cách kiểm tra HTML structure.
Testable: yes - property

5.2 WHEN hiển thị thông tin THEN CV_System SHALL định dạng đúng các trường thông tin
Thoughts: Đây là về data formatting, có thể test bằng cách kiểm tra format của từng field.
Testable: yes - property

5.3 WHEN thiếu thông tin bắt buộc THEN CV_System SHALL hiển thị placeholder hoặc ẩn section đó
Thoughts: Đây là về handling missing data, có thể test bằng cách tạo profile thiếu thông tin.
Testable: yes - property

5.4 WHEN có ảnh đại diện THEN CV_System SHALL hiển thị ảnh với kích thước chuẩn
Thoughts: Đây là về image handling, có thể test bằng cách kiểm tra image attributes.
Testable: yes - property

5.5 WHEN không có ảnh đại diện THEN CV_System SHALL hiển thị ảnh mặc định
Thoughts: Đây là về default image handling, có thể test bằng cách tạo profile không có ảnh.
Testable: yes - property

**Property Reflection:**

Sau khi phân tích các tiêu chí chấp nhận, tôi nhận thấy một số thuộc tính có thể được hợp nhất để tránh trùng lặp:

- Properties 1.2 và 5.1 đều về việc tạo CV theo template - có thể hợp nhất thành một property về template consistency
- Properties 2.2, 3.3, và 5.2 đều về việc hiển thị CV theo định dạng mẫu - có thể hợp nhất
- Properties 2.3 và 2.4 về state transitions có thể hợp nhất thành một property về approval workflow
- Properties 5.4 và 5.5 về image handling có thể hợp nhất

**Property 1: CV Creation Validation**
*For any* user profile, the system should only allow CV creation when all required information (summary, location, skills, professions) is complete
**Validates: Requirements 1.1, 1.5**

**Property 2: CV Template Consistency**
*For any* CV created from a user profile, the generated CV should use the professional template and contain all information from the source profile
**Validates: Requirements 1.2, 5.1, 5.2**

**Property 3: CV Initial State**
*For any* newly created CV, the system should save it with "Pending" approval status and set creation timestamp
**Validates: Requirements 1.3**

**Property 4: CV Creation Feedback**
*For any* successful CV creation, the system should provide success notification to the user
**Validates: Requirements 1.4**

**Property 5: Admin CV Filtering**
*For any* admin accessing the CV approval page, the system should display only CVs with "Pending" status
**Validates: Requirements 2.1**

**Property 6: CV Approval State Transition**
*For any* CV in "Pending" status, when admin performs approval or rejection action, the system should update status accordingly and record approver and timestamp
**Validates: Requirements 2.3, 2.4, 2.5**

**Property 7: Homepage CV Display**
*For any* visitor accessing the homepage, the system should display only CVs with "Approved" status, sorted by approval date (newest first)
**Validates: Requirements 3.1, 3.2, 3.4**

**Property 8: CV Template Rendering**
*For any* CV being displayed (admin review, homepage, or detail view), the system should render it using the consistent professional template format
**Validates: Requirements 2.2, 3.3**

**Property 9: CV Detail Navigation**
*For any* approved CV clicked on homepage, the system should display the complete CV detail with all profile information
**Validates: Requirements 3.5**

**Property 10: User CV Status Tracking**
*For any* user accessing their CV management page, the system should display current status of all their CVs
**Validates: Requirements 4.1**

**Property 11: CV Status Notification**
*For any* CV status change (approved/rejected), the system should create appropriate notification for the CV owner
**Validates: Requirements 4.2**

**Property 12: Rejection Reason Display**
*For any* rejected CV, if rejection reason exists, the system should display it to the CV owner
**Validates: Requirements 4.3**

**Property 13: CV Resubmission Workflow**
*For any* rejected CV, the system should allow the owner to edit and resubmit, which resets status to "Pending"
**Validates: Requirements 4.4, 4.5**

**Property 14: Missing Information Handling**
*For any* CV with incomplete profile data, the system should either show placeholders or hide empty sections gracefully
**Validates: Requirements 5.3**

**Property 15: Avatar Image Handling**
*For any* CV being rendered, the system should display user avatar with standard dimensions if available, or default avatar if not
**Validates: Requirements 5.4, 5.5**

## Error Handling

### Validation Errors
- **Incomplete Profile**: Kiểm tra profile có đầy đủ thông tin bắt buộc trước khi cho phép tạo CV
- **Invalid CV State**: Kiểm tra trạng thái CV hợp lệ trước khi thực hiện các hành động
- **Permission Validation**: Đảm bảo user chỉ có thể thao tác với CV của mình
- **Admin Authorization**: Kiểm tra quyền admin trước khi cho phép duyệt CV

### Database Errors
- **Concurrency Handling**: Sử dụng optimistic concurrency để xử lý cập nhật đồng thời
- **Transaction Management**: Đảm bảo tính nhất quán dữ liệu khi cập nhật trạng thái CV
- **Foreign Key Constraints**: Xử lý lỗi khi profile hoặc user không tồn tại

### File Handling Errors
- **Template Loading**: Xử lý lỗi khi không thể load template CV
- **Image Processing**: Xử lý lỗi khi ảnh đại diện không hợp lệ hoặc không tồn tại
- **PDF Generation**: Xử lý lỗi khi tạo file PDF từ CV

### Business Logic Errors
- **Duplicate CV**: Ngăn chặn tạo nhiều CV từ cùng một profile (nếu cần)
- **Invalid State Transition**: Ngăn chặn chuyển đổi trạng thái không hợp lệ
- **Approval Workflow**: Đảm bảo chỉ admin mới có thể duyệt CV

## Testing Strategy

### Dual Testing Approach

Hệ thống sẽ sử dụng cả unit testing và property-based testing để đảm bảo tính đúng đắn:

**Unit Testing:**
- Test các method cụ thể trong CVService và CVTemplateService
- Test các controller action với input/output cụ thể
- Test các edge case như profile thiếu thông tin, CV không tồn tại
- Test integration giữa các component

**Property-Based Testing:**
- Sử dụng **FsCheck** cho .NET để implement property-based testing
- Mỗi property-based test sẽ chạy tối thiểu **100 iterations** với dữ liệu ngẫu nhiên
- Mỗi property test sẽ được tag với comment tham chiếu đến correctness property tương ứng
- Format tag: **Feature: cv-generation-approval, Property {number}: {property_text}**

**Property-Based Test Requirements:**
- Property 1: Test với random profiles có/không có đầy đủ thông tin
- Property 2: Test với random profile data và verify CV content
- Property 3: Test với random CV creation và verify initial state
- Property 4: Test với random successful CV creation và verify notification
- Property 5: Test với random CV states và verify admin filtering
- Property 6: Test với random approval/rejection actions và verify state changes
- Property 7: Test với random approved CVs và verify homepage display
- Property 8: Test với random CVs và verify template consistency
- Property 9: Test với random approved CVs và verify detail navigation
- Property 10: Test với random user CVs và verify status display
- Property 11: Test với random status changes và verify notifications
- Property 12: Test với random rejected CVs và verify reason display
- Property 13: Test với random rejected CVs và verify resubmission workflow
- Property 14: Test với random incomplete profiles và verify graceful handling
- Property 15: Test với random avatar states và verify image handling

**Test Data Generation:**
- Random UserProfile với các trường thông tin ngẫu nhiên
- Random CV với các trạng thái khác nhau
- Random User với quyền admin/user
- Random timestamps để test sorting và workflow

**Integration Testing:**
- Test end-to-end workflow từ tạo CV đến hiển thị trên homepage
- Test admin approval workflow
- Test user notification system
- Test CV template rendering với real data

**Performance Testing:**
- Test hiệu suất khi có nhiều CV trên homepage
- Test hiệu suất template rendering với profile phức tạp
- Test database query performance với large dataset