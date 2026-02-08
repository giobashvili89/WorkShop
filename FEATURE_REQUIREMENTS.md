# Feature Requirements and Improvements

## Overview
This document outlines the required features and improvements for the WorkShop book-selling platform based on user requests and development needs.

---

## 1. Admin Panel - Enhanced Order Management Page

### Description
Create an enhanced admin panel page where administrators can view, filter, and manage all purchased orders in the system.

### Current State
- ✅ Basic `OrderHistory.jsx` component exists showing orders for both admin and customers
- ✅ Orders display basic information (order ID, date, status, items, delivery info)
- ✅ Backend `OrdersController` has endpoints for getting all orders (admin) and user orders

### Required Enhancements

#### 1.1 Enhanced Filtering Options
Add comprehensive filtering capabilities for administrators:
- **Filter by Order Status**: Pending, Completed, Cancelled
- **Filter by Tracking Status**: Order Placed, Processing, In Warehouse, On The Way, Out for Delivery, Delivered
- **Filter by Date Range**: Start date and end date pickers
- **Filter by Customer**: Search/filter by username or user ID
- **Filter by Order ID**: Quick search by specific order number
- **Filter by Total Amount Range**: Min and max price filters

#### 1.2 Additional Order Fields Display
Expand the order details view to show:
- **Email Sent Status**: Display whether confirmation email was sent (`EmailSent` field)
- **Completion Date**: Show `CompletedDate` when order is completed
- **Customer Email**: Display customer's email address (requires User entity join)
- **Individual Item Details**: Enhanced display of order items with more book information

#### 1.3 Sorting Capabilities
Implement sorting options:
- Sort by Order Date (newest/oldest)
- Sort by Total Amount (highest/lowest)
- Sort by Status
- Sort by Customer Name

#### 1.4 Pagination
- Implement pagination for large order lists
- Configurable items per page (10, 25, 50, 100)
- Page navigation controls

#### 1.5 Export Functionality (Optional)
- Export filtered orders to CSV/Excel
- Include all relevant order and delivery information

### Technical Requirements
- **Component**: Create or enhance `AdminOrderManagement.jsx` component
- **Service**: Extend `orderService.js` with filtering parameters
- **Backend**: Update `OrdersController` GET endpoint to support query parameters for filtering
- **UI/UX**: Use consistent design with existing admin components (like `AdminBookManagement.jsx`)

---

## 2. Delivery Information Management Modal

### Description
Implement a modal (popup window) where administrators can update delivery and tracking information for orders.

### Current State
- ✅ Orders have delivery fields: `PhoneNumber`, `AlternativePhoneNumber`, `HomeAddress`
- ✅ Orders have `TrackingStatus` field
- ✅ Delivery information is collected during checkout
- ❌ No admin interface to update delivery/tracking information after order placement

### Required Features

#### 2.1 Modal Functionality
Create a modal component that allows administrators to:
- **Update Tracking Status**: Dropdown to change order tracking status
  - Order Placed
  - Processing
  - In Warehouse
  - On The Way
  - Out for Delivery
  - Delivered
- **Update Delivery Information**: Edit delivery details if needed
  - Phone Number
  - Alternative Phone Number
  - Home Address
- **Add Delivery Notes**: Optional notes field for internal tracking
- **Update Order Status**: Change order status (Pending, Completed, Cancelled)

#### 2.2 Modal Trigger
- Add "Update Delivery" button on each order card in the admin view
- Modal should pre-populate with current order information

#### 2.3 Validation
- Validate phone numbers (existing regex: `^\+?[0-9\s\-()]{7,20}$`)
- Validate address (required, min length)
- Validate status changes (business logic validation)

#### 2.4 Email Notification
- Option to send email notification to customer when tracking status is updated
- Auto-mark `EmailSent` field when notification is sent

### Technical Requirements
- **Component**: Create `DeliveryInfoModal.jsx` component
- **Backend**: Create PUT endpoint `/api/orders/{id}/delivery` in `OrdersController`
- **Service**: Add `updateDeliveryInfo` method to `orderService.js`
- **Validation**: Create or update validator for delivery information updates
- **Styling**: Use Tailwind CSS for modal styling (consistent with existing UI)

---

## 3. Unit Tests for API Features

### Description
Create comprehensive unit tests for the new and existing order management features in the backend.

### Current State
- ✅ Test infrastructure exists: `tests/WorkShop.API.Tests/`
- ✅ Existing tests: `AuthServiceTests.cs`, `BookServiceTests.cs`, `ValidationTests.cs`, `SwaggerTests.cs`
- ❌ No tests for `OrderService` or order-related functionality

### Required Tests

