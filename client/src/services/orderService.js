import { authService } from './authService';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class OrderService {
  async createOrder(items, deliveryInfo) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/orders`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify({ 
        items,
        phoneNumber: deliveryInfo.phoneNumber,
        alternativePhoneNumber: deliveryInfo.alternativePhoneNumber,
        homeAddress: deliveryInfo.homeAddress
      }),
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to create order');
    }
    return response.json();
  }

  async getMyOrders() {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/orders/my-orders`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch orders');
    }
    return response.json();
  }

  async getAllOrders() {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/orders`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch all orders');
    }
    return response.json();
  }

  async cancelOrder(id) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/orders/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to cancel order');
    }
  }
}

export const orderService = new OrderService();
