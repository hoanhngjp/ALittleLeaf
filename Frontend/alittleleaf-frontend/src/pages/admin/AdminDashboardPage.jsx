import { useState } from 'react'
import { Link } from 'react-router-dom'
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
  PieChart, Pie, Cell, Legend,
} from 'recharts'
import { useDashboardStats } from '../../hooks/useAdminDashboard'

// ── helpers ──────────────────────────────────────────────────────────────────
const VND = (n) => new Intl.NumberFormat('vi-VN').format(n) + '₫'
const MONTH_LABEL = (year, month) => `${String(month).padStart(2, '0')}/${year}`

// ── chart colours ─────────────────────────────────────────────────────────────
const PIE_COLORS    = ['#3b82f6', '#f59e0b', '#10b981', '#ef4444', '#8b5cf6', '#ec4899', '#14b8a6', '#f97316']
const DONUT_COLORS  = ['#3b82f6', '#f59e0b', '#10b981', '#ef4444', '#8b5cf6']

// ── default date range helpers ────────────────────────────────────────────────
function toDateInputValue(d) {
  return d.toISOString().slice(0, 10)
}
const today    = new Date()
const lastYear = new Date(today.getFullYear(), today.getMonth() - 11, 1)

// ── custom tooltip for bar chart ─────────────────────────────────────────────
function RevenueTooltip({ active, payload, label }) {
  if (!active || !payload?.length) return null
  return (
    <div style={{
      background: '#fff', border: '1px solid #e5e7eb', borderRadius: 8,
      padding: '8px 14px', boxShadow: '0 4px 12px rgba(0,0,0,.1)', fontSize: 13,
    }}>
      <p style={{ margin: 0, fontWeight: 600, color: '#374151' }}>{label}</p>
      <p style={{ margin: '4px 0 0', color: '#3b82f6' }}>{VND(payload[0].value)}</p>
    </div>
  )
}

function CategoryTooltip({ active, payload }) {
  if (!active || !payload?.length) return null
  const { name, value } = payload[0]
  return (
    <div style={{
      background: '#fff', border: '1px solid #e5e7eb', borderRadius: 8,
      padding: '8px 14px', boxShadow: '0 4px 12px rgba(0,0,0,.1)', fontSize: 13,
    }}>
      <p style={{ margin: 0, fontWeight: 600, color: '#374151' }}>{name}</p>
      <p style={{ margin: '4px 0 0', color: '#10b981' }}>{VND(value)}</p>
    </div>
  )
}

// ── KPI small-box ─────────────────────────────────────────────────────────────
function SmallBox({ color, icon, value, label, to, linkLabel }) {
  return (
    <div className="col-lg-3 col-6">
      <div className={`small-box text-bg-${color}`}>
        <div className="inner">
          <h3>{value}</h3>
          <p>{label}</p>
        </div>
        <i className={`small-box-icon ${icon}`} />
        <Link to={to} className="small-box-footer">
          {linkLabel} <i className="fas fa-arrow-circle-right" />
        </Link>
      </div>
    </div>
  )
}

function SkeletonBox() {
  return (
    <div className="col-lg-3 col-6">
      <div className="small-box text-bg-secondary" style={{ opacity: 0.35 }}>
        <div className="inner"><h3>—</h3><p>&nbsp;</p></div>
      </div>
    </div>
  )
}

