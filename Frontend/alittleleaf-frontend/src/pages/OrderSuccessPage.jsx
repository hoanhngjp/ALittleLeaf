import { useSearchParams, Link } from 'react-router-dom'

/**
 * Shown after a successful COD order is placed.
 * URL: /order-success?billId=123
 */
export default function OrderSuccessPage() {
  const [searchParams] = useSearchParams()
  const billId = searchParams.get('billId')

  return (
    <div className="d-flex align-items-center justify-content-center min-vh-100 bg-light">
      <div className="card shadow-sm text-center p-5" style={{ maxWidth: 520, width: '100%' }}>

        <div className="text-success mb-3" style={{ fontSize: '4rem' }}>
          <i className="fas fa-check-circle" />
        </div>

        <h3 className="fw-bold mb-2">Đặt hàng thành công!</h3>

        <p className="text-muted mb-4">
          Cảm ơn bạn đã mua sắm tại ALittleLeaf. Đơn hàng của bạn đang được xử lý.
          {billId && (
            <> Mã đơn hàng của bạn là <strong>#{billId}</strong>.</>
          )}
        </p>

        <div className="d-flex gap-2 justify-content-center flex-wrap">
          <Link to="/" className="btn btn-success px-4">
            <i className="fas fa-home me-2" />Tiếp tục mua sắm
          </Link>
          {billId && (
            <Link to={`/profile/orders/${billId}`} className="btn btn-outline-success px-4">
              <i className="fas fa-receipt me-2" />Xem đơn hàng
            </Link>
          )}
        </div>

      </div>
    </div>
  )
}