#### 3.1 OrderService Tests
Create `OrderServiceTests.cs` with tests for:
- ✅ Creating a new order
- ✅ Retrieving orders by user ID
- ✅ Retrieving all orders (admin)
- ✅ Canceling an order within cancellation window
- ✅ Preventing cancellation after time limit
- ✅ Updating order status
- ✅ Updating tracking status
- ✅ Updating delivery information
- ✅ Stock validation during order creation
- ✅ Total amount calculation validation

#### 3.2 Order Validation Tests
Extend `ValidationTests.cs` or create `OrderValidationTests.cs`:
- ✅ OrderRequestModel validation (phone numbers, address)
- ✅ Delivery information update validation
- ✅ Business rule validations (status transitions, cancellation rules)

#### 3.3 OrdersController Tests
Create `OrdersControllerTests.cs` with integration tests:
- ✅ GET all orders (admin only)
- ✅ GET user orders (authenticated user)
- ✅ POST create order
- ✅ DELETE cancel order
- ✅ PUT update delivery information
- ✅ Authorization tests (ensure endpoints are properly secured)

#### 3.4 Test Coverage Goals
- Minimum 80% code coverage for order-related code
- All happy paths covered
- All edge cases and error scenarios covered
- All validation rules covered

### Technical Requirements
- **Framework**: xUnit (already in use)
- **Mocking**: Moq or NSubstitute for mocking dependencies
- **Test Structure**: Follow existing test patterns in the codebase
- **Naming**: Follow existing naming conventions (e.g., `MethodName_Scenario_ExpectedBehavior`)

---

## 4. Client-Side Unit Tests

### Description
Implement unit testing infrastructure and tests for the React client application.

### Current State
- ❌ No testing framework configured for the client
- ❌ No test files exist for React components
- ✅ Client uses React 19 with Vite build tool

### Required Setup

#### 4.1 Testing Framework Setup
Add testing dependencies to `client/package.json`:
- **Vitest**: Test runner (integrates well with Vite)
- **@testing-library/react**: React component testing utilities
- **@testing-library/jest-dom**: Custom matchers for DOM assertions
- **@testing-library/user-event**: User interaction simulation
- **jsdom**: DOM environment for Node.js

#### 4.2 Test Structure
Create test directory structure:
```
client/
├── src/
│   ├── components/
│   │   ├── __tests__/
│   │   │   ├── OrderHistory.test.jsx
│   │   │   ├── AdminBookManagement.test.jsx
│   │   │   ├── BookList.test.jsx
│   │   │   ├── Login.test.jsx
│   │   │   └── DeliveryInfoModal.test.jsx (new)
│   │   └── ...
│   └── services/
│       └── __tests__/
│           ├── orderService.test.js
│           ├── authService.test.js
│           └── bookService.test.js
```

#### 4.3 Required Tests

##### Component Tests
**OrderHistory.test.jsx**:
- ✅ Renders order list correctly
- ✅ Displays admin vs customer views appropriately
- ✅ Handles loading state
- ✅ Handles error state
- ✅ Cancel order functionality
- ✅ Displays delivery information

**AdminBookManagement.test.jsx**:
- ✅ Renders book list with filters
- ✅ Add/Edit/Delete functionality
- ✅ Pagination works correctly
- ✅ Search and filter functionality

**BookList.test.jsx**:
- ✅ Displays books correctly
- ✅ Add to cart functionality
- ✅ Checkout process
- ✅ Quantity validation
- ✅ Stock availability checks

**Login.test.jsx**:
- ✅ Form validation
- ✅ Successful login
- ✅ Failed login handling
- ✅ Token storage

**DeliveryInfoModal.test.jsx** (new component):
- ✅ Opens and closes correctly
- ✅ Pre-populates with order data
- ✅ Form validation
- ✅ Submission handling
- ✅ Error handling

##### Service Tests
**orderService.test.js**:
- ✅ API calls are made correctly
- ✅ Response data is transformed correctly
- ✅ Error handling
- ✅ Authentication headers included

**authService.test.js**:
- ✅ Login/logout functionality
- ✅ Token management
- ✅ Role checking (isAdmin)

**bookService.test.js**:
- ✅ CRUD operations
- ✅ Filtering and searching
- ✅ Error handling

#### 4.4 Configuration Files
Create necessary configuration:
- **vitest.config.js**: Vitest configuration
- **setupTests.js**: Global test setup
- Update **package.json** with test scripts:
  ```json
  "scripts": {
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest --coverage"
  }
  ```

### Technical Requirements
- **Testing Library**: @testing-library/react (React 19 compatible version)
- **Test Runner**: Vitest (native Vite integration)
- **Coverage Tool**: c8 or vitest coverage reporter
- **Coverage Goals**: Minimum 70% code coverage for components and services

---

## 5. Implementation Priority

### Phase 1: Backend Foundation
1. ✅ Update OrdersController with filtering support
2. ✅ Create delivery information update endpoint
3. ✅ Add OrderService tests
4. ✅ Add OrdersController tests
5. ✅ Add validation tests

### Phase 2: Admin UI Enhancement
1. ✅ Enhance AdminOrderManagement component with filters
2. ✅ Create DeliveryInfoModal component
3. ✅ Update orderService.js with new API calls
4. ✅ Test admin features manually

### Phase 3: Client Testing Infrastructure
1. ✅ Set up Vitest and testing libraries
2. ✅ Create test utilities and mocks
3. ✅ Write component tests
4. ✅ Write service tests
5. ✅ Achieve coverage goals

### Phase 4: Integration and Quality Assurance
1. ✅ Run all tests (backend + frontend)
2. ✅ Fix any failing tests
3. ✅ Code review and refinement
4. ✅ Documentation updates
5. ✅ Create pull request

---

## 6. Testing Checklist

### Before Creating Pull Request
- [ ] All backend unit tests pass
- [ ] All frontend unit tests pass
- [ ] Code coverage meets minimum thresholds (80% backend, 70% frontend)
- [ ] Manual testing of new features completed
- [ ] No console errors or warnings
- [ ] All linting checks pass (`dotnet format` for backend, `npm run lint` for frontend)
- [ ] Documentation updated (README.md if needed)
- [ ] Migration files created for any database schema changes
- [ ] Security considerations addressed (input validation, authorization)

---

## 7. Additional Considerations

### Security
- Ensure only administrators can access order management endpoints
- Validate all user inputs (SQL injection, XSS prevention)
- Sanitize file uploads if any
- Use HTTPS in production
- Validate JWT tokens on all protected endpoints

### Performance
- Implement database indexing for frequently queried fields (OrderDate, Status, UserId)
- Use pagination to limit data transfer
- Consider caching for frequently accessed data
- Optimize database queries (use Include() for eager loading)

### User Experience
- Loading indicators for all async operations
- Clear error messages for users
- Confirmation dialogs for destructive actions
- Responsive design for mobile devices
- Accessibility (ARIA labels, keyboard navigation)

### Scalability
- Consider implementing server-side filtering for large datasets
- Use efficient database queries (avoid N+1 problems)
- Implement rate limiting on API endpoints
- Monitor API performance and database query times

---

## 8. Success Criteria

The features will be considered successfully implemented when:
1. ✅ Admin users can filter orders by multiple criteria
2. ✅ Admin users can update delivery information via modal
3. ✅ All backend tests pass with ≥80% coverage
4. ✅ All frontend tests pass with ≥70% coverage
5. ✅ Manual testing confirms all features work as expected
6. ✅ Code review identifies no critical issues
7. ✅ No regression in existing functionality
8. ✅ Documentation is complete and accurate

---

## Related Files

### Backend
- `src/WorkShop.API/Controllers/OrdersController.cs`
- `src/WorkShop.Application/Interfaces/IOrderService.cs`
- `src/WorkShop.Infrastructure/Services/OrderService.cs`
- `src/WorkShop.Domain/Entities/Order.cs`
- `src/WorkShop.Application/Models/Request/OrderRequestModel.cs`
- `src/WorkShop.Application/Models/Response/OrderResponseModel.cs`
- `src/WorkShop.Application/Validators/OrderRequestModelValidator.cs`
- `tests/WorkShop.API.Tests/OrderServiceTests.cs` (to be created)
- `tests/WorkShop.API.Tests/OrdersControllerTests.cs` (to be created)

### Frontend
- `client/src/components/OrderHistory.jsx`
- `client/src/components/AdminOrderManagement.jsx` (to be created/enhanced)
- `client/src/components/DeliveryInfoModal.jsx` (to be created)
- `client/src/services/orderService.js`
- `client/src/components/__tests__/` (to be created)
- `client/src/services/__tests__/` (to be created)

---

## Notes for Developers

1. **Test-Driven Development**: Consider writing tests before implementing features
2. **Existing Patterns**: Follow existing code patterns in the repository
3. **Clean Architecture**: Maintain separation of concerns (Domain, Application, Infrastructure, API layers)
4. **Code Review**: Request peer review before merging
5. **Database Migrations**: Remember to create and test migrations for any schema changes
6. **Environment Variables**: Use environment variables for configuration (see `.env.example`)

---

## Issue Template for GitHub

**Title**: Enhanced Admin Order Management with Filtering, Delivery Updates, and Comprehensive Testing

**Description**:
Implement enhanced order management features for the admin panel including advanced filtering, delivery information updates via modal, and comprehensive unit testing for both backend and frontend.

**Labels**: enhancement, testing, admin-panel, high-priority

**Assignee**: [To be assigned]

**Milestone**: [To be determined]

**Linked Issues/PRs**: [Any related issues]

---

*Document created: 2026-02-08*
*Last updated: 2026-02-08*
