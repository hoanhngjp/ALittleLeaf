import { useState, useEffect } from 'react'
import { Link, useParams, useSearchParams } from 'react-router-dom'
import { useCategories } from '../hooks/useCategories'
import { useProducts }   from '../hooks/useProducts'
import ProductCard       from '../components/common/ProductCard'

// ── Price filter options (mirrors Index.cshtml filter list) ───────────────────
const PRICE_FILTERS = [
  { label: 'Dưới 500.000₫',           min: null,    max: 500000  },
  { label: '500.000₫ - 1.000.000₫',   min: 500000,  max: 1000000 },
  { label: '1.000.000₫ - 2.000.000₫', min: 1000000, max: 2000000 },
  { label: 'Trên 2.000.000₫',         min: 2000000, max: null    },
]

// ── Category menu (mirrors CollectionsMenu/Default.cshtml) ────────────────────
function CategoryMenu({ categories, activeCategoryId, onSelect, isOpen }) {
  const [openId, setOpenId] = useState(null)
  const topLevel = categories.filter((c) => !c.categoryParentId)

  return (
    <div className="menu-wrap">
      <ul className={`menu-tree collapse d-lg-block${isOpen ? ' show' : ''}`}>
        {topLevel.map((cat) => {
          const subs = cat.subCategories ?? []
          const isOpen = openId === cat.categoryId
          return (
            <li className="menu-item" key={cat.categoryId}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <a
                  href="#"
                  onClick={(e) => { e.preventDefault(); onSelect(cat.categoryId) }}
                  style={{ fontWeight: activeCategoryId === cat.categoryId ? 'bold' : 'normal' }}
                >
                  {cat.categoryName}
                </a>
                {subs.length > 0 && (
                  <button
                    onClick={() => setOpenId(isOpen ? null : cat.categoryId)}
                    style={{ background: 'none', border: 'none', cursor: 'pointer', padding: 0, fontSize: 12 }}
                    aria-label="Toggle sub-menu"
                  >
                    <i className={`fa-solid fa-chevron-${isOpen ? 'up' : 'down'}`} style={{ fontSize: 10 }} />
                  </button>
                )}
              </div>
              {subs.length > 0 && isOpen && (
                <ul className="sub-menu active">
                  {subs.map((sub) => (
                    <li className="sub-menu-item" key={sub.categoryId}>
                      <a
                        href="#"
                        onClick={(e) => { e.preventDefault(); onSelect(sub.categoryId) }}
                        style={{ fontWeight: activeCategoryId === sub.categoryId ? 'bold' : 'normal' }}
                      >
                        {sub.categoryName}
                      </a>
                    </li>
                  ))}
                </ul>
              )}
            </li>
          )
        })}
      </ul>
    </div>
  )
}

// ── Price filter sidebar (mirrors Index.cshtml price filter section) ───────────
function PriceFilter({ minPrice, maxPrice, onSelect, onClear, isOpen }) {
  return (
    <div className={`filter-price-wrap collapse d-lg-block${isOpen ? ' show' : ''}`}>
      <h3>Mức giá</h3>
      <ul className="price-list">
        {PRICE_FILTERS.map((f) => {
          const isActive = f.min === minPrice && f.max === maxPrice
          return (
            <li key={f.label}>
              <a
                href="#"
                onClick={(e) => { e.preventDefault(); onSelect(f.min, f.max) }}
                className={isActive ? 'active' : ''}
              >
                <i className={`fa-regular ${isActive ? 'fa-check-square' : 'fa-square'}`} />
                <span>{f.label}</span>
              </a>
            </li>
          )
        })}
        {(minPrice !== null || maxPrice !== null) && (
          <li className="clear-filter">
            <a href="#" onClick={(e) => { e.preventDefault(); onClear() }}>
              <i className="fa-solid fa-xmark" /> Bỏ lọc giá
            </a>
          </li>
        )}
      </ul>
    </div>
  )
}

// ── Pagination (mirrors Index.cshtml footer pagination) ────────────────────────
function Pagination({ page, totalPages, onPageChange }) {
  if (totalPages <= 1) return null

  // Build visible page numbers: always show first 4, last 4, and window around current
  const pages = new Set()
  for (let i = 1; i <= Math.min(4, totalPages); i++) pages.add(i)
  for (let i = Math.max(totalPages - 3, 1); i <= totalPages; i++) pages.add(i)
  for (let i = Math.max(1, page - 1); i <= Math.min(page + 1, totalPages); i++) pages.add(i)
  const sorted = [...pages].sort((a, b) => a - b)

  // Insert ellipsis gaps
  const withGaps = []
  sorted.forEach((p, idx) => {
    if (idx > 0 && p - sorted[idx - 1] > 1) withGaps.push('…')
    withGaps.push(p)
  })

  return (
    <div className="collections-footer-wrap">
      {page > 1 && (
        <a href="#" onClick={(e) => { e.preventDefault(); onPageChange(page - 1) }} className="prevPage">
          <i className="fa-solid fa-arrow-left-long" />
        </a>
      )}
      <ul className="page-list">
        {withGaps.map((p, i) =>
          p === '…' ? (
            <li key={`gap-${i}`}><span>...</span></li>
          ) : (
            <li key={p}>
              <a
                href="#"
                onClick={(e) => { e.preventDefault(); onPageChange(p) }}
                className={p === page ? 'current' : ''}
              >
                {p}
              </a>
            </li>
          )
        )}
      </ul>
      {page < totalPages && (
        <a href="#" onClick={(e) => { e.preventDefault(); onPageChange(page + 1) }} className="nextPage">
          <i className="fa-solid fa-arrow-right-long" />
        </a>
      )}
    </div>
  )
}

