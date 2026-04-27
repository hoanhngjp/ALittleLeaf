import { useCartStore } from '../../store/useCartStore'
import { useCartQuery } from '../../hooks/useCart'

/**
 * Sidebar order summary — mirrors _SideBarContent.cshtml.
 * Calls useCartQuery so the cart is always fetched on hard reload,
 * even on checkout routes that bypass MainLayout.
 */
export default function OrderSummary({ shippingFee = null }) {
  useCartQuery()
  const items    = useCartStore((s) => s.items)
  const subtotal = items.reduce((sum, i) => sum + i.productPrice * i.quantity, 0)
  const total    = subtotal + (shippingFee ?? 0)

  return (
    <div className="sidebar-content">
      <div className="order-summary-sections">

        {/* ── Product list ─────────────────────────────────────────────── */}
        <div className="order-summary-section order-summary-section-product-list">
          <table className="product-table">
            <tbody>
              {items.length === 0 ? (
                <tr><td>Hiện chưa có sản phẩm nào</td></tr>
              ) : (
                items.map((item) => (
                  <tr className="product" key={item.productId}>
                    <td className="product-image">
                      <div className="product-thumbnail">
                        <div className="product-thumbnail-wrapper">
                          <img
                            src={item.productImage}
                            alt={item.productName}
                            className="product-thumbnail-image"
                          />
                        </div>
                      </div>
                    </td>
                    <td className="product-description">
                      <span className="product-description-name order-summary-emphasis">
                        {item.productName}
                      </span>
                    </td>
                    <td className="product-quantity">{item.quantity}</td>
                    <td className="product-price">
                      <span className="order-summary-emphasis">
                        {(item.productPrice * item.quantity).toLocaleString('vi-VN')}₫
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* ── Total lines ──────────────────────────────────────────────── */}
        <div className="order-summary-section order-summary-section-total-lines payment-line">
          <table className="total-line-table">
            <tbody>
              <tr className="total-line total-line-subtotal">
                <td className="total-line-name">Tạm tính</td>
                <td className="total-line-price">
                  <span className="order-summary-emphasis">
                    {subtotal.toLocaleString('vi-VN')}₫
                  </span>
                </td>
              </tr>
              <tr className="total-line total-line-shipping">
                <td className="total-line-name">Phí vận chuyển</td>
                <td className="total-line-price">
                  <span className="order-summary-emphasis">
                    {shippingFee == null
                      ? 'Đang tính...'
                      : shippingFee === 0
                        ? 'Miễn phí'
                        : `${shippingFee.toLocaleString('vi-VN')}₫`}
                  </span>
                </td>
              </tr>
            </tbody>
            <tfoot className="total-line-table-footer">
              <tr className="total-line">
                <td className="total-line-name payment-due-label">
                  <span className="payment-due-label-total">Tổng cộng</span>
                </td>
                <td className="total-line-name payment-due">
                  <span className="payment-due-currency">VND</span>
                  <span className="payment-due-price">
                    {total.toLocaleString('vi-VN')}₫
                  </span>
                </td>
              </tr>
            </tfoot>
          </table>
        </div>

      </div>
    </div>
  )
}
