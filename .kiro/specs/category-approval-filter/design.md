# Design Document

## Overview

Thiết kế giải pháp để sửa lỗi hiển thị danh mục không có hồ sơ được duyệt. Hệ thống hiện tại đang hiển thị tất cả danh mục mà không kiểm tra trạng thái duyệt của hồ sơ, gây nhầm lẫn cho người dùng khi họ click vào danh mục trống.

## Architecture

Giải pháp sẽ cập nhật 3 thành phần chính:
1. **CategoriesMenuViewComponent**: Lọc danh mục có hồ sơ được duyệt cho dropdown menu
2. **CategoriesController.Index**: Đảm bảo chỉ hiển thị danh mục có hồ sơ được duyệt
3. **CategoriesController.Profiles**: Đảm bảo logic lọc hồ sơ được duyệt hoạt động chính xác

## Components and Interfaces

### CategoriesMenuViewComponent
- **Current Issue**: Lấy tất cả categories mà không kiểm tra approved profiles
- **Solution**: Thêm filter để chỉ lấy categories có ít nhất 1 approved profile
- **Method**: `InvokeAsync()` - cần cập nhật query logic

### CategoriesController
- **Index Action**: Đã có logic đúng nhưng cần verify
- **Profiles Action**: Đã có logic đúng nhưng cần verify
- **GetCategoryProfiles Action**: Đã có logic đúng nhưng cần verify

## Data Models

### Category
```csharp
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryDescription { get; set; }
    public ICollection<Profession> Professions { get; set; }
}
```

### UserProfile
```csharp
public class UserProfile
{
    public int ProfileId { get; set; }
    public string ApprovalStatus { get; set; } // "Approved", "Pending", "Rejected"
    public ICollection<UserProfileProfession> UserProfileProfessions { get; set; }
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property Reflection

After reviewing the prework analysis, I identified several redundant properties that can be consolidated:

- Properties 1.1 and 2.1 (dropdown and index page category filtering) can be combined into one comprehensive property
- Properties 1.4 and 2.4 (count accuracy) can be combined into one property
- Properties 2.2 and 3.2, 3.4 (counting logic) can be combined into one property
- Properties 1.2 and 2.3 are edge cases that will be handled by the main filtering property
- Properties 3.1 and 3.5 (profile filtering) can be combined into one property

### Correctness Properties

Property 1: Category visibility based on approved profiles
*For any* category in the system, it should only appear in dropdown menus and index pages if and only if it has at least one profile with "Approved" status
**Validates: Requirements 1.1, 1.2, 2.1, 2.3**

Property 2: Approved profile count accuracy
*For any* category displayed in the system, the profile count shown should equal the exact number of profiles with "Approved" status for that category
**Validates: Requirements 1.4, 2.4, 2.2**

Property 3: Profile filtering consistency
*For any* category profile page (including AJAX loads), only profiles with "Approved" status should be displayed to users
**Validates: Requirements 3.1, 3.5**

Property 4: Pagination calculation correctness
*For any* category profile page, pagination calculations (total pages, current page bounds) should be based exclusively on the count of approved profiles
**Validates: Requirements 3.2, 3.4**

## Error Handling

### Empty States
- **No approved profiles in any category**: Display helpful message in dropdown and index page
- **Category with no approved profiles**: Should not appear in listings
- **Category page with no approved profiles**: Display appropriate empty state message

### Data Consistency
- Ensure approval status checks are case-sensitive and exact match ("Approved")
- Handle null/empty approval status as non-approved
- Maintain consistency between different views (dropdown, index, profile pages)

## Testing Strategy

### Unit Testing
- Test individual controller actions with mock data
- Test ViewComponent with different approval status scenarios
- Test edge cases like empty categories, mixed approval statuses

### Property-Based Testing
Using xUnit with FsCheck for .NET Core:
- Generate random categories with various approval status combinations
- Verify filtering properties hold across all generated test cases
- Test pagination calculations with random approved profile counts
- Minimum 100 iterations per property test

Each property-based test will be tagged with format: **Feature: category-approval-filter, Property {number}: {property_text}**

### Integration Testing
- Test complete workflow from dropdown click to profile display
- Verify AJAX pagination maintains filtering
- Test empty state scenarios end-to-end