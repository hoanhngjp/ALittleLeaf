import { useEffect, useState } from 'react'
import { useSearchParams, Link, useNavigate } from 'react-router-dom'
import apiClient from '../lib/apiClient'

/**
 * VNPay redirects the browser here after the user completes (or cancels) payment.
 * The URL contains all vnp_* query params, e.g.:
 *   /payment-result?vnp_Amount=...&vnp_ResponseCode=00&vnp_SecureHash=...
 *
 * This page forwards those params verbatim to the backend callback endpoint,
 * which validates the HMAC signature and confirms the order idempotently.
 */
export default function PaymentResultPage() {
  const [searchParams]          = useSearchParams()
  const navigate                = useNavigate()
  const [status, setStatus]     = useState('loading') // 'loading' | 'success' | 'error'
  const [billId, setBillId]     = useState(null)
  const [errorMsg, setErrorMsg] = useState('')

  useEffect(() => {
    // Convert URLSearchParams → plain object so Axios serialises them as query string
    const params = Object.fromEntries(searchParams.entries())

    // Quick client-side guard: if VNPay already signalled failure, skip the API call
    const vnpCode = params['vnp_ResponseCode']
    if (vnpCode && vnpCode !== '00') {
      setErrorMsg(vnpCodeToMessage(vnpCode))
      setStatus('error')
      return
    }

    apiClient
      .get('/api/payment/vnpay-callback', { params })
      .then((res) => {
        if (res.data?.success) {
          setBillId(res.data.billId)
          setStatus('success')
        } else {
          setErrorMsg(res.data?.message || 'Xác thực thanh toán thất bại.')
          setStatus('error')
        }
      })
      .catch((err) => {
        const msg =
          err.response?.data?.message ||
          err.response?.data?.error ||
          'Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại đơn hàng của bạn.'
        setErrorMsg(msg)
        setStatus('error')
      })
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  return (
    <div className="d-flex align-items-center justify-content-center min-vh-100 bg-light">
      <div className="card shadow-sm text-center p-5" style={{ maxWidth: 520, width: '100%' }}>

        {/* ── Loading ────────────────────────────────────────────────── */}
        {status === 'loading' && (
          <>
            <div className="spinner-border text-success mb-4" style={{ width: '3rem', height: '3rem' }} role="status">
              <span className="visually-hidden">Đang xử lý...</span>
            </div>
            <h4 className="fw-semibold mb-2">Đang xác thực thanh toán</h4>
            <p className="text-muted mb-0">Vui lòng không đóng trình duyệt...</p>
          </>
        )}

        {/* ── Success ────────────────────────────────────────────────── */}
        {status === 'success' && (
          <>
            <div className="text-success mb-3" style={{ fontSize: '4rem' }}>
              <i className="fas fa-check-circle" />
            </div>
            <h3 className="fw-bold mb-2">Thanh toán thành công!</h3>
            <p className="text-muted mb-4">
              Cảm ơn bạn đã mua hàng tại ALittleLeaf.
              {billId && (
                <> Đơn hàng <strong>#{billId}</strong> của bạn đã được xác nhận.</>
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
          </>
        )}

        {/* ── Error / Cancelled ──────────────────────────────────────── */}
        {status === 'error' && (
          <>
            <div className="text-danger mb-3" style={{ fontSize: '4rem' }}>
              <i className="fas fa-times-circle" />
            </div>
            <h3 className="fw-bold mb-2">Thanh toán bị hủy hoặc thất bại</h3>
            <p className="text-muted mb-2">
              {errorMsg || 'Đã có lỗi xảy ra hoặc bạn đã hủy giao dịch.'}
            </p>
            <p className="text-muted mb-4" style={{ fontSize: '0.9rem' }}>
              Đơn hàng của bạn đã được lưu lại. Bạn có thể vào mục{' '}
              <strong>Quản lý đơn hàng</strong> để thanh toán lại sau.
            </p>
            <div className="d-flex gap-2 justify-content-center flex-wrap">
              <Link to="/" className="btn btn-outline-secondary px-4">
                <i className="fas fa-home me-2" />Về trang chủ
              </Link>
              <Link to="/profile" className="btn btn-danger px-4">
                <i className="fas fa-receipt me-2" />Xem đơn hàng của tôi
              </Link>
            </div>
          </>
        )}

      </div>
    </div>
  )
}

/** Map common VNPay response codes to Vietnamese messages. */
function vnpCodeToMessage(code) {
  const map = {
    '07': 'Giao dịch bị nghi ngờ (trừ tiền thành công). Vui lòng liên hệ hỗ trợ.',
    '09': 'Thẻ/tài khoản chưa đăng ký dịch vụ InternetBanking.',
    '10': 'Xác thực thông tin thẻ/tài khoản không đúng quá 3 lần.',
    '11': 'Phiên thanh toán đã hết hạn. Vui lòng thử lại.',
    '12': 'Thẻ/tài khoản bị khóa.',
    '13': 'Mã OTP không đúng. Vui lòng thử lại.',
    '24': 'Bạn đã hủy giao dịch.',
    '51': 'Tài khoản không đủ số dư.',
    '65': 'Tài khoản vượt quá hạn mức giao dịch trong ngày.',
    '75': 'Ngân hàng thanh toán đang bảo trì.',
    '79': 'Nhập sai mật khẩu thanh toán quá số lần quy định.',
    '99': 'Lỗi không xác định. Vui lòng thử lại hoặc liên hệ hỗ trợ.',
  }
  return map[code] ?? `Thanh toán thất bại (mã lỗi: ${code}).`
}
