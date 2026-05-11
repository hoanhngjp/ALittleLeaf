import { useState } from 'react'
import toast from 'react-hot-toast'
import { useAdminReviews, useAdminDeleteReview } from '../../hooks/useReviews'
import { useDebounce } from '../../hooks/useDebounce'

const STARS = [1, 2, 3, 4, 5]

function StarDisplay({ rating }) {
  return (
    <span>
      {STARS.map((s) => (
        <i
          key={s}
          className={s <= rating ? 'fa-solid fa-star text-warning' : 'fa-regular fa-star text-warning'}
          style={{ fontSize: 12 }}
        />
      ))}
    </span>
  )
}

export default function AdminReviewsPage() {
  const [page, setPage]               = useState(1)
  const [productIdInput, setProductIdInput] = useState('')
  const debouncedProductId = useDebounce(productIdInput, 400)

  const productId = debouncedProductId && !isNaN(Number(debouncedProductId))
    ? Number(debouncedProductId)
    : undefined

  const { data, isLoading } = useAdminReviews({ productId, page, pageSize: 20 })
  const deleteReview = useAdminDeleteReview()

  const totalPages = data ? Math.ceil(data.totalCount / 20) : 1

  function handleDelete(review) {
    if (!window.confirm(`Xóa đánh giá của "${review.userName}" về "${review.productName}"?`)) return
    toast.promise(deleteReview.mutateAsync(review.reviewId), {
      loading: 'Đang xóa...',
      success: 'Đã xóa đánh giá.',
      error:   'Xóa thất bại.',
    })
  }

  function handleProductIdChange(e) {
    setProductIdInput(e.target.value)
    setPage(1)
  }

  return (
    <div className="app-content">

      {/* Header */}
      <div className="app-content-header">
        <div className="container-fluid">
          <div className="row">
            <div className="col-sm-6">
              <h3 className="mb-0">Quản lý đánh giá</h3>
            </div>
            <div className="col-sm-6">
              <ol className="breadcrumb float-sm-end">
                <li className="breadcrumb-item"><a href="/admin">Trang chủ</a></li>
                <li className="breadcrumb-item active">Đánh giá</li>
              </ol>
            </div>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="app-content-body">
        <div className="container-fluid">
          <div className="card shadow-sm">

            {/* Filters */}
            <div className="card-header d-flex align-items-center gap-3 flex-wrap">
              <div style={{ maxWidth: 220 }}>
                <input
                  type="number"
                  className="form-control form-control-sm"
                  placeholder="Lọc theo Product ID..."
                  value={productIdInput}
                  onChange={handleProductIdChange}
                  min={1}
                />
              </div>
              {data && (
                <span className="text-muted" style={{ fontSize: 13 }}>
                  {data.totalCount} đánh giá
                </span>
              )}
            </div>

            {/* Table */}
            <div className="card-body p-0">
              <div className="table-responsive">
                <table className="table table-hover table-sm mb-0">
                  <thead className="table-light">
                    <tr>
                      <th style={{ width: 60 }}>ID</th>
                      <th>Khách hàng</th>
                      <th>Sản phẩm</th>
                      <th style={{ width: 110 }}>Đánh giá</th>
                      <th>Nội dung</th>
                      <th style={{ width: 120 }}>Ngày</th>
                      <th style={{ width: 70 }}></th>
                    </tr>
                  </thead>
                  <tbody>
                    {isLoading ? (
                      Array.from({ length: 8 }).map((_, i) => (
                        <tr key={i}>
                          {Array.from({ length: 7 }).map((_, j) => (
                            <td key={j}>
                              <div className="placeholder-glow">
                                <span className="placeholder col-10 rounded" />
                              </div>
                            </td>
                          ))}
                        </tr>
                      ))
                    ) : data?.items?.length === 0 ? (
                      <tr>
                        <td colSpan={7} className="text-center text-muted py-4">
                          Không có đánh giá nào.
                        </td>
                      </tr>
                    ) : (
                      data?.items?.map((r) => (
                        <tr key={r.reviewId}>
                          <td className="text-muted" style={{ fontSize: 12 }}>#{r.reviewId}</td>
                          <td>
                            <div style={{ fontSize: 13, fontWeight: 500 }}>{r.userName}</div>
                            <div className="text-muted" style={{ fontSize: 11 }}>{r.userEmail}</div>
                          </td>
                          <td>
                            <div style={{ fontSize: 13 }}>{r.productName}</div>
                            <div className="text-muted" style={{ fontSize: 11 }}>ID: {r.productId}</div>
                          </td>
                          <td><StarDisplay rating={r.rating} /></td>
                          <td>
                            <span style={{ fontSize: 13 }}>
                              {r.comment
                                ? r.comment.length > 80
                                  ? r.comment.slice(0, 80) + '…'
                                  : r.comment
                                : <span className="text-muted fst-italic">Không có nhận xét</span>}
                            </span>
                          </td>
                          <td className="text-muted" style={{ fontSize: 12 }}>
                            {new Date(r.createdAt).toLocaleDateString('vi-VN')}
                          </td>
                          <td>
                            <button
                              className="btn btn-sm btn-outline-danger"
                              onClick={() => handleDelete(r)}
                              disabled={deleteReview.isPending}
                              title="Xóa đánh giá"
                            >
                              <i className="fa-solid fa-trash" />
                            </button>
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="card-footer d-flex justify-content-between align-items-center">
                <span className="text-muted" style={{ fontSize: 13 }}>
                  Trang {page} / {totalPages}
                </span>
                <div className="d-flex gap-2">
                  <button
                    className="btn btn-sm btn-outline-secondary"
                    disabled={page === 1}
                    onClick={() => setPage((p) => p - 1)}
                  >
                    ‹ Trước
                  </button>
                  <button
                    className="btn btn-sm btn-outline-secondary"
                    disabled={page === totalPages}
                    onClick={() => setPage((p) => p + 1)}
                  >
                    Sau ›
                  </button>
                </div>
              </div>
            )}

          </div>
        </div>
      </div>
    </div>
  )
}
