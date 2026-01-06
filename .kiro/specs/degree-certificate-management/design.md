# Design Document

## Overview

Hệ thống quản lý hồ sơ cá nhân hiện tại gặp vấn đề không thể thêm được bằng cấp và chứng chỉ. Sau khi phân tích code và database, tôi đã xác định được các nguyên nhân chính:

1. **Thiếu thư mục upload**: Thư mục `wwwroot/uploads/certificates` không tồn tại
2. **Thiếu dữ liệu tham chiếu**: Có thể không có dữ liệu trong bảng `DegreeTypes` và `CertificateTypes`
3. **Vấn đề database mapping**: Có inconsistency trong column mapping giữa model và database
4. **Error handling không đầy đủ**: Một số lỗi không được hiển thị rõ ràng cho người dùng

Giải pháp sẽ tập trung vào việc khắc phục các vấn đề này và cải thiện trải nghiệm người dùng.

## Architecture

Hệ thống sử dụng kiến trúc MVC với các thành phần chính:

- **Controllers**: ProfileController xử lý logic thêm bằng cấp/chứng chỉ
- **Models**: Degree, Certificate, DegreeType, CertificateType
- **Views**: Modal forms để thêm thông tin
- **Database**: SQL Server với Entity Framework Core
- **File Storage**: Local file system trong wwwroot/uploads

## Components and Interfaces

### 1. ProfileController
- **AddDegree**: Xử lý thêm bằng cấp mới
- **AddCertificate**: Xử lý thêm chứng chỉ mới
- **Index**: Hiển thị hồ sơ với modal forms

### 2. Database Models
- **Degree**: Thông tin bằng cấp
- **Certificate**: Thông tin chứng chỉ
- **DegreeType**: Loại bằng cấp (Master data)
- **CertificateType**: Loại chứng chỉ (Master data)

### 3. File Upload System
- **Upload Directory**: wwwroot/uploads/{degrees|certificates}
- **File Validation**: Type và size validation
- **Error Handling**: Graceful degradation khi upload fail

### 4. Modal Forms
- **_AddDegreeModal.cshtml**: Form thêm bằng cấp
- **_AddCertificateModal.cshtml**: Form thêm chứng chỉ

## Data Models

### Degree Model
```csharp
public class Degree
{
    public int DegreeId { get; set; }
    public int ProfileId { get; set; }
    public int? DegreeTypeId { get; set; }
    public string? DegreeName { get; set; }
    public string? Major { get; set; }
    public string? SchoolName { get; set; }
    public DateTime? GraduationYear { get; set; }
    public string? DegreeImg { get; set; }
}
```

### Certificate Model
```csharp
public class Certificate
{
    public int CertificateId { get; set; }
    public int ProfileId { get; set; }
    public int CertificateTypeId { get; set; }
    public string? CertificateName { get; set; }
    public string? IssueBy { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificateImg { get; set; }
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

Property 1: Degree validation consistency
*For any* degree input data, the validation logic should consistently accept valid data and reject invalid data with appropriate error messages
**Validates: Requirements 1.2, 1.4**

Property 2: Degree persistence round trip
*For any* valid degree data, saving to database and then retrieving should return equivalent degree information
**Validates: Requirements 1.3, 1.5**

Property 3: Certificate validation consistency
*For any* certificate input data, the validation logic should consistently accept valid data and reject invalid data with appropriate error messages
**Validates: Requirements 2.2, 2.4**

Property 4: Certificate persistence round trip
*For any* valid certificate data, saving to database and then retrieving should return equivalent certificate information
**Validates: Requirements 2.3, 2.5**

Property 5: Dropdown population consistency
*For any* set of degree types or certificate types in the database, the dropdown lists should be populated with all available options
**Validates: Requirements 3.2**

Property 6: CRUD operations consistency
*For any* valid admin user, all CRUD operations on degree types and certificate types should work correctly
**Validates: Requirements 3.4**

Property 7: File validation consistency
*For any* uploaded file, the validation should correctly identify valid and invalid files based on type and size constraints
**Validates: Requirements 4.1**

Property 8: File upload round trip
*For any* valid file, uploading and then accessing should return the same file content
**Validates: Requirements 4.2, 4.3**

Property 9: Image link consistency
*For any* degree or certificate with an associated image, the system should provide a valid link to view the uploaded file
**Validates: Requirements 4.5**

Property 10: Validation error collection
*For any* set of validation errors, the system should collect and display all errors clearly to the user
**Validates: Requirements 5.2**

Property 11: Logging completeness
*For any* system operation, sufficient logging information should be provided for troubleshooting purposes
**Validates: Requirements 5.5**

## Error Handling

### 1. Database Errors
- **Connection failures**: Graceful degradation with user-friendly messages
- **Constraint violations**: Clear validation error messages
- **Timeout errors**: Retry logic with fallback options

### 2. File Upload Errors
- **Directory not found**: Auto-create directories if missing
- **File size exceeded**: Clear size limit messages
- **Invalid file types**: List of accepted file types
- **Disk space issues**: Graceful degradation without breaking main functionality

### 3. Validation Errors
- **Required field validation**: Clear field-specific error messages
- **Data type validation**: Format-specific error guidance
- **Business rule validation**: Context-aware error explanations
- **Cross-field validation**: Relationship-specific error messages

### 4. UI/UX Error Handling
- **Modal display errors**: Fallback to inline forms
- **JavaScript errors**: Progressive enhancement approach
- **Network errors**: Retry mechanisms with user feedback
- **Session timeout**: Automatic redirect to login with state preservation

## Testing Strategy

### Unit Testing Approach
- **Controller Actions**: Test each AddDegree and AddCertificate action with various input scenarios
- **Validation Logic**: Test validation methods with edge cases and boundary values
- **File Upload Logic**: Test file handling with different file types and sizes
- **Database Operations**: Test CRUD operations with mock data
- **Error Handling**: Test exception scenarios and error message generation

### Property-Based Testing Approach
- **Framework**: Use NUnit with FsCheck.NUnit for property-based testing in C#
- **Test Configuration**: Run minimum 100 iterations per property test
- **Property Test Tagging**: Each property-based test must include a comment with format: '**Feature: degree-certificate-management, Property {number}: {property_text}**'
- **Generator Strategy**: Create smart generators that produce realistic test data within valid input domains
- **Shrinking Strategy**: Use FsCheck's built-in shrinking to find minimal failing examples

### Integration Testing
- **Database Integration**: Test with real database connections and transactions
- **File System Integration**: Test actual file upload and retrieval operations
- **End-to-End Workflows**: Test complete user journeys from modal display to data persistence
- **Error Recovery**: Test system behavior under various failure conditions

### Test Data Management
- **Reference Data**: Ensure test databases have required DegreeTypes and CertificateTypes
- **File Cleanup**: Automatic cleanup of test files after test execution
- **Database Isolation**: Use transactions or separate test databases for isolation
- **Mock Services**: Mock external dependencies while testing core functionality

The dual testing approach ensures comprehensive coverage: unit tests catch specific bugs and edge cases, while property tests verify that universal properties hold across all possible inputs, providing confidence in system correctness.