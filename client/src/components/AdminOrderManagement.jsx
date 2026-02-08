import { useState, useEffect, useCallback } from 'react';
import { orderService } from '../services/orderService';
import { authService } from '../services/authService';
import DeliveryInfoModal from './DeliveryInfoModal';

function AdminOrderManagement() {
  const [orders, setOrders] = useState([]);
  const [filteredOrders, setFilteredOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [showDeliveryModal, setShowDeliveryModal] = useState(false);

  // Filter states
  const [filters, setFilters] = useState({
    status: '',
    trackingStatus: '',
    startDate: '',
    endDate: '',
    customerSearch: '',
    orderId: '',
    minAmount: '',
    maxAmount: ''
  });

  // Sorting and pagination
  const [sortBy, setSortBy] = useState('date');
  const [sortOrder, setSortOrder] = useState('desc');
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);

  const isAdmin = authService.isAdmin();

  useEffect(() => {
    if (!isAdmin) {
      setError('Access denied. Admin privileges required.');
      setLoading(false);
      return;
    }
    loadOrders();
  }, [isAdmin]);

  const applyFiltersAndSort = useCallback(() => {
    let result = [...orders];

    // Apply filters
    if (filters.status) {
      result = result.filter(order => order.status === filters.status);
    }
    if (filters.trackingStatus) {
      result = result.filter(order => order.trackingStatus === filters.trackingStatus);
    }
    if (filters.startDate) {
      result = result.filter(order => new Date(order.orderDate) >= new Date(filters.startDate));
    }
    if (filters.endDate) {
      const endDate = new Date(filters.endDate);
      endDate.setHours(23, 59, 59);
      result = result.filter(order => new Date(order.orderDate) <= endDate);
    }
    if (filters.customerSearch) {
      result = result.filter(order => 
        order.username.toLowerCase().includes(filters.customerSearch.toLowerCase()) ||
        order.userId.toString().includes(filters.customerSearch)
      );
    }
    if (filters.orderId) {
      result = result.filter(order => order.id.toString().includes(filters.orderId));
    }
    if (filters.minAmount) {
      result = result.filter(order => order.totalAmount >= parseFloat(filters.minAmount));
    }
    if (filters.maxAmount) {
      result = result.filter(order => order.totalAmount <= parseFloat(filters.maxAmount));
    }

    // Apply sorting
    result.sort((a, b) => {
      let aVal, bVal;
      switch (sortBy) {
        case 'date':
          aVal = new Date(a.orderDate);
          bVal = new Date(b.orderDate);
          break;
        case 'amount':
          aVal = a.totalAmount;
          bVal = b.totalAmount;
          break;
        case 'customer':
          aVal = a.username.toLowerCase();
          bVal = b.username.toLowerCase();
          break;
        case 'status':
          aVal = a.status;
          bVal = b.status;
          break;
        default:
          return 0;
      }
      
      if (sortOrder === 'asc') {
        return aVal > bVal ? 1 : aVal < bVal ? -1 : 0;
      } else {
        return aVal < bVal ? 1 : aVal > bVal ? -1 : 0;
      }
    });

    setFilteredOrders(result);
    setCurrentPage(1); // Reset to first page when filters change
  }, [orders, filters, sortBy, sortOrder]);

  useEffect(() => {
    applyFiltersAndSort();
  }, [applyFiltersAndSort]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await orderService.getAllOrders();
      setOrders(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (name, value) => {
    setFilters(prev => ({ ...prev, [name]: value }));
  };

  const clearFilters = () => {
    setFilters({
      status: '',
      trackingStatus: '',
      startDate: '',
      endDate: '',
      customerSearch: '',
      orderId: '',
      minAmount: '',
      maxAmount: ''
    });
  };

  const handleUpdateDelivery = (order) => {
    setSelectedOrder(order);
    setShowDeliveryModal(true);
  };

  const handleDeliveryUpdated = () => {
    setShowDeliveryModal(false);
    setSelectedOrder(null);
    loadOrders();
  };

  // Pagination
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentOrders = filteredOrders.slice(indexOfFirstItem, indexOfLastItem);
  const totalPages = Math.ceil(filteredOrders.length / itemsPerPage);

  const goToPage = (page) => {
    setCurrentPage(Math.max(1, Math.min(page, totalPages)));
  };

  if (!isAdmin) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-xl text-red-600">Access denied. Admin privileges required.</div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-xl text-gray-600">Loading orders...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-xl text-red-600">Error: {error}</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-8">Order Management</h1>

      {/* Filters Section */}
      <div className="bg-white rounded-lg shadow-lg p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Filters</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {/* Order ID Search */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Order ID</label>
            <input
              type="text"
              value={filters.orderId}
              onChange={(e) => handleFilterChange('orderId', e.target.value)}
              placeholder="Search by order ID"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Customer Search */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Customer</label>
            <input
              type="text"
              value={filters.customerSearch}
              onChange={(e) => handleFilterChange('customerSearch', e.target.value)}
              placeholder="Username or User ID"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Status Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Order Status</label>
            <select
              value={filters.status}
              onChange={(e) => handleFilterChange('status', e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </div>

          {/* Tracking Status Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Tracking Status</label>
            <select
              value={filters.trackingStatus}
              onChange={(e) => handleFilterChange('trackingStatus', e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Tracking Statuses</option>
              <option value="Order Placed">Order Placed</option>
              <option value="Processing">Processing</option>
              <option value="In Warehouse">In Warehouse</option>
              <option value="On The Way">On The Way</option>
              <option value="Out for Delivery">Out for Delivery</option>
              <option value="Delivered">Delivered</option>
            </select>
          </div>

          {/* Start Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
            <input
              type="date"
              value={filters.startDate}
              onChange={(e) => handleFilterChange('startDate', e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* End Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">End Date</label>
            <input
              type="date"
              value={filters.endDate}
              onChange={(e) => handleFilterChange('endDate', e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Min Amount */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Min Amount ($)</label>
            <input
              type="number"
              step="0.01"
              value={filters.minAmount}
              onChange={(e) => handleFilterChange('minAmount', e.target.value)}
              placeholder="0.00"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Max Amount */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Max Amount ($)</label>
            <input
              type="number"
              step="0.01"
              value={filters.maxAmount}
              onChange={(e) => handleFilterChange('maxAmount', e.target.value)}
              placeholder="999999.99"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        <div className="mt-4 flex gap-2">
          <button
            onClick={clearFilters}
            className="px-4 py-2 bg-gray-500 text-white rounded-lg hover:bg-gray-600 transition"
          >
            Clear Filters
          </button>
        </div>
      </div>

      {/* Sorting and Pagination Controls */}
      <div className="bg-white rounded-lg shadow-lg p-4 mb-6 flex flex-wrap items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <label className="text-sm font-medium text-gray-700">Sort by:</label>
          <select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="date">Order Date</option>
            <option value="amount">Total Amount</option>
            <option value="customer">Customer Name</option>
            <option value="status">Status</option>
          </select>
          <select
            value={sortOrder}
            onChange={(e) => setSortOrder(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="desc">Descending</option>
            <option value="asc">Ascending</option>
          </select>
        </div>

        <div className="flex items-center gap-4">
          <label className="text-sm font-medium text-gray-700">Items per page:</label>
          <select
            value={itemsPerPage}
            onChange={(e) => {
              setItemsPerPage(Number(e.target.value));
              setCurrentPage(1);
            }}
            className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="10">10</option>
            <option value="25">25</option>
            <option value="50">50</option>
            <option value="100">100</option>
          </select>
        </div>

        <div className="text-sm text-gray-600">
          Showing {indexOfFirstItem + 1}-{Math.min(indexOfLastItem, filteredOrders.length)} of {filteredOrders.length} orders
        </div>
      </div>

      {/* Orders List */}
      {currentOrders.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          No orders found matching the filters.
        </div>
      ) : (
        <div className="space-y-6">
          {currentOrders.map(order => (
            <div key={order.id} className="bg-white rounded-lg shadow-lg p-6">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h2 className="text-xl font-bold text-gray-800">
                    Order #{order.id}
                  </h2>
                  <p className="text-gray-600">Customer: {order.username} (ID: {order.userId})</p>
                  <p className="text-gray-600">
                    Date: {new Date(order.orderDate).toLocaleString()}
                  </p>
                  {order.completedDate && (
                    <p className="text-gray-600">
                      Completed: {new Date(order.completedDate).toLocaleString()}
                    </p>
                  )}
                  {order.trackingStatus && (
                    <p className="text-sm text-blue-600 font-medium mt-1">
                      📦 {order.trackingStatus}
                    </p>
                  )}
                </div>
                <div className="text-right">
                  <span className={`inline-block px-4 py-2 rounded-lg font-semibold ${
                    order.status === 'Completed' ? 'bg-green-100 text-green-800' :
                    order.status === 'Cancelled' ? 'bg-red-100 text-red-800' :
                    'bg-yellow-100 text-yellow-800'
                  }`}>
                    {order.status}
                  </span>
                  <p className="text-2xl font-bold text-gray-800 mt-2">
                    ${order.totalAmount.toFixed(2)}
                  </p>
                </div>
              </div>

              <div className="border-t pt-4">
                <h3 className="font-semibold mb-2">Items:</h3>
                <div className="space-y-2">
                  {order.items.map(item => (
                    <div key={item.id} className="flex justify-between items-center bg-gray-50 p-3 rounded">
                      <div>
                        <p className="font-medium">{item.bookTitle}</p>
                        <p className="text-sm text-gray-600">
                          Quantity: {item.quantity} × ${item.unitPrice.toFixed(2)}
                        </p>
                      </div>
                      <p className="font-semibold">${item.totalPrice.toFixed(2)}</p>
                    </div>
                  ))}
                </div>
              </div>

              {/* Delivery Information */}
              {(order.phoneNumber || order.alternativePhoneNumber || order.homeAddress) && (
                <div className="border-t pt-4 mt-4">
                  <h3 className="font-semibold mb-2">Delivery Information:</h3>
                  <div className="bg-gray-50 p-3 rounded space-y-1">
                    {order.phoneNumber && (
                      <p className="text-sm text-gray-700">
                        <span className="font-medium">Phone:</span> {order.phoneNumber}
                      </p>
                    )}
                    {order.alternativePhoneNumber && (
                      <p className="text-sm text-gray-700">
                        <span className="font-medium">Alt. Phone:</span> {order.alternativePhoneNumber}
                      </p>
                    )}
                    {order.homeAddress && (
                      <p className="text-sm text-gray-700">
                        <span className="font-medium">Address:</span> {order.homeAddress}
                      </p>
                    )}
                  </div>
                </div>
              )}

              {/* Admin Actions */}
              <div className="mt-4 border-t pt-4">
                <button
                  onClick={() => handleUpdateDelivery(order)}
                  className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition"
                >
                  Update Delivery Info
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="mt-6 flex justify-center items-center gap-2">
          <button
            onClick={() => goToPage(currentPage - 1)}
            disabled={currentPage === 1}
            className="px-4 py-2 bg-gray-200 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-300 transition"
          >
            Previous
          </button>
          
          {Array.from({ length: Math.min(totalPages, 7) }, (_, i) => {
            let pageNum;
            if (totalPages <= 7) {
              pageNum = i + 1;
            } else if (currentPage <= 4) {
              pageNum = i + 1;
            } else if (currentPage >= totalPages - 3) {
              pageNum = totalPages - 6 + i;
            } else {
              pageNum = currentPage - 3 + i;
            }
            
            return (
              <button
                key={pageNum}
                onClick={() => goToPage(pageNum)}
                className={`px-4 py-2 rounded-lg transition ${
                  currentPage === pageNum
                    ? 'bg-blue-500 text-white'
                    : 'bg-gray-200 hover:bg-gray-300'
                }`}
              >
                {pageNum}
              </button>
            );
          })}

          <button
            onClick={() => goToPage(currentPage + 1)}
            disabled={currentPage === totalPages}
            className="px-4 py-2 bg-gray-200 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-300 transition"
          >
            Next
          </button>
        </div>
      )}

      {/* Delivery Info Modal */}
      {showDeliveryModal && selectedOrder && (
        <DeliveryInfoModal
          order={selectedOrder}
          onClose={() => {
            setShowDeliveryModal(false);
            setSelectedOrder(null);
          }}
          onUpdate={handleDeliveryUpdated}
        />
      )}
    </div>
  );
}

export default AdminOrderManagement;
