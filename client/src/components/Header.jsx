import { NavLink, useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';

function Header({ onLogout }) {
  const username = authService.getUsername();
  const isAdmin = authService.isAdmin();
  const isAuthenticated = authService.isAuthenticated();
  const navigate = useNavigate();

  const handleLogout = () => {
    onLogout();
    navigate('/login', { replace: true });
  };

  const navLinkClass = ({ isActive }) =>
    `px-4 py-2 rounded-lg transition ${
      isActive 
        ? 'bg-blue-700 text-white' 
        : 'text-white hover:bg-white hover:text-blue-600'
    }`;

  return (
    <header className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4 py-4">
        <div className="flex justify-between items-center">
          <div className="flex items-center space-x-6">
            <h1 className="text-2xl font-bold">📚 BookShop</h1>
            <nav className="flex space-x-4">
              <NavLink to="/books" className={navLinkClass}>
                Books
              </NavLink>
              {isAuthenticated && (
                <>
                  {isAdmin && (
                    <>
                      <NavLink to="/admin/books" className={navLinkClass}>
                        Manage Books
                      </NavLink>
                      <NavLink to="/admin/orders" className={navLinkClass}>
                        Manage Orders
                      </NavLink>
                      <NavLink to="/admin/users" className={navLinkClass}>
                        Manage Users
                      </NavLink>
                    </>
                  )}
                  {!isAdmin && (
                    <NavLink to="/orders" className={navLinkClass}>
                      My Orders
                    </NavLink>
                  )}
                </>
              )}
            </nav>
          </div>
          <div className="flex items-center space-x-4">
            {isAuthenticated ? (
              <>
                <span className="text-sm">
                  Welcome, <span className="font-semibold">{username}</span>
                  {isAdmin && <span className="ml-2 px-2 py-1 bg-yellow-500 text-xs rounded">Admin</span>}
                </span>
                <button
                  onClick={handleLogout}
                  className="bg-red-500 hover:bg-red-600 px-4 py-2 rounded-lg transition"
                >
                  Logout
                </button>
              </>
            ) : (
              <button
                onClick={() => navigate('/login')}
                className="bg-green-500 hover:bg-green-600 px-4 py-2 rounded-lg transition"
              >
                Login
              </button>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}

export default Header;
