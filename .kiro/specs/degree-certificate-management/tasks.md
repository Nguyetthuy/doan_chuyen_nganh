# Implementation Plan

- [ ] 1. Fix infrastructure and setup issues
  - Create missing upload directories for certificates
  - Verify database connection and table structures
  - Ensure reference data exists in DegreeTypes and CertificateTypes tables
  - _Requirements: 3.1, 4.2_

- [x] 1.1 Create missing upload directories


  - Create wwwroot/uploads/certificates directory if it doesn't exist
  - Verify permissions for file upload operations
  - _Requirements: 4.2_

- [ ]* 1.2 Write property test for directory creation
  - **Property 8: File upload round trip**
  - **Validates: Requirements 4.2, 4.3**







- [ ] 1.3 Seed reference data for degree and certificate types
  - Add default DegreeTypes (Cử nhân, Thạc sĩ, Tiến sĩ, Cao đẳng, Trung cấp)
  - Add default CertificateTypes (Công nghệ, Ngoại ngữ, Quản lý, Kỹ năng mềm, Chuyên môn)

  - _Requirements: 3.1_

- [ ]* 1.4 Write unit tests for reference data seeding
  - Test that default data is created correctly
  - Test that existing data is not duplicated
  - _Requirements: 3.1_

- [ ] 2. Fix database mapping and model issues
  - Review and fix ApplicationDbContext column mappings



  - Ensure model properties match database columns


  - Fix any inconsistencies in foreign key relationships
  - _Requirements: 1.3, 2.3_

- [ ] 2.1 Fix database column mapping inconsistencies
  - Review DegreeImg mapping (currently mapped to "Degreeing" column)
  - Review IssueBy mapping (currently mapped to "IssuedBy" column)
  - Update ProfileId mappings to match actual column names
  - _Requirements: 1.3, 2.3_

- [ ]* 2.2 Write property test for database mapping
  - **Property 2: Degree persistence round trip**
  - **Validates: Requirements 1.3, 1.5**

- [ ]* 2.3 Write property test for certificate database mapping
  - **Property 4: Certificate persistence round trip**
  - **Validates: Requirements 2.3, 2.5**



- [ ] 3. Improve validation and error handling
  - Enhance input validation for degree and certificate forms
  - Improve error message collection and display
  - Add comprehensive validation for file uploads
  - _Requirements: 1.2, 1.4, 2.2, 2.4, 4.1_

- [x] 3.1 Enhance degree validation logic



  - Add comprehensive validation for all degree fields
  - Implement proper date validation for graduation year
  - Add business rule validation (e.g., graduation year not in future)
  - _Requirements: 1.2, 1.4_

- [ ]* 3.2 Write property test for degree validation
  - **Property 1: Degree validation consistency**
  - **Validates: Requirements 1.2, 1.4**

- [ ] 3.3 Enhance certificate validation logic
  - Add comprehensive validation for all certificate fields
  - Implement proper date validation for issue and expiry dates
  - Add business rule validation (e.g., expiry date after issue date)
  - _Requirements: 2.2, 2.4_

- [ ]* 3.4 Write property test for certificate validation
  - **Property 3: Certificate validation consistency**
  - **Validates: Requirements 2.2, 2.4**

- [ ] 3.5 Improve file upload validation
  - Enhance file type and size validation
  - Add proper error handling for file operations
  - Implement graceful degradation when file upload fails
  - _Requirements: 4.1, 4.4_

- [ ]* 3.6 Write property test for file validation
  - **Property 7: File validation consistency**
  - **Validates: Requirements 4.1**

- [ ] 4. Fix dropdown data loading issues
  - Ensure ViewBag data is properly loaded for modals
  - Add error handling for dropdown data loading failures
  - Implement fallback options when reference data is missing
  - _Requirements: 3.2, 3.3, 3.5_

- [ ] 4.1 Fix dropdown data loading in ProfileController
  - Ensure DegreeTypes and CertificateTypes are loaded correctly
  - Add proper error handling for database query failures
  - Implement fallback empty lists when data loading fails
  - _Requirements: 3.2, 3.3_

- [ ]* 4.2 Write property test for dropdown population
  - **Property 5: Dropdown population consistency**
  - **Validates: Requirements 3.2**

- [ ] 4.3 Add fallback handling for missing reference data
  - Display clear messages when no degree/certificate types exist
  - Provide links to admin pages for adding reference data

  - Ensure forms still work with empty dropdown lists
  - _Requirements: 3.5_

- [ ] 5. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 6. Improve user experience and UI
  - Enhance modal forms with better validation feedback
  - Improve success and error message display
  - Add loading states for form submissions
  - _Requirements: 1.5, 2.5_

- [ ] 6.1 Enhance modal form user experience
  - Add client-side validation for immediate feedback
  - Improve form layout and field organization
  - Add loading indicators during form submission
  - _Requirements: 1.1, 2.1_

- [ ] 6.2 Improve success and error message display
  - Enhance TempData message styling and positioning
  - Add proper error message aggregation and display
  - Implement toast notifications for better UX
  - _Requirements: 1.4, 1.5, 2.4, 2.5_

- [ ]* 6.3 Write property test for error message collection
  - **Property 10: Validation error collection**
  - **Validates: Requirements 5.2**

- [ ] 7. Add comprehensive logging and monitoring
  - Add structured logging for all operations
  - Implement proper exception logging
  - Add performance monitoring for file uploads
  - _Requirements: 5.1, 5.4, 5.5_

- [ ] 7.1 Implement comprehensive logging
  - Add structured logging using ILogger
  - Log all database operations and their results
  - Log file upload operations and any failures
  - _Requirements: 5.1, 5.4, 5.5_

- [ ]* 7.2 Write property test for logging completeness
  - **Property 11: Logging completeness**
  - **Validates: Requirements 5.5**

- [ ] 8. Add admin management improvements
  - Enhance CRUD operations for degree and certificate types
  - Add bulk operations for reference data management
  - Improve admin UI for better usability
  - _Requirements: 3.4_

- [ ] 8.1 Enhance admin CRUD operations
  - Improve error handling in ManagementController
  - Add validation for admin operations
  - Ensure proper transaction handling
  - _Requirements: 3.4_

- [ ]* 8.2 Write property test for admin CRUD operations
  - **Property 6: CRUD operations consistency**
  - **Validates: Requirements 3.4**

- [ ] 9. Add file management and viewing features
  - Implement proper file viewing functionality
  - Add file deletion capabilities
  - Ensure file links work correctly
  - _Requirements: 4.5_

- [ ] 9.1 Implement file viewing and management
  - Ensure uploaded files can be viewed correctly
  - Add proper file serving with security considerations
  - Implement file deletion when records are removed
  - _Requirements: 4.5_

- [ ]* 9.2 Write property test for image link consistency
  - **Property 9: Image link consistency**
  - **Validates: Requirements 4.5**

- [ ] 10. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.