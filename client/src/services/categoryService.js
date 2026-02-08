const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export const categoryService = {
  async getAllCategories() {
    try {
      const response = await fetch(`${API_URL}/api/Categories`);
      
      if (!response.ok) {
        throw new Error('Failed to fetch categories');
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error fetching categories:', error);
      throw error;
    }
  }
};
