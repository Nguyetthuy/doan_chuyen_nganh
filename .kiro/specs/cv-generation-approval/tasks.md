# Implementation Plan

- [ ] 1. Thiết lập cấu trúc dự án và models cơ bản


  - Tạo CV model với các thuộc tính cần thiết
  - Cập nhật ApplicationDbContext để include CV entity
  - Tạo migration cho bảng CVs
  - Cập nhật UserProfile model để include navigation property
  - _Requirements: 1.1, 1.3, 2.3, 2.4_

- [ ]* 1.1 Viết property test cho CV model validation
  - **Property 1: CV Creation Validation**
  - **Validates: Requirements 1.1, 1.5**

- [ ]* 1.2 Viết property test cho CV initial state
  - **Property 3: CV Initial State**
  - **Validates: Requirements 1.3**

- [ ] 2. Implement CV Service layer
  - Tạo ICVService interface với các method cần thiết
  - Implement CVService với logic tạo CV từ profile
  - Implement logic validation profile đầy đủ thông tin
  - Implement logic quản lý trạng thái CV (approve/reject)
  - _Requirements: 1.1, 1.2, 1.3, 2.3, 2.4, 2.5_

- [ ]* 2.1 Viết property test cho CV creation từ profile
  - **Property 2: CV Template Consistency**
  - **Validates: Requirements 1.2, 5.1, 5.2**

- [ ]* 2.2 Viết property test cho CV approval workflow
  - **Property 6: CV Approval State Transition**
  - **Validates: Requirements 2.3, 2.4, 2.5**

- [ ] 3. Implement CV Template Service
  - Tạo ICVTemplateService interface
  - Implement CVTemplateService với logic render CV HTML
  - Tạo CV template HTML theo mẫu thiết kế
  - Implement logic xử lý thông tin thiếu (placeholder/hide sections)
  - Implement logic xử lý ảnh đại diện (có/không có ảnh)
  - _Requirements: 1.2, 5.1, 5.2, 5.3, 5.4, 5.5_

- [ ]* 3.1 Viết property test cho CV template rendering
  - **Property 8: CV Template Rendering**
  - **Validates: Requirements 2.2, 3.3**

- [ ]* 3.2 Viết property test cho missing information handling
  - **Property 14: Missing Information Handling**
  - **Validates: Requirements 5.3**

- [ ]* 3.3 Viết property test cho avatar image handling
  - **Property 15: Avatar Image Handling**
  - **Validates: Requirements 5.4, 5.5**

- [ ] 4. Checkpoint - Đảm bảo tất cả tests đều pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 5. Implement CV Controller
  - Tạo CVController với các action cơ bản
  - Implement Index action để hiển thị danh sách CV của user
  - Implement Create action để tạo CV từ profile
  - Implement Preview action để xem trước CV
  - Implement Edit và Delete actions
  - Implement Resubmit action cho CV bị từ chối
  - _Requirements: 1.1, 1.2, 1.4, 4.1, 4.4, 4.5_

- [ ]* 5.1 Viết property test cho CV creation feedback
  - **Property 4: CV Creation Feedback**
  - **Validates: Requirements 1.4**

- [ ]* 5.2 Viết property test cho user CV status tracking
  - **Property 10: User CV Status Tracking**
  - **Validates: Requirements 4.1**

- [ ]* 5.3 Viết property test cho CV resubmission workflow
  - **Property 13: CV Resubmission Workflow**
  - **Validates: Requirements 4.4, 4.5**

- [ ] 6. Implement Admin CV Management
  - Mở rộng Admin/ManagementController với CV approval functions
  - Implement CVApproval action để hiển thị danh sách CV pending
  - Implement ApproveCV và RejectCV actions
  - Implement CVDetails action để xem chi tiết CV
  - Add authorization checks cho admin functions
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [ ]* 6.1 Viết property test cho admin CV filtering
  - **Property 5: Admin CV Filtering**
  - **Validates: Requirements 2.1**

