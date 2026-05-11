import StarRating from './StarRating'

export default function ReviewCard({ review }) {
  const date = new Date(review.createdAt).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })

  return (
    <div className="card mb-3 border-0 border-bottom pb-3">
      <div className="d-flex align-items-center gap-2 mb-1">
        <div
          className="rounded-circle bg-secondary text-white d-flex align-items-center justify-content-center"
          style={{ width: 36, height: 36, fontSize: 14, flexShrink: 0 }}
        >
          {review.firstName?.charAt(0)?.toUpperCase() ?? '?'}
        </div>
        <div>
          <div className="fw-semibold" style={{ fontSize: 14 }}>{review.firstName}</div>
          <StarRating rating={review.rating} size="0.85rem" />
        </div>
        <span className="ms-auto text-muted" style={{ fontSize: 12 }}>{date}</span>
      </div>
      {review.comment && (
        <p className="mb-0 text-secondary" style={{ fontSize: 14, paddingLeft: 44 }}>
          {review.comment}
        </p>
      )}
    </div>
  )
}
