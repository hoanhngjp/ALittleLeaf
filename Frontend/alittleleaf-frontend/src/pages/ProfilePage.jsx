import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuthStore } from '../store/useAuthStore'
import { useOrders, useRetryPayment } from '../hooks/useOrders'
import AccountSidebar from '../components/account/AccountSidebar'
import ShippingBadge from '../components/common/ShippingBadge'

const FORMAT_VND = (n) =>
  new Intl.NumberFormat('vi-VN').format(n) + '₫'

const FORMAT_DATE = (dateStr) => {
  const d = new Date(dateStr)
  return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`
}

function PaymentStatusBadge({ status, method }) {
  if (status === 'paid')
    return <span className="status-paid">Đã thanh toán</span>
  if (status === 'pending_vnpay')
    return <span className="status-pending_vnpay">Chờ thanh toán VNPAY</span>
  if (status === 'pending' || method === 'cod' || method === 'online')
    return <span className="status-cod">Chờ thanh toán (COD)</span>
  return <span className="status-pending">{status}</span>
}


const PAGE_SIZE = 5

export default function ProfilePage() {
  const user = useAuthStore((s) => s.user)
  const { data: orders = [], isLoading } = useOrders()
  const [page, setPage] = useState(1)
  const retryPayment = useRetryPayment()

  const totalPages = Math.max(1, Math.ceil(orders.length / PAGE_SIZE))
  const safePage   = Math.min(page, totalPages)
  const slice      = orders.slice((safePage - 1) * PAGE_SIZE, safePage * PAGE_SIZE)

  return (
    <div className="layout-account">
      <div className="account-header">
        <h1>Tài khoản của bạn</h1>
      </div>
      <div className="account-content-wrap">
        <div className="row-account">

          <AccountSidebar />

          <div className="customer-sidebar-wrap">
            <div className="row-inside">

              {/* ── Profile info card ─────────────────────────────────── */}
              <div className="customer-sidebar">
                <p className="title-detail">Thông tin tài khoản</p>
                <h2 className="account-name">{user?.fullName}</h2>
                <p className="account-email">{user?.email}</p>
                <div className="address">
                  <Link id="view-address" to="/profile/addresses">Xem địa chỉ</Link>
                </div>
              </div>

              {/* ── Orders table ──────────────────────────────────────── */}
              <div className="customer-table-wrap">
                <div className="customer-table-bg">
                  {isLoading ? (
                    <p>Đang tải đơn hàng...</p>
                  ) : orders.length === 0 ? (
                    <p>Bạn chưa đặt mua sản phẩm</p>
                  ) : (
                    <>
                      <p className="title-detail">Danh sách các đơn hàng</p>
                      <div className="table-wrap">
                        <table className="account-table">
                          <thead>
                            <tr>
                              <th className="text-center">Mã đơn hàng</th>
                              <th className="text-center">Ngày đặt</th>
                              <th className="text-center">Thành tiền</th>
                              <th className="text-center">Trạng thái thanh toán</th>
                              <th className="text-center">Vận chuyển</th>
                              <th className="text-center">Hành động</th>
                            </tr>
                          </thead>
                          <tbody>
                            {slice.map((bill) => (
                              <tr key={bill.billId} className="order">
                                <td className="text-center">
                                  <span>#{bill.billId}</span>
                                </td>
                                <td className="text-center">
                                  <span>{FORMAT_DATE(bill.dateCreated)}</span>
                                </td>
                                <td className="text-center">
                                  <span className="total-money">{FORMAT_VND(bill.totalAmount)}</span>
                                </td>
                                <td className="text-center">
                                  <PaymentStatusBadge status={bill.paymentStatus} method={bill.paymentMethod} />
                                </td>
                                <td className="text-center">
                                  <ShippingBadge status={bill.shippingStatus} />
                                </td>
                                <td className="text-center">
                                  {bill.paymentStatus === 'pending_vnpay' && (
                                    <button
                                      className="btn-action btn-retry-payment"
                                      disabled={retryPayment.isPending}
                                      onClick={() =>
                                        retryPayment.mutate(bill.billId, {
                                          onSuccess: (data) => { window.location.href = data.paymentUrl },
                                          onError:   () => alert('Không thể tạo lại liên kết thanh toán. Vui lòng thử lại.'),
                                        })
                                      }
                                    >
                                      {retryPayment.isPending ? '...' : 'Thanh toán lại'}
                                    </button>
                                  )}
                                  <Link to={`/profile/orders/${bill.billId}`} className="btn-action btn-details">
                                    Chi tiết
                                  </Link>
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>

                      {/* ── Pagination ──────────────────────────────────── */}
                      {totalPages > 1 && (
                        <div className="order-pagination">
                          <button
                            className="pg-btn"
                            disabled={safePage === 1}
                            onClick={() => setPage((p) => Math.max(1, p - 1))}
                          >
                            &laquo;
                          </button>
                          {Array.from({ length: totalPages }, (_, i) => i + 1).map((n) => (
                            <button
                              key={n}
                              className={`pg-btn${n === safePage ? ' pg-btn-active' : ''}`}
                              onClick={() => setPage(n)}
                            >
                              {n}
                            </button>
                          ))}
                          <button
                            className="pg-btn"
                            disabled={safePage === totalPages}
                            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                          >
                            &raquo;
                          </button>
                          <span className="pg-info">
                            Trang {safePage} / {totalPages}
                          </span>
                        </div>
                      )}
                    </>
                  )}
                </div>
              </div>

            </div>
          </div>

        </div>
      </div>
    </div>
  )
}
