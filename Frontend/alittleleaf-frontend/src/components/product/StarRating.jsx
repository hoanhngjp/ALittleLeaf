export default function StarRating({ rating, maxStars = 5, size = '1rem' }) {
  return (
    <span style={{ fontSize: size, lineHeight: 1 }}>
      {Array.from({ length: maxStars }, (_, i) => {
        const filled = i + 1 <= Math.floor(rating)
        const half   = !filled && i < rating
        return (
          <i
            key={i}
            className={
              filled
                ? 'fa-solid fa-star text-warning'
                : half
                ? 'fa-solid fa-star-half-stroke text-warning'
                : 'fa-regular fa-star text-warning'
            }
            style={{ marginRight: '1px' }}
          />
        )
      })}
    </span>
  )
}
