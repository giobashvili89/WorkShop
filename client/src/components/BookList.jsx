import { useState, useEffect } from 'react';
import { bookService } from '../services/bookService';
import { orderService } from '../services/orderService';

function BookList() {
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [cart, setCart] = useState([]);
  const [showCart, setShowCart] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [categories, setCategories] = useState([]);
  const [imageErrors, setImageErrors] = useState({});

  useEffect(() => {
    loadBooks();
  }, []);

  const loadBooks = async () => {
    try {
      setLoading(true);
      const data = await bookService.getAllBooks();
      setBooks(data);
      
      // Extract unique categories
      const uniqueCategories = ['All', ...new Set(data.map(book => book.category))];
      setCategories(uniqueCategories);
      
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const addToCart = (book) => {
    const existingItem = cart.find(item => item.bookId === book.id);
    const currentQuantityInCart = existingItem ? existingItem.quantity : 0;
    
    if (currentQuantityInCart >= book.stockQuantity) {
      alert('Cannot add more items than available in stock');
      return;
    }
    
    if (existingItem) {
      setCart(cart.map(item => 
        item.bookId === book.id 
          ? { ...item, quantity: item.quantity + 1 }
          : item
      ));
    } else {
      setCart([...cart, { bookId: book.id, quantity: 1, book }]);
    }
  };

  const removeFromCart = (bookId) => {
    setCart(cart.filter(item => item.bookId !== bookId));
  };

  const updateQuantity = (bookId, newQuantity) => {
    if (newQuantity < 1) {
      removeFromCart(bookId);
    } else {
      const cartItem = cart.find(item => item.bookId === bookId);
      if (cartItem && newQuantity > cartItem.book.stockQuantity) {
        alert('Cannot exceed available stock quantity');
        return;
      }
      
      setCart(cart.map(item => 
        item.bookId === bookId 
          ? { ...item, quantity: newQuantity }
          : item
      ));
    }
  };

  const handleCheckout = async () => {
    try {
      const orderItems = cart.map(item => ({
        bookId: item.bookId,
        quantity: item.quantity
      }));
      
      await orderService.createOrder(orderItems);
      alert('Order placed successfully!');
      setCart([]);
      setShowCart(false);
      loadBooks(); // Reload to update stock
    } catch (err) {
      alert('Failed to place order: ' + err.message);
    }
  };

  const filteredBooks = selectedCategory === 'All' 
    ? books 
    : books.filter(book => book.category === selectedCategory);

  const cartTotal = cart.reduce((sum, item) => sum + (item.book.price * item.quantity), 0);

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-xl text-gray-600">Loading books...</div>
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
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800">Book Store</h1>
        <button
          onClick={() => setShowCart(!showCart)}
          className="relative bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          Cart ({cart.length})
          {cart.length > 0 && (
            <span className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm">
              {cart.length}
            </span>
          )}
        </button>
      </div>

      {/* Category Filter */}
      <div className="mb-6 flex flex-wrap gap-2">
        {categories.map(category => (
          <button
            key={category}
            onClick={() => setSelectedCategory(category)}
            className={`px-4 py-2 rounded-lg transition ${
              selectedCategory === category
                ? 'bg-blue-600 text-white'
                : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
            }`}
          >
            {category}
          </button>
        ))}
      </div>

      {/* Shopping Cart Modal */}
      {showCart && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold">Shopping Cart</h2>
              <button
                onClick={() => setShowCart(false)}
                className="text-gray-500 hover:text-gray-700 text-2xl"
              >
                ×
              </button>
            </div>

            {cart.length === 0 ? (
              <p className="text-gray-500 text-center py-8">Your cart is empty</p>
            ) : (
              <>
                <div className="space-y-4">
                  {cart.map(item => (
                    <div key={item.bookId} className="flex items-center justify-between border-b pb-4">
                      <div className="flex-1">
                        <h3 className="font-semibold">{item.book.title}</h3>
                        <p className="text-gray-600">${item.book.price.toFixed(2)}</p>
                      </div>
                      <div className="flex items-center gap-4">
                        <div className="flex items-center gap-2">
                          <button
                            onClick={() => updateQuantity(item.bookId, item.quantity - 1)}
                            className="bg-gray-200 px-3 py-1 rounded hover:bg-gray-300"
                          >
                            -
                          </button>
                          <span className="w-12 text-center">{item.quantity}</span>
                          <button
                            onClick={() => updateQuantity(item.bookId, item.quantity + 1)}
                            className="bg-gray-200 px-3 py-1 rounded hover:bg-gray-300"
                          >
                            +
                          </button>
                        </div>
                        <button
                          onClick={() => removeFromCart(item.bookId)}
                          className="text-red-600 hover:text-red-800"
                        >
                          Remove
                        </button>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="mt-6 border-t pt-4">
                  <div className="flex justify-between text-xl font-bold mb-4">
                    <span>Total:</span>
                    <span>${cartTotal.toFixed(2)}</span>
                  </div>
                  <button
                    onClick={handleCheckout}
                    className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 transition"
                  >
                    Checkout
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      )}

      {/* Books Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {filteredBooks.map(book => (
          <div
            key={book.id}
            className="bg-white rounded-lg shadow-lg overflow-hidden hover:shadow-xl transition-shadow duration-300"
          >
            {book.coverImagePath && !imageErrors[book.id] ? (
              <div className="h-48 overflow-hidden">
                <img 
                  src={`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}${book.coverImagePath}`}
                  alt={book.title}
                  className="w-full h-full object-cover"
                  onError={() => {
                    setImageErrors(prev => ({ ...prev, [book.id]: true }));
                  }}
                />
              </div>
            ) : (
              <div className="h-48 bg-gradient-to-br from-blue-400 to-purple-500 flex items-center justify-center">
                <div className="text-white text-center px-4">
                  <div className="text-4xl mb-2">📚</div>
                  <div className="text-xs opacity-75">{book.category}</div>
                </div>
              </div>
            )}
            
            <div className="p-4">
              <h3 className="font-bold text-lg mb-2 text-gray-800 line-clamp-2">
                {book.title}
              </h3>
              <p className="text-gray-600 text-sm mb-2">by {book.author}</p>
              <p className="text-gray-500 text-xs mb-4 line-clamp-3">
                {book.description}
              </p>
              
              <div className="flex items-center justify-between mb-3">
                <span className="text-2xl font-bold text-green-600">
                  ${book.price.toFixed(2)}
                </span>
                <span className={`text-sm ${book.stockQuantity > 0 ? 'text-gray-600' : 'text-red-600'}`}>
                  {book.stockQuantity > 0 ? `${book.stockQuantity} in stock` : 'Out of stock'}
                </span>
              </div>

              <button
                onClick={() => addToCart(book)}
                disabled={book.stockQuantity === 0}
                className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition"
              >
                {book.stockQuantity > 0 ? 'Add to Cart' : 'Out of Stock'}
              </button>
            </div>
          </div>
        ))}
      </div>

      {filteredBooks.length === 0 && (
        <div className="text-center py-12 text-gray-500">
          No books found in this category.
        </div>
      )}
    </div>
  );
}

export default BookList;
