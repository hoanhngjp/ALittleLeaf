import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useAdminProducts, useDeleteProduct } from '../../hooks/useAdminProducts'
import { useCategories } from '../../hooks/useCategories'
import { useDebounce } from '../../hooks/useDebounce'
import { CategoryOptions } from '../../utils/categoryOptions'

// ── helpers ───────────────────────────────────────────────────────────────────
const VND = (n) => new Intl.NumberFormat('vi-VN').format(n) + '₫'

const FMT_DATE = (d) =>
  d
    ? new Intl.DateTimeFormat('vi-VN', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit',
      }).format(new Date(d))
    : '—'

// ── sort icon ────────────────────────────────────────────────────────────────
function SortIcon({ col, sortCol, sortOrder }) {
  if (sortCol !== col) return <i className="fas fa-sort ms-1 text-muted" style={{ fontSize: 10 }} />
  return sortOrder === 'asc'
    ? <i className="fas fa-sort-up ms-1 text-primary" style={{ fontSize: 10 }} />
    : <i className="fas fa-sort-down ms-1 text-primary" style={{ fontSize: 10 }} />
}

function SortTh({ col, label, sortCol, sortOrder, onSort, className = '', style = {} }) {
  return (
    <th
      className={className}
      style={{ ...thStyle, cursor: 'pointer', userSelect: 'none', ...style }}
      onClick={() => onSort(col)}
    >
      {label}
      <SortIcon col={col} sortCol={sortCol} sortOrder={sortOrder} />
    </th>
  )
}

function SkeletonRow() {
  return (
    <tr style={{ opacity: 0.35 }}>
      {[...Array(9)].map((_, i) => (
        <td key={i}><div style={{ height: 16, background: '#e5e7eb', borderRadius: 4 }} /></td>
      ))}
    </tr>
  )
}

