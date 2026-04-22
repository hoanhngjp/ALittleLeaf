import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useAdminOrders } from '../../hooks/useAdminOrders'
import { useDebounce } from '../../hooks/useDebounce'

// ── Constants ────────────────────────────────────────────────────────────────

const SHIPPING_OPTIONS = [
  { value: '',              label: 'Tất cả trạng thái giao hàng' },
  { value: 'not_fulfilled', label: 'Chưa giao hàng' },
  { value: 'fulfilled',     label: 'Đã giao hàng' },
]

const PAYMENT_OPTIONS = [
  { value: '',       label: 'Tất cả thanh toán' },
  { value: 'pending', label: 'Chưa thanh toán' },
  { value: 'paid',    label: 'Đã thanh toán' },
]

const FMT_DATE = new Intl.DateTimeFormat('vi-VN', {
  day: '2-digit', month: '2-digit', year: 'numeric',
})

const FMT_CURRENCY = new Intl.NumberFormat('vi-VN')

// ── SortTh ───────────────────────────────────────────────────────────────────

function SortIcon({ col, sortCol, sortOrder }) {
  if (sortCol !== col) return <i className="bi bi-chevron-expand ms-1 text-muted small" />
  return sortOrder === 'asc'
    ? <i className="bi bi-chevron-up ms-1 small" />
    : <i className="bi bi-chevron-down ms-1 small" />
}

function SortTh({ col, label, sortCol, sortOrder, onSort }) {
  return (
    <th
      role="button"
      style={{ cursor: 'pointer', userSelect: 'none', whiteSpace: 'nowrap' }}
      onClick={() => onSort(col)}
    >
      {label}<SortIcon col={col} sortCol={sortCol} sortOrder={sortOrder} />
    </th>
  )
}

// ── Status badges ─────────────────────────────────────────────────────────────

function ShippingBadge({ status }) {
  const map = {
    fulfilled:     ['bg-success', 'Đã giao hàng'],
    not_fulfilled: ['bg-warning text-dark', 'Chưa giao hàng'],
  }
  const [cls, label] = map[status] ?? ['bg-secondary', status]
  return <span className={`badge ${cls}`}>{label}</span>
}

function PaymentBadge({ status }) {
  const map = {
    paid:    ['bg-success', 'Đã thanh toán'],
    pending: ['bg-danger',  'Chưa thanh toán'],
  }
  const [cls, label] = map[status] ?? ['bg-secondary', status]
  return <span className={`badge ${cls}`}>{label}</span>
}

function ConfirmedBadge({ isConfirmed }) {
  return isConfirmed
    ? <span className="badge bg-success">Đã xác nhận</span>
    : <span className="badge bg-secondary">Chưa xác nhận</span>
}

// ── Pagination helper ─────────────────────────────────────────────────────────

function pageNumbers(current, total) {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)
  const pages = []
  pages.push(1)
  if (current > 3) pages.push('…')
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++)
    pages.push(i)
  if (current < total - 2) pages.push('…')
  pages.push(total)
  return pages
}

// ── AdminOrdersPage ───────────────────────────────────────────────────────────

