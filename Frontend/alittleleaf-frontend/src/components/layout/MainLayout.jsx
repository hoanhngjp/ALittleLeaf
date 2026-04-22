import { Outlet }         from 'react-router-dom'
import Header          from './Header'
import Footer          from './Footer'
import MobileSearchBar from './MobileSearchBar'
import Sidebar         from '../Sidebar'

/**
 * MainLayout wraps all customer-facing pages.
 *
 * Fixed elements stacked at the top:
 *   1. Header          — 70px   (all screens)
 *   2. MobileSearchBar — 52px   (mobile/tablet only, d-md-none)
 *
 * .page-body adds padding-top to push content clear of both fixed elements.
 * On md+ the search bar is hidden so only the 70px header offset applies.
 */
export default function MainLayout() {
  return (
    <div className="d-flex flex-column min-vh-100">
      <Header />
      <MobileSearchBar />
      <Sidebar />
      <main className="page-body flex-grow-1">
        <Outlet />
      </main>
      <Footer />
    </div>
  )
}