function CardSpinner({ height = 260, color = 'primary' }) {
  return (
    <div style={{ height, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div className={`spinner-border text-${color}`} role="status" />
    </div>
  )
}

// ── main component ────────────────────────────────────────────────────────────
export default function AdminDashboardPage() {
  const [startDate, setStartDate] = useState(toDateInputValue(lastYear))
  const [endDate,   setEndDate]   = useState(toDateInputValue(today))

  const { data, isLoading, isError } = useDashboardStats({ startDate, endDate })

  const revenueData = (data?.revenueByMonth ?? []).map((r) => ({
    name:    MONTH_LABEL(r.year, r.month),
    revenue: r.revenue,
  }))

  const topPieData = (data?.topSellingProducts ?? []).slice(0, 5).map((p) => ({
    name:  p.productName,
    value: p.totalSold,
  }))

  const categoryData = (data?.revenueByCategory ?? []).map((c) => ({
    name:  c.categoryName,
    value: c.revenue,
  }))

  return (
    <>
      {/* ── content header ────────────────────────────────────────────── */}
      <div className="app-content-header">
        <div className="container-fluid">
          <div className="row">
            <div className="col-sm-6">
              <h3 className="mb-0">Bảng chính</h3>
            </div>
            <div className="col-sm-6">
              <ol className="breadcrumb float-sm-end">
                <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
                <li className="breadcrumb-item active">Bảng chính</li>
              </ol>
            </div>
          </div>
        </div>
      </div>

      {/* ── content body ──────────────────────────────────────────────── */}
      <div className="app-content">
        <div className="container-fluid">

          {isError && (
            <div className="alert alert-danger rounded">
              Không thể tải dữ liệu. Vui lòng thử lại.
            </div>
          )}

          {/* ── Date filter ───────────────────────────────────────────── */}
          <div className="card mb-3">
            <div className="card-body py-2">
              <div className="row g-2 align-items-center">
                <div className="col-auto">
                  <i className="fas fa-calendar-alt text-primary me-1" />
                  <span className="fw-semibold" style={{ fontSize: 14 }}>Lọc theo khoảng thời gian:</span>
                </div>
                <div className="col-auto">
                  <label className="form-label mb-0 me-1 small text-muted">Từ</label>
                  <input
                    type="date"
                    className="form-control form-control-sm d-inline-block"
                    style={{ width: 150 }}
                    value={startDate}
                    max={endDate}
                    onChange={(e) => setStartDate(e.target.value)}
                  />
                </div>
                <div className="col-auto">
                  <label className="form-label mb-0 me-1 small text-muted">Đến</label>
                  <input
                    type="date"
                    className="form-control form-control-sm d-inline-block"
                    style={{ width: 150 }}
                    value={endDate}
                    min={startDate}
                    max={toDateInputValue(today)}
                    onChange={(e) => setEndDate(e.target.value)}
                  />
                </div>
                <div className="col-auto">
                  <button
                    className="btn btn-outline-secondary btn-sm"
                    onClick={() => {
                      setStartDate(toDateInputValue(lastYear))
                      setEndDate(toDateInputValue(today))
                    }}
                  >
                    <i className="bi bi-arrow-counterclockwise me-1" />Đặt lại
                  </button>
                </div>
              </div>
            </div>
          </div>

          {/* ── KPI row ───────────────────────────────────────────────── */}
          <div className="row">
            {isLoading ? (
              [1, 2, 3, 4].map((k) => <SkeletonBox key={k} />)
            ) : (
              <>
                <SmallBox color="info"    icon="fas fa-shopping-cart"        value={data?.totalOrders ?? 0}                label="Tổng đơn hàng" to="/admin/orders"   linkLabel="Chi tiết" />
                <SmallBox color="warning" icon="fas fa-user-friends"          value={data?.totalUsers ?? 0}                 label="Người dùng"    to="/admin/users"    linkLabel="Chi tiết" />
                <SmallBox color="success" icon="fas fa-box"                   value={data?.totalProducts ?? 0}              label="Sản phẩm"      to="/admin/products" linkLabel="Cập nhật" />
                <SmallBox color="danger"  icon="fas fa-exclamation-triangle"  value={(data?.lowStockProducts ?? []).length} label="Sắp hết hàng"  to="/admin/products" linkLabel="Xem ngay" />
              </>
            )}
          </div>

          {/* ── Revenue bar chart ─────────────────────────────────────── */}
          <div className="row">
            <div className="col-12">
              <div className="card shadow-sm">
                <div className="card-header border-0 pb-0">
                  <h3 className="card-title font-weight-bold">
                    <i className="fas fa-chart-bar mr-2 text-primary" />
                    Doanh thu theo tháng
                  </h3>
                </div>
                <div className="card-body">
                  {isLoading ? (
                    <CardSpinner height={300} color="primary" />
                  ) : revenueData.length === 0 ? (
                    <p className="text-muted text-center py-5">Chưa có dữ liệu doanh thu.</p>
                  ) : (
                    <div style={{ width: '100%', height: 300, minWidth: 0 }}>
                      <ResponsiveContainer width="100%" height="100%">
                        <AreaChart data={revenueData} margin={{ top: 10, right: 16, left: 8, bottom: 5 }}>
                          <defs>
                            <linearGradient id="revenueGradient" x1="0" y1="0" x2="0" y2="1">
                              <stop offset="5%"  stopColor="#3b82f6" stopOpacity={0.3} />
                              <stop offset="95%" stopColor="#3b82f6" stopOpacity={0.02} />
                            </linearGradient>
                          </defs>
                          <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#e5e7eb" />
                          <XAxis dataKey="name" tick={{ fontSize: 12, fill: '#6b7280' }} axisLine={false} tickLine={false} />
                          <YAxis
                            tickFormatter={(v) => new Intl.NumberFormat('vi-VN', { notation: 'compact' }).format(v)}
                            tick={{ fontSize: 12, fill: '#6b7280' }}
                            axisLine={false}
                            tickLine={false}
                          />
                          <Tooltip content={<RevenueTooltip />} />
                          <Area
                            type="monotone"
                            dataKey="revenue"
                            stroke="#3b82f6"
                            strokeWidth={2.5}
                            fill="url(#revenueGradient)"
                            dot={{ r: 3, fill: '#3b82f6', strokeWidth: 0 }}
                            activeDot={{ r: 5, fill: '#3b82f6' }}
                          />
                        </AreaChart>
                      </ResponsiveContainer>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* ── Top-selling donut + Category revenue pie ──────────────── */}
          <div className="row">

            {/* Top-selling donut */}
            <div className="col-lg-6 col-12">
              <div className="card shadow-sm h-100">
                <div className="card-header border-0 pb-0">
                  <h3 className="card-title font-weight-bold">
                    <i className="fas fa-chart-pie mr-2 text-warning" />
                    Sản phẩm bán chạy nhất
                  </h3>
                </div>
                <div className="card-body">
                  {isLoading ? (
                    <CardSpinner height={260} color="warning" />
                  ) : topPieData.length === 0 ? (
                    <p className="text-muted text-center py-5">Chưa có dữ liệu.</p>
                  ) : (
                    <div style={{ width: '100%', height: 260, minWidth: 0 }}>
                      <ResponsiveContainer width="100%" height="100%">
                        <PieChart>
                          <Pie
                            data={topPieData}
                            cx="50%" cy="45%"
                            innerRadius={55} outerRadius={90}
                            paddingAngle={3}
                            dataKey="value"
                          >
                            {topPieData.map((_, i) => (
                              <Cell key={i} fill={DONUT_COLORS[i % DONUT_COLORS.length]} />
                            ))}
                          </Pie>
                          <Legend
                            iconType="circle"
                            iconSize={8}
                            formatter={(value) =>
                              <span style={{ fontSize: 12, color: '#374151' }}>
                                {value.length > 20 ? value.slice(0, 20) + '…' : value}
                              </span>
                            }
                          />
                          <Tooltip formatter={(v) => [v + ' sp', 'Đã bán']} />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                  )}
                </div>
              </div>
            </div>

            {/* Revenue by category doughnut */}
            <div className="col-lg-6 col-12">
              <div className="card shadow-sm h-100">
                <div className="card-header border-0 pb-0">
                  <h3 className="card-title font-weight-bold">
                    <i className="fas fa-tags mr-2 text-success" />
                    Doanh thu theo danh mục
                  </h3>
                </div>
                <div className="card-body">
                  {isLoading ? (
                    <CardSpinner height={260} color="success" />
                  ) : categoryData.length === 0 ? (
                    <p className="text-muted text-center py-5">Chưa có dữ liệu.</p>
                  ) : (
                    <div style={{ width: '100%', height: 260, minWidth: 0 }}>
                      <ResponsiveContainer width="100%" height="100%">
                        <PieChart>
                          <Pie
                            data={categoryData}
                            cx="50%" cy="45%"
                            innerRadius={55} outerRadius={90}
                            paddingAngle={3}
                            dataKey="value"
                            label={({ percent }) => `${(percent * 100).toFixed(0)}%`}
                            labelLine={false}
                          >
                            {categoryData.map((_, i) => (
                              <Cell key={i} fill={PIE_COLORS[i % PIE_COLORS.length]} />
                            ))}
                          </Pie>
                          <Legend
                            iconType="circle"
                            iconSize={8}
                            formatter={(value) =>
                              <span style={{ fontSize: 12, color: '#374151' }}>
                                {value.length > 22 ? value.slice(0, 22) + '…' : value}
                              </span>
                            }
                          />
                          <Tooltip content={<CategoryTooltip />} />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                  )}
                </div>
              </div>
            </div>

          </div>

          {/* ── Low-stock table ────────────────────────────────────────── */}
          <div className="row mt-3">
            <div className="col-12">
              <div className="card shadow-sm">
                <div className="card-header border-0 pb-0">
                  <h3 className="card-title font-weight-bold">
                    <i className="fas fa-exclamation-triangle mr-2 text-danger" />
                    Sản phẩm sắp hết hàng
                  </h3>
                </div>
                <div className="card-body p-0">
                  {isLoading ? (
                    <CardSpinner height={200} color="danger" />
                  ) : (data?.lowStockProducts ?? []).length === 0 ? (
                    <p className="text-muted text-center py-4">Không có sản phẩm sắp hết hàng.</p>
                  ) : (
                    <div className="table-responsive">
                      <table className="table table-hover table-borderless align-middle mb-0">
                        <thead>
                          <tr style={{ borderBottom: '2px solid #f3f4f6' }}>
                            <th className="pl-3" style={{ width: 56, fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Ảnh</th>
                            <th style={{ fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Tên sản phẩm</th>
                            <th className="text-center" style={{ width: 80, fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Tồn kho</th>
                            <th className="text-center pr-3" style={{ width: 72, fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Thao tác</th>
                          </tr>
                        </thead>
                        <tbody>
                          {data.lowStockProducts.map((p) => (
                            <tr key={p.productId}>
                              <td className="pl-3">
                                <img
                                  src={p.primaryImage ?? '/placeholder.png'}
                                  alt={p.productName}
                                  className="rounded"
                                  style={{ width: 40, height: 40, objectFit: 'cover' }}
                                />
                              </td>
                              <td style={{ fontSize: 13, color: '#111827' }}>{p.productName}</td>
                              <td className="text-center">
                                <span className={`badge ${p.quantityInStock === 0 ? 'text-bg-danger' : 'text-bg-warning'}`}
                                  style={{ fontSize: 12, padding: '4px 8px' }}>
                                  {p.quantityInStock}
                                </span>
                              </td>
                              <td className="text-center pr-3">
                                <Link
                                  to={`/admin/products/${p.productId}`}
                                  className="btn btn-sm btn-outline-primary py-0 px-2"
                                  title="Sửa sản phẩm"
                                >
                                  <i className="fas fa-edit" />
                                </Link>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* ── Top-selling detail table ───────────────────────────────── */}
          <div className="row mt-3">
            <div className="col-12">
              <div className="card shadow-sm">
                <div className="card-header border-0 pb-0">
                  <h3 className="card-title font-weight-bold">
                    <i className="fas fa-trophy mr-2 text-warning" />
                    Top sản phẩm bán chạy
                  </h3>
                </div>
                <div className="card-body p-0">
                  {isLoading ? (
                    <CardSpinner height={120} color="warning" />
                  ) : (data?.topSellingProducts ?? []).length === 0 ? (
                    <p className="text-muted text-center py-4">Chưa có dữ liệu.</p>
                  ) : (
                    <div className="table-responsive">
                      <table className="table table-hover table-borderless align-middle mb-0">
                        <thead>
                          <tr style={{ borderBottom: '2px solid #f3f4f6' }}>
                            <th className="pl-3" style={{ width: 40, fontWeight: 600, color: '#6b7280', fontSize: 12 }}>#</th>
                            <th style={{ width: 56, fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Ảnh</th>
                            <th style={{ fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Tên sản phẩm</th>
                            <th className="text-right" style={{ fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Đã bán</th>
                            <th className="text-right pr-3" style={{ fontWeight: 600, color: '#6b7280', fontSize: 12 }}>Doanh thu</th>
                          </tr>
                        </thead>
                        <tbody>
                          {data.topSellingProducts.map((p, i) => (
                            <tr key={p.productId}>
                              <td className="pl-3">
                                <span style={{
                                  display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                                  width: 24, height: 24, borderRadius: '50%',
                                  background: i < 3 ? ['#f59e0b','#9ca3af','#cd7f32'][i] : '#e5e7eb',
                                  color: i < 3 ? '#fff' : '#374151',
                                  fontSize: 11, fontWeight: 700,
                                }}>
                                  {i + 1}
                                </span>
                              </td>
                              <td>
                                <img
                                  src={p.primaryImage ?? '/placeholder.png'}
                                  alt={p.productName}
                                  className="rounded"
                                  style={{ width: 44, height: 44, objectFit: 'cover' }}
                                />
                              </td>
                              <td style={{ fontSize: 13, color: '#111827' }}>{p.productName}</td>
                              <td className="text-right" style={{ fontSize: 13, fontWeight: 600 }}>{p.totalSold}</td>
                              <td className="text-right pr-3" style={{ fontSize: 13, fontWeight: 600, color: '#10b981' }}>{VND(p.totalRevenue)}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </>
  )
}
