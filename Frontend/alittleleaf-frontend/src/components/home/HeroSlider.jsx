import { useState, useEffect, useCallback } from 'react'
import { usePublicBanners } from '../../hooks/useBanners'

// R4 fallback — rendered when the API returns an empty list
const STATIC_SLIDES = [
  { imageUrl: 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268220/slider1_kzfyxn.webp', targetUrl: null },
  { imageUrl: 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268221/slider2_oryoac.webp', targetUrl: null },
  { imageUrl: 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268221/slider3_gyv5pc.webp', targetUrl: null },
  { imageUrl: 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776268222/slider4_mphagw.webp', targetUrl: null },
]

function SliderTrack({ slides, active }) {
  return (
    <div
      className="slider-track"
      style={{
        transform: `translateX(-${active * 100}%)`,
        transition: 'transform 0.5s ease-in-out',
      }}
    >
      {slides.map((slide, i) => (
        <div className="slider-item" key={i}>
          {slide.targetUrl ? (
            <a href={slide.targetUrl} target="_blank" rel="noopener noreferrer">
              <img src={slide.imageUrl} alt={`Banner ${i + 1}`} />
            </a>
          ) : (
            <img src={slide.imageUrl} alt={`Banner ${i + 1}`} />
          )}
        </div>
      ))}
    </div>
  )
}

function SliderSkeleton() {
  return (
    <div
      className="slider"
      style={{ background: '#e9ecef', display: 'flex', alignItems: 'center', justifyContent: 'center' }}
      aria-busy="true"
      aria-label="Đang tải banner..."
    >
      <div className="spinner-border text-secondary" role="status">
        <span className="visually-hidden">Đang tải...</span>
      </div>
    </div>
  )
}

export default function HeroSlider() {
  const { data: apiBanners, isLoading } = usePublicBanners()

  // R4: use static fallback if API returned nothing
  const slides = apiBanners && apiBanners.length > 0 ? apiBanners : STATIC_SLIDES

  const [active, setActive] = useState(0)
  const len = slides.length

  const goNext = useCallback(() => setActive((p) => (p + 1 >= len ? 0 : p + 1)), [len])
  const goPrev = useCallback(() => setActive((p) => (p - 1 < 0 ? len - 1 : p - 1)), [len])

  // Reset active index when slide source changes (e.g. data loads)
  useEffect(() => { setActive(0) }, [len])

  // Auto-play
  useEffect(() => {
    const timer = setInterval(goNext, 5000)
    return () => clearInterval(timer)
  }, [goNext, active])

  if (isLoading) return <SliderSkeleton />

  return (
    <div className="slider">
      <SliderTrack slides={slides} active={active} />

      <div className="buttons">
        <button onClick={goPrev} aria-label="Trước">
          <i className="fa-solid fa-arrow-left" />
        </button>
        <button onClick={goNext} aria-label="Tiếp">
          <i className="fa-solid fa-arrow-right" />
        </button>
      </div>

      <ul className="dots">
        {slides.map((_, i) => (
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
