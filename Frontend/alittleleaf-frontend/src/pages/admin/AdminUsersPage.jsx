import { useState } from 'react'
import { Link } from 'react-router-dom'
import toast from 'react-hot-toast'
import { useAdminUsers, useToggleUserActive } from '../../hooks/useAdminUsers'
import { useDebounce } from '../../hooks/useDebounce'

// ── Constants ────────────────────────────────────────────────────────────────

const ROLE_OPTIONS = [
  { value: '',         label: 'Tất cả vai trò' },
  { value: 'admin',    label: 'Admin' },
  { value: 'customer', label: 'Khách hàng' },
]

const SEX_OPTIONS = [
  { value: '',      label: 'Tất cả giới tính' },
  { value: 'false', label: 'Nam' },
  { value: 'true',  label: 'Nữ' },
]

const ACTIVE_OPTIONS = [
  { value: '',      label: 'Tất cả trạng thái' },
  { value: 'true',  label: 'Đang hoạt động' },
  { value: 'false', label: 'Đã khóa' },
]

const FMT_DATE = new Intl.DateTimeFormat('vi-VN', {
  day: '2-digit', month: '2-digit', year: 'numeric',
})

// ── SortTh / SortIcon ────────────────────────────────────────────────────────

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

// ── Badges ───────────────────────────────────────────────────────────────────

function ActiveBadge({ isActive }) {
  return isActive
    ? <span className="badge bg-success">Hoạt động</span>
    : <span className="badge bg-danger">Đã khóa</span>
}

