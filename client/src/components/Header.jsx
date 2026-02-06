import { authService } from '../services/authService';

function Header({ onLogout, currentView, setCurrentView }) {
  const username = authService.getUsername();
  const isAdmin = authService.isAdmin();

  return (
    <header className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4 py-4">
        <div className="flex justify-between items-center">
          <div className="flex items-center space-x-6">
            <h1 className="text-2xl font-bold">📚 BookShop</h1>
            <nav className="flex space-x-4">
              <button
                onClick={() => setCurrentView('books')}
                className={`px-4 py-2 rounded-lg transition text-white ${
                  currentView === 'books'
                    ? 'bg-blue-700'
                    : 'bg-blue-600 hover:bg-blue-500'
                }`}
              >
                Browse Books
              </button>
              {isAdmin && (
                <button
                  onClick={() => setCurrentView('admin')}
                  className={`px-4 py-2 rounded-lg transition text-white ${
                    currentView === 'admin'
                      ? 'bg-blue-700'
                      : 'bg-blue-600 hover:bg-blue-500'
                  }`}
                >
                  Manage Books
                </button>
              )}
              <button
                onClick={() => setCurrentView('orders')}
                className={`px-4 py-2 rounded-lg transition text-white ${
                  currentView === 'orders'
                    ? 'bg-blue-700'
                    : 'bg-blue-600 hover:bg-blue-500'
                }`}
              >
                My Orders
              </button>
            </nav>
          </div>
          <div className="flex items-center space-x-4">
            <span className="text-sm">
              Welcome, <span className="font-semibold">{username}</span>
              {isAdmin && <span className="ml-2 px-2 py-1 bg-yellow-500 text-xs rounded">Admin</span>}
            </span>
            <button
              onClick={onLogout}
              className="bg-red-500 hover:bg-red-600 px-4 py-2 rounded-lg transition"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}

export default Header;
