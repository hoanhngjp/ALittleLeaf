import { create } from 'zustand'

export const useNotificationStore = create((set) => ({
  unreadCount:   0,
  notifications: [],

  setUnreadCount: (count) => set({ unreadCount: count }),

  setNotifications: (items) => set({ notifications: items }),

  prependNotification: (notification) =>
    set((state) => ({
      notifications: [notification, ...state.notifications].slice(0, 10),
      unreadCount:   state.unreadCount + 1,
    })),

  markOneRead: (id) =>
    set((state) => ({
      notifications: state.notifications.map((n) =>
        n.notificationId === id ? { ...n, isRead: true } : n
      ),
      unreadCount: Math.max(0, state.unreadCount - 1),
    })),

  clearUnread: () =>
    set((state) => ({
      notifications: state.notifications.map((n) => ({ ...n, isRead: true })),
      unreadCount:   0,
    })),

  reset: () => set({ unreadCount: 0, notifications: [] }),
}))
