import { useState } from 'react'
import { useNavigate, useLocation, Link } from 'react-router-dom'
import { usePlaceOrder, useCreateVnPayUrl } from '../hooks/useOrders'
import CheckoutLayout from '../components/checkout/CheckoutLayout'

// Payment method icons — served from project Cloudinary account
const VNPAY_LOGO = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776453419/VNPAY_id-sVSMjm2_0_no8bvg.png'
const COD_ICON   = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265312/cod_uojdzz.svg'

export default function CheckoutPaymentPage() {
  const navigate  = useNavigate()
  const location  = useLocation()
  const state     = location.state ?? {}

  // Shipping data forwarded from CheckoutPage
  const { note, addressId, newFullName, newPhone, newAddress } = state

  // ── Selected payment method ────────────────────────────────────────────────
  const [method, setMethod] = useState('VNPAY')

  // ── Mutations ──────────────────────────────────────────────────────────────
  const placeOrder     = usePlaceOrder()
  const createVnPayUrl = useCreateVnPayUrl()

  const isLoading = placeOrder.isPending || createVnPayUrl.isPending

  // ── Error feedback ─────────────────────────────────────────────────────────
  const [errorMsg, setErrorMsg] = useState(null)

  // ── Handle "Hoàn tất đơn hàng" ────────────────────────────────────────────
  const handleComplete = () => {
    setErrorMsg(null)

    const orderDto = {
      paymentMethod: method,
      note: note ?? '',
      ...(addressId
        ? { addressId }
        : { newFullName, newPhone, newAddress }),
    }

    placeOrder.mutate(orderDto, {
      onSuccess: async (order) => {
        if (method === 'COD') {
          // COD → redirect to dedicated order success page
          navigate(`/order-success?billId=${order.billId}`)
        } else {
          // VNPAY → generate URL and redirect browser
          createVnPayUrl.mutate(order.billId, {
            onSuccess: (res) => {
              window.location.href = res.paymentUrl
            },
            onError: () => setErrorMsg('Không thể tạo liên kết thanh toán VNPay. Vui lòng thử lại.'),
          })
        }
      },
      onError: (err) => {
        const msg = err?.response?.data?.error ?? 'Đặt hàng thất bại. Vui lòng thử lại.'
        setErrorMsg(msg)
      },
    })
  }

  return (
    <CheckoutLayout step={2}>
      <div className="step">
        <div className="step-actions">

          {/* ── Shipping method section ───────────────────────────────── */}
          <div id="section-shipping-rate" className="section">
            <div className="section-header">
              <h2 className="section-title">Phương thức vận chuyển</h2>
            </div>
            <div className="section-content">
              <div className="content-box">
                <div className="content-box-row">
                  <div className="radio-wrapper">
                    <label>
                      <div className="radio-input">
                        <input className="input-radio" type="radio" defaultChecked readOnly />
                      </div>
                      <span className="radio-label-primary">
                        Để phí vận chuyển được chính xác nhất, tụi mình sẽ liên hệ báo phí vận chuyển sau khi xác nhận đơn hàng ạ
                      </span>
                    </label>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* ── Payment method section ───────────────────────────────── */}
          <div id="section-payment-method" className="section">
            <div className="section-header">
              <h2 className="section-title">Phương thức thanh toán</h2>
            </div>
            <div className="section-content">
              <div className="content-box">

                {/* VNPay */}
                <div
                  className="radio-wrapper content-box-row"
                  onClick={() => setMethod('VNPAY')}
                >
                  <label htmlFor="vnpay_method" className="two-page">
                    <div className="radio-input payment-method-checkbox">
                      <input
                        name="checkoutMethod"
                        type="radio"
                        className="input-radio"
                        value="VNPAY"
                        id="vnpay_method"
                        checked={method === 'VNPAY'}
                        onChange={() => setMethod('VNPAY')}
                      />
                    </div>
                    <div className="radio-content-input">
                      <img src={VNPAY_LOGO} alt="VNPAY" style={{ width: 40, height: 40, marginRight: 10, objectFit: 'contain' }} />
                      <div className="content-wrapper">
                        <span className="radio-label-primary">Thanh toán qua VNPAY</span>
                      </div>
                    </div>
                  </label>
                </div>

                {/* COD */}
                <div
                  className="radio-wrapper content-box-row"
                  onClick={() => setMethod('COD')}
                >
                  <label className="two-page">
                    <div className="radio-input payment-method-checkbox">
                      <input
                        name="checkoutMethod"
                        type="radio"
                        className="input-radio"
                        value="COD"
                        checked={method === 'COD'}
                        onChange={() => setMethod('COD')}
                      />
                    </div>
                    <div className="radio-content-input">
                      <img src={COD_ICON} alt="COD" className="main-img" />
                      <div className="content-wrapper">
                        <span className="radio-label-primary">Thanh toán khi giao hàng (COD)</span>
                      </div>
                    </div>
                  </label>
                </div>

                {/* COD — secondary info row */}
                <div
                  className="radio-wrapper content-box-row content-box-row-secondary"
                  style={{ display: method === 'COD' ? 'block' : 'none' }}
                >
                  <div className="blank-slate">
                    Tụi mình nhận ship cod với đơn hàng dưới 1tr và không có sản phẩm dễ vỡ.
                  </div>
                </div>

              </div>
            </div>
          </div>

        </div>

        {/* ── Error message ──────────────────────────────────────────── */}
        {errorMsg && (
          <div style={{
            color: '#721c24',
            backgroundColor: '#f8d7da',
            padding: '10px 14px',
            borderRadius: '5px',
            marginBottom: '12px',
            fontSize: '14px',
          }}>
            <i className="fa-solid fa-triangle-exclamation" /> {errorMsg}
          </div>
        )}

        {/* ── Footer actions ─────────────────────────────────────────── */}
        <div className="step-footer" id="step-footer-checkout">
          <button
            type="button"
            className="step-footer-continue-btn btn"
            id="btn-complete-order"
            onClick={handleComplete}
            disabled={isLoading}
          >
            <span className="btn-content">
              {isLoading ? 'Đang xử lý...' : 'Hoàn tất đơn hàng'}
            </span>
          </button>
          <Link to="/checkout" className="step-footer-previous-link">
            Thông tin đặt hàng
          </Link>
        </div>

      </div>
    </CheckoutLayout>
  )
}
