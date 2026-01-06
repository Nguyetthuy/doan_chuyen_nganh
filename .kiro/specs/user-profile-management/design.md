# Design Document

## Overview

The User Profile Management System is designed to fix critical errors in the existing Finder Work application's profile functionality and enhance the user experience for creating, viewing, and editing user profiles. The system will provide robust error handling, comprehensive data validation, and seamless integration with the existing database schema.

The design focuses on fixing the current navigation errors, improving data loading performance through proper Entity Framework includes, and ensuring all profile-related data from the database tables is correctly displayed and managed.

## Architecture

The system follows the existing ASP.NET Core MVC architecture with the following key components:

### Controller Layer
- **ProfileController**: Handles all profile-related HTTP requests
- Enhanced error handling with try-catch blocks
- Proper Entity Framework includes for efficient data loading
- Comprehensive validation for all user inputs

### View Layer
- **Profile Views**: Index, Create, Edit views with improved error handling
- **Partial Views**: Modals for adding work experiences, degrees, and certificates
- Enhanced client-side validation with JavaScript
- Responsive design with Bootstrap components

### Data Layer
- **ApplicationDbContext**: Entity Framework context with proper configurations
- **Models**: User, UserProfile, and all related entities with navigation properties
- **Repository Pattern**: Implicit through Entity Framework DbContext

### Service Layer (New)
- **ProfileService**: Business logic for profile operations
- **ValidationService**: Centralized validation logic
- **FileUploadService**: Handle avatar uploads with error recovery

## Components and Interfaces

### ProfileController Enhancements
```csharp
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;
    private readonly ProfileService _profileService;
    
    // Enhanced methods with proper error handling
    public async Task<IActionResult> Index()
    public async Task<IActionResult> Create()
    public async Task<IActionResult> Edit()
    // Additional helper methods for error handling
}
```

### ProfileService Interface
```csharp
public interface IProfileService
{
    Task<UserProfile?> GetUserProfileAsync(int userId);
    Task<bool> CreateProfileAsync(UserProfile profile, ProfileCreationData data);
    Task<bool> UpdateProfileAsync(UserProfile profile, ProfileUpdateData data);
    Task<ProfileDropdownData> GetDropdownDataAsync();
}
```

### Error Handling Components
```csharp
public class ProfileErrorHandler
{
    public static IActionResult HandleDatabaseError(Exception ex, ILogger logger);
    public static IActionResult HandleValidationError(string message);
    public static IActionResult HandleFileUploadError(Exception ex);
}
```

## Data Models

The system uses the existing database schema with enhanced navigation properties and validation:

### Core Entities
- **User**: Basic user information with authentication data
- **UserProfile**: Main profile entity with approval workflow
- **UserProfileSkill**: Many-to-many relationship for skills
- **UserProfileProfession**: Many-to-many relationship for professions

### Related Entities
- **WorkExperience**: Work history with work type classification
- **Degree**: Educational background with degree type classification
- **Certificate**: Professional certifications with certificate type classification
- **Skill**: Skills organized by skill types
- **Profession**: Professions organized by categories

### Enhanced Navigation Properties
All entities will have properly configured navigation properties to ensure efficient data loading and prevent N+1 query problems.

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property Reflection

After reviewing all properties identified in the prework analysis, I identified several areas where properties can be consolidated:

- Properties 4.2, 4.3, and 4.4 (specific status displays) can be combined into a single comprehensive status display property
- Properties 5.1-5.5 (display formatting) can be consolidated into fewer comprehensive display properties
- Properties 6.1-6.5 (error handling) can be grouped into comprehensive error handling properties

### Core Navigation and Loading Properties

**Property 1: Profile page navigation reliability**
*For any* authenticated user, clicking the "Hồ sơ của tôi" navigation link should successfully load the profile page without throwing exceptions
**Validates: Requirements 1.1**

**Property 2: Profile existence handling**
*For any* user without an existing profile, the profile page should display a create profile message and button, while users with existing profiles should see their complete profile data
**Validates: Requirements 1.2, 1.3**

**Property 3: Efficient data loading**
*For any* profile page load, all related data should be loaded using proper Entity Framework includes to avoid N+1 queries
**Validates: Requirements 1.4**

### Profile Creation Properties

**Property 4: Form data loading completeness**
*For any* profile creation form access, all required dropdown data (skills, professions, work types, degree types, certificate types) should be loaded from the database
**Validates: Requirements 2.1**

