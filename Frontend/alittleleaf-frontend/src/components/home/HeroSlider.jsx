import { useState, useEffect, useCallback } from 'react'

const SLIDES = [
  'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268220/slider1_kzfyxn.webp',
  'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268221/slider2_oryoac.webp',
  'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268221/slider3_gyv5pc.webp',
  'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268222/slider4_mphagw.webp',
]

export default function HeroSlider() {
  const [active, setActive] = useState(0)
  const len = SLIDES.length

  const goNext = useCallback(() => {
    setActive((prev) => (prev + 1 >= len ? 0 : prev + 1))
  }, [len])

  const goPrev = useCallback(() => {
    setActive((prev) => (prev - 1 < 0 ? len - 1 : prev - 1))
  }, [len])

  // Auto-play: restart whenever active changes
  useEffect(() => {
    const timer = setInterval(goNext, 5000)
    return () => clearInterval(timer)
  }, [goNext, active])

  return (
    <div className="slider">
      <div
        className="slider-track"
        style={{
          transform: `translateX(-${active * 100}%)`,
          transition: 'transform 0.5s ease-in-out',
        }}
      >
        {SLIDES.map((src, i) => (
          <div className="slider-item" key={i}>
            <img src={src} alt={`Slide ${i + 1}`} />
          </div>
        ))}
      </div>

      <div className="buttons">
        <button onClick={goPrev} aria-label="Trước">
          <i className="fa-solid fa-arrow-left" />
        </button>
        <button onClick={goNext} aria-label="Tiếp">
          <i className="fa-solid fa-arrow-right" />
        </button>
      </div>

      <ul className="dots">
        {SLIDES.map((_, i) => (
          <li
            key={i}
            className={active === i ? 'active' : ''}
            onClick={() => setActive(i)}
          />
        ))}
      </ul>
    </div>
  )
}