// ── main component ────────────────────────────────────────────────────────────
export default function AdminProductsPage() {
  const [page, setPage]               = useState(1)
  const [search, setSearch]           = useState('')
  const [categoryId, setCategoryId]   = useState('')
  // sortCol values must match C# switch keys: productName | productPrice | quantityInStock | createdAt | updatedAt
  const [sortCol, setSortCol]         = useState('createdAt')
  const [sortOrder, setSortOrder]     = useState('desc')

  const debouncedSearch = useDebounce(search, 400)

  const { data: categoriesData } = useCategories()

  const { data, isLoading, isError } = useAdminProducts({
    page,
    pageSize:   10,
    search:     debouncedSearch,
    sortBy:     sortCol,
    sortOrder,
    categoryId: categoryId || undefined,
  })

  const deleteMutation = useDeleteProduct()

  const products   = data?.items ?? []
  const totalItems = data?.totalItems ?? 0
  const totalPages = data?.totalPages ?? (Math.ceil(totalItems / 10) || 1)

  function handleSort(col) {
    if (sortCol === col) {
      setSortOrder((o) => (o === 'asc' ? 'desc' : 'asc'))
    } else {
      setSortCol(col)
      setSortOrder('asc')
    }
    setPage(1)
  }

  // Reset to page 1 when debounced search changes
  // (handled implicitly — page is reset on search change via useEffect not needed;
  //  user can also manually paginate after a search result)

  function handleDelete(id, name) {
    if (!window.confirm(`Bạn có chắc muốn xóa sản phẩm "${name}"?`)) return
    deleteMutation.mutate(id)
  }

  // ── pagination helpers ────────────────────────────────────────────────────
  function pageNumbers() {
    // Show at most 7 page buttons with ellipsis handled via Bootstrap disabled items
    if (totalPages <= 7) return Array.from({ length: totalPages }, (_, i) => i + 1)
    const pages = []
    pages.push(1)
    if (page > 3) pages.push('…')
    for (let i = Math.max(2, page - 1); i <= Math.min(totalPages - 1, page + 1); i++) pages.push(i)
    if (page < totalPages - 2) pages.push('…')
    pages.push(totalPages)
    return pages
  }

  return (
    <>
      {/* ── Content header ──────────────────────────────────────────────── */}
      <div className="app-content-header">
        <div className="container-fluid">
          <div className="row">
            <div className="col-sm-6">
              <h3 className="mb-0">Quản lý sản phẩm</h3>
            </div>
            <div className="col-sm-6">
              <ol className="breadcrumb float-sm-end">
                <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
                <li className="breadcrumb-item active">Sản phẩm</li>
              </ol>
            </div>
          </div>
        </div>
      </div>

      {/* ── Content body ────────────────────────────────────────────────── */}
      <div className="app-content">
        <div className="container-fluid">

          {isError && (
            <div className="alert alert-danger rounded">
              Không thể tải danh sách sản phẩm. Vui lòng thử lại.
            </div>
          )}

          <div className="card shadow-sm">

            {/* ── Card header ─────────────────────────────────────────── */}
            <div className="card-header border-0">
              <div className="row align-items-center g-2 flex-wrap">
                <div className="col-auto me-auto">
                  <h3 className="card-title mb-0 fw-semibold">
                    <i className="fas fa-box me-2 text-primary" />
                    Danh sách sản phẩm
                    {totalItems > 0 && (
                      <span className="badge text-bg-secondary ms-2" style={{ fontSize: 11 }}>
                        {totalItems}
                      </span>
                    )}
                  </h3>
                </div>

                {/* Category filter dropdown */}
                <div className="col-auto">
                  <select
                    className="form-select form-select-sm"
                    value={categoryId}
                    onChange={(e) => { setCategoryId(e.target.value); setPage(1) }}
                    style={{ minWidth: 160 }}
                  >
                    <option value="">Tất cả danh mục</option>
                    <CategoryOptions categories={categoriesData ?? []} />
                  </select>
                </div>

                {/* Live search — debounced, no submit button needed */}
                <div className="col-auto">
                  <div className="input-group input-group-sm">
                    <span className="input-group-text bg-white border-end-0">
                      <i className="fas fa-search text-muted" />
                    </span>
                    <input
                      type="text"
                      className="form-control border-start-0 ps-0"
                      placeholder="Tìm tên sản phẩm..."
                      value={search}
                      onChange={(e) => { setSearch(e.target.value); setPage(1) }}
                      style={{ minWidth: 180 }}
                    />
                    {search && (
                      <button
                        type="button"
                        className="btn btn-outline-secondary btn-sm"
                        onClick={() => { setSearch(''); setPage(1) }}
                        title="Xóa tìm kiếm"
                      >
                        <i className="fas fa-times" />
                      </button>
                    )}
                  </div>
                </div>

                <div className="col-auto">
                  <Link to="/admin/products/new" className="btn btn-sm btn-primary">
                    <i className="fas fa-plus me-1" />Thêm sản phẩm
                  </Link>
                </div>
              </div>
            </div>

            {/* ── Table ───────────────────────────────────────────────── */}
            <div className="card-body p-0">
              <div className="table-responsive">
                <table className="table table-hover table-borderless align-middle mb-0">
                  <thead>
                    <tr style={{ borderBottom: '2px solid #f3f4f6' }}>
                      <th className="ps-3" style={thStyle}>Ảnh</th>
                      <SortTh col="productName"     label="Tên sản phẩm" sortCol={sortCol} sortOrder={sortOrder} onSort={handleSort} />
                      <th style={thStyle}>Danh mục</th>
                      <SortTh col="productPrice"    label="Giá"           sortCol={sortCol} sortOrder={sortOrder} onSort={handleSort} className="text-end" />
                      <SortTh col="quantityInStock" label="Tồn kho"       sortCol={sortCol} sortOrder={sortOrder} onSort={handleSort} className="text-center" />
                      <th className="text-center" style={thStyle}>Trạng thái</th>
                      <SortTh col="createdAt" label="Ngày tạo"  sortCol={sortCol} sortOrder={sortOrder} onSort={handleSort} />
                      <SortTh col="updatedAt" label="Cập nhật"  sortCol={sortCol} sortOrder={sortOrder} onSort={handleSort} />
                      <th className="text-center pe-3" style={thStyle}>Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    {isLoading
                      ? [...Array(8)].map((_, i) => <SkeletonRow key={i} />)
                      : products.length === 0
                        ? (
                          <tr>
                            <td colSpan={9} className="text-center text-muted py-5">
                              {search ? `Không tìm thấy sản phẩm khớp với "${debouncedSearch}".` : 'Chưa có sản phẩm nào.'}
                            </td>
                          </tr>
                        )
                        : products.map((p) => (
                          <tr key={p.productId}>
                            <td className="ps-3">
                              <img
                                src={p.primaryImage ?? '/placeholder.png'}
                                alt={p.productName}
                                className="rounded"
                                style={{ width: 44, height: 44, objectFit: 'cover' }}
                              />
                            </td>
                            <td style={{ fontSize: 13, color: '#111827', maxWidth: 200 }}>
                              <span className="d-block text-truncate" title={p.productName}>
                                {p.productName}
                              </span>
                            </td>
                            <td style={{ fontSize: 13, color: '#6b7280', whiteSpace: 'nowrap' }}>
                              {p.categoryName ?? '—'}
                            </td>
                            <td className="text-end" style={{ fontSize: 13, fontWeight: 600, whiteSpace: 'nowrap' }}>
                              {VND(p.productPrice)}
                            </td>
                            <td className="text-center">
                              <span
                                className={`badge ${
                                  p.quantityInStock === 0
                                    ? 'text-bg-danger'
                                    : p.quantityInStock <= 5
                                      ? 'text-bg-warning'
                                      : 'text-bg-success'
                                }`}
                                style={{ fontSize: 12, padding: '4px 8px' }}
                              >
                                {p.quantityInStock}
                              </span>
                            </td>
                            <td className="text-center">
                              <span
                                className={`badge ${p.isOnSale ? 'text-bg-success' : 'text-bg-secondary'}`}
                                style={{ fontSize: 11, padding: '3px 8px' }}
                              >
                                {p.isOnSale ? 'Đang bán' : 'Đã ẩn'}
                              </span>
                            </td>
                            <td style={{ fontSize: 12, color: '#6b7280', whiteSpace: 'nowrap' }}>
                              {FMT_DATE(p.createdAt)}
                            </td>
                            <td style={{ fontSize: 12, color: '#6b7280', whiteSpace: 'nowrap' }}>
                              {FMT_DATE(p.updatedAt)}
                            </td>
                            <td className="text-center pe-3">
                              <div className="d-flex gap-1 justify-content-center">
                                <Link
                                  to={`/admin/products/${p.productId}`}
                                  className="btn btn-sm btn-outline-primary py-0 px-2"
                                  style={{ fontSize: 11 }}
                                >
                                  <i className="fas fa-edit me-1" />Sửa
                                </Link>
                                <button
                                  className="btn btn-sm btn-outline-danger py-0 px-2"
                                  style={{ fontSize: 11 }}
                                  onClick={() => handleDelete(p.productId, p.productName)}
                                  disabled={deleteMutation.isPending}
                                >
                                  <i className="fas fa-trash me-1" />Xóa
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))
                    }
                  </tbody>
                </table>
              </div>
            </div>

            {/* ── Pagination ──────────────────────────────────────────── */}
            {totalPages > 1 && (
              <div className="card-footer border-0 d-flex align-items-center justify-content-between flex-wrap gap-2">
                <small className="text-muted">
                  Trang {page} / {totalPages} ({totalItems} sản phẩm)
                </small>
                <nav>
                  <ul className="pagination pagination-sm mb-0">
                    <li className={`page-item${page <= 1 ? ' disabled' : ''}`}>
                      <button className="page-link" onClick={() => setPage((p) => p - 1)}>
                        &laquo;
                      </button>
                    </li>
                    {pageNumbers().map((n, i) =>
                      n === '…'
                        ? (
                          <li key={`ellipsis-${i}`} className="page-item disabled">
                            <span className="page-link">…</span>
                          </li>
                        )
                        : (
                          <li key={n} className={`page-item${page === n ? ' active' : ''}`}>
                            <button className="page-link" onClick={() => setPage(n)}>{n}</button>
                          </li>
                        )
                    )}
                    <li className={`page-item${page >= totalPages ? ' disabled' : ''}`}>
                      <button className="page-link" onClick={() => setPage((p) => p + 1)}>
                        &raquo;
                      </button>
                    </li>
                  </ul>
                </nav>
              </div>
            )}

          </div>
        </div>
      </div>
    </>
  )
}

const thStyle = { fontWeight: 600, color: '#6b7280', fontSize: 12 }
