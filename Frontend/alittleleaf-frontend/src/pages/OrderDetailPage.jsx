import { Link, useParams } from 'react-router-dom'
import { useOrderDetail } from '../hooks/useOrders'
import AccountSidebar from '../components/account/AccountSidebar'
import { SHIPPING_STATUS_LABEL, ORDER_STATUS_LABEL, ORDER_STATUS_COLOR } from '../constants/orderConstants'

const FORMAT_VND = (n) =>
  new Intl.NumberFormat('vi-VN').format(n) + '₫'

const FORMAT_DATE = (dateStr) => {
  if (!dateStr) return '—'
  const d = new Date(dateStr)
  return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`
}

function PaymentMethodLabel({ method }) {
  if (method === 'cod')   return 'Thanh toán khi nhận hàng (COD)'
  if (method === 'vnpay') return 'VNPAY'
  return method ?? '—'
}

function PaymentStatusBadge({ status }) {
  if (status === 'paid')          return <span className="status-paid">Đã thanh toán</span>
  if (status === 'pending_vnpay') return <span className="status-pending_vnpay">Chờ VNPAY</span>
  return <span className="status-cod">Chờ thanh toán</span>
}

export default function OrderDetailPage() {
  const { id } = useParams()
  const { data: order, isLoading, isError } = useOrderDetail(id)

  return (
    <div className="od-layout">
      <div className="od-container">

        {/* ── Breadcrumb ──────────────────────────────────────────── */}
        <div className="od-nav-wrap">
          <div className="od-nav">
            <ol>
              <li><Link to="/">Trang chủ</Link></li>
              <li><Link to="/profile">Tài khoản</Link></li>
              <li className="od-nav-active">Chi tiết đơn hàng</li>
            </ol>
          </div>
        </div>

        <div className="od-page">
          <div className="od-heading">
            <h1>Chi tiết đơn hàng</h1>
          </div>

          {isLoading && <p className="od-loading">Đang tải...</p>}
          {isError   && <p className="od-error">Không thể tải đơn hàng. Vui lòng thử lại.</p>}

          {order && (
            <div className="od-content-wrap">

              {/* ── Order meta ──────────────────────────────────────── */}
              <div className="od-meta">
                <div className="od-meta-item">
                  <span className="od-meta-label">Mã đơn hàng:</span>
                  <span className="od-meta-value">#{order.billId}</span>
                </div>
                <div className="od-meta-item">
                  <span className="od-meta-label">Ngày đặt:</span>
                  <span className="od-meta-value">{FORMAT_DATE(order.dateCreated)}</span>
                </div>
                <div className="od-meta-item">
                  <span className="od-meta-label">Trạng thái:</span>
                  <PaymentStatusBadge status={order.paymentStatus} />
                </div>
                <div className="od-meta-item">
                  <span className="od-meta-label">Phương thức:</span>
                  <span className="od-meta-value"><PaymentMethodLabel method={order.paymentMethod} /></span>
                </div>
                <div className="od-meta-item">
                  <span className="od-meta-label">Trạng thái đơn:</span>
                  <span
                    className={`badge ${ORDER_STATUS_COLOR[order.orderStatus] ?? 'bg-secondary'}`}
                    style={{ fontSize: '0.85rem' }}
                  >
                    {ORDER_STATUS_LABEL[order.orderStatus] ?? order.orderStatus}
                  </span>
                </div>
                <div className="od-meta-item">
                  <span className="od-meta-label">Vận chuyển:</span>
                  <span className="od-meta-value">
                    {SHIPPING_STATUS_LABEL[order.shippingStatus] ?? order.shippingStatus}
                    {order.trackingMessage && (
                      <span className="text-muted fst-italic ms-2">— {order.trackingMessage}</span>
                    )}
                  </span>
                </div>
              </div>

              {/* ── Shipping address ────────────────────────────────── */}
              {order.shippingAddress && (
                <div className="od-address-block">
                  <h3 className="od-section-title">Địa chỉ giao hàng</h3>
                  <p><strong>{order.shippingAddress.adrsFullname}</strong></p>
                  <p>
                    {[
                      order.shippingAddress.adrsAddress,
                      order.shippingAddress.wardName,
                      order.shippingAddress.districtName,
                      order.shippingAddress.provinceName,
                    ].filter(Boolean).join(', ')}
                  </p>
                  <p>Điện thoại: {order.shippingAddress.adrsPhone}</p>
                </div>
              )}

              {/* ── Note ────────────────────────────────────────────── */}
              {order.note && (
                <div className="od-note-block">
                  <span className="od-meta-label">Ghi chú:</span> {order.note}
                </div>
              )}

              {/* ── Product list ────────────────────────────────────── */}
              <div className="od-display-items">
                <table className="od-table-cart">
                  <tbody>
                    {(order.items ?? []).map((item) => (
                      <tr key={item.billDetailId} className="od-line-item">
                        <td className="od-img-cell">
                          <div className="od-product-img">
                            <img
                              src={item.productImg ?? '/placeholder.png'}
                              alt={item.productName}
                            />
                          </div>
                        </td>
                        <td className="od-item-cell">
                          <div className="od-item-inner">
                            <a href={`/products/${item.productId}`} className="od-product-name">
                              {item.productName}
                            </a>
                            <p className="od-unit-price">{FORMAT_VND(item.unitPrice)}</p>
                            <div className="od-qty-wrap">
                              <input
                                type="text"
                                className="od-qty-input"
                                value={item.quantity}
                                disabled
                                readOnly
                              />
                            </div>
                            <p className="od-line-total">
                              <span className="od-line-total-label">Thành tiền:&nbsp;</span>
                              <span className="od-line-total-value">{FORMAT_VND(item.totalPrice)}</span>
                            </p>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* ── Grand total ─────────────────────────────────────── */}
              {order.ghnOrderCode && (
                <div className="od-note-block">
                  <span className="od-meta-label">Mã vận đơn GHN:</span>{' '}
                  <a
                    href={`https://donhang.ghn.vn/?order_code=${order.ghnOrderCode}`}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    {order.ghnOrderCode}
                  </a>
                </div>
              )}

              <div className="od-cart-others">
                <div className="od-order-actions">
                  {order.shippingFee > 0 && (
                    <p className="od-order-infor">
                      Phí vận chuyển
                      <span className="od-total-price">{FORMAT_VND(order.shippingFee)}</span>
                    </p>
                  )}
                  <p className="od-order-infor">
                    Tổng tiền
                    <span className="od-total-price">
                      <b>{FORMAT_VND(order.totalAmount)}</b>
                    </span>
                  </p>
                </div>
              </div>

              {/* ── Back button ─────────────────────────────────────── */}
              <div className="od-back-wrap">
                <Link to="/profile" className="od-back-btn">
                  ← Quay lại danh sách đơn hàng
                </Link>
              </div>

            </div>
          )}
        </div>
      </div>
    </div>
  )
}