// ── Main page ─────────────────────────────────────────────────────────────────
export default function CollectionsPage() {
  const { slug }                      = useParams()          // categoryId passed as slug
  const { data: categories = [] }     = useCategories()

  const parsedSlug = slug ? Number(slug) : null
  const [categoryId, setCategoryId]   = useState(Number.isFinite(parsedSlug) ? parsedSlug : null)
  const [minPrice,   setMinPrice]     = useState(null)
  const [maxPrice,   setMaxPrice]     = useState(null)
  const [page,       setPage]         = useState(1)
  const [isCategoryOpen, setIsCategoryOpen] = useState(false)
  const [isPriceOpen,    setIsPriceOpen]    = useState(false)

  // When the URL param changes (user navigates from Home category card), sync state
  useEffect(() => {
    const id = slug ? Number(slug) : null
    setCategoryId(Number.isFinite(id) ? id : null)
    setPage(1)
  }, [slug])

  // Reset page to 1 whenever filters change
  const handleCategorySelect = (id) => { setCategoryId(id); setPage(1) }
  const handlePriceSelect    = (min, max) => { setMinPrice(min); setMaxPrice(max); setPage(1) }
  const handlePriceClear     = () => { setMinPrice(null); setMaxPrice(null); setPage(1) }

  const { data, isLoading } = useProducts({
    categoryId: categoryId ?? undefined,
    minPrice:   minPrice   ?? undefined,
    maxPrice:   maxPrice   ?? undefined,
    page,
    pageSize: 12,
  })

  const products   = data?.items      ?? []
  const totalPages = data?.totalPages ?? 1

  // Recursive find: checks top-level and nested subCategories
  const findCategory = (cats, id) => {
    for (const c of cats) {
      if (c.categoryId === id) return c
      const found = findCategory(c.subCategories ?? [], id)
      if (found) return found
    }
    return null
  }
  const activeCategory = categoryId != null ? findCategory(categories, categoryId) : null
  const collectionName = activeCategory?.categoryName ?? 'Tất cả sản phẩm'

  return (
    <div className="collections-wrap container-fluid pt-4">

      {/* Breadcrumb */}
      <div className="direct">
        <div className="direct-items">
          <Link to="/">Trang chủ</Link>
          <p> / <Link to="/collections">Danh mục</Link></p>
          {activeCategory && <p> / {collectionName}</p>}
        </div>
      </div>

      {/* Two-column layout: sidebar (col-lg-3) + product grid (col-lg-9) */}
      <div className="row collections-content">

        {/* SIDEBAR COLUMN — 30% on desktop, full-width on mobile */}
        <div className="col-12 col-lg-3 sidebar-wrap">

          {/* Mobile-only toggle: Category */}
          <button
            className="w-100 border-0 bg-transparent shadow-none py-2 mb-3 d-flex justify-content-between align-items-center d-lg-none text-dark fw-bold"
            onClick={() => setIsCategoryOpen((prev) => !prev)}
            type="button"
            style={{ cursor: 'pointer' }}
          >
            <span>Danh mục sản phẩm</span>
            <i className={`fa-solid ${isCategoryOpen ? 'fa-chevron-up' : 'fa-chevron-down'}`} />
          </button>

          <CategoryMenu
            categories={categories}
            activeCategoryId={categoryId}
            onSelect={handleCategorySelect}
            isOpen={isCategoryOpen}
          />

          {/* Mobile-only toggle: Price filter */}
          <button
            className="w-100 border-0 bg-transparent shadow-none py-2 mb-3 d-flex justify-content-between align-items-center d-lg-none text-dark fw-bold"
            onClick={() => setIsPriceOpen((prev) => !prev)}
            type="button"
            style={{ cursor: 'pointer' }}
          >
            <span>Bộ lọc sản phẩm</span>
            <i className={`fa-solid ${isPriceOpen ? 'fa-chevron-up' : 'fa-chevron-down'}`} />
          </button>

          <PriceFilter
            minPrice={minPrice}
            maxPrice={maxPrice}
            onSelect={handlePriceSelect}
            onClear={handlePriceClear}
            isOpen={isPriceOpen}
          />
        </div>

        {/* MAIN PRODUCT COLUMN — 70% on desktop, full-width on mobile */}
        <div className="col-12 col-lg-9 collections-item-wrap">
          <div className="collections-heading-wrap">
            <div className="collections-title">
              <h1>{collectionName}</h1>
            </div>
          </div>

          {isLoading ? (
            <div className="py-5 text-center">
              <div className="spinner-border text-secondary" role="status" />
            </div>
          ) : products.length === 0 ? (
            <p className="py-5 text-center text-muted">Không tìm thấy sản phẩm.</p>
          ) : (
            <div className="collections-item">
              {products.map((p) => (
                <ProductCard key={p.productId} product={p} />
              ))}
            </div>
          )}

          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>

      </div>
    </div>
  )
}
