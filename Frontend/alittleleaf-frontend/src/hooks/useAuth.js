import { useMutation } from '@tanstack/react-query'
import { useNavigate, useLocation } from 'react-router-dom'
import apiClient from '../lib/apiClient'
import { useAuthStore } from '../store/useAuthStore'

// ── Login ─────────────────────────────────────────────────────────────────────

export function useLogin() {
  const login    = useAuthStore((s) => s.login)
  const navigate = useNavigate()
  const location = useLocation()

  return useMutation({
    mutationFn: (credentials) =>
      apiClient.post('/api/auth/login', credentials).then((r) => r.data),

    onSuccess: (data) => {
      login({
        user:         data.user,
        accessToken:  data.accessToken,
        refreshToken: data.refreshToken,
      })
      // Role-based default: admins go to /admin, customers go to /
      const role         = data.user?.role?.toLowerCase()
      const roleDefault  = role === 'admin' ? '/admin' : '/'
      const params       = new URLSearchParams(location.search)
      const returnUrl    = params.get('returnUrl') ?? roleDefault
      navigate(returnUrl, { replace: true })
    },
  })
}

// ── Register ──────────────────────────────────────────────────────────────────

export function useRegister() {
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload) =>
      apiClient.post('/api/auth/register', payload).then((r) => r.data),

    onSuccess: () => {
      navigate('/login', { replace: true })
    },
  })
}

// ── Logout ────────────────────────────────────────────────────────────────────

export function useLogout() {
  const logout   = useAuthStore((s) => s.logout)
  const navigate = useNavigate()

  return () => {
    logout()
    navigate('/login', { replace: true })
  }
}
