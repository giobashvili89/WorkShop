import { useState } from 'react';
import { orderService } from '../services/orderService';

function DeliveryInfoModal({ order, onClose, onUpdate }) {
  const [formData, setFormData] = useState({
    trackingStatus: order.trackingStatus || '',
    status: order.status || '',
    phoneNumber: order.phoneNumber || '',
    alternativePhoneNumber: order.alternativePhoneNumber || '',
    homeAddress: order.homeAddress || '',
    sendEmail: false
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const trackingStatuses = [
    'Order Placed',
    'Processing',
    'In Warehouse',
    'On The Way',
    'Out for Delivery',
    'Delivered'
  ];

  const orderStatuses = ['Pending', 'Completed', 'Cancelled'];

  const phoneRegex = /^\+?[0-9\s\-()]{7,20}$/;

  const validateForm = () => {
    if (formData.phoneNumber && !phoneRegex.test(formData.phoneNumber)) {
      setError('Phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.');
      return false;
    }
    if (formData.alternativePhoneNumber && !phoneRegex.test(formData.alternativePhoneNumber)) {
      setError('Alternative phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.');
      return false;
    }
    if (formData.homeAddress && formData.homeAddress.length < 10) {
      setError('Address must be at least 10 characters long.');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      await orderService.updateDeliveryInfo(order.id, formData);
      alert('Delivery information updated successfully!' + (formData.sendEmail ? ' Email notification sent.' : ''));
      onUpdate();
    } catch (err) {
      setError(err.message || 'Failed to update delivery information');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white border-b px-6 py-4 flex justify-between items-center">
          <h2 className="text-2xl font-bold text-gray-800">
            Update Delivery Information - Order #{order.id}
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
              {error}
            </div>
          )}

          {/* Customer Info (Read-only) */}
          <div className="mb-6 p-4 bg-gray-50 rounded-lg">
            <h3 className="font-semibold text-gray-700 mb-2">Customer Information</h3>
            <p className="text-sm text-gray-600">Customer: {order.username}</p>
            <p className="text-sm text-gray-600">Order Date: {new Date(order.orderDate).toLocaleString()}</p>
            <p className="text-sm text-gray-600">Total Amount: ${order.totalAmount.toFixed(2)}</p>
          </div>

          {/* Order Status */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Order Status
            </label>
            <select
              name="status"
              value={formData.status}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {orderStatuses.map(status => (
                <option key={status} value={status}>{status}</option>
              ))}
            </select>
          </div>

          {/* Tracking Status */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tracking Status
            </label>
            <select
              name="trackingStatus"
              value={formData.trackingStatus}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {trackingStatuses.map(status => (
                <option key={status} value={status}>{status}</option>
              ))}
            </select>
          </div>

          {/* Phone Number */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Phone Number
            </label>
            <input
              type="text"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              placeholder="+1234567890"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <p className="text-xs text-gray-500 mt-1">
              7-20 characters, can include digits, spaces, hyphens, parentheses, and optional leading +
            </p>
          </div>

          {/* Alternative Phone Number */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Alternative Phone Number
            </label>
            <input
              type="text"
              name="alternativePhoneNumber"
              value={formData.alternativePhoneNumber}
              onChange={handleChange}
              placeholder="+0987654321"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Home Address */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Home Address
            </label>
            <textarea
              name="homeAddress"
              value={formData.homeAddress}
              onChange={handleChange}
              placeholder="123 Main St, City, State, ZIP"
              rows="3"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <p className="text-xs text-gray-500 mt-1">
              Minimum 10 characters
            </p>
          </div>

          {/* Send Email Checkbox */}
          <div className="mb-6">
            <label className="flex items-center">
              <input
                type="checkbox"
                name="sendEmail"
                checked={formData.sendEmail}
                onChange={handleChange}
                className="mr-2 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <span className="text-sm font-medium text-gray-700">
                Send email notification to customer
              </span>
            </label>
          </div>

          {/* Action Buttons */}
          <div className="flex justify-end gap-3">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="px-4 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition disabled:opacity-50"
            >
              {loading ? 'Updating...' : 'Update Delivery Info'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default DeliveryInfoModal;
