const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class AuthService {
  async register(username, email, password) {
    const response = await fetch(`${API_URL}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, email, password }),
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Registration failed');
    }

    const data = await response.json();
    this.setAuthData(data);
    return data;
  }

  async login(username, password) {
    const response = await fetch(`${API_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, password }),
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Login failed');
    }

    const data = await response.json();
    this.setAuthData(data);
    return data;
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
  }

  setAuthData(data) {
    localStorage.setItem('token', data.token);
    localStorage.setItem('username', data.username);
    localStorage.setItem('role', data.role);
    localStorage.setItem('userId', data.userId);
  }

  getToken() {
    return localStorage.getItem('token');
  }

  getUsername() {
    return localStorage.getItem('username');
  }

  getRole() {
    return localStorage.getItem('role');
  }

  getUserId() {
    return localStorage.getItem('userId');
  }

  isAuthenticated() {
    return !!this.getToken();
  }

  isAdmin() {
    return this.getRole() === 'Admin';
  }
}

export const authService = new AuthService();
