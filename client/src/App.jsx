import { useEffect } from 'react';
import { BrowserRouter, Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import Login from './components/Login';
import Header from './components/Header';
import BookList from './components/BookList';
import AdminBookManagement from './components/AdminBookManagement';
import AdminOrderManagement from './components/AdminOrderManagement';
import OrderHistory from './components/OrderHistory';
import { authService } from './services/authService';
import './App.css';

// Protected Route wrapper
function ProtectedRoute({ children, adminOnly = false }) {
  const isAuthenticated = authService.isAuthenticated();
  const isAdmin = authService.isAdmin();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (adminOnly && !isAdmin) {
    return <Navigate to="/" replace />;
  }

  return children;
}

// Login page wrapper
function LoginPage() {
  const navigate = useNavigate();
  const isAuthenticated = authService.isAuthenticated();

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  if (isAuthenticated) {
    return null;
  }

  return <Login />;
}

// Main layout wrapper
function Layout({ children, onLogout }) {
  return (
    <div className="min-h-screen bg-gray-100">
      <Header onLogout={onLogout} />
      <main>{children}</main>
    </div>
  );
}

function App() {
  const handleLogout = () => {
    authService.logout();
  };

  return (
    <BrowserRouter>
      <Routes>
        {/* Login route */}
        <Route path="/login" element={<LoginPage />} />

        {/* Public routes with layout */}
        <Route
          path="/"
          element={
            <Layout onLogout={handleLogout}>
              <BookList />
            </Layout>
          }
        />

        <Route
          path="/books"
          element={
            <Layout onLogout={handleLogout}>
              <BookList />
            </Layout>
          }
        />

        <Route
          path="/orders"
          element={
            <ProtectedRoute>
              <Layout onLogout={handleLogout}>
                <OrderHistory />
              </Layout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/books"
          element={
            <ProtectedRoute adminOnly={true}>
              <Layout onLogout={handleLogout}>
                <AdminBookManagement />
              </Layout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/orders"
          element={
            <ProtectedRoute adminOnly={true}>
              <Layout onLogout={handleLogout}>
                <AdminOrderManagement />
              </Layout>
            </ProtectedRoute>
          }
        />

        {/* Catch all - redirect to home */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
