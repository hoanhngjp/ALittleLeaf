import { useRef, useEffect, useState } from 'react'
import { createPortal } from 'react-dom'
import { useNavigate } from 'react-router-dom'
import { useNotificationStore } from '../../store/useNotificationStore'
import { useMarkAllAsRead } from '../../hooks/useNotifications'
import NotificationItem from './NotificationItem'

export default function NotificationDropdown({ iconStyle = {} }) {
  const [open, setOpen]    = useState(false)
  const navigate           = useNavigate()
  const [panelPos, setPanelPos] = useState({ top: 62, left: 'auto' })
  const bellRef            = useRef(null)

  const unreadCount   = useNotificationStore((s) => s.unreadCount)
  const notifications = useNotificationStore((s) => s.notifications)
  const markAllAsRead = useMarkAllAsRead()

  // Recalculate panel position whenever the dropdown opens
  useEffect(() => {
    if (!open || !bellRef.current) return
    const rect       = bellRef.current.getBoundingClientRect()
    const panelWidth = Math.min(340, window.innerWidth - 24)
    // Right-align panel to bell's right edge, then clamp to viewport
    let left = rect.right - panelWidth
    left = Math.max(12, left)                                  // don't overflow left
    left = Math.min(left, window.innerWidth - panelWidth - 12) // don't overflow right
    setPanelPos({ top: rect.bottom + 8, left })
  }, [open])

  // Close on outside click
  useEffect(() => {
    if (!open) return
    function onMouseDown(e) {
      const insideRoot  = bellRef.current?.closest('[data-notification-root]')?.contains(e.target)
      const insidePanel = !!e.target.closest('[data-notification-panel]')
      if (!insideRoot && !insidePanel) setOpen(false)
    }
    document.addEventListener('mousedown', onMouseDown)
    return () => document.removeEventListener('mousedown', onMouseDown)
  }, [open])

  const panel = open && createPortal(
    <div
      data-notification-panel
      style={{
        position: 'fixed',
        top:      panelPos.top,
        left:     panelPos.left,
        width:    Math.min(340, window.innerWidth - 24),
        background: '#fff',
        border:     '1px solid rgba(0,0,0,.12)',
        borderRadius: 10,
        boxShadow:  '0 6px 24px rgba(0,0,0,.14)',
        zIndex:     99999,
        fontFamily: 'inherit',
        fontSize:   'initial',
        lineHeight: 'initial',
        color:      '#1a1a1a',
      }}
    >
      {/* Header row */}
      <div style={{
        display:        'flex',
        justifyContent: 'space-between',
        alignItems:     'center',
        padding:        '10px 14px',
        borderBottom:   '1px solid rgba(0,0,0,.08)',
      }}>
        <span style={{ fontWeight: 700, fontSize: 14, color: '#1a1a1a' }}>
          Thông báo
          {unreadCount > 0 && (
            <span style={{
              marginLeft:   6,
              background:   '#dc3545',
              color:        '#fff',
              borderRadius: 10,
              padding:      '1px 6px',
              fontSize:     11,
              fontWeight:   600,
            }}>
              {unreadCount}
            </span>
          )}
        </span>
        {unreadCount > 0 && (
          <button
            type="button"
            onClick={() => markAllAsRead.mutate()}
            style={{
              background:  'none',
              border:      'none',
              padding:     0,
              cursor:      'pointer',
              color:       '#198754',
              fontSize:    12,
              fontWeight:  500,
              whiteSpace:  'nowrap',
            }}
          >
            Đọc tất cả
          </button>
        )}
      </div>

      {/* Notification list */}
      <ul style={{
        listStyle:  'none',
        margin:     0,
        padding:    0,
        maxHeight:  360,
        overflowY:  'auto',
      }}>
        {notifications.length === 0 ? (
          <li style={{
            textAlign:  'center',
            color:      '#aaa',
            padding:    '32px 16px',
            fontSize:   13,
          }}>
            <i className="fa-regular fa-bell-slash" style={{ fontSize: 28, display: 'block', marginBottom: 8 }} />
            Không có thông báo nào.
          </li>
        ) : (
          notifications.map((n) => (
            <NotificationItem
              key={n.notificationId}
              notification={n}
              navigate={navigate}
              onClose={() => setOpen(false)}
            />
          ))
        )}
      </ul>
    </div>,
    document.body
  )

  return (
    <div data-notification-root style={{ position: 'relative', display: 'flex', alignItems: 'center' }}>
      {/* Bell button */}
      <button
        ref={bellRef}
        type="button"
        aria-label="Thông báo"
        onClick={() => setOpen((o) => !o)}
        style={{ ...iconStyle, position: 'relative' }}
      >
        <i className="fa-regular fa-bell" />
        {unreadCount > 0 && (
          <span style={{
            position:      'absolute',
            top:           -5,
            right:         -7,
            background:    '#dc3545',
            color:         '#fff',
            borderRadius:  '50%',
            width:         16,
            height:        16,
            fontSize:      10,
            display:       'flex',
            alignItems:    'center',
            justifyContent:'center',
            fontWeight:    700,
            lineHeight:    1,
            pointerEvents: 'none',
          }}>
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </button>

      {panel}
    </div>
  )
}
