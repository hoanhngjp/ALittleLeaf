import { useMarkAsRead } from '../../hooks/useNotifications'

export default function NotificationItem({ notification, navigate, onClose }) {
  const markAsRead = useMarkAsRead()

  function handleClick() {
    if (!notification.isRead)
      markAsRead.mutate(notification.notificationId)
    if (onClose) onClose()
    if (notification.relatedOrderId)
      navigate(`/profile/orders/${notification.relatedOrderId}`)
  }

  const timeStr = new Date(notification.createdAt).toLocaleString('vi-VN', {
    day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit',
  })

  return (
    <li style={{ borderBottom: '1px solid rgba(0,0,0,.06)', listStyle: 'none' }}>
      <button
        type="button"
        onClick={handleClick}
        style={{
          display:    'flex',
          alignItems: 'flex-start',
          gap:        10,
          width:      '100%',
          padding:    '10px 14px',
          background: notification.isRead ? 'transparent' : 'rgba(25,135,84,.06)',
          border:     'none',
          cursor:     'pointer',
          textAlign:  'left',
          boxSizing:  'border-box',
          font:       'inherit',
          color:      '#1a1a1a',
          flex:       'none',
        }}
      >
        <i
          className="fa-solid fa-bell"
          style={{
            fontSize:   13,
            marginTop:  3,
            flexShrink: 0,
            color:      notification.isRead ? '#bbb' : '#198754',
          }}
        />

        <div style={{ flex: '1 1 0', minWidth: 0, overflow: 'hidden' }}>
          <div style={{
            fontWeight:   notification.isRead ? 500 : 700,
            fontSize:     13,
            color:        '#1a1a1a',
            marginBottom: 2,
            whiteSpace:   'nowrap',
            overflow:     'hidden',
            textOverflow: 'ellipsis',
          }}>
            {notification.title}
          </div>
          <div style={{
            fontSize:     12,
            color:        '#555',
            lineHeight:   1.45,
            whiteSpace:   'normal',
            wordBreak:    'break-word',
            overflowWrap: 'break-word',
          }}>
            {notification.message}
          </div>
          <div style={{ fontSize: 11, color: '#aaa', marginTop: 3 }}>
            {timeStr}
          </div>
        </div>

        {!notification.isRead && (
          <span style={{
            width:        7,
            height:       7,
            borderRadius: '50%',
            background:   '#198754',
            flexShrink:   0,
            marginTop:    5,
            flex:         'none',
          }} />
        )}
      </button>
    </li>
  )
}