export default function AdminOrdersPage() {
  const [page, setPage]             = useState(1)
  const [search, setSearch]         = useState('')
  const [shippingStatus, setShip]   = useState('')
  const [paymentStatus, setPay]     = useState('')
  const [startDate, setStart]       = useState('')
  const [endDate, setEnd]           = useState('')
  const [sortCol, setSortCol]       = useState('dateCreated')
  const [sortOrder, setSortOrder]   = useState('desc')

  const keyword = useDebounce(search, 400)

  const { data, isLoading, isError } = useAdminOrders({
    page,
    pageSize: 10,
    keyword:        keyword || undefined,
    shippingStatus: shippingStatus || undefined,
    paymentStatus:  paymentStatus  || undefined,
    startDate:      startDate      || undefined,
    endDate:        endDate        || undefined,
    sortBy:    sortCol,
    sortOrder,
  })

  const orders     = data?.items    ?? []
  const totalItems = data?.totalItems ?? 0
  const totalPages = data?.totalPages ?? (Math.ceil(totalItems / 10) || 1)

  function handleSort(col) {
    if (sortCol === col) setSortOrder((o) => (o === 'asc' ? 'desc' : 'asc'))
    else { setSortCol(col); setSortOrder('desc') }
    setPage(1)
  }

  function handleSearchChange(e) {
    setSearch(e.target.value)
    setPage(1)
  }

  function handleReset() {
    setSearch(''); setShip(''); setPay('')
    setStart(''); setEnd('')
    setSortCol('dateCreated'); setSortOrder('desc')
    setPage(1)
  }

  const sortProps = { sortCol, sortOrder, onSort: handleSort }

  return (
    <div className="app-content-header">
      <div className="container-fluid">
        {/* ── Page header ── */}
        <div className="row mb-2">
          <div className="col-sm-6">
            <h1 className="m-0">Quản lý đơn hàng</h1>
          </div>
          <div className="col-sm-6">
            <ol className="breadcrumb float-sm-end">
              <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
              <li className="breadcrumb-item active">Đơn hàng</li>
            </ol>
          </div>
        </div>
      </div>

      <div className="app-content">
        <div className="container-fluid">

          {/* ── Filter card ── */}
          <div className="card mb-3">
            <div className="card-header bg-primary text-white">
              <h5 className="mb-0"><i className="bi bi-funnel me-2" />Bộ lọc đơn hàng</h5>
            </div>
            <div className="card-body">
              <div className="row g-3 align-items-end">

                {/* Search */}
                <div className="col-md-4">
                  <label className="form-label">Tìm kiếm</label>
                  <div className="input-group">
                    <span className="input-group-text"><i className="bi bi-search" /></span>
                    <input
                      className="form-control"
                      placeholder="Mã đơn, tên khách hàng…"
                      value={search}
                      onChange={handleSearchChange}
                    />
                    {search && (
                      <button className="btn btn-outline-secondary" onClick={() => { setSearch(''); setPage(1) }}>
                        ×
                      </button>
                    )}
                  </div>
                </div>

                {/* Shipping status */}
                <div className="col-md-2">
                  <label className="form-label">Giao hàng</label>
                  <select className="form-select" value={shippingStatus}
                    onChange={(e) => { setShip(e.target.value); setPage(1) }}>
                    {SHIPPING_OPTIONS.map((o) => (
                      <option key={o.value} value={o.value}>{o.label}</option>
                    ))}
                  </select>
                </div>

                {/* Payment status */}
                <div className="col-md-2">
                  <label className="form-label">Thanh toán</label>
                  <select className="form-select" value={paymentStatus}
                    onChange={(e) => { setPay(e.target.value); setPage(1) }}>
                    {PAYMENT_OPTIONS.map((o) => (
                      <option key={o.value} value={o.value}>{o.label}</option>
                    ))}
                  </select>
                </div>

                {/* Date range */}
                <div className="col-md-2">
                  <label className="form-label">Từ ngày</label>
                  <input type="date" className="form-control" value={startDate}
                    onChange={(e) => { setStart(e.target.value); setPage(1) }} />
                </div>
                <div className="col-md-2">
                  <label className="form-label">Đến ngày</label>
                  <input type="date" className="form-control" value={endDate}
                    onChange={(e) => { setEnd(e.target.value); setPage(1) }} />
                </div>

                {/* Reset */}
                <div className="col-12 d-flex justify-content-end">
                  <button className="btn btn-outline-secondary btn-sm" onClick={handleReset}>
                    <i className="bi bi-arrow-counterclockwise me-1" />Đặt lại bộ lọc
                  </button>
                </div>
              </div>
            </div>
          </div>

          {/* ── Table card ── */}
          <div className="card">
            <div className="card-header d-flex justify-content-between align-items-center">
              <h5 className="mb-0">Danh sách đơn hàng</h5>
              <span className="badge bg-primary">{totalItems} đơn hàng</span>
            </div>
            <div className="card-body p-0">
              {isLoading ? (
                <div className="text-center py-5"><div className="spinner-border" /></div>
              ) : isError ? (
                <div className="alert alert-danger m-3">Không thể tải dữ liệu.</div>
              ) : orders.length === 0 ? (
                <div className="text-center py-5 text-muted">Không tìm thấy đơn hàng nào.</div>
              ) : (
                <div className="table-responsive">
                  <table className="table table-hover table-striped mb-0 align-middle">
                    <thead className="table-dark">
                      <tr>
                        <SortTh col="billId"       label="Mã đơn"     {...sortProps} />
                        <SortTh col="customername" label="Khách hàng" {...sortProps} />
                        <SortTh col="dateCreated"   label="Ngày đặt"   {...sortProps} />
                        <SortTh col="totalamount"   label="Tổng tiền"  {...sortProps} />
                        <th>Thanh toán</th>
                        <th>Xác nhận</th>
                        <th>Giao hàng</th>
                        <th className="text-center">Chi tiết</th>
                      </tr>
                    </thead>
                    <tbody>
                      {orders.map((o) => (
                        <tr key={o.billId}>
                          <td><span className="fw-semibold">#{o.billId}</span></td>
                          <td>
                            <div className="fw-semibold">{o.customerName}</div>
                            <small className="text-muted">{o.customerEmail}</small>
                          </td>
                          <td style={{ whiteSpace: 'nowrap' }}>
                            {FMT_DATE.format(new Date(o.dateCreated))}
                          </td>
                          <td style={{ whiteSpace: 'nowrap' }}>
                            {FMT_CURRENCY.format(o.totalAmount)}₫
                          </td>
                          <td><PaymentBadge status={o.paymentStatus} /></td>
                          <td><ConfirmedBadge isConfirmed={o.isConfirmed} /></td>
                          <td><ShippingBadge status={o.shippingStatus} /></td>
                          <td className="text-center">
                            <Link
                              to={`/admin/orders/${o.billId}`}
                              className="btn btn-sm btn-outline-primary"
                            >
                              <i className="bi bi-eye me-1" />Chi tiết
                            </Link>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>

            {/* ── Pagination footer ── */}
            {totalPages > 1 && (
              <div className="card-footer d-flex justify-content-between align-items-center flex-wrap gap-2">
                <small className="text-muted">
                  Trang {page} / {totalPages} ({totalItems} đơn hàng)
                </small>
                <nav>
                  <ul className="pagination pagination-sm mb-0">
                    <li className={`page-item ${page === 1 ? 'disabled' : ''}`}>
                      <button className="page-link" onClick={() => setPage(page - 1)}>‹</button>
                    </li>
                    {pageNumbers(page, totalPages).map((p, i) =>
                      p === '…'
                        ? <li key={`e${i}`} className="page-item disabled"><span className="page-link">…</span></li>
                        : <li key={p} className={`page-item ${p === page ? 'active' : ''}`}>
                            <button className="page-link" onClick={() => setPage(p)}>{p}</button>
                          </li>
                    )}
                    <li className={`page-item ${page === totalPages ? 'disabled' : ''}`}>
                      <button className="page-link" onClick={() => setPage(page + 1)}>›</button>
                    </li>
                  </ul>
                </nav>
              </div>
            )}
          </div>

        </div>
      </div>
    </div>
  )
}
