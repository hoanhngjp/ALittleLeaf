import { Link } from 'react-router-dom'

/**
 * Reusable product card matching the .item-wrap DOM from Collections/Index.cshtml.
 * Used in CollectionsPage, SearchResultsPage, and anywhere a product grid appears.
 */
export default function ProductCard({ product }) {
  const { productId, productName, productPrice, primaryImage, quantityInStock } = product

  return (
    <div className="item-wrap">
      <div className="item-picture">
        <Link to={`/products/${productId}`}>
          {quantityInStock === 0 && (
            <div className="sold-out">
              <span>Hết hàng</span>
            </div>
          )}
          <img src={primaryImage} alt={productName} />
        </Link>
      </div>
      <div className="item-detail">
        <h3>
          <Link to={`/products/${productId}`}>{productName}</Link>
        </h3>
        <p className="item-price">{productPrice.toLocaleString('vi-VN')}₫</p>
      </div>
    </div>
  )
}
