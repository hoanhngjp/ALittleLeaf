import { useState } from 'react'
import { useProductReviews, useMyReview } from '../../hooks/useReviews'
import { useAuthStore } from '../../store/useAuthStore'
import ReviewCard from './ReviewCard'
import ReviewForm from './ReviewForm'
import StarRating from './StarRating'

export default function ReviewSection({ productId, averageRating, reviewCount }) {
  const accessToken = useAuthStore((s) => s.accessToken)
  const [page, setPage] = useState(1)
  const PAGE_SIZE = 5

  const { data, isLoading } = useProductReviews(productId, page, PAGE_SIZE)
  const { data: myReview }  = useMyReview(accessToken ? productId : null)

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1

  return (
    <div className="mt-5">
      <h2 className="mb-3" style={{ fontSize: '1.2rem', fontWeight: 600 }}>
        Đánh giá sản phẩm
      </h2>

      {/* Rating summary */}
      {reviewCount > 0 && (
        <div className="d-flex align-items-center gap-2 mb-4">
          <span style={{ fontSize: '2rem', fontWeight: 700 }}>
            {averageRating?.toFixed(1)}
          </span>
          <div>
            <StarRating rating={averageRating ?? 0} size="1.1rem" />
            <div className="text-muted" style={{ fontSize: 13 }}>{reviewCount} đánh giá</div>
          </div>
        </div>
      )}

      {/* Review form — only if user hasn't reviewed yet */}
      {!myReview && (
        <div className="mb-4">
          <ReviewForm productId={productId} />
        </div>
      )}

      {myReview && (
        <div className="alert alert-success py-2 mb-4" style={{ fontSize: 13 }}>
          <i className="fa-solid fa-check me-1" />
          Bạn đã đánh giá sản phẩm này.
        </div>
      )}

      {/* Review list */}
      {isLoading ? (
        <div className="text-center py-3">
          <div className="spinner-border spinner-border-sm text-secondary" />
        </div>
      ) : data?.items?.length > 0 ? (
        <>
          {data.items.map((r) => (
            <ReviewCard key={r.reviewId} review={r} />
          ))}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="d-flex gap-2 mt-3">
              <button
                className="btn btn-sm btn-outline-secondary"
                disabled={page === 1}
                onClick={() => setPage((p) => p - 1)}
              >
                ‹ Trước
              </button>
              <span className="align-self-center text-muted" style={{ fontSize: 13 }}>
                {page} / {totalPages}
              </span>
              <button
                className="btn btn-sm btn-outline-secondary"
                disabled={page === totalPages}
                onClick={() => setPage((p) => p + 1)}
              >
                Sau ›
              </button>
            </div>
          )}
        </>
      ) : (
        <p className="text-muted" style={{ fontSize: 14 }}>
          Chưa có đánh giá nào. Hãy là người đầu tiên!
        </p>
      )}
    </div>
  )
}
