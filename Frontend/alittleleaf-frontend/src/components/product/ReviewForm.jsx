import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCreateReview } from '../../hooks/useReviews'
import { useAuthStore } from '../../store/useAuthStore'

export default function ReviewForm({ productId }) {
  const navigate    = useNavigate()
  const accessToken = useAuthStore((s) => s.accessToken)
  const [rating, setRating]   = useState(0)
  const [hovered, setHovered] = useState(0)
  const [comment, setComment] = useState('')
  const [error, setError]     = useState(null)

  const createReview = useCreateReview()

  if (!accessToken) {
    return (
      <p className="text-muted" style={{ fontSize: 14 }}>
        <a href="#" onClick={(e) => { e.preventDefault(); navigate(`/login?returnUrl=${encodeURIComponent(window.location.pathname)}`) }}>
          Đăng nhập
        </a>{' '}
        để viết đánh giá.
      </p>
    )
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setError(null)
    if (rating === 0) { setError('Vui lòng chọn số sao.'); return }

    createReview.mutate(
      { productId, rating, comment: comment.trim() || null },
      {
        onError: (err) => setError(err?.response?.data?.message ?? 'Có lỗi xảy ra.'),
      }
    )
  }

  const displayRating = hovered || rating

  return (
    <form onSubmit={handleSubmit} className="mt-3">
      <div className="mb-2">
        <label className="form-label fw-semibold" style={{ fontSize: 14 }}>Đánh giá của bạn</label>
        <div className="d-flex gap-1">
          {[1, 2, 3, 4, 5].map((s) => (
            <i
              key={s}
              className={`fa-star cursor-pointer ${s <= displayRating ? 'fa-solid text-warning' : 'fa-regular text-warning'}`}
              style={{ fontSize: '1.4rem', cursor: 'pointer' }}
              onMouseEnter={() => setHovered(s)}
              onMouseLeave={() => setHovered(0)}
              onClick={() => setRating(s)}
            />
          ))}
        </div>
      </div>

      <div className="mb-2">
        <textarea
          className="form-control"
          rows={3}
          maxLength={500}
          placeholder="Nhận xét của bạn (tùy chọn)"
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          style={{ fontSize: 14 }}
        />
        <div className="text-end text-muted" style={{ fontSize: 12 }}>{comment.length}/500</div>
      </div>

      {error && <div className="alert alert-danger py-2" style={{ fontSize: 13 }}>{error}</div>}

      <button
        type="submit"
        className="btn btn-sm btn-dark"
        disabled={createReview.isPending}
      >
        {createReview.isPending ? 'Đang gửi...' : 'Gửi đánh giá'}
      </button>
    </form>
  )
}
