import { useEffect } from 'react';

function BookDetail({ book, onClose }) {
  useEffect(() => {
    const handleEscKey = (event) => {
      if (event.key === 'Escape') {
        onClose();
      }
    };

    document.addEventListener('keydown', handleEscKey);
    return () => {
      document.removeEventListener('keydown', handleEscKey);
    };
  }, [onClose]);

  if (!book) return null;

  return (
    <div 
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      onClick={onClose}
    >
      <div 
        className="bg-white rounded-lg p-6 max-w-3xl w-full max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex justify-between items-start mb-4">
          <h2 className="text-3xl font-bold text-gray-800">{book.title}</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-3xl leading-none"
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <div className="grid md:grid-cols-2 gap-6">
          {/* Book Cover */}
          <div>
            {book.coverImagePath ? (
              <img 
                src={`${(import.meta.env.VITE_API_URL || 'http://localhost:5000/api').replace('/api', '')}${book.coverImagePath}`}
                alt={book.title}
                className="w-full rounded-lg shadow-lg object-cover"
              />
            ) : (
              <div className="w-full h-96 bg-gradient-to-br from-blue-400 to-purple-500 flex items-center justify-center rounded-lg shadow-lg">
                <div className="text-white text-center px-4">
                  <div className="text-6xl mb-2">📚</div>
                  <div className="text-xl opacity-75">{book.categoryName}</div>
                </div>
              </div>
            )}
          </div>

          {/* Book Details */}
          <div className="space-y-4">
            <div>
              <h3 className="text-sm font-semibold text-gray-500 uppercase mb-1">Author</h3>
              <p className="text-lg text-gray-800">{book.author}</p>
            </div>

            <div>
              <h3 className="text-sm font-semibold text-gray-500 uppercase mb-1">Category</h3>
              <p className="text-lg text-gray-800">{book.categoryName}</p>
            </div>

            <div>
              <h3 className="text-sm font-semibold text-gray-500 uppercase mb-1">Price</h3>
              <p className="text-3xl font-bold text-green-600">${book.price.toFixed(2)}</p>
            </div>

            <div>
              <h3 className="text-sm font-semibold text-gray-500 uppercase mb-1">Availability</h3>
              <p className={`text-lg font-semibold ${book.stockQuantity > 0 ? 'text-green-600' : 'text-red-600'}`}>
                {book.stockQuantity > 0 ? `${book.stockQuantity} in stock` : 'Out of stock'}
              </p>
            </div>

            {book.isbn && (
              <div>
                <h3 className="text-sm font-semibold text-gray-500 uppercase mb-1">ISBN</h3>
                <p className="text-gray-800">{book.isbn}</p>
              </div>
            )}

            {book.publicationDate && (
              <div>
                <h3 className="text-sm font-semibold text-gray-500 uppercase mb-1">Publication Date</h3>
                <p className="text-gray-800">{new Date(book.publicationDate).toLocaleDateString()}</p>
              </div>
            )}
          </div>
        </div>

        {/* Description */}
        <div className="mt-6">
          <h3 className="text-sm font-semibold text-gray-500 uppercase mb-2">Description</h3>
          <p className="text-gray-700 leading-relaxed">{book.description}</p>
        </div>

        {/* Close button at bottom */}
        <div className="mt-6 flex justify-end">
          <button
            onClick={onClose}
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
}

export default BookDetail;
