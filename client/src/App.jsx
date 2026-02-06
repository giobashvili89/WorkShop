import { useState, useEffect } from 'react';
import Login from './components/Login';
import Header from './components/Header';
import BookList from './components/BookList';
import AdminBookManagement from './components/AdminBookManagement';
import OrderHistory from './components/OrderHistory';
import { authService } from './services/authService';
import './App.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [currentView, setCurrentView] = useState('books');

  useEffect(() => {
    setIsAuthenticated(authService.isAuthenticated());
  }, []);

  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
    setCurrentView('books');
  };

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setCurrentView('books');
  };

  if (!isAuthenticated) {
    return <Login onLoginSuccess={handleLoginSuccess} />;
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <Header 
        onLogout={handleLogout}
        currentView={currentView}
        setCurrentView={setCurrentView}
      />
      
      <main>
        {currentView === 'books' && <BookList />}
        {currentView === 'admin' && authService.isAdmin() && <AdminBookManagement />}
        {currentView === 'orders' && <OrderHistory />}
      </main>
    </div>
  );
}

export default App;
