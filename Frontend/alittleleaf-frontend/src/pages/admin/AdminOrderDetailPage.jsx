import { useState, useEffect } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'
import { useAdminOrder, useUpdateOrderStatus } from '../../hooks/useAdminOrders'

// ── Constants ────────────────────────────────────────────────────────────────

const FMT_DATE = new Intl.DateTimeFormat('vi-VN', {
  day: '2-digit', month: '2-digit', year: 'numeric',
})
const FMT_CURRENCY = new Intl.NumberFormat('vi-VN')

const PAYMENT_STATUS_OPTIONS = [
  { value: 'pending', label: 'Chưa thanh toán' },
  { value: 'paid',    label: 'Đã thanh toán'   },
]

const SHIPPING_STATUS_OPTIONS = [
  { value: 'not_fulfilled', label: 'Chưa giao hàng' },
  { value: 'fulfilled',     label: 'Đã giao hàng'   },
]

// ── Badges ───────────────────────────────────────────────────────────────────

function ShippingBadge({ status }) {
  const map = {
    fulfilled:     'bg-success',
    not_fulfilled: 'bg-warning text-dark',
  }
  const labels = { fulfilled: 'Đã giao hàng', not_fulfilled: 'Chưa giao hàng' }
  return <span className={`badge ${map[status] ?? 'bg-secondary'}`}>{labels[status] ?? status}</span>
}

function PaymentBadge({ status }) {
  return status === 'paid'
    ? <span className="badge bg-success">Đã thanh toán</span>
    : <span className="badge bg-danger">Chưa thanh toán</span>
}

// ── AdminOrderDetailPage ──────────────────────────────────────────────────────

export default function AdminOrderDetailPage() {
  const { id }    = useParams()
  const navigate  = useNavigate()

  const { data: order, isLoading, isError } = useAdminOrder(Number(id))
  const updateMutation = useUpdateOrderStatus(Number(id))

  const [paymentStatus,  setPaymentStatus]  = useState('')
  const [shippingStatus, setShippingStatus] = useState('')
  const [isConfirmed,    setIsConfirmed]    = useState(false)

  useEffect(() => {
    if (order) {
      setPaymentStatus(order.paymentStatus)
      setShippingStatus(order.shippingStatus)
      setIsConfirmed(order.isConfirmed)
    }
  }, [order])

  function handleUpdate() {
    updateMutation.mutate(
      { paymentStatus, shippingStatus, isConfirmed },
      {
        onSuccess: () => toast.success('Cập nhật trạng thái thành công!'),
        onError:   () => toast.error('Cập nhật thất bại. Vui lòng thử lại.'),
      }
    )
  }

  if (isLoading)
    return (
      <div className="app-content">
        <div className="container-fluid text-center py-5">
          <div className="spinner-border" />
        </div>
      </div>
    )

  if (isError || !order)
    return (
      <div className="app-content">
        <div className="container-fluid">
          <div className="alert alert-danger">
            Không tìm thấy đơn hàng #{id}.{' '}
            <Link to="/admin/orders">Quay lại danh sách</Link>
          </div>
        </div>
      </div>
    )

  const subtotal = order.items?.reduce((s, i) => s + i.totalPrice, 0) ?? 0

  return (
    <div className="app-content-header">
      <div className="container-fluid">
        <div className="row mb-2">
          <div className="col-sm-6">
            <h1 className="m-0">Chi tiết đơn hàng #{order.billId}</h1>
          </div>
          <div className="col-sm-6">
            <ol className="breadcrumb float-sm-end">
              <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
              <li className="breadcrumb-item"><Link to="/admin/orders">Đơn hàng</Link></li>
              <li className="breadcrumb-item active">#{order.billId}</li>
            </ol>
          </div>
        </div>
      </div>

      <div className="app-content">
        <div className="container-fluid">
          <div className="row">

            {/* ── LEFT: Customer info + Status update ── */}
            <div className="col-lg-5 mb-4">

              {/* Customer & shipping info */}
              <div className="card mb-3">
                <div className="card-header bg-primary text-white">
                  <h5 className="mb-0"><i className="bi bi-person me-2" />Thông tin khách hàng</h5>
                </div>
                <div className="card-body">
                  <dl className="row mb-0">
                    <dt className="col-sm-5">Tên khách hàng</dt>
                    <dd className="col-sm-7">{order.customerName}</dd>

                    <dt className="col-sm-5">Email</dt>
                    <dd className="col-sm-7">{order.customerEmail}</dd>

                    <dt className="col-sm-5">Ngày đặt hàng</dt>
                    <dd className="col-sm-7">
                      {FMT_DATE.format(new Date(order.dateCreated))}
                    </dd>

                    <dt className="col-sm-5">Phương thức TT</dt>
                    <dd className="col-sm-7">{order.paymentMethod}</dd>

                    <dt className="col-sm-5">Địa chỉ giao</dt>
                    <dd className="col-sm-7">{order.shippingAddress || '—'}</dd>

                    {order.note && (
                      <>
                        <dt className="col-sm-5">Ghi chú</dt>
                        <dd className="col-sm-7">{order.note}</dd>
                      </>
                    )}
                  </dl>
                </div>
              </div>

              {/* Status update */}
              <div className="card">
                <div className="card-header bg-warning text-dark">
                  <h5 className="mb-0"><i className="bi bi-pencil-square me-2" />Cập nhật trạng thái</h5>
                </div>
                <div className="card-body">
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Trạng thái thanh toán</label>
                    <select
                      className="form-select"
                      value={paymentStatus}
                      onChange={(e) => setPaymentStatus(e.target.value)}
                    >
                      {PAYMENT_STATUS_OPTIONS.map((o) => (
                        <option key={o.value} value={o.value}>{o.label}</option>
                      ))}
                    </select>
                  </div>

                  <div className="mb-3">
                    <label className="form-label fw-semibold">Trạng thái giao hàng</label>
                    <select
                      className="form-select"
                      value={shippingStatus}
                      onChange={(e) => setShippingStatus(e.target.value)}
                    >
                      {SHIPPING_STATUS_OPTIONS.map((o) => (
                        <option key={o.value} value={o.value}>{o.label}</option>
                      ))}
                    </select>
                  </div>

                  <div className="mb-3">
                    <label className="form-label fw-semibold">Tình trạng xác nhận</label>
                    <select
                      className="form-select"
                      value={isConfirmed ? 'true' : 'false'}
                      onChange={(e) => setIsConfirmed(e.target.value === 'true')}
                    >
                      <option value="false">Chưa xác nhận</option>
                      <option value="true">Đã xác nhận</option>
                    </select>
                  </div>

                  <div className="d-flex gap-2">
                    <button
                      className="btn btn-primary"
                      onClick={handleUpdate}
                      disabled={updateMutation.isPending}
                    >
                      {updateMutation.isPending
                        ? <><span className="spinner-border spinner-border-sm me-2" />Đang lưu…</>
                        : <><i className="bi bi-check-circle me-2" />Cập nhật trạng thái</>
                      }
                    </button>
                    <button
                      className="btn btn-outline-secondary"
                      onClick={() => navigate('/admin/orders')}
                    >
                      <i className="bi bi-arrow-left me-1" />Quay lại
                    </button>
                  </div>
                </div>
              </div>
            </div>

            {/* ── RIGHT: Order items ── */}
            <div className="col-lg-7 mb-4">
              <div className="card">
                <div className="card-header">
                  <h5 className="mb-0">
                    <i className="bi bi-bag me-2" />
                    Sản phẩm trong đơn ({order.itemCount} sản phẩm)
                  </h5>
                </div>
                <div className="card-body p-0">
                  <div className="table-responsive">
                    <table className="table table-hover mb-0 align-middle">
                      <thead className="table-light">
                        <tr>
                          <th style={{ width: 60 }}>STT</th>
                          <th style={{ width: 70 }}>Ảnh</th>
                          <th>Tên sản phẩm</th>
                          <th className="text-center">SL</th>
                          <th className="text-end">Đơn giá</th>
                          <th className="text-end">Thành tiền</th>
                        </tr>
                      </thead>
                      <tbody>
                        {(order.items ?? []).map((item, idx) => (
                          <tr key={item.billDetailId}>
                            <td className="text-muted">{idx + 1}</td>
                            <td>
                              {item.productImg
                                ? <img
                                    src={item.productImg}
                                    alt={item.productName}
                                    style={{ width: 48, height: 48, objectFit: 'cover', borderRadius: 4 }}
                                  />
                                : <div
                                    className="bg-light d-flex align-items-center justify-content-center"
                                    style={{ width: 48, height: 48, borderRadius: 4 }}
                                  >
                                    <i className="bi bi-image text-muted" />
                                  </div>
                              }
                            </td>
                            <td>
                              <div className="fw-semibold">{item.productName}</div>
                              <small className="text-muted">ID: {item.productId}</small>
                            </td>
                            <td className="text-center">{item.quantity}</td>
                            <td className="text-end" style={{ whiteSpace: 'nowrap' }}>
                              {FMT_CURRENCY.format(item.unitPrice)}₫
                            </td>
                            <td className="text-end fw-semibold" style={{ whiteSpace: 'nowrap' }}>
                              {FMT_CURRENCY.format(item.totalPrice)}₫
                            </td>
                          </tr>
                        ))}
                      </tbody>
                      <tfoot className="table-light">
                        <tr>
                          <td colSpan={5} className="text-end fw-bold">Tổng cộng:</td>
                          <td className="text-end fw-bold text-primary" style={{ whiteSpace: 'nowrap' }}>
                            {FMT_CURRENCY.format(subtotal)}₫
                          </td>
                        </tr>
                      </tfoot>
                    </table>
                  </div>
                </div>

                {/* Summary */}
                <div className="card-footer">
                  <div className="row justify-content-end">
                    <div className="col-sm-6">
                      <table className="table table-sm mb-0">
                        <tbody>
                          <tr>
                            <td className="text-muted">Trạng thái thanh toán</td>
                            <td className="text-end"><PaymentBadge status={order.paymentStatus} /></td>
                          </tr>
                          <tr>
                            <td className="text-muted">Trạng thái giao hàng</td>
                            <td className="text-end"><ShippingBadge status={order.shippingStatus} /></td>
                          </tr>
                          <tr className="fw-bold">
                            <td>Tổng thanh toán</td>
                            <td className="text-end text-primary" style={{ whiteSpace: 'nowrap' }}>
                              {FMT_CURRENCY.format(order.totalAmount)}₫
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>

          </div>
        </div>
      </div>
    </div>
  )
}
