import { Link } from 'react-router-dom'
import OrderSummary from './OrderSummary'

const LOGO_URL = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265291/logo_vx5wwh.webp'

export default function CheckoutLayout({ step = 1, children, shippingFee = null }) {
  return (
    <div className="checkout-wrap">

      {/* ── Left: main panel ─────────────────────────────────────────── */}
      <div className="checkout-main">
        <div className="main-header">
          <Link to="/" className="logo">
            <img src={LOGO_URL} alt="A Little Leaf" style={{ height: '48px' }} />
          </Link>

          <ul className="breadcrumb">
            {/* Cart — always a past step, always clickable */}
            <li className="breadcrumb-item">
              <Link to="/cart">Giỏ hàng</Link>
            </li>

            {/* Shipping info — link only when we are on step 2 (past step) */}
            <li className={`breadcrumb-item${step === 1 ? ' breadcrumb-item-active' : ''}`}>
              {step === 2
                ? <Link to="/checkout">Thông tin giao hàng</Link>
                : <span>Thông tin giao hàng</span>}
            </li>

            {/* Payment — never a link; user must go through the form to get here */}
            <li className={`breadcrumb-item${step === 2 ? ' breadcrumb-item-active' : ''}`}>
              <span>Phương thức thanh toán</span>
            </li>
          </ul>
        </div>

        <div className="main-content">
          {children}
        </div>

        <div className="main-footer footer-powered-by">Powered by HoanhNgjp</div>
      </div>

      {/* ── Right: order summary sidebar ─────────────────────────────── */}
      <div className="checkout-sidebar">
        <OrderSummary shippingFee={shippingFee} />
      </div>

    </div>
  )
}
