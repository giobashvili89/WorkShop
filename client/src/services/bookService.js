import { authService } from './authService';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class BookService {
  async getAllBooks() {
    const response = await fetch(`${API_URL}/books`);
    if (!response.ok) {
      throw new Error('Failed to fetch books');
    }
    return response.json();
  }

  async getBookById(id) {
    const response = await fetch(`${API_URL}/books/${id}`);
    if (!response.ok) {
      throw new Error('Failed to fetch book');
    }
    return response.json();
  }

  async getBooksByCategory(category) {
    const response = await fetch(`${API_URL}/books/category/${category}`);
    if (!response.ok) {
      throw new Error('Failed to fetch books');
    }
    return response.json();
  }

  async createBook(book) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/books`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(book),
    });

    if (!response.ok) {
      throw new Error('Failed to create book');
    }
    return response.json();
  }

  async updateBook(id, book) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/books/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(book),
    });

    if (!response.ok) {
      throw new Error('Failed to update book');
    }
    return response.json();
  }

  async deleteBook(id) {
    const token = authService.getToken();
    const response = await fetch(`${API_URL}/books/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to delete book');
    }
  }
}

export const bookService = new BookService();
