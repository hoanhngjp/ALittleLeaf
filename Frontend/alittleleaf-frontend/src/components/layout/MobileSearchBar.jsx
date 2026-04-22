import { useSidebarStore } from '../../store/useSidebarStore'

/**
 * A fixed search bar that sits directly below the Header on mobile/tablet only.
 * Tapping it opens the Sidebar in 'search' mode so the user gets the full
 * live-search experience (React Query + results list) without duplicating logic.
 *
 * Hidden on md+ — desktop uses the icon button in the Header.
 */
export default function MobileSearchBar() {
  const openSearch = useSidebarStore((s) => s.openSearch)

  return (
    <div className="mobile-search-bar d-md-none" aria-label="Thanh tìm kiếm di động">
      <div className="mobile-search-inner" onClick={openSearch} role="button" tabIndex={0} onKeyDown={(e) => e.key === 'Enter' && openSearch()}>
        <input
          type="text"
          className="mobile-search-input"
          placeholder="Tìm kiếm sản phẩm..."
          readOnly
          tabIndex={-1}
          aria-hidden="true"
        />
        <i className="fa-solid fa-magnifying-glass mobile-search-icon" />
      </div>
    </div>
  )
}
