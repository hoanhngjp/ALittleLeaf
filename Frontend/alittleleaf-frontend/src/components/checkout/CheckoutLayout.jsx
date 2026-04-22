import { Link } from 'react-router-dom'
import OrderSummary from './OrderSummary'

const LOGO_URL         = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265291/logo_vx5wwh.webp'
const CHEVRON_ICON_URL = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265311/angle-right-solid_ivecdj.svg'

/**
 * Standalone checkout layout — mirrors the custom Layout = null structure in
 * Index.cshtml / Payment.cshtml. No MainLayout (no global header/footer).
 *
 * Props:
 *   step         : 1 | 2  — controls active breadcrumb item
 *   canGoPayment : bool    — when false the "Phương thức thanh toán" breadcrumb
 *                           is plain text (not a link), blocking skip-ahead navigation
 *   children               — the main-content area
 */
export default function CheckoutLayout({ step = 1, canGoPayment = false, children }) {
  return (
    <div className="checkout-wrap">

      {/* ── Left: main panel ─────────────────────────────────────────── */}
      <div className="checkout-main">
        <div className="main-header">
          <Link to="/" className="logo">
            <img src={LOGO_URL} alt="A Little Leaf" style={{ height: '48px' }} />
          </Link>

          <ul className="breadcrumb">
            {/* Step 0: Cart */}
            <li className="breadcrumb-item">
              <Link to="/cart">Giỏ hàng</Link>
              <img src={CHEVRON_ICON_URL} alt="" className="breadcrumb-chevron" />
            </li>

            {/* Step 1: Shipping info */}
            <li className={`breadcrumb-item${step === 1 ? ' breadcrumb-item-active' : ''}`}>
              {step === 1
                ? <span>Thông tin giao hàng</span>
                : <Link to="/checkout">Thông tin giao hàng</Link>}
              <img src={CHEVRON_ICON_URL} alt="" className="breadcrumb-chevron" />
            </li>

            {/* Step 2: Payment — only a link when canGoPayment is true AND we're not already on it */}
            <li className={`breadcrumb-item${step === 2 ? ' breadcrumb-item-active' : ''}`}>
              {step === 2 || !canGoPayment
                ? <span>Phương thức thanh toán</span>
                : <Link to="/checkout/payment">Phương thức thanh toán</Link>}
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
        <OrderSummary />
      </div>

    </div>
  )
}
