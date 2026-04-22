import { useState, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useQuery }          from '@tanstack/react-query'
import { useSidebarStore }   from '../store/useSidebarStore'
import { useCartStore }      from '../store/useCartStore'
import { useAuthStore }      from '../store/useAuthStore'
import { useCartQuery, useRemoveCartItem } from '../hooks/useCart'
import { useCategories }     from '../hooks/useCategories'
import apiClient             from '../lib/apiClient'

// ── Search panel ──────────────────────────────────────────────────────────────
function SearchPanel() {
  const [keyword, setKeyword]       = useState('')
  const [debouncedKeyword, setDebouncedKeyword] = useState('')
  const navigate                    = useNavigate()
  const close                       = useSidebarStore((s) => s.close)

  // Debounce: wait 350ms after the user stops typing before updating the query key
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedKeyword(keyword.trim()), 350)
    return () => clearTimeout(timer)
  }, [keyword])

  // Live search — fire when debounced keyword has ≥1 char
  // Return the full paginated response so we can read totalItems for the "View more" count
  const { data: searchData } = useQuery({
    queryKey: ['sidebar-search', debouncedKeyword],
    queryFn:  () =>
      apiClient.get('/api/products', { params: { keyword: debouncedKeyword, pageSize: 5 } }).then((r) => r.data),
    enabled:  debouncedKeyword.length >= 1,
    staleTime: 30_000,
  })

  const firstFive      = searchData?.items      ?? []
  const totalItems     = searchData?.totalItems ?? 0
  const remainingCount = totalItems - firstFive.length

  const handleSubmit = (e) => {
    e.preventDefault()
    if (keyword.trim()) {
      close()
      navigate(`/search?q=${encodeURIComponent(keyword.trim())}`)
    }
  }

  return (
    <div id="site-search" className="site-nav-container" style={{ display: 'block' }}>
      <div className="site-nav-container-last">
        <p className="title">Tìm kiếm</p>

        <div className="search-box-wrap">
          <form onSubmit={handleSubmit} className="searchForm">
            <div className="search-inner">
              <input
                type="search"
                className="searchInput"
                placeholder="Tìm kiếm sản phẩm"
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
                autoFocus
              />
            </div>
            <button type="submit" className="btn-search" aria-label="Tìm kiếm">
              <i className="fa-solid fa-magnifying-glass" style={{ fontSize: '16px', opacity: 0.5 }} />
            </button>
          </form>

          {debouncedKeyword.length >= 1 && (
            <div className="result-wrap">
              <div className="result-content">
                {firstFive.length === 0 ? (
                  <p style={{ padding: '10px 0', fontSize: '14px' }}>Không tìm thấy sản phẩm nào.</p>
                ) : (
                  <>
                    {firstFive.map((p) => (
                      <div key={p.productId} className="item-ult">
                        <div className="thumbs">
                          <Link to={`/products/${p.productId}`} onClick={close}>
                            <img src={p.primaryImage} alt={p.productName} />
                          </Link>
                        </div>
                        <div className="title">
                          <Link to={`/products/${p.productId}`} onClick={close}>
                            {p.productName}
                          </Link>
                          <p className="f-initial">{Number(p.productPrice).toLocaleString('vi-VN')}₫</p>
                        </div>
                      </div>
                    ))}
                    {remainingCount > 0 && (
                      <div className="resultMore">
                        <Link
                          to={`/search?q=${encodeURIComponent(debouncedKeyword)}`}
                          onClick={close}
                        >
                          Xem thêm <strong>{remainingCount}</strong> sản phẩm
                        </Link>
                      </div>
                    )}
                  </>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

// ── Cart panel ────────────────────────────────────────────────────────────────
function CartPanel() {
  const close                             = useSidebarStore((s) => s.close)
  const items                             = useCartStore((s) => s.items)
  const { isLoading }                     = useCartQuery()
  const { mutate: removeItem, isPending } = useRemoveCartItem()

  const total = items.reduce((sum, i) => sum + i.productPrice * i.quantity, 0)

  return (
    <div id="site-cart" className="site-nav-container" style={{ display: 'block' }}>
      <div className="site-nav-container-last">
        <p className="title">Giỏ hàng</p>

        <div className="cart-view">
          {isLoading ? (
            <p style={{ fontSize: '14px' }}>Đang tải...</p>
          ) : (
            <table id="cart-inside">
              <tbody>
                {items.length === 0 ? (
                  <tr id="cart-empty-message">
                    <td colSpan="2">Hiện chưa có sản phẩm nào</td>
                  </tr>
                ) : (
                  items.map((item) => (
                    <tr key={item.productId} className="item-in-cart">
                      <td className="img">
                        <Link to={`/products/${item.productId}`} onClick={close}>
                          <img src={item.productImage} alt={item.productName} />
                        </Link>
                      </td>
                      <td>
                        <Link
                          to={`/products/${item.productId}`}
                          onClick={close}
                          className="pro-title-cart"
                        >
                          {item.productName}
                        </Link>
                        <span className="variant" />
                        <span className="pro-quantity-cart">{item.quantity}</span>
                        <span className="pro-price-cart">
                          {(item.productPrice * item.quantity).toLocaleString('vi-VN')}₫
                        </span>
                        <span className="remove-in-cart">
                          <button
                            onClick={() => removeItem(item.productId)}
                            disabled={isPending}
                            aria-label="Xóa"
                            style={{
                              background: 'none',
                              border: 'none',
                              cursor: 'pointer',
                              padding: 0,
                              color: '#001812',
                            }}
                          >
                            <i className="fa-solid fa-xmark" style={{ fontSize: '16px' }} />
                          </button>
                        </span>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          )}

          <span className="line" />

          <table className="table-total">
            <tbody>
              <tr>
                <td className="text-left" style={{ fontSize: '14px' }}>TỔNG TIỀN: </td>
                <td className="text-right cart-total-display" style={{ fontSize: '14px' }}>
                  {total.toLocaleString('vi-VN')}₫
                </td>
              </tr>
              <tr>
                <td colSpan="2" style={{ paddingTop: '10px', paddingBottom: '25px' }}>
                  <div className="d-flex gap-2">
                    <Link to="/cart" onClick={close} style={{ flex: 1 }}>
                      <button style={{ width: '100%', marginTop: 0, marginBottom: 0 }}>XEM GIỎ HÀNG</button>
                    </Link>
                    <Link to="/checkout" onClick={close} style={{ flex: 1 }}>
                      <button style={{ width: '100%', marginTop: 0, marginBottom: 0 }}>THANH TOÁN</button>
                    </Link>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}

// ── Menu panel (mobile nav) ───────────────────────────────────────────────────
function MenuPanel() {
  const close                   = useSidebarStore((s) => s.close)
  const user                    = useAuthStore((s) => s.user)
  const { data: categories = [] } = useCategories()
  const [openCatId, setOpenCatId] = useState(null)
  const [openFooter, setOpenFooter] = useState(null)

  const toggleCat    = (id) => setOpenCatId((prev) => (prev === id ? null : id))
  const toggleFooter = (id) => setOpenFooter((prev) => (prev === id ? null : id))

  return (
    <div className="site-nav-container" style={{ display: 'block' }}>
      <div className="site-nav-container-last">
        <p className="title">Menu</p>

        <nav className="mobile-nav">
          {/* Trang chủ */}
          <div className="mobile-nav-item">
            <Link to="/" className="mobile-nav-link" onClick={close}>
              Trang chủ
            </Link>
          </div>

          {/* Sản phẩm — accordion for categories */}
          <div className="mobile-nav-item">
            <button
              className="mobile-nav-link mobile-nav-toggle"
              onClick={() => toggleCat('products')}
              aria-expanded={openCatId === 'products'}
            >
              Sản phẩm
              <i
                className={`fa-solid fa-chevron-${openCatId === 'products' ? 'up' : 'down'}`}
                style={{ fontSize: '11px', marginLeft: '8px' }}
              />
            </button>

            {openCatId === 'products' && (
              <ul className="mobile-nav-sub">
                <li>
                  <Link to="/collections/all" className="mobile-nav-sub-link" onClick={close}>
                    Tất cả sản phẩm
                  </Link>
                </li>
                {categories.map((cat) => (
                  <li key={cat.categoryId}>
                    {/* Category row — tap name to navigate, tap chevron to expand subcats */}
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <Link
                        to={`/collections/${cat.categoryId}`}
                        className="mobile-nav-sub-link"
                        style={{ flex: 1 }}
                        onClick={close}
                      >
                        {cat.categoryName}
                      </Link>
                      {cat.subCategories?.length > 0 && (
                        <button
                          className="mobile-nav-sub-toggle"
                          onClick={() => toggleCat(cat.categoryId)}
                          aria-label="Mở rộng"
                        >
                          <i
                            className={`fa-solid fa-chevron-${openCatId === cat.categoryId ? 'up' : 'down'}`}
                            style={{ fontSize: '10px' }}
                          />
                        </button>
                      )}
                    </div>

                    {openCatId === cat.categoryId && cat.subCategories?.length > 0 && (
                      <ul className="mobile-nav-sub mobile-nav-sub--nested">
                        {cat.subCategories.map((sub) => (
                          <li key={sub.categoryId}>
                            <Link
                              to={`/collections/${sub.categoryId}`}
                              className="mobile-nav-sub-link"
                              onClick={close}
                            >
                              {sub.categoryName}
                            </Link>
                          </li>
                        ))}
                      </ul>
                    )}
                  </li>
                ))}
              </ul>
            )}
          </div>

          {/* Về A Little Leaf */}
          <div className="mobile-nav-item">
            <Link to="/about" className="mobile-nav-link" onClick={close}>
              Về A Little Leaf
            </Link>
          </div>

          {/* Divider */}
          <div className="mobile-nav-divider" />

          {/* Account link */}
          <div className="mobile-nav-item">
            <Link
              to={user ? '/profile' : '/login'}
              className="mobile-nav-link"
              onClick={close}
            >
              <i className="fa-regular fa-circle-user" style={{ marginRight: '10px' }} />
              {user ? 'Tài khoản' : 'Đăng nhập'}
            </Link>
          </div>

          {/* ── Footer sections (mobile only) ─────────────────────────── */}
          <div className="mobile-nav-divider" />

          {/* Liên kết — collapsible */}
          <div className="mobile-nav-item">
            <button
              className="mobile-nav-link mobile-nav-toggle"
              onClick={() => toggleFooter('links')}
              aria-expanded={openFooter === 'links'}
            >
              Liên kết
              <i
                className={`fa-solid fa-chevron-${openFooter === 'links' ? 'up' : 'down'}`}
                style={{ fontSize: '11px', marginLeft: '8px' }}
              />
            </button>
            {openFooter === 'links' && (
              <ul className="mobile-nav-sub">
                <li><Link to="/search" className="mobile-nav-sub-link" onClick={close}>Tìm kiếm</Link></li>
                <li><a href="#" className="mobile-nav-sub-link">Giới thiệu</a></li>
                <li><a href="#" className="mobile-nav-sub-link">Chính sách đổi trả</a></li>
              </ul>
            )}
          </div>

          {/* Showroom — collapsible */}
          <div className="mobile-nav-item">
            <button
              className="mobile-nav-link mobile-nav-toggle"
              onClick={() => toggleFooter('showroom')}
              aria-expanded={openFooter === 'showroom'}
            >
              Showroom
              <i
                className={`fa-solid fa-chevron-${openFooter === 'showroom' ? 'up' : 'down'}`}
                style={{ fontSize: '11px', marginLeft: '8px' }}
              />
            </button>
            {openFooter === 'showroom' && (
              <ul className="mobile-nav-sub mobile-footer-info">
                <li>
                  <i className="fa-solid fa-location-dot" />
                  212/A51 Nguyễn Trãi, P. Nguyễn Cư Trinh, Quận 1
                </li>
                <li>
                  <i className="fa-solid fa-phone" />
                  098.873.55.00
                </li>
                <li>
                  <i className="fa-solid fa-envelope" />
                  alittleleaf.homedecor@gmail.com
                </li>
              </ul>
            )}
          </div>

          {/* Mạng xã hội — always visible, icon row */}
          <div className="mobile-nav-item">
            <p className="mobile-nav-link" style={{ pointerEvents: 'none', marginBottom: '8px' }}>
              Mạng xã hội
            </p>
            <div className="mobile-social-row">
              <div className="wrapSocial"><i className="fa-brands fa-facebook" /></div>
              <div className="wrapSocial"><i className="fa-brands fa-instagram" /></div>
              <div className="wrapSocial"><i className="fa-brands fa-tiktok" /></div>
              <div className="wrapSocial"><i className="fa-brands fa-youtube" /></div>
            </div>
          </div>
        </nav>
      </div>
    </div>
  )
}

// ── Sidebar shell (matches #site-nav.site-nav from _SideNav.cshtml) ───────────
export default function Sidebar() {
  const { isOpen, mode, close } = useSidebarStore()

  return (
    <>
      {/* Overlay — click outside to close */}
      {isOpen && (
        <div
          onClick={close}
          style={{
            position: 'fixed',
            inset: 0,
            background: 'rgba(0,0,0,0.35)',
            zIndex: 8887,
          }}
        />
      )}

      <div id="site-nav" className={`site-nav${isOpen ? ' active' : ''}`}>
        {mode === 'cart'   && <CartPanel   />}
        {mode === 'search' && <SearchPanel />}
        {mode === 'menu'   && <MenuPanel   />}

        <button
          id="site-close-handle"
          className="site-close-handle"
          onClick={close}
          aria-label="Đóng"
        >
          <span className="close">
            <i className="fa-solid fa-xmark" style={{ fontSize: '28px' }} />
          </span>
        </button>
      </div>
    </>
  )
}