function RoleBadge({ role }) {
  return role === 'admin'
    ? <span className="badge bg-primary">Admin</span>
    : <span className="badge bg-secondary">Khách hàng</span>
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

// ── ToggleActiveButton ────────────────────────────────────────────────────────

function ToggleActiveButton({ userId, isActive }) {
  const mutation = useToggleUserActive(userId)

  function handleClick() {
    const next = !isActive
    mutation.mutate(next, {
      onSuccess: () =>
        toast.success(next ? 'Đã kích hoạt tài khoản.' : 'Đã khóa tài khoản.'),
      onError: () =>
        toast.error('Thao tác thất bại. Vui lòng thử lại.'),
    })
  }

  return (
    <button
      className={`btn btn-sm ${isActive ? 'btn-outline-danger' : 'btn-outline-success'}`}
      onClick={handleClick}
      disabled={mutation.isPending}
      title={isActive ? 'Khóa tài khoản' : 'Mở khóa tài khoản'}
    >
      {mutation.isPending
        ? <span className="spinner-border spinner-border-sm" />
        : <i className={`fas ${isActive ? 'fa-lock' : 'fa-unlock'}`} />
      }
    </button>
  )
}

// ── AdminUsersPage ────────────────────────────────────────────────────────────

export default function AdminUsersPage() {
  const [page, setPage]           = useState(1)
  const [search, setSearch]       = useState('')
  const [isActive, setIsActive]   = useState('')
  const [userRole, setUserRole]   = useState('')
  const [userSex, setUserSex]     = useState('')
  const [sortCol, setSortCol]     = useState('createdAt')
  const [sortOrder, setSortOrder] = useState('desc')

  const keyword = useDebounce(search, 400)

  const { data, isLoading, isError } = useAdminUsers({
    page,
    pageSize: 10,
    keyword:  keyword  || undefined,
    isActive: isActive !== '' ? isActive === 'true' : undefined,
    userRole: userRole || undefined,
    userSex:  userSex  !== '' ? userSex  === 'true' : undefined,
    sortBy:   sortCol,
    sortOrder,
  })

  const users      = data?.items      ?? []
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
    setSearch(''); setIsActive(''); setUserRole(''); setUserSex('')
    setSortCol('createdAt'); setSortOrder('desc')
    setPage(1)
  }

  const sortProps = { sortCol, sortOrder, onSort: handleSort }

  return (
    <div className="app-content-header">
      <div className="container-fluid">
        <div className="row mb-2">
          <div className="col-sm-6">
            <h1 className="m-0">Quản lý người dùng</h1>
          </div>
          <div className="col-sm-6">
            <ol className="breadcrumb float-sm-end">
              <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
              <li className="breadcrumb-item active">Người dùng</li>
            </ol>
          </div>
        </div>
      </div>

      <div className="app-content">
        <div className="container-fluid">

          {/* ── Filter card ── */}
          <div className="card mb-3">
            <div className="card-header bg-primary text-white">
              <h5 className="mb-0"><i className="bi bi-funnel me-2" />Bộ lọc người dùng</h5>
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
                      placeholder="ID, tên, email…"
                      value={search}
                      onChange={handleSearchChange}
                    />
                    {search && (
                      <button
                        className="btn btn-outline-secondary"
                        onClick={() => { setSearch(''); setPage(1) }}
                      >×</button>
                    )}
                  </div>
                </div>

                {/* Role */}
                <div className="col-md-2">
                  <label className="form-label">Vai trò</label>
                  <select className="form-select" value={userRole}
                    onChange={(e) => { setUserRole(e.target.value); setPage(1) }}>
                    {ROLE_OPTIONS.map((o) => (
                      <option key={o.value} value={o.value}>{o.label}</option>
                    ))}
                  </select>
                </div>

                {/* Sex */}
                <div className="col-md-2">
                  <label className="form-label">Giới tính</label>
                  <select className="form-select" value={userSex}
                    onChange={(e) => { setUserSex(e.target.value); setPage(1) }}>
                    {SEX_OPTIONS.map((o) => (
                      <option key={o.value} value={o.value}>{o.label}</option>
                    ))}
                  </select>
                </div>

                {/* Active status */}
                <div className="col-md-2">
                  <label className="form-label">Trạng thái</label>
                  <select className="form-select" value={isActive}
                    onChange={(e) => { setIsActive(e.target.value); setPage(1) }}>
                    {ACTIVE_OPTIONS.map((o) => (
                      <option key={o.value} value={o.value}>{o.label}</option>
                    ))}
                  </select>
                </div>

                {/* Reset */}
                <div className="col-md-2 d-flex align-items-end">
                  <button className="btn btn-outline-secondary btn-sm w-100" onClick={handleReset}>
                    <i className="bi bi-arrow-counterclockwise me-1" />Đặt lại
                  </button>
                </div>
              </div>
            </div>
          </div>

          {/* ── Table card ── */}
          <div className="card">
            <div className="card-header d-flex justify-content-between align-items-center">
              <h5 className="mb-0">Danh sách người dùng</h5>
              <span className="badge bg-primary">{totalItems} người dùng</span>
            </div>
            <div className="card-body p-0">
              {isLoading ? (
                <div className="text-center py-5"><div className="spinner-border" /></div>
              ) : isError ? (
                <div className="alert alert-danger m-3">Không thể tải dữ liệu.</div>
              ) : users.length === 0 ? (
                <div className="text-center py-5 text-muted">Không tìm thấy người dùng nào.</div>
              ) : (
                <div className="table-responsive">
                  <table className="table table-hover table-striped mb-0 align-middle">
                    <thead className="table-dark">
                      <tr>
                        <SortTh col="userId"       label="ID"        {...sortProps} />
                        <SortTh col="userFullname" label="Họ và tên" {...sortProps} />
                        <SortTh col="userEmail"    label="Email"     {...sortProps} />
                        <th style={{ whiteSpace: 'nowrap' }}>Giới tính</th>
                        <SortTh col="userRole"     label="Vai trò"   {...sortProps} />
                        <th>Trạng thái</th>
                        <SortTh col="createdAt"    label="Ngày tạo"  {...sortProps} />
                        <SortTh col="updatedAt"    label="Cập nhật"  {...sortProps} />
                        <th className="text-center" style={{ whiteSpace: 'nowrap' }}>Thao tác</th>
                      </tr>
                    </thead>
                    <tbody>
                      {users.map((u) => (
                        <tr key={u.userId}>
                          <td><span className="fw-semibold">#{u.userId}</span></td>
                          <td className="fw-semibold">{u.userFullname}</td>
                          <td>
                            <small>{u.userEmail}</small>
                          </td>
                          <td>{u.userSex ? 'Nữ' : 'Nam'}</td>
                          <td><RoleBadge role={u.userRole} /></td>
                          <td><ActiveBadge isActive={u.userIsActive} /></td>
                          <td style={{ whiteSpace: 'nowrap' }}>
                            {FMT_DATE.format(new Date(u.createdAt))}
                          </td>
                          <td style={{ whiteSpace: 'nowrap' }}>
                            {FMT_DATE.format(new Date(u.updatedAt))}
                          </td>
                          <td className="text-center">
                            <div className="d-flex gap-1 justify-content-center">
                              <Link
                                to={`/admin/users/${u.userId}`}
                                className="btn btn-sm btn-outline-primary"
                                title="Sửa"
                              >
                                <i className="fas fa-edit" />
                              </Link>
                              <ToggleActiveButton userId={u.userId} isActive={u.userIsActive} />
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>

            {/* ── Pagination ── */}
            {totalPages > 1 && (
              <div className="card-footer d-flex justify-content-between align-items-center flex-wrap gap-2">
                <small className="text-muted">
                  Trang {page} / {totalPages} ({totalItems} người dùng)
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
