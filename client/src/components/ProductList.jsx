import { useState, useEffect } from 'react';
import { productService } from '../services/productService';
import './ProductList.css';

function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [newProduct, setNewProduct] = useState({
    name: '',
    description: '',
    price: 0
  });

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productService.getAllProducts();
      setProducts(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await productService.createProduct(newProduct);
      setNewProduct({ name: '', description: '', price: 0 });
      await loadProducts();
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDelete = async (id) => {
    try {
      await productService.deleteProduct(id);
      await loadProducts();
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) return <div className="loading">Loading products...</div>;
  if (error) return <div className="error">Error: {error}</div>;

  return (
    <div className="product-container">
      <h2>Product Management</h2>
      
      <div className="add-product-form">
        <h3>Add New Product</h3>
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            placeholder="Product Name"
            value={newProduct.name}
            onChange={(e) => setNewProduct({ ...newProduct, name: e.target.value })}
            required
          />
          <input
            type="text"
            placeholder="Description"
            value={newProduct.description}
            onChange={(e) => setNewProduct({ ...newProduct, description: e.target.value })}
            required
          />
          <input
            type="number"
            step="0.01"
            placeholder="Price"
            value={newProduct.price}
            onChange={(e) => setNewProduct({ ...newProduct, price: parseFloat(e.target.value) })}
            required
          />
          <button type="submit">Add Product</button>
        </form>
      </div>

      <div className="product-list">
        <h3>Products</h3>
        {products.length === 0 ? (
          <p>No products available</p>
        ) : (
          <div className="products-grid">
            {products.map((product) => (
              <div key={product.id} className="product-card">
                <h4>{product.name}</h4>
                <p>{product.description}</p>
                <p className="price">${product.price.toFixed(2)}</p>
                <button 
                  className="delete-btn"
                  onClick={() => handleDelete(product.id)}
                >
                  Delete
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default ProductList;
