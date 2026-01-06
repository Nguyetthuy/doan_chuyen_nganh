# Requirements Document

## Introduction

Hệ thống quản lý hồ sơ cá nhân hiện tại gặp vấn đề không thể thêm được bằng cấp và chứng chỉ. Tính năng này rất quan trọng để người dùng có thể hoàn thiện hồ sơ của mình và tăng cơ hội tìm việc làm.

## Glossary

- **System**: Hệ thống quản lý hồ sơ cá nhân finder_work
- **User**: Người dùng có tài khoản trong hệ thống
- **Profile**: Hồ sơ cá nhân của người dùng
- **Degree**: Bằng cấp học vấn của người dùng
- **Certificate**: Chứng chỉ nghề nghiệp của người dùng
- **DegreeType**: Loại bằng cấp (Cử nhân, Thạc sĩ, Tiến sĩ, v.v.)
- **CertificateType**: Loại chứng chỉ (Công nghệ, Ngoại ngữ, Quản lý, v.v.)
- **Modal**: Cửa sổ popup để thêm thông tin
- **Database**: Cơ sở dữ liệu SQL Server lưu trữ thông tin

## Requirements

### Requirement 1

**User Story:** Là một người dùng đã đăng nhập, tôi muốn thêm bằng cấp vào hồ sơ của mình, để nhà tuyển dụng có thể thấy trình độ học vấn của tôi.

#### Acceptance Criteria

1. WHEN a user clicks the "Thêm" button in the Học vấn section THEN the System SHALL display the degree addition modal
2. WHEN a user fills in required degree information and submits THEN the System SHALL validate the input data
3. WHEN degree data is valid THEN the System SHALL save the degree to the database and display success message
4. WHEN degree data is invalid THEN the System SHALL display appropriate error messages
5. WHEN a degree is successfully added THEN the System SHALL refresh the profile page and show the new degree

### Requirement 2

**User Story:** Là một người dùng đã đăng nhập, tôi muốn thêm chứng chỉ vào hồ sơ của mình, để chứng minh kỹ năng chuyên môn của tôi.

#### Acceptance Criteria

1. WHEN a user clicks the "Thêm" button in the Chứng chỉ section THEN the System SHALL display the certificate addition modal
2. WHEN a user fills in required certificate information and submits THEN the System SHALL validate the input data
3. WHEN certificate data is valid THEN the System SHALL save the certificate to the database and display success message
4. WHEN certificate data is invalid THEN the System SHALL display appropriate error messages
5. WHEN a certificate is successfully added THEN the System SHALL refresh the profile page and show the new certificate

### Requirement 3

**User Story:** Là một admin, tôi muốn quản lý các loại bằng cấp và chứng chỉ, để người dùng có thể chọn đúng loại khi thêm thông tin.

#### Acceptance Criteria

1. WHEN the System starts THEN the System SHALL ensure default degree types and certificate types exist in the database
2. WHEN loading degree or certificate forms THEN the System SHALL populate dropdown lists with available types
3. WHEN dropdown data fails to load THEN the System SHALL handle the error gracefully and show appropriate messages
4. WHEN admin accesses management pages THEN the System SHALL allow CRUD operations on degree types and certificate types
5. WHEN reference data is missing THEN the System SHALL provide fallback options or clear error messages

### Requirement 4

**User Story:** Là một người dùng, tôi muốn upload ảnh bằng cấp và chứng chỉ, để có thể chứng minh tính xác thực của thông tin.

#### Acceptance Criteria

1. WHEN a user selects an image file for degree or certificate THEN the System SHALL validate file type and size
2. WHEN file validation passes THEN the System SHALL upload the file to the appropriate directory
3. WHEN file upload succeeds THEN the System SHALL save the file path to the database
4. WHEN file upload fails THEN the System SHALL save the record without image and show warning message
5. WHEN viewing degrees or certificates with images THEN the System SHALL provide links to view the uploaded files

### Requirement 5

**User Story:** Là một developer, tôi muốn hệ thống có logging và error handling tốt, để có thể debug và khắc phục lỗi nhanh chóng.

#### Acceptance Criteria

1. WHEN any database operation fails THEN the System SHALL log the error details and show user-friendly messages
2. WHEN validation fails THEN the System SHALL collect all validation errors and display them clearly
3. WHEN file operations fail THEN the System SHALL handle the error gracefully without breaking the main functionality
4. WHEN unexpected errors occur THEN the System SHALL log the full exception and redirect to safe state
5. WHEN debugging is needed THEN the System SHALL provide sufficient logging information for troubleshooting