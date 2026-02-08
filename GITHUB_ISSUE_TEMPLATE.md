# GitHub Issue Template

Copy this content to create a new GitHub issue:

---

**Title**: Enhanced Admin Order Management with Filtering, Delivery Updates, and Comprehensive Testing

## Overview
Implement enhanced order management features for the admin panel including advanced filtering capabilities, delivery information updates via modal interface, and comprehensive unit testing for both backend and frontend.

## Features to Implement

### 1. Enhanced Admin Order Management Page
**Current State**: Basic order listing exists for both admin and customers

**Required Enhancements**:
- [ ] **Advanced Filtering**:
  - Filter by order status (Pending, Completed, Cancelled)
  - Filter by tracking status (Order Placed, Processing, In Warehouse, On The Way, Out for Delivery, Delivered)
  - Filter by date range (start and end dates)
  - Filter by customer (username/user ID)
  - Filter by order ID (quick search)
  - Filter by total amount range (min/max)

- [ ] **Enhanced Display**:
  - Show email sent status
  - Display completion date for completed orders
  - Show customer email address
  - Enhanced item details display

- [ ] **Sorting & Pagination**:
  - Sort by date, amount, status, customer name
  - Implement pagination (10/25/50/100 items per page)
  - Page navigation controls

- [ ] **Optional**: Export to CSV/Excel

**Technical Approach**:
- Create/enhance `AdminOrderManagement.jsx` component
- Update backend `OrdersController` to support query parameters
- Follow patterns from existing `AdminBookManagement.jsx`

### 2. Delivery Information Management Modal
**Current State**: Delivery info is collected during checkout but cannot be updated by admins

**Required Features**:
- [ ] Modal component for updating delivery information
- [ ] Update tracking status dropdown (6 statuses)
- [ ] Edit delivery details (phone numbers, address)
- [ ] Optional delivery notes field
- [ ] Update order status
- [ ] Email notification option when tracking status changes
- [ ] Form validation (phone numbers, address)

**Technical Approach**:
- Create `DeliveryInfoModal.jsx` component
- Create PUT endpoint `/api/orders/{id}/delivery`
- Add `updateDeliveryInfo` to `orderService.js`
- Use existing phone validation regex: `^\+?[0-9\s\-()]{7,20}$`

### 3. Backend Unit Tests
**Current State**: Test infrastructure exists but no order-related tests

**Required Tests**:
- [ ] **OrderServiceTests.cs**:
  - Create order
  - Retrieve orders (by user, all orders)
  - Cancel order (within/after time limit)
  - Update order status and tracking
  - Update delivery information
  - Stock validation
  - Total amount calculation

- [ ] **OrdersControllerTests.cs**:
  - GET all orders (admin authorization)
  - GET user orders (authentication)
  - POST create order
  - DELETE cancel order
  - PUT update delivery info
  - Authorization tests

- [ ] **Order Validation Tests**:
  - OrderRequestModel validation
  - Delivery info update validation
  - Business rule validations

**Coverage Goal**: Minimum 80% code coverage

### 4. Frontend Unit Tests
**Current State**: No testing framework configured for React client

**Required Setup**:
- [ ] **Configure Testing Framework**:
  - Add Vitest, @testing-library/react, @testing-library/jest-dom
  - Add jsdom, @testing-library/user-event
  - Create vitest.config.js and setupTests.js
  - Add test scripts to package.json

- [ ] **Component Tests**:
  - OrderHistory.test.jsx (renders, admin/customer views, cancel order)
  - AdminBookManagement.test.jsx (CRUD, filters, pagination)
  - BookList.test.jsx (display, cart, checkout)
  - Login.test.jsx (validation, login/logout)
  - DeliveryInfoModal.test.jsx (new component tests)

- [ ] **Service Tests**:
  - orderService.test.js (API calls, transformations, errors)
  - authService.test.js (login/logout, tokens, roles)
  - bookService.test.js (CRUD, filtering, errors)

**Coverage Goal**: Minimum 70% code coverage

## Implementation Phases

### Phase 1: Backend Foundation
1. Update OrdersController with filtering support
2. Create delivery information update endpoint
3. Implement all backend tests

### Phase 2: Admin UI Enhancement
1. Enhance admin order management with filters
2. Create delivery info modal
3. Update frontend services

### Phase 3: Client Testing
1. Set up Vitest and testing libraries
2. Write component tests
3. Write service tests

### Phase 4: QA
1. Run all tests
2. Manual testing
3. Code review
4. Create pull request

## Testing Checklist (Before PR)
- [ ] All backend unit tests pass
- [ ] All frontend unit tests pass
- [ ] Code coverage meets thresholds (80% backend, 70% frontend)
- [ ] Manual testing completed
- [ ] No console errors/warnings
- [ ] Linting passes (`dotnet format`, `npm run lint`)
- [ ] Documentation updated
- [ ] Security validated (authorization, input validation)

## Success Criteria
- ✅ Admin users can filter orders by multiple criteria
- ✅ Admin users can update delivery info via modal
- ✅ All tests pass with coverage goals met
- ✅ Manual testing confirms features work
- ✅ No regression in existing functionality
- ✅ Code review shows no critical issues

## Labels
`enhancement`, `testing`, `admin-panel`, `high-priority`

## Related Files

**Backend**:
- `src/WorkShop.API/Controllers/OrdersController.cs`
- `src/WorkShop.Infrastructure/Services/OrderService.cs`
- `src/WorkShop.Domain/Entities/Order.cs`
- `tests/WorkShop.API.Tests/` (new test files needed)

**Frontend**:
- `client/src/components/OrderHistory.jsx`
- `client/src/components/DeliveryInfoModal.jsx` (new)
- `client/src/services/orderService.js`
- `client/src/components/__tests__/` (new directory)

## Additional Considerations
- **Security**: Admin-only endpoints, input validation, XSS prevention
- **Performance**: Database indexing, pagination, query optimization
- **UX**: Loading indicators, error messages, responsive design
- **Scalability**: Server-side filtering, efficient queries, rate limiting

---

*Issue created: 2026-02-08*
