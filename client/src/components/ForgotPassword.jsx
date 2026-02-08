import { useState } from 'react';
import { Link } from 'react-router-dom';
import { authService } from '../services/authService';

function ForgotPassword() {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');
    setLoading(true);

    try {
      const response = await authService.forgotPassword(email);
      setMessage('Password reset instructions have been sent to your email.');
      
      // For development/demo purposes, also show the token
      if (response.token) {
        setMessage(`Password reset token: ${response.token}\n\nIn production, this would be sent via email. You can use this token to reset your password.`);
      }
      
      setEmail('');
    } catch (err) {
      setError(err.message || 'Failed to send password reset email. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="max-w-md w-full bg-white rounded-lg shadow-md p-8">
        <h2 className="text-3xl font-bold text-center mb-6 text-gray-800">
          Forgot Password
        </h2>

        <p className="text-gray-600 text-center mb-6">
          Enter your email address and we'll send you instructions to reset your password.
        </p>
        
        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        {message && (
          <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4 whitespace-pre-line">
            {message}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-gray-700 mb-2">Email Address</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
              placeholder="Enter your email"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 disabled:opacity-50 transition duration-200"
          >
            {loading ? 'Sending...' : 'Send Reset Instructions'}
          </button>
        </form>

        <div className="mt-6 text-center space-y-2">
          <Link
            to="/login"
            className="block text-blue-600 hover:underline"
          >
            Back to Login
          </Link>
          
          {message && (
            <Link
              to="/reset-password"
              className="block text-blue-600 hover:underline font-semibold"
            >
              Go to Reset Password
            </Link>
          )}
        </div>
      </div>
    </div>
  );
}

export default ForgotPassword;
