import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuthStore } from '../store/useAuthStore'

export default function ProtectedRoute({ requiredRole }) {
  const { user, accessToken, _hasHydrated } = useAuthStore()
  const location = useLocation()

  // Wait for Zustand persist to finish reading from localStorage (Zustand v5 is async)
  if (!_hasHydrated) return null

  // Not logged in → send to login with return URL
  if (!accessToken)
    return <Navigate to={`/login?returnUrl=${encodeURIComponent(location.pathname)}`} replace />

  const role    = user?.role?.toLowerCase() ?? ''
  const isAdmin = role === 'admin'

  // Admins are superusers — they can access every protected route, including
  // customer pages (cart, account, addresses, checkout) regardless of requiredRole.
  // Role-gating only blocks non-admin users from role-restricted areas.
  if (requiredRole && !isAdmin && role !== requiredRole.toLowerCase())
    return <Navigate to="/" replace />

  return <Outlet />
}
