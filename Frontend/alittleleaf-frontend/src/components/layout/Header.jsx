import { useRef, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuthStore }    from '../../store/useAuthStore'
import { useCartStore }    from '../../store/useCartStore'
import { useSidebarStore } from '../../store/useSidebarStore'
import HeaderCategory from '../HeaderCategory'

const LOGO_URL = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265291/logo_vx5wwh.webp'

// Shared inline style for icon buttons (search, cart, hamburger)
const iconBtnStyle = {
  background: 'none',
  border: 'none',
  padding: 0,
  cursor: 'pointer',
  color: '#001812',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  fontSize: '26px',
}

export default function Header() {
  const user       = useAuthStore((s) => s.user)
  const count      = useCartStore((s) => s.count)
  const openCart   = useSidebarStore((s) => s.openCart)
  const openSearch = useSidebarStore((s) => s.openSearch)
  const openMenu   = useSidebarStore((s) => s.openMenu)

  // Shake animation: trigger on count increase
  const prevCount  = useRef(count)
  const [shaking, setShaking] = useState(false)
  useEffect(() => {
    if (count > prevCount.current) {
      setShaking(true)
      const t = setTimeout(() => setShaking(false), 600)
      return () => clearTimeout(t)
    }
    prevCount.current = count
  }, [count])

  return (
    <header>

      {/* ── Left: Logo ── flex: 1 keeps it left-anchored ───────────────── */}
      <div className="homeLogo">
        <Link to="/"><img src={LOGO_URL} alt="A Little Leaf" /></Link>
      </div>

      {/* ── Center: Nav — flex: 2, centered, hidden below md ───────────── */}
      <div className="menu d-none d-md-flex justify-content-center align-items-center">
        <ul className="mb-0">
          <li><Link to="/">Trang chủ</Link></li>
          <li>
            <Link to="/collections/all">
              Sản phẩm <i className="fa-solid fa-chevron-down" style={{ fontSize: '11px' }} />
            </Link>
            <HeaderCategory />
          </li>
          <li><Link to="/about">Về A Little Leaf</Link></li>
        </ul>
      </div>

      {/* ── Right: Icon actions ─────────────────────────────────────────── */}
      <div className="headerOthers">
        <ul className="others">

          {/* Account — hidden on mobile (hamburger covers navigation) */}
          <li className="d-none d-md-flex">
            <Link to={user ? '/profile' : '/login'} aria-label="Tài khoản">
              <i className="fa-regular fa-circle-user" />
            </Link>
          </li>

          {/* Search — desktop only; mobile uses the inline bar below the header */}
          <li className="d-none d-md-flex">
            <button id="open-site-search" aria-label="Tìm kiếm" onClick={openSearch} style={iconBtnStyle}>
              <i className="fa-solid fa-magnifying-glass" />
            </button>
          </li>

          {/* Cart — opens cart sidebar (with count badge + shake animation) */}
          <li>
            <button
              id="open-site-cart"
              aria-label="Giỏ hàng"
              onClick={openCart}
              style={{ ...iconBtnStyle, position: 'relative' }}
              className={shaking ? 'cart-shake' : ''}
            >
              <i className="fa-solid fa-bag-shopping" />
              {count > 0 && (
                <span className={`count-holder${shaking ? ' count-pop' : ''}`}>
                  {count}
                </span>
              )}
            </button>
          </li>

          {/* Hamburger — opens menu sidebar, visible ONLY below md ───── */}
          <li className="d-md-none">
            <button aria-label="Menu" onClick={openMenu} style={iconBtnStyle}>
              <i className="fa-solid fa-bars" />
            </button>
          </li>

        </ul>
      </div>

    </header>
  )
}
