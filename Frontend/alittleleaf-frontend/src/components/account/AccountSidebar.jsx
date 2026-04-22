import { Link, useLocation } from 'react-router-dom'
import { useAuthStore } from '../../store/useAuthStore'
import apiClient from '../../lib/apiClient'
import { useNavigate } from 'react-router-dom'

export default function AccountSidebar() {
  const { pathname } = useLocation()
  const logout       = useAuthStore((s) => s.logout)
  const navigate     = useNavigate()

  const handleLogout = async () => {
    try {
      const refreshToken = useAuthStore.getState().refreshToken
      if (refreshToken) {
        await apiClient.post('/api/auth/logout', { refreshToken })
      }
    } catch { /* swallow */ }
    logout()
    navigate('/login')
  }

  return (
    <div className="sidebar-account">
      <div className="AccountSidebar">
        <h3 className="account-title">Tài khoản</h3>
        <div className="account-content">
          <div className="account-list">
            <ul>
              <li>
                <Link
                  to="/profile"
                  className={pathname === '/profile' ? 'active' : ''}
                >
                  Thông tin tài khoản
                </Link>
              </li>
              <li>
                <Link
                  to="/profile/addresses"
                  className={pathname === '/profile/addresses' ? 'active' : ''}
                >
                  Danh sách địa chỉ
                </Link>
              </li>
              <li>
                <a href="#" onClick={(e) => { e.preventDefault(); handleLogout() }}>
                  Đăng xuất
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  )
}
