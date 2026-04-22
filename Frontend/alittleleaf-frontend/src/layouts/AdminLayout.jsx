import { useState, useEffect, useCallback } from 'react'
import { Outlet } from 'react-router-dom'
import { Toaster } from 'react-hot-toast'
import AdminHeader  from '../components/admin/AdminHeader'
import AdminSidebar from '../components/admin/AdminSidebar'

// ── AdminLTE v4 CSS isolated — injected on mount, removed on unmount ─────────
const ADMINLTE_HREF = '/css/adminlte.min.css'
const LINK_ID       = 'adminlte-stylesheet'

// v4 body classes (layout-fixed + sidebar-expand-lg is the standard v4 setup)
const BODY_BASE = ['layout-fixed', 'sidebar-expand-lg']

const BREAKPOINT = 992 // matches AdminLTE v4 sidebarBreakpoint

function injectAdminLTE() {
  if (document.getElementById(LINK_ID)) return
  const link = document.createElement('link')
  link.id    = LINK_ID
  link.rel   = 'stylesheet'
  link.href  = ADMINLTE_HREF
  document.head.appendChild(link)
}

function removeAdminLTE() {
  document.getElementById(LINK_ID)?.remove()
}

const isMobile = () => window.innerWidth <= BREAKPOINT

export default function AdminLayout() {
  // null = uninitialised; true = sidebar open/expanded; false = collapsed
  const [sidebarOpen, setSidebarOpen] = useState(null)
  const [openMenu, setOpenMenu]       = useState(null)

  // ── CSS + base body classes ─────────────────────────────────────────────
  useEffect(() => {
    injectAdminLTE()
    document.body.classList.add(...BODY_BASE)

    // Default state: desktop open, mobile closed (mirrors v4 JS logic)
    setSidebarOpen(!isMobile())

    return () => {
      removeAdminLTE()
      document.body.classList.remove(
        ...BODY_BASE,
        'sidebar-collapse',
        'sidebar-open',
        'sidebar-overlay',
      )
    }
  }, [])

  // ── Keep body classes in sync with sidebarOpen state ───────────────────
  // v4 expand(): removes sidebar-collapse, adds sidebar-open only on mobile
  // v4 collapse(): removes sidebar-open, adds sidebar-collapse
  useEffect(() => {
    if (sidebarOpen === null) return

    if (sidebarOpen) {
      document.body.classList.remove('sidebar-collapse')
      if (isMobile()) {
        document.body.classList.add('sidebar-open', 'sidebar-overlay')
      }
    } else {
      document.body.classList.remove('sidebar-open', 'sidebar-overlay')
      document.body.classList.add('sidebar-collapse')
    }
  }, [sidebarOpen])

  // ── Re-evaluate on viewport resize ─────────────────────────────────────
  useEffect(() => {
    const onResize = () => {
      // When resizing to desktop: expand; resizing to mobile: collapse
      setSidebarOpen(!isMobile())
    }
    window.addEventListener('resize', onResize)
    return () => window.removeEventListener('resize', onResize)
  }, [])

  const toggleSidebar    = useCallback(() => setSidebarOpen((v) => !v), [])
  const closeSidebar     = useCallback(() => setSidebarOpen(false), [])
  const handleToggleMenu = useCallback(
    (id) => setOpenMenu((prev) => (prev === id ? null : id)),
    [],
  )

  return (
    <div className="app-wrapper">

      <AdminHeader onToggleSidebar={toggleSidebar} />
      <AdminSidebar openMenu={openMenu} onToggleMenu={handleToggleMenu} />

      {/* Mobile overlay — clicking outside closes sidebar (mirrors v4 sidebar-overlay behaviour) */}
      {sidebarOpen && isMobile() && (
        <div
          className="sidebar-overlay"
          onClick={closeSidebar}
          style={{ cursor: 'pointer' }}
        />
      )}

      <main className="app-main">
        <Outlet />
      </main>

      <Toaster position="top-right" toastOptions={{ duration: 3000 }} />
    </div>
  )
}
