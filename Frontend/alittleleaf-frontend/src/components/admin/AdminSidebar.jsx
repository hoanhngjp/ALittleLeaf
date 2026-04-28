import { NavLink, Link } from 'react-router-dom'
import { useAuthStore } from '../../store/useAuthStore'

const LOGO_URL = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265291/footerLogo_bpfe7m.webp'

// v4 uses nav-arrow instead of fa-angle-left for treeview indicators
const NAV_ITEMS = [
  {
    id: 'dashboard',
    label: 'Tổng quan',
    icon: 'fas fa-tachometer-alt',
    to: '/admin',
    exact: true,
  },
  {
    id: 'users',
    label: 'Quản lý người dùng',
    icon: 'fas fa-user-alt',
    children: [
      { label: 'Thêm mới',    icon: 'fas fa-plus',   to: '/admin/users/new' },
      { label: 'Danh sách',   icon: 'fas fa-wrench', to: '/admin/users' },
    ],
  },
  {
    id: 'products',
    label: 'Quản lý sản phẩm',
    icon: 'fas fa-box',
    children: [
      { label: 'Thêm sản phẩm',        icon: 'fas fa-plus',   to: '/admin/products/new' },
      { label: 'Sửa - Xóa sản phẩm',   icon: 'fas fa-wrench', to: '/admin/products' },
    ],
  },
  {
    id: 'orders',
    label: 'Quản lý đơn hàng',
    icon: 'fas fa-paper-plane',
    children: [
      { label: 'Xem đơn hàng', icon: 'fas fa-eye', to: '/admin/orders' },
    ],
  },
  {
    id: 'banners',
    label: 'Quản lý Banner',
    icon: 'fas fa-images',
    to: '/admin/banners',
  },
  {
    id: 'ghn-simulator',
    label: 'GHN Webhook Test',
    icon: 'fas fa-truck',
    to: '/admin/ghn-simulator',
  },
]

export default function AdminSidebar({ openMenu, onToggleMenu }) {
  const user = useAuthStore((s) => s.user)

  return (
    <aside className="app-sidebar bg-body-secondary shadow" data-bs-theme="dark">

      {/* ── Brand ──────────────────────────────────────────────────── */}
      <div className="sidebar-brand">
        <Link to="/admin" className="brand-link">
          <img
            src={LOGO_URL}
            alt="A Little Leaf"
            className="brand-image"
            style={{ opacity: 0.85, height: 33, width: 'auto' }}
          />
          <span className="brand-text fw-light ms-2" style={{ fontSize: 14 }}>
            A Little Leaf <small>(admin)</small>
          </span>
        </Link>
      </div>

      {/* ── Scrollable menu ─────────────────────────────────────────── */}
      <div className="sidebar-wrapper">
        <nav className="mt-2">

          {/* User panel */}
          <div className="sidebar-user-panel px-3 pb-3 mb-2" style={{ borderBottom: '1px solid rgba(255,255,255,.1)' }}>
            <span style={{ fontSize: 13, color: 'rgba(255,255,255,.7)' }}>
              <i className="fas fa-user-circle me-1" />
              {user?.fullName ?? user?.email ?? 'Admin'} — Admin
            </span>
          </div>

          <ul
            className="nav sidebar-menu flex-column"
            role="navigation"
            aria-label="Main navigation"
          >
            {NAV_ITEMS.map((item) => {
              // Leaf item (no children)
              if (!item.children) {
                return (
                  <li key={item.id} className="nav-item">
                    <NavLink
                      to={item.to}
                      end={item.exact}
                      className={({ isActive }) =>
                        `nav-link${isActive ? ' active' : ''}`
                      }
                    >
                      <i className={`nav-icon ${item.icon}`} />
                      <p>{item.label}</p>
                    </NavLink>
                  </li>
                )
              }

              const isOpen = openMenu === item.id

              return (
                <li key={item.id} className={`nav-item${isOpen ? ' menu-open' : ''}`}>
                  {/* Parent toggle button */}
                  <button
                    type="button"
                    className="nav-link"
                    onClick={() => onToggleMenu(item.id)}
                    style={{
                      width: '100%',
                      background: 'none',
                      border: 'none',
                      textAlign: 'left',
                      cursor: 'pointer',
                    }}
                    aria-expanded={isOpen}
                  >
                    <i className={`nav-icon ${item.icon}`} />
                    <p>
                      {item.label}
                      {/* v4 uses nav-arrow class for the chevron */}
                      <i className={`nav-arrow fas fa-chevron-${isOpen ? 'down' : 'right'}`} />
                    </p>
                  </button>

                  {/* Sub-menu */}
                  <ul
                    className="nav nav-treeview"
                    style={{ display: isOpen ? 'block' : 'none' }}
                  >
                    {item.children.map((child) => (
                      <li key={child.to} className="nav-item">
                        <NavLink
                          to={child.to}
                          className={({ isActive }) =>
                            `nav-link${isActive ? ' active' : ''}`
                          }
                        >
                          <i className={`nav-icon ${child.icon}`} />
                          <p>{child.label}</p>
                        </NavLink>
                      </li>
                    ))}
                  </ul>
                </li>
              )
            })}
          </ul>
        </nav>
      </div>

    </aside>
  )
}
