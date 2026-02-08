import { authService } from './authService';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class UserService {
  async getAllUsers() {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/users`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch users');
    }
    return response.json();
  }

  async blockUser(id) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/users/${id}/block`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to block user');
    }
    return response.json();
  }

  async unblockUser(id) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/users/${id}/unblock`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to unblock user');
    }
    return response.json();
  }
}

export const userService = new UserService();
