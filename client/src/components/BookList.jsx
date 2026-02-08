import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { bookService } from '../services/bookService';
import { orderService } from '../services/orderService';
import { authService } from '../services/authService';
import { categoryService } from '../services/categoryService';
import BookDetail from './BookDetail';

function BookList() {
  const navigate = useNavigate();
  const location = useLocation();
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [cart, setCart] = useState([]);
  const [showCart, setShowCart] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [categories, setCategories] = useState([]);
  const [imageErrors, setImageErrors] = useState({});
  const [visibleCount, setVisibleCount] = useState(10);
  const [showDeliveryForm, setShowDeliveryForm] = useState(false);
  const [deliveryInfo, setDeliveryInfo] = useState({
    phoneNumber: '',
    alternativePhoneNumber: '',
    homeAddress: ''
  });
  const [deliveryErrors, setDeliveryErrors] = useState({});
  const [selectedBook, setSelectedBook] = useState(null);

  // Load cart from localStorage on mount
  useEffect(() => {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      try {
        const parsedCart = JSON.parse(savedCart);
        // Validate that it's an array and has expected structure
        if (Array.isArray(parsedCart)) {
          const validCart = parsedCart.filter(item => 
            item && 
            typeof item === 'object' && 
            typeof item.bookId === 'number' && 
            typeof item.quantity === 'number' && 
            item.quantity > 0 &&
            item.book &&
            typeof item.book === 'object'
          );
          setCart(validCart);
        }
      } catch (err) {
        console.error('Failed to load cart from localStorage:', err);
        localStorage.removeItem('cart'); // Clear invalid data
      }
    }
  }, []);

  // Save cart to localStorage whenever it changes
  useEffect(() => {
    if (cart.length > 0) {
      localStorage.setItem('cart', JSON.stringify(cart));
    } else {
      // Remove cart from localStorage when empty to keep storage clean
      localStorage.removeItem('cart');
    }
  }, [cart]);

  useEffect(() => {
    loadBooks();
    loadCategories();
  }, []);

  const loadBooks = async () => {
    try {
      setLoading(true);
      const data = await bookService.getAllBooks();
      setBooks(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const loadCategories = async () => {
    try {
      const data = await categoryService.getAllCategories();
      // Add 'All' option at the beginning, then category names
      const categoryNames = ['All', ...data.map(cat => cat.name)];
      setCategories(categoryNames);
    } catch (err) {
      console.error('Error loading categories:', err);
      // Fallback to empty categories if API fails
      setCategories(['All']);
    }
  };

  const filteredBooks = selectedCategory === 'All' 
    ? books 
    : books.filter(book => book.categoryName === selectedCategory);

  const visibleBooks = filteredBooks.slice(0, visibleCount);

  useEffect(() => {
    // Reset visible count when category changes
    setVisibleCount(10);
  }, [selectedCategory]);

  useEffect(() => {
    const handleScroll = () => {
      const scrollPosition = window.innerHeight + window.scrollY;
      const threshold = document.documentElement.scrollHeight - 200;
      
      if (scrollPosition >= threshold && visibleCount < filteredBooks.length) {
        setVisibleCount(prev => Math.min(prev + 10, filteredBooks.length));
      }
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [visibleCount, filteredBooks.length]);

  const addToCart = (book) => {
    // Check if user is authenticated
    if (!authService.isAuthenticated()) {
      // Redirect to login page with state to return here after login
      navigate('/login', { state: { from: location.pathname } });
      return;
    }

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

  const validateDeliveryInfo = () => {
    const errors = {};
    const phoneRegex = /^[+]?[0-9\s\-()]{7,20}$/;
    
    if (!deliveryInfo.phoneNumber.trim()) {
      errors.phoneNumber = 'Phone number is required';
    } else if (!phoneRegex.test(deliveryInfo.phoneNumber)) {
      errors.phoneNumber = 'Phone number must be 7-20 characters and contain only numbers, +, spaces, -, and parentheses';
    }
    
    if (!deliveryInfo.alternativePhoneNumber.trim()) {
      errors.alternativePhoneNumber = 'Alternative phone number is required';
    } else if (!phoneRegex.test(deliveryInfo.alternativePhoneNumber)) {
      errors.alternativePhoneNumber = 'Alternative phone number must be 7-20 characters and contain only numbers, +, spaces, -, and parentheses';
    }
    
    if (!deliveryInfo.homeAddress.trim()) {
      errors.homeAddress = 'Home address is required';
    } else if (deliveryInfo.homeAddress.trim().length < 10) {
      errors.homeAddress = 'Home address must be at least 10 characters';
    } else if (deliveryInfo.homeAddress.trim().length > 500) {
      errors.homeAddress = 'Home address must not exceed 500 characters';
    }
    
    setDeliveryErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleProceedToDelivery = () => {
    if (cart.length === 0) {
      alert('Your cart is empty');
      return;
    }
    setShowDeliveryForm(true);
  };

  const handleCheckout = async () => {
    if (!validateDeliveryInfo()) {
      return;
    }
    
    try {
      const orderItems = cart.map(item => ({
        bookId: item.bookId,
        quantity: item.quantity
      }));
      
      await orderService.createOrder(orderItems, deliveryInfo);
      alert('Order placed successfully!');
      setCart([]);
      setShowCart(false);
      setShowDeliveryForm(false);
      setDeliveryInfo({
        phoneNumber: '',
        alternativePhoneNumber: '',
        homeAddress: ''
      });
      setDeliveryErrors({});
      loadBooks(); // Reload to update stock
    } catch (err) {
      alert('Failed to place order: ' + err.message);
    }
  };

  const handleDeliveryInfoChange = (field, value) => {
    setDeliveryInfo(prev => ({
      ...prev,
      [field]: value
    }));
    // Clear error for this field when user starts typing
    if (deliveryErrors[field]) {
      setDeliveryErrors(prev => ({
        ...prev,
        [field]: ''
      }));
    }
  };

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

  const isAdmin = authService.isAdmin();
  const isAuthenticated = authService.isAuthenticated();

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold text-gray-800">Book Store</h1>
        {!isAdmin && isAuthenticated && (
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
        )}
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
      {showCart && !isAdmin && isAuthenticated && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold">
                {showDeliveryForm ? 'Delivery Information' : 'Shopping Cart'}
              </h2>
              <button
                onClick={() => {
                  setShowCart(false);
                  setShowDeliveryForm(false);
                  setDeliveryErrors({});
                }}
                className="text-gray-500 hover:text-gray-700 text-2xl"
              >
                ×
              </button>
            </div>

            {!showDeliveryForm ? (
              // Cart View
              cart.length === 0 ? (
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
                      onClick={handleProceedToDelivery}
                      className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 transition"
                    >
                      Proceed to Delivery Information
                    </button>
                  </div>
                </>
              )
            ) : (
              // Delivery Form View
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Phone Number <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="tel"
                    value={deliveryInfo.phoneNumber}
                    onChange={(e) => handleDeliveryInfoChange('phoneNumber', e.target.value)}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 ${
                      deliveryErrors.phoneNumber 
                        ? 'border-red-500 focus:ring-red-500' 
                        : 'border-gray-300 focus:ring-blue-500'
                    }`}
                    placeholder="+1 234 567 8900"
                  />
                  {deliveryErrors.phoneNumber && (
                    <p className="text-red-500 text-sm mt-1">{deliveryErrors.phoneNumber}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Alternative Phone Number <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="tel"
                    value={deliveryInfo.alternativePhoneNumber}
                    onChange={(e) => handleDeliveryInfoChange('alternativePhoneNumber', e.target.value)}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 ${
                      deliveryErrors.alternativePhoneNumber 
                        ? 'border-red-500 focus:ring-red-500' 
                        : 'border-gray-300 focus:ring-blue-500'
                    }`}
                    placeholder="+1 234 567 8901"
                  />
                  {deliveryErrors.alternativePhoneNumber && (
                    <p className="text-red-500 text-sm mt-1">{deliveryErrors.alternativePhoneNumber}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Home Address <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    value={deliveryInfo.homeAddress}
                    onChange={(e) => handleDeliveryInfoChange('homeAddress', e.target.value)}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 ${
                      deliveryErrors.homeAddress 
                        ? 'border-red-500 focus:ring-red-500' 
                        : 'border-gray-300 focus:ring-blue-500'
                    }`}
                    rows="3"
                    placeholder="123 Main St, Apt 4, City, State, ZIP"
                  />
                  {deliveryErrors.homeAddress && (
                    <p className="text-red-500 text-sm mt-1">{deliveryErrors.homeAddress}</p>
                  )}
                </div>

                <div className="mt-6 border-t pt-4">
                  <div className="flex justify-between text-xl font-bold mb-4">
                    <span>Total:</span>
                    <span>${cartTotal.toFixed(2)}</span>
                  </div>
                  <div className="flex gap-3">
                    <button
                      onClick={() => setShowDeliveryForm(false)}
                      className="flex-1 bg-gray-500 text-white py-3 rounded-lg hover:bg-gray-600 transition"
                    >
                      Back to Cart
                    </button>
                    <button
                      onClick={handleCheckout}
                      className="flex-1 bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 transition"
                    >
                      Place Order
                    </button>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Book Detail Modal */}
      {selectedBook && (
        <BookDetail book={selectedBook} onClose={() => setSelectedBook(null)} />
      )}

      {/* Books Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {visibleBooks.map(book => (
          <div
            key={book.id}
            className="bg-white rounded-lg shadow-lg overflow-hidden hover:shadow-xl transition-shadow duration-300 cursor-pointer"
            onClick={() => setSelectedBook(book)}
          >
            {book.coverImagePath && !imageErrors[book.id] ? (
              <div className="h-48 overflow-hidden">
                <img 
                  src={`${(import.meta.env.VITE_API_URL || 'http://localhost:5000/api').replace('/api', '')}${book.coverImagePath}`}
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

              {!isAdmin && (
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    addToCart(book);
                  }}
                  disabled={book.stockQuantity === 0}
                  className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition"
                >
                  {book.stockQuantity > 0 ? 'Add to Cart' : 'Out of Stock'}
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Show message when all books are loaded */}
      {visibleCount >= filteredBooks.length && filteredBooks.length > 10 && (
        <div className="text-center py-8 text-gray-500">
          All {filteredBooks.length} books loaded
        </div>
      )}

      {filteredBooks.length === 0 && (
        <div className="text-center py-12 text-gray-500">
          No books found in this category.
        </div>
      )}
    </div>
  );
}

export default BookList;
