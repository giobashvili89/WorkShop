import { useState, useEffect } from 'react';
import { orderService } from '../services/orderService';
import { authService } from '../services/authService';

function OrderHistory() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const isAdmin = authService.isAdmin();

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = isAdmin 
        ? await orderService.getAllOrders()
        : await orderService.getMyOrders();
      setOrders(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleCancelOrder = async (orderId) => {
    if (!confirm('Are you sure you want to cancel this order?')) return;
    
    try {
      await orderService.cancelOrder(orderId);
      alert('Order cancelled successfully! A confirmation email has been sent.');
      loadOrders();
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.message;
      alert('Failed to cancel order: ' + errorMessage);
    }
  };

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
      <h1 className="text-3xl font-bold text-gray-800 mb-8">
        {isAdmin ? 'All Orders' : 'My Order History'}
      </h1>

      {orders.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          No orders found.
        </div>
      ) : (
        <div className="space-y-6">
          {orders.map(order => (
            <div key={order.id} className="bg-white rounded-lg shadow-lg p-6">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h2 className="text-xl font-bold text-gray-800">
                    Order #{order.id}
                  </h2>
                  {isAdmin && (
                    <p className="text-gray-600">Customer: {order.username}</p>
                  )}
                  <p className="text-gray-600">
                    Date: {new Date(order.orderDate).toLocaleString()}
                  </p>
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

              {!isAdmin && order.canCancel && (
                <div className="mt-4 border-t pt-4">
                  <div className="flex justify-between items-center">
                    <p className="text-sm text-gray-600">
                      ℹ️ You can cancel this order within 1 hour of placement
                    </p>
                    <button
                      onClick={() => handleCancelOrder(order.id)}
                      className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600 transition"
                    >
                      Cancel Order
                    </button>
                  </div>
                </div>
              )}

              {!isAdmin && order.status === 'Completed' && !order.canCancel && (
                <div className="mt-4 border-t pt-4">
                  <p className="text-sm text-gray-500 italic">
                    ⚠️ Cancellation period (1 hour) has expired for this order
                  </p>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default OrderHistory;
