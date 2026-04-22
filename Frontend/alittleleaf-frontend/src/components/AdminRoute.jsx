import { Navigate, Outlet } from 'react-router-dom'
import { useAuthStore } from '../store/useAuthStore'

export default function AdminRoute() {
  const user = useAuthStore((s) => s.user)

  if (!user) return <Navigate to="/login" replace />
  if (user.role?.toLowerCase() !== 'admin') return <Navigate to="/" replace />

  return <Outlet />
}