**Property 5: Profile creation validation**
*For any* profile creation submission with invalid data, the system should display specific validation errors and preserve user input
**Validates: Requirements 2.2, 2.5**

**Property 6: Successful profile creation workflow**
*For any* valid profile creation submission, the profile should be saved with "Pending" approval status and the user should be redirected to the profile view
**Validates: Requirements 2.3**

**Property 7: Related entity persistence**
*For any* profile creation with work experiences, degrees, or certificates, these items should be saved with correct foreign key relationships to the main profile
**Validates: Requirements 2.4**

### Profile Editing Properties

**Property 8: Edit form pre-population**
*For any* profile edit form access, all form fields should be pre-populated with the current profile data
**Validates: Requirements 3.1**

**Property 9: Profile update persistence**
*For any* profile update submission, changes should be saved to the database and the modification timestamp should be updated
**Validates: Requirements 3.2**

**Property 10: Many-to-many relationship updates**
*For any* skills or professions modification, the many-to-many relationships should be updated correctly in the junction tables
**Validates: Requirements 3.3**

**Property 11: Modal-based entity addition**
*For any* work experience, degree, or certificate added through modals, the item should be saved and the profile display should be refreshed
**Validates: Requirements 3.4**

**Property 12: Update success feedback**
*For any* successful profile update, a success message should be displayed and the updated information should be shown
**Validates: Requirements 3.5**

### Status Display Properties

**Property 13: Approval status display consistency**
*For any* profile with any approval status (Pending, Approved, Rejected), the appropriate visual indicator and explanatory message should be displayed
**Validates: Requirements 4.1, 4.2, 4.3, 4.4**

**Property 14: Status reset on rejected profile updates**
*For any* rejected profile that is updated, the approval status should be reset to "Pending" for re-review
**Validates: Requirements 4.5**

### Data Display Properties

**Property 15: Structured data display formatting**
*For any* profile data display, skills should be grouped by type, professions by category, work experiences in chronological order, and all related information should be properly formatted
**Validates: Requirements 5.1, 5.2, 5.3, 5.4, 5.5**

### Error Handling Properties

**Property 16: Graceful error handling**
*For any* system error (database failures, missing data, file upload issues, concurrency conflicts, or unhandled exceptions), the system should display user-friendly error messages and continue operation where possible
**Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5**

## Error Handling

### Database Error Handling
- Connection failures: Display friendly message with retry suggestion
- Query failures: Log error and show generic database error message
- Concurrency conflicts: Handle optimistic concurrency with user notification

### Validation Error Handling
- Client-side validation: Immediate feedback with JavaScript
- Server-side validation: Comprehensive validation with specific error messages
- Form state preservation: Maintain user input on validation failures

### File Upload Error Handling
- Avatar upload failures: Continue with profile creation, notify user
- File size/type validation: Clear error messages with requirements
- Storage failures: Graceful degradation with default avatar

### General Exception Handling
- Global exception filter for unhandled exceptions
- Structured logging for debugging and monitoring
- User-friendly error pages with navigation options

## Testing Strategy

### Unit Testing Approach
The system will use comprehensive unit testing to verify specific functionality:

- **Controller Action Tests**: Test each controller action with various input scenarios
- **Service Layer Tests**: Test business logic in isolation with mocked dependencies
- **Validation Tests**: Test all validation rules with edge cases
- **Error Handling Tests**: Test exception scenarios and error recovery

### Property-Based Testing Approach
The system will implement property-based testing using **NUnit** with **FsCheck.NUnit** for .NET to verify universal properties:

- **Property Test Configuration**: Each property-based test will run a minimum of 100 iterations
- **Test Tagging**: Each property-based test will include a comment with the format: **Feature: user-profile-management, Property {number}: {property_text}**
- **Generator Strategy**: Smart generators that create realistic test data within valid input spaces
- **Property Verification**: Each correctness property will be implemented as a single property-based test

### Integration Testing
- **Database Integration**: Test Entity Framework operations with in-memory database
- **End-to-End Scenarios**: Test complete user workflows from navigation to data persistence
- **Error Scenario Testing**: Test system behavior under various failure conditions

### Testing Framework Requirements
- **Unit Testing**: NUnit framework for .NET
- **Property-Based Testing**: FsCheck.NUnit for property-based test generation
- **Mocking**: Moq framework for dependency mocking
- **Database Testing**: Entity Framework In-Memory provider for isolated testing