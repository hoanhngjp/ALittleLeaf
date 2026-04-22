import { useState } from 'react'
import { Link }     from 'react-router-dom'
import { useCartStore }      from '../store/useCartStore'
import { useCartQuery, useRemoveCartItem, useUpdateCartItem } from '../hooks/useCart'

export default function CartPage() {
  const items                             = useCartStore((s) => s.items)
  const { isLoading }                     = useCartQuery()
  const { mutate: removeItem, isPending: isRemoving } = useRemoveCartItem()
  const { mutate: updateItem, isPending: isUpdating } = useUpdateCartItem()

  // Note textarea (stored locally — sent on checkout)
  const [note, setNote] = useState('')

  const total     = items.reduce((sum, i) => sum + i.productPrice * i.quantity, 0)
  const itemCount = items.reduce((sum, i) => sum + i.quantity, 0)
  const isEmpty   = !isLoading && items.length === 0

  const handleQty = (productId, delta, currentQty) => {
    const next = currentQty + delta
    if (next < 1) return
    updateItem({ productId, quantity: next })
  }

  return (
    <div className="layout-cart">
      <div className="container-fluid">

        {/* ── Breadcrumb ─────────────────────────────────────────────── */}
        <div className="nav-header-wrap">
          <div className="nav-header">
            <ol>
              <li><Link to="/"><span>Trang chủ</span></Link></li>
              <li className="active">
                <span className="total-items">
                  Giỏ hàng ({isLoading ? '…' : itemCount})
                </span>
              </li>
            </ol>
          </div>
        </div>

        {/* ── Page heading ───────────────────────────────────────────── */}
        <div className="cart-page">
          <div className="heading-page">
            <div className="header-page">
              <h1>Giỏ hàng của bạn</h1>
              {!isLoading && (
                <p className="count-cart">
                  Có <span>{itemCount} sản phẩm</span> trong giỏ hàng
                </p>
              )}
            </div>
          </div>

          <div className="cart-content-wrap">

            {/* ── Loading ─────────────────────────────────────────────── */}
            {isLoading && (
              <div className="py-5 text-center">
                <div className="spinner-border text-secondary" role="status" />
              </div>
            )}

            {/* ── Empty state ─────────────────────────────────────────── */}
            {isEmpty && (
              <div className="notifications">
                Giỏ hàng của bạn đang trống
                <p className="link-continue">
                  <Link to="/collections">
                    <button>
                      <i className="fa fa-reply" /> TIẾP TỤC MUA HÀNG
                    </button>
                  </Link>
                </p>
              </div>
            )}

            {/* ── Cart content ────────────────────────────────────────── */}
            {!isLoading && !isEmpty && (
              <div className="cart-container">
                <div className="main-content-cart">

                  {/* Table */}
                  <div className="display-items">
                    <table className="table-cart">
                      <tbody>
                        {items.map((item) => (
                          <tr key={item.productId} className="line-item-container">

                            {/* Image */}
                            <td className="image">
                              <div className="product-img">
                                <Link to={`/products/${item.productId}`}>
                                  <img src={item.productImage} alt={item.productName} />
                                </Link>
                              </div>
                            </td>

                            {/* Name + price + qty + line total */}
                            <td className="item">
                              <Link to={`/products/${item.productId}`}>
                                {item.productName}
                              </Link>
                              <p>
                                <span>{item.productPrice.toLocaleString('vi-VN')}₫</span>
                              </p>

                              <div className="qty-click">
                                <button
                                  type="button"
                                  className="qtyminus qty-btn"
                                  onClick={() => handleQty(item.productId, -1, item.quantity)}
                                  disabled={isUpdating || item.quantity <= 1}
                                >
                                  -
                                </button>
                                <input
                                  type="text"
                                  readOnly
                                  className="item-quantity"
                                  value={item.quantity}
                                />
                                <button
                                  type="button"
                                  className="qtyplus qty-btn"
                                  onClick={() => handleQty(item.productId, 1, item.quantity)}
                                  disabled={isUpdating}
                                >
                                  +
                                </button>
                              </div>

                              <p className="price">
                                <span className="text">Thành tiền</span>
                                <span className="line-item-total">
                                  {(item.productPrice * item.quantity).toLocaleString('vi-VN')}₫
                                </span>
                              </p>
                            </td>

                            {/* Remove */}
                            <td className="remove">
                              <button
                                onClick={() => removeItem(item.productId)}
                                disabled={isRemoving}
                                aria-label="Xóa"
                                style={{
                                  background: 'none',
                                  border: 'none',
                                  cursor: 'pointer',
                                  padding: 0,
                                  color: '#001812',
                                }}
                              >
                                <i className="fa-solid fa-xmark" style={{ fontSize: '18px' }} />
                              </button>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>

                  {/* Note + total + buttons */}
                  <div className="cart-others">
                    <div className="note-wrap">
                      <div className="checkout-note">
                        <textarea
                          value={note}
                          onChange={(e) => setNote(e.target.value)}
                          cols="50"
                          rows="8"
                          placeholder="Ghi chú"
                        />
                      </div>
                    </div>

                    <div className="order-actions-wrap">
                      <p className="order-infor">
                        Tổng tiền
                        <span className="total-price">
                          <b className="cart-total-display">
                            {total.toLocaleString('vi-VN')}₫
                          </b>
                        </span>
                      </p>

                      <div className="cart-buttons d-flex gap-3 justify-content-end">
                        <Link to="/collections">
                          <button type="button">
                            <i className="fa fa-reply" /> TIẾP TỤC MUA HÀNG
                          </button>
                        </Link>
                        <Link to="/checkout" state={{ note }}>
                          <button type="button" className="btn-checkout">
                            Thanh toán
                          </button>
                        </Link>
                      </div>
                    </div>
                  </div>

                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
