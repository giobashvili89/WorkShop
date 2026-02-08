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
      let errorMessage = 'Failed to create order';
      try {
        const errorData = await response.json();
        // Handle ASP.NET Core validation error format
        if (errorData.errors) {
          const messages = [];
          for (const [field, fieldErrors] of Object.entries(errorData.errors)) {
            if (Array.isArray(fieldErrors)) {
              messages.push(...fieldErrors);
            }
          }
          errorMessage = messages.join('. ') || errorMessage;
        }
        // Handle custom middleware error format
        else if (errorData.error) {
          errorMessage = errorData.error;
        }
        // Handle title from problem details
        else if (errorData.title) {
          errorMessage = errorData.title;
        }
      } catch (e) {
        // If JSON parsing fails, try to get text
        try {
          const text = await response.text();
          if (text) errorMessage = text;
        } catch (textError) {
          // Use default error message
        }
      }
      throw new Error(errorMessage);
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

  async updateDeliveryInfo(id, deliveryInfo) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/orders/${id}/delivery`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(deliveryInfo),
    });

    if (!response.ok) {
      let errorMessage = 'Failed to update delivery information';
      try {
        const errorData = await response.json();
        // Handle ASP.NET Core validation error format
        if (errorData.errors) {
          const messages = [];
          for (const [field, fieldErrors] of Object.entries(errorData.errors)) {
            if (Array.isArray(fieldErrors)) {
              messages.push(...fieldErrors);
            }
          }
          errorMessage = messages.join('. ') || errorMessage;
        }
        // Handle custom middleware error format
        else if (errorData.error) {
          errorMessage = errorData.error;
        }
        // Handle title from problem details
        else if (errorData.title) {
          errorMessage = errorData.title;
        }
      } catch (e) {
        // If JSON parsing fails, try to get text
        try {
          const text = await response.text();
          if (text) errorMessage = text;
        } catch (textError) {
          // Use default error message
        }
      }
      throw new Error(errorMessage);
    }
    return response.json();
  }
}

export const orderService = new OrderService();
