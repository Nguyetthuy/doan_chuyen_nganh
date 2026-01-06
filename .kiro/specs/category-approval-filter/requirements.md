# Requirements Document

## Introduction

Sửa lỗi hiển thị danh mục trong dropdown menu và trang danh sách category. Hiện tại hệ thống đang hiển thị các danh mục ngay cả khi chúng không có hồ sơ nào được admin duyệt, gây nhầm lẫn cho người dùng.

## Glossary

- **Category**: Nhóm ngành nghề (ví dụ: Điện lạnh, Giáo dục)
- **UserProfile**: Hồ sơ ứng viên do người dùng tạo
- **ApprovalStatus**: Trạng thái duyệt hồ sơ ("Approved", "Pending", "Rejected")
- **CategoriesMenuViewComponent**: Component hiển thị dropdown menu danh mục ở header
- **CategoriesController**: Controller xử lý trang danh sách và chi tiết danh mục

## Requirements

### Requirement 1

**User Story:** Là một người dùng, tôi muốn chỉ thấy các danh mục có hồ sơ đã được duyệt trong dropdown menu, để tôi không bị nhầm lẫn khi click vào danh mục trống.

#### Acceptance Criteria

1. WHEN the system loads the categories dropdown menu, THE system SHALL only display categories that have at least one approved profile
2. WHEN a category has profiles but none are approved, THE system SHALL not display that category in the dropdown menu
3. WHEN a category has both approved and unapproved profiles, THE system SHALL display the category and show only approved profiles count
4. WHEN the dropdown menu displays a category, THE system SHALL show the correct count of approved profiles for that category
5. WHEN no categories have approved profiles, THE system SHALL display an appropriate message in the dropdown menu

### Requirement 2

**User Story:** Là một người dùng, tôi muốn trang danh sách danh mục chỉ hiển thị các danh mục có hồ sơ đã được duyệt, để tôi có thể dễ dàng tìm thấy hồ sơ ứng viên phù hợp.

#### Acceptance Criteria

1. WHEN the system displays the categories index page, THE system SHALL only show categories that have at least one approved profile
2. WHEN calculating profile counts for categories, THE system SHALL count only profiles with "Approved" status
3. WHEN a category has no approved profiles, THE system SHALL not display that category on the index page
4. WHEN displaying category cards, THE system SHALL show accurate approved profile counts
5. WHEN no categories have approved profiles, THE system SHALL display an appropriate empty state message

### Requirement 3

**User Story:** Là một người dùng, tôi muốn khi click vào một danh mục, tôi chỉ thấy các hồ sơ đã được admin duyệt, để đảm bảo chất lượng thông tin.

#### Acceptance Criteria

1. WHEN displaying profiles for a specific category, THE system SHALL only show profiles with "Approved" status
2. WHEN counting total profiles for pagination, THE system SHALL count only approved profiles
3. WHEN a category page has no approved profiles, THE system SHALL display an appropriate empty state message
4. WHEN calculating pagination, THE system SHALL base calculations on approved profiles count only
5. WHEN loading profiles via AJAX, THE system SHALL maintain the approved-only filter