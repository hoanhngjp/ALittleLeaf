import { useState, useEffect } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'
import { useAdminOrder, useUpdateOrderStatus, useConfirmOrder, useSyncOrderToGhn } from '../../hooks/useAdminOrders'
import ShippingBadge     from '../../components/common/ShippingBadge'
import PaymentBadge      from '../../components/common/PaymentBadge'
import OrderStatusBadge  from '../../components/common/OrderStatusBadge'
import {
  ORDER_STATUS_OPTIONS,
  PAYMENT_STATUS_OPTIONS,
} from '../../constants/orderConstants'

// ── Constants ────────────────────────────────────────────────────────────────

const FMT_DATE = new Intl.DateTimeFormat('vi-VN', {
  day: '2-digit', month: '2-digit', year: 'numeric',
})
const FMT_CURRENCY = new Intl.NumberFormat('vi-VN')

// ── AdminOrderDetailPage ──────────────────────────────────────────────────────

export default function AdminOrderDetailPage() {
  const { id }    = useParams()
  const navigate  = useNavigate()

  const { data: order, isLoading, isError } = useAdminOrder(Number(id))
  const updateMutation  = useUpdateOrderStatus(Number(id))
  const confirmMutation = useConfirmOrder(Number(id))
  const syncGhnMutation = useSyncOrderToGhn(Number(id))

  const [paymentStatus, setPaymentStatus] = useState('')
  const [orderStatus,   setOrderStatus]   = useState('')

  useEffect(() => {
    if (order) {
      setPaymentStatus(order.paymentStatus)
      setOrderStatus(order.orderStatus ?? 'PENDING')
    }
  }, [order])

  function handleUpdate() {
    updateMutation.mutate(
      { orderStatus, paymentStatus },
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

                    <dt className="col-sm-5">Trạng thái đơn</dt>
                    <dd className="col-sm-7">
                      <OrderStatusBadge status={order.orderStatus} />
                    </dd>

                    <dt className="col-sm-5">Phương thức TT</dt>
                    <dd className="col-sm-7">{order.paymentMethod}</dd>

                    <dt className="col-sm-5">Người nhận</dt>
                    <dd className="col-sm-7">{order.recipientName || '—'}</dd>

                    <dt className="col-sm-5">Điện thoại</dt>
                    <dd className="col-sm-7">{order.recipientPhone || '—'}</dd>

                    <dt className="col-sm-5">Địa chỉ</dt>
                    <dd className="col-sm-7">
                      {[
                        order.streetAddress,
                        order.wardName,
                        order.districtName,
                        order.provinceName,
                      ].filter(Boolean).join(', ') || '—'}
                    </dd>

                    {order.note && (
                      <>
                        <dt className="col-sm-5">Ghi chú</dt>
                        <dd className="col-sm-7">{order.note}</dd>
                      </>
                    )}

                    <dt className="col-sm-5">Mã vận đơn GHN</dt>
                    <dd className="col-sm-7">
                      {order.ghnOrderCode ? (
                        <a
                          href={`https://donhang.ghn.vn/?order_code=${order.ghnOrderCode}`}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="fw-semibold"
                        >
                          {order.ghnOrderCode}
                        </a>
                      ) : (
                        <span className="text-muted">Chưa có</span>
                      )}
                    </dd>

                    {order.trackingMessage && (
                      <>
                        <dt className="col-sm-5">Cập nhật vận chuyển</dt>
                        <dd className="col-sm-7 text-muted fst-italic">{order.trackingMessage}</dd>
                      </>
                    )}
                  </dl>

                  {/* Action buttons */}
                  <div className="d-flex flex-wrap gap-2 mt-3">
                    {/* Confirm button — only when PENDING */}
                    {order.orderStatus === 'PENDING' && (
                      <button
                        className="btn btn-success btn-sm"
                        onClick={() =>
                          confirmMutation.mutate(undefined, {
                            onSuccess: () => toast.success('Đã xác nhận đơn hàng!'),
                            onError:   (e) => toast.error(e?.response?.data?.error ?? 'Xác nhận thất bại.'),
                          })
                        }
                        disabled={confirmMutation.isPending}
                      >
                        {confirmMutation.isPending
                          ? <><span className="spinner-border spinner-border-sm me-2" />Đang xử lý…</>
                          : <><i className="bi bi-check-circle me-2" />Xác nhận đơn hàng</>
                        }
                      </button>
                    )}

                    {/* Push to GHN — only when CONFIRMED and not yet synced */}
                    {order.orderStatus === 'CONFIRMED' && !order.ghnOrderCode && (
                      <button
                        className="btn btn-outline-primary btn-sm"
                        onClick={() =>
                          syncGhnMutation.mutate(undefined, {
                            onSuccess: () => toast.success('Đã đẩy đơn sang GHN thành công!'),
                            onError:   (e) => toast.error(e?.response?.data?.error ?? 'Đẩy đơn sang GHN thất bại.'),
                          })
                        }
                        disabled={syncGhnMutation.isPending}
                      >
                        {syncGhnMutation.isPending
                          ? <><span className="spinner-border spinner-border-sm me-2" />Đang xử lý…</>
                          : <><i className="bi bi-truck me-2" />Đẩy đơn sang GHN</>
                        }
                      </button>
                    )}
                  </div>
                </div>
              </div>

              {/* Status update */}
              <div className="card">
                <div className="card-header bg-warning text-dark">
                  <h5 className="mb-0"><i className="bi bi-pencil-square me-2" />Cập nhật trạng thái</h5>
                </div>
                <div className="card-body">
                  {/* OrderStatus — manually overridable by admin in edge cases */}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Trạng thái đơn hàng</label>
                    <select
                      className="form-select"
                      value={orderStatus}
                      onChange={(e) => setOrderStatus(e.target.value)}
                    >
                      {ORDER_STATUS_OPTIONS.map((o) => (
                        <option key={o.value} value={o.value}>{o.label}</option>
                      ))}
                    </select>
                  </div>

                  {/* PaymentStatus — admin can manually mark COD as paid, etc. */}
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

                  {/* ShippingStatus — read-only, controlled exclusively by GHN webhooks */}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">
                      Trạng thái vận chuyển GHN
                      <span
                        className="ms-2 badge bg-secondary"
                        style={{ fontSize: '0.7rem', verticalAlign: 'middle' }}
                      >
                        Chỉ đọc
                      </span>
                    </label>
                    <div className="form-control d-flex align-items-center gap-2" style={{ background: '#f8f9fa', cursor: 'not-allowed' }}>
                      <ShippingBadge status={order.shippingStatus} />
                      {order.trackingMessage && (
                        <small className="text-muted fst-italic">{order.trackingMessage}</small>
                      )}
                    </div>
                    <div className="form-text">
                      <i className="bi bi-info-circle me-1" />
                      Trạng thái này được cập nhật tự động bởi GHN Webhook.
                    </div>
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
                            <td className="text-muted">Trạng thái đơn</td>
                            <td className="text-end"><OrderStatusBadge status={order.orderStatus} /></td>
                          </tr>
                          <tr>
                            <td className="text-muted">Vận chuyển GHN</td>
                            <td className="text-end">
                              <ShippingBadge status={order.shippingStatus} />
                            </td>
                          </tr>
                          <tr>
                            <td className="text-muted">Phí vận chuyển</td>
                            <td className="text-end" style={{ whiteSpace: 'nowrap' }}>
                              {order.shippingFee > 0
                                ? `${FMT_CURRENCY.format(order.shippingFee)}₫`
                                : 'Miễn phí'}
                            </td>
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
