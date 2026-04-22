import { Link, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../../store/useAuthStore'
import apiClient from '../../lib/apiClient'

export default function AdminHeader({ onToggleSidebar }) {
  const logout   = useAuthStore((s) => s.logout)
  const user     = useAuthStore((s) => s.user)
  const navigate = useNavigate()

  const handleLogout = async () => {
    try { await apiClient.post('/api/auth/logout') } catch (_) {}
    logout()
    navigate('/login')
  }

  return (
    <nav className="app-header navbar navbar-expand bg-body">
      <div className="container-fluid">

        {/* ── Left: sidebar toggle + breadcrumb links ─────────────── */}
        <ul className="navbar-nav">
          <li className="nav-item">
            <button
              type="button"
              className="nav-link"
              onClick={onToggleSidebar}
              style={{ background: 'none', border: 'none', cursor: 'pointer' }}
              aria-label="Toggle sidebar"
            >
              <i className="fas fa-bars" />
            </button>
          </li>
          <li className="nav-item d-none d-sm-inline-block">
            <Link to="/admin" className="nav-link">Trang chủ</Link>
          </li>
          <li className="nav-item d-none d-sm-inline-block">
            <Link to="/" className="nav-link">Về trang khách</Link>
          </li>
        </ul>

        {/* ── Right: user info + logout ────────────────────────────── */}
        <ul className="navbar-nav ms-auto">
          <li className="nav-item">
            <span className="nav-link d-none d-md-inline" style={{ color: '#6c757d', fontSize: 13 }}>
              <i className="fas fa-user-circle me-1" />
              {user?.fullName ?? user?.email}
            </span>
          </li>
          <li className="nav-item">
            <button
              type="button"
              className="nav-link"
              onClick={handleLogout}
              style={{ background: 'none', border: 'none', cursor: 'pointer' }}
            >
              <i className="fas fa-sign-out-alt me-1" />
              <span className="d-none d-sm-inline">Đăng xuất</span>
            </button>
          </li>
        </ul>

      </div>
    </nav>
  )
}
