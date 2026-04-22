import { useState, useEffect } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useProducts } from '../hooks/useProducts'
import ProductCard from '../components/common/ProductCard'

// ── Pagination (same logic as CollectionsPage) ─────────────────────────────────
function Pagination({ page, totalPages, onPageChange }) {
  if (totalPages <= 1) return null

  const pages = new Set()
  for (let i = 1; i <= Math.min(4, totalPages); i++) pages.add(i)
  for (let i = Math.max(totalPages - 3, 1); i <= totalPages; i++) pages.add(i)
  for (let i = Math.max(1, page - 1); i <= Math.min(page + 1, totalPages); i++) pages.add(i)
  const sorted = [...pages].sort((a, b) => a - b)

  const withGaps = []
  sorted.forEach((p, idx) => {
    if (idx > 0 && p - sorted[idx - 1] > 1) withGaps.push('…')
    withGaps.push(p)
  })

  return (
    <div className="search-footer-wrap">
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
export default function SearchResultsPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const keyword = searchParams.get('q') ?? ''

  const [inputValue, setInputValue] = useState(keyword)
  const [page, setPage] = useState(1)

  // Sync input box when URL ?q= changes (e.g. user arrives from sidebar search)
  useEffect(() => {
    setInputValue(keyword)
    setPage(1)
  }, [keyword])

  const { data, isLoading } = useProducts({
    keyword: keyword.trim() || undefined,
    page,
    pageSize: 10,
  })

  const products   = data?.items      ?? []
  const totalPages = data?.totalPages ?? 1
  const totalItems = data?.totalItems ?? (data?.totalCount ?? products.length)

  const handleSearch = (e) => {
    e.preventDefault()
    const q = inputValue.trim()
    if (q) {
      setPage(1)
      setSearchParams({ q })
    }
  }

  const hasKeyword = keyword.trim().length > 0
  const noResults  = hasKeyword && !isLoading && products.length === 0

  return (
    <div className="searchPage" id="layout-search">
      <div className="container-fluid">
        <div className="row pd-page">
          <div className="col-md-12 col-xs-12">

            {/* Heading */}
            <div className="heading-page">
              <h1>Tìm kiếm</h1>
              {hasKeyword && !isLoading && (
                <p className="subtxt">
                  Có <span>{totalItems} sản phẩm</span> cho tìm kiếm
                </p>
              )}
              {noResults && (
                <p className="subtxt">
                  Có <span>0 sản phẩm</span> cho tìm kiếm
                </p>
              )}
            </div>

            <div className="wrapbox-content-page">
              <div className="content-page clearfix" id="search">

                {/* Search input form */}
                <div className="expanded-message">
                  <div className="search-field">
                    <form onSubmit={handleSearch} className="search-page">
                      <input
                        type="text"
                        className="search_box"
                        name="q"
                        placeholder="Tìm kiếm"
                        value={inputValue}
                        onChange={(e) => setInputValue(e.target.value)}
                      />
                      <button type="submit" id="go" aria-label="Tìm kiếm" />
                    </form>
                  </div>

                  {/* No-results message */}
                  {noResults && (
                    <div className="message-txt clearfix">
                      <p>Rất tiếc, chúng tôi không tìm thấy kết quả cho từ khóa của bạn</p>
                      <p>Vui lòng kiểm tra chính tả, sử dụng các từ tổng quát hơn và thử lại!</p>
                    </div>
                  )}
                </div>

                {/* Keyword label */}
                {hasKeyword && products.length > 0 && (
                  <p className="subtext-result">
                    Kết quả tìm kiếm cho <strong>{keyword}</strong>
                  </p>
                )}

                {/* Loading spinner */}
                {isLoading ? (
                  <div className="py-5 text-center">
                    <div className="spinner-border text-secondary" role="status" />
                  </div>
                ) : (
                  <div className="results content-product-list">
                    <div className="search-list-results">
                      {products.map((p) => (
                        <ProductCard key={p.productId} product={p} />
                      ))}
                    </div>
                  </div>
                )}

                <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />

              </div>
            </div>

          </div>
        </div>
      </div>
    </div>
  )
}