- [ ]* 6.2 Viết unit tests cho admin authorization
  - Test admin permission validation
  - Test non-admin access denial
  - _Requirements: 2.1, 2.3, 2.4_

- [ ] 7. Implement Homepage CV Display
  - Cập nhật HomeController để hiển thị approved CVs
  - Implement logic filtering và sorting CVs theo approval date
  - Implement CVDetail action cho public CV view
  - Add pagination cho danh sách CV nếu cần
  - _Requirements: 3.1, 3.2, 3.4, 3.5_

- [ ]* 7.1 Viết property test cho homepage CV display
  - **Property 7: Homepage CV Display**
  - **Validates: Requirements 3.1, 3.2, 3.4**

- [ ]* 7.2 Viết property test cho CV detail navigation
  - **Property 9: CV Detail Navigation**
  - **Validates: Requirements 3.5**

- [ ] 8. Implement Notification System
  - Tạo notification system cho CV status changes
  - Implement logic gửi notification khi CV được approve/reject
  - Implement hiển thị rejection reason cho user
  - Add notification display trong user interface
  - _Requirements: 4.2, 4.3_

- [ ]* 8.1 Viết property test cho CV status notification
  - **Property 11: CV Status Notification**
  - **Validates: Requirements 4.2**

- [ ]* 8.2 Viết property test cho rejection reason display
  - **Property 12: Rejection Reason Display**
  - **Validates: Requirements 4.3**

- [ ] 9. Create Views và UI Components
  - Tạo Views/CV/Index.cshtml cho danh sách CV của user
  - Tạo Views/CV/Create.cshtml cho tạo CV mới
  - Tạo Views/CV/Preview.cshtml cho xem trước CV
  - Tạo Views/Shared/_CVTemplate.cshtml cho template CV
  - Tạo Views/Shared/_CVCard.cshtml cho CV card trên homepage
  - _Requirements: 1.4, 4.1, 4.3_

- [ ] 10. Create Admin Views
  - Tạo Areas/Admin/Views/Management/CVApproval.cshtml
  - Tạo Areas/Admin/Views/Management/CVDetails.cshtml
  - Cập nhật admin sidebar để include CV management
  - Add styling và JavaScript cho admin CV management
  - _Requirements: 2.1, 2.2_

- [ ] 11. Update Homepage Views
  - Cập nhật Views/Home/Index.cshtml để hiển thị approved CVs
  - Tạo CV detail view cho public access
  - Add responsive design cho CV display
  - Implement search/filter functionality nếu cần
  - _Requirements: 3.1, 3.3, 3.5_

- [ ] 12. Implement CSS Styling
  - Tạo CSS cho CV template theo mẫu thiết kế
  - Implement responsive design cho CV display
  - Add styling cho CV status indicators
  - Create consistent styling cho admin CV management
  - _Requirements: 5.1, 5.2_

- [ ] 13. Add Dependency Injection Configuration
  - Register CVService và CVTemplateService trong Program.cs
  - Configure service lifetimes appropriately
  - Add any required middleware configuration
  - _Requirements: All_

- [ ]* 13.1 Viết integration tests cho service registration
  - Test service resolution từ DI container
  - Test service lifecycle management
  - _Requirements: All_

- [ ] 14. Database Migration và Seeding
  - Tạo và apply migration cho CV table
  - Add seed data cho testing nếu cần
  - Update database indexes cho performance
  - Test migration rollback functionality
  - _Requirements: 1.3, 2.3, 2.4_

- [ ] 15. Final Checkpoint - Đảm bảo tất cả tests đều pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 16. End-to-End Testing và Bug Fixes
  - Test complete workflow từ tạo CV đến hiển thị trên homepage
  - Test admin approval workflow end-to-end
  - Test error handling và edge cases
  - Fix any bugs discovered during testing
  - _Requirements: All_

- [ ]* 16.1 Viết integration tests cho complete workflow
  - Test user tạo CV → admin duyệt → hiển thị homepage
  - Test user resubmit CV sau khi bị reject
  - Test concurrent access scenarios
  - _Requirements: All_