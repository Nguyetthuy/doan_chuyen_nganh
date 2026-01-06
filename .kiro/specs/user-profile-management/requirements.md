# Requirements Document

## Introduction

Hệ thống quản lý hồ sơ người dùng trong ứng dụng Finder Work hiện tại đang gặp lỗi khi người dùng truy cập vào phần "Hồ sơ của tôi". Cần sửa lỗi và cải thiện chức năng để người dùng có thể tạo, xem và chỉnh sửa hồ sơ cá nhân một cách ổn định và đầy đủ theo cấu trúc cơ sở dữ liệu hiện có.

## Glossary

- **User_Profile_System**: Hệ thống quản lý hồ sơ người dùng trong ứng dụng Finder Work
- **Profile_Data**: Thông tin hồ sơ bao gồm thông tin cá nhân, kỹ năng, ngành nghề, kinh nghiệm, học vấn và chứng chỉ
- **Database_Tables**: Các bảng cơ sở dữ liệu bao gồm Users, UserProfiles, Skills, Professions, WorkExperiences, Degrees, Certificates và các bảng liên kết
- **Approval_Status**: Trạng thái duyệt hồ sơ có thể là "Pending", "Approved", hoặc "Rejected"
- **Navigation_Menu**: Menu điều hướng trong header với dropdown "Hồ sơ của tôi"

## Requirements

### Requirement 1

**User Story:** As a logged-in user, I want to access my profile page without errors, so that I can view and manage my personal profile information.

#### Acceptance Criteria

1. WHEN a user clicks on "Hồ sơ của tôi" in the navigation dropdown THEN the User_Profile_System SHALL load the profile page without throwing any exceptions
2. WHEN a user has no existing profile THEN the User_Profile_System SHALL display a clear message and provide a "Create Profile" button
3. WHEN a user has an existing profile THEN the User_Profile_System SHALL display all Profile_Data including personal information, skills, professions, work experiences, degrees, and certificates
4. WHEN the profile page loads THEN the User_Profile_System SHALL include all related data from Database_Tables using proper Entity Framework includes
5. WHEN any database error occurs THEN the User_Profile_System SHALL handle the error gracefully and display a user-friendly error message

### Requirement 2

**User Story:** As a user, I want to create a comprehensive profile with all my information, so that employers can find and evaluate my qualifications effectively.

#### Acceptance Criteria

1. WHEN a user accesses the profile creation form THEN the User_Profile_System SHALL load all dropdown data from Database_Tables including skills, professions, work types, degree types, and certificate types
2. WHEN a user submits the profile creation form THEN the User_Profile_System SHALL validate all required fields including personal information, summary, location, skills, and professions
3. WHEN profile creation is successful THEN the User_Profile_System SHALL save the profile with Approval_Status set to "Pending" and redirect to the profile view page
4. WHEN a user adds work experiences, degrees, or certificates THEN the User_Profile_System SHALL save these items with proper relationships to the main profile
5. WHEN validation fails THEN the User_Profile_System SHALL display specific error messages and preserve user input data

### Requirement 3

**User Story:** As a user, I want to edit my existing profile information, so that I can keep my profile up-to-date and accurate.

#### Acceptance Criteria

1. WHEN a user accesses the profile edit form THEN the User_Profile_System SHALL pre-populate all fields with current Profile_Data
2. WHEN a user updates basic profile information THEN the User_Profile_System SHALL save changes and update the modification timestamp
3. WHEN a user modifies skills or professions THEN the User_Profile_System SHALL update the many-to-many relationships correctly
4. WHEN a user adds additional work experiences, degrees, or certificates through modals THEN the User_Profile_System SHALL save these items and refresh the profile display
5. WHEN profile updates are successful THEN the User_Profile_System SHALL display a success message and show updated information

### Requirement 4

**User Story:** As a user, I want to see my profile approval status clearly, so that I understand whether my profile is visible to employers.

#### Acceptance Criteria

1. WHEN a user views their profile THEN the User_Profile_System SHALL display the current Approval_Status with appropriate visual indicators
2. WHEN the profile status is "Pending" THEN the User_Profile_System SHALL show a warning badge and explain that the profile is under review
3. WHEN the profile status is "Approved" THEN the User_Profile_System SHALL show a success badge and confirm the profile is publicly visible
4. WHEN the profile status is "Rejected" THEN the User_Profile_System SHALL show an error badge and display the rejection reason if available
5. WHEN a rejected profile is updated THEN the User_Profile_System SHALL reset the status to "Pending" for re-review

### Requirement 5

**User Story:** As a user, I want all my profile data to be displayed correctly with proper formatting, so that I can verify my information is complete and accurate.

#### Acceptance Criteria

1. WHEN displaying skills THEN the User_Profile_System SHALL group skills by skill type and show skill names clearly
2. WHEN displaying professions THEN the User_Profile_System SHALL group professions by category and show profession names with descriptions
3. WHEN displaying work experiences THEN the User_Profile_System SHALL show company name, position, dates, work type, and description in chronological order
4. WHEN displaying degrees THEN the User_Profile_System SHALL show degree name, school, major, graduation year, and degree type
5. WHEN displaying certificates THEN the User_Profile_System SHALL show certificate name, issuing organization, issue date, and certificate type

### Requirement 6

**User Story:** As a system administrator, I want the profile system to handle errors gracefully, so that users have a smooth experience even when issues occur.

#### Acceptance Criteria

1. WHEN database connection fails THEN the User_Profile_System SHALL display a friendly error message and suggest trying again later
2. WHEN required dropdown data is missing THEN the User_Profile_System SHALL handle the null data gracefully and show appropriate messages
3. WHEN file upload fails THEN the User_Profile_System SHALL continue with profile creation and notify the user about the upload issue
4. WHEN concurrent updates occur THEN the User_Profile_System SHALL handle optimistic concurrency conflicts appropriately
5. WHEN any unhandled exception occurs THEN the User_Profile_System SHALL log the error and display a generic error message to the user