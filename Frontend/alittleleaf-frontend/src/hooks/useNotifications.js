import { useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'
import { useAuthStore } from '../store/useAuthStore'
import { useNotificationStore } from '../store/useNotificationStore'
import { getHubConnection, startHubConnection, stopHubConnection } from '../lib/notificationHub'

export function useNotifications() {
  const user = useAuthStore((s) => s.user)
  const setNotifications = useNotificationStore((s) => s.setNotifications)

  return useQuery({
    queryKey: ['notifications'],
    queryFn: () => apiClient.get('/api/notifications').then((r) => r.data),
    enabled: !!user,
    staleTime: 30 * 1000,
    onSuccess: (data) => setNotifications(data),
  })
}

export function useUnreadCount() {
  const user = useAuthStore((s) => s.user)
  const setUnreadCount = useNotificationStore((s) => s.setUnreadCount)

  return useQuery({
    queryKey: ['notifications', 'unread-count'],
    queryFn: () =>
      apiClient.get('/api/notifications/unread-count').then((r) => r.data.count),
    enabled: !!user,
    staleTime: 30 * 1000,
    onSuccess: (count) => setUnreadCount(count),
  })
}

export function useMarkAsRead() {
  const qc = useQueryClient()
  const markOneRead = useNotificationStore((s) => s.markOneRead)

  return useMutation({
    mutationFn: (id) => apiClient.post(`/api/notifications/${id}/read`),
    onSuccess: (_, id) => {
      markOneRead(id)
      qc.invalidateQueries({ queryKey: ['notifications', 'unread-count'] })
    },
  })
}

export function useMarkAllAsRead() {
  const qc = useQueryClient()
  const clearUnread = useNotificationStore((s) => s.clearUnread)

  return useMutation({
    mutationFn: () => apiClient.post('/api/notifications/read-all'),
    onSuccess: () => {
      clearUnread()
      qc.invalidateQueries({ queryKey: ['notifications', 'unread-count'] })
    },
  })
}

export function useNotificationHub() {
  const user = useAuthStore((s) => s.user)
  const prependNotification = useNotificationStore((s) => s.prependNotification)
  const setUnreadCount      = useNotificationStore((s) => s.setUnreadCount)
  const setNotifications    = useNotificationStore((s) => s.setNotifications)
  const reset               = useNotificationStore((s) => s.reset)
  const qc = useQueryClient()

  useEffect(() => {
    if (!user) return

    const conn = getHubConnection()

    conn.on('ReceiveNotification', (notification) => {
      prependNotification(notification)
      qc.invalidateQueries({ queryKey: ['notifications', 'unread-count'] })
    })

    startHubConnection().catch((err) =>
      console.warn('SignalR connection failed:', err)
    )

    // Seed initial state
    apiClient.get('/api/notifications').then((r) => setNotifications(r.data)).catch(() => {})
    apiClient.get('/api/notifications/unread-count').then((r) => setUnreadCount(r.data.count)).catch(() => {})

    return () => {
      conn.off('ReceiveNotification')
      stopHubConnection()
      reset()
    }
  }, [user?.userId])
}
