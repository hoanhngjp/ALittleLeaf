import { useState } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import { useProductDetail, useRelatedProducts } from '../hooks/useProductDetail'
import { useProducts } from '../hooks/useProducts'
import { useAddToCart } from '../hooks/useCart'
import { useAuthStore } from '../store/useAuthStore'

const RELATED_COUNT = 8

// ── Desktop: thumbnail strip + main image ─────────────────────────────────────
function DesktopGallery({ images, productName }) {
  const [activeImage, setActiveImage] = useState(null)
  const displayed = activeImage ?? images[0] ?? ''

  return (
    <div className="product-gallery d-none d-md-flex">
      {/* Thumbnail column */}
      <div className="product-gallery-container">
        <div className="product-gallery-thumbs">
          {images.map((img, idx) => (
            <div
              key={idx}
              className={`product-gallery-thumb${displayed === img ? ' active' : ''}`}
            >
              <a
                href="#"
                className="thumb-placeholder"
                onClick={(e) => { e.preventDefault(); setActiveImage(img) }}
              >
                <img src={img} alt={`${productName} ${idx + 1}`} />
              </a>
            </div>
          ))}
        </div>
      </div>

      {/* Main image */}
      <div className="product-image-detail">
        <div className="product-image-wrap">
          <img className="product-img-feature" src={displayed} alt={productName} />
        </div>
      </div>
    </div>
  )
}

// ── Mobile: full-width translateX slider ──────────────────────────────────────
function MobileSlider({ images, productName }) {
  const [activeIdx, setActiveIdx] = useState(0)
  const len = images.length

  const goPrev = () => setActiveIdx((i) => (i - 1 < 0 ? len - 1 : i - 1))
  const goNext = () => setActiveIdx((i) => (i + 1 >= len ? 0 : i + 1))

  return (
    <div className="mobile-img-slider d-md-none">
      {/* Slide track */}
      <div
        className="mobile-slider-track"
        style={{
          transform: `translateX(-${activeIdx * 100}%)`,
          transition: 'transform 0.4s ease-in-out',
        }}
      >
        {images.map((img, idx) => (
          <div className="mobile-slider-item" key={idx}>
            <img src={img} alt={`${productName} ${idx + 1}`} />
          </div>
        ))}
      </div>

      {/* Prev / Next arrows — only show when >1 image */}
      {len > 1 && (
        <>
          <button className="mobile-slider-btn mobile-slider-prev" onClick={goPrev} aria-label="Ảnh trước">
            <i className="fa-solid fa-chevron-left" />
          </button>
          <button className="mobile-slider-btn mobile-slider-next" onClick={goNext} aria-label="Ảnh sau">
            <i className="fa-solid fa-chevron-right" />
          </button>
        </>
      )}

      {/* Dot indicators */}
      {len > 1 && (
        <ul className="mobile-slider-dots">
          {images.map((_, i) => (
            <li
              key={i}
              className={activeIdx === i ? 'active' : ''}
              onClick={() => setActiveIdx(i)}
            />
          ))}
        </ul>
      )}
    </div>
  )
}

// ── Related Products: up to 8, broadened via useProducts fallback ─────────────
function RelatedProducts({ productId, categoryId }) {
  const { data: directRelated = [], isLoading: loadingDirect } = useRelatedProducts(productId)

  // If direct related < 8, supplement with same-category products (excluding current)
  const needMore = !loadingDirect && directRelated.length < RELATED_COUNT
  const { data: catData } = useProducts({
    categoryId: needMore ? categoryId : undefined,
    pageSize: RELATED_COUNT + 1, // +1 so we can exclude current product
    page: 1,
  })

  if (loadingDirect) return null

  let items = [...directRelated]

  if (needMore && catData?.items) {
    const supplement = catData.items.filter(
      (p) => p.productId !== productId && !items.some((r) => r.productId === p.productId)
    )
    items = [...items, ...supplement].slice(0, RELATED_COUNT)
  } else {
    items = items.slice(0, RELATED_COUNT)
  }

  if (items.length === 0) return null

  return (
    <div className="listProduct-related">
      <div className="heading-title">
        <h2>Sản phẩm liên quan</h2>
      </div>
      <div className="related-product-grid">
        {items.map((p) => (
          <div className="pro-loop" key={p.productId}>
            <div className="product-block">
              <div className="pro-loop-img">
                {p.quantityInStock <= 0 && (
                  <div className="sold-out"><span>Hết hàng</span></div>
                )}
                <Link to={`/products/${p.productId}`}>
                  <picture>
                    <img src={p.primaryImage} alt={p.productName} />
                  </picture>
                </Link>
              </div>
              <div className="product-detail">
                <div className="box-proDetail">
                  <h3 className="pro-name">
                    <Link to={`/products/${p.productId}`}>{p.productName}</Link>
                  </h3>
                </div>
                <div className="box-proPrice">
                  <p className="pro-pice">
                    <span>{p.productPrice.toLocaleString('vi-VN')}₫</span>
                  </p>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

// ── Main page ─────────────────────────────────────────────────────────────────
export default function ProductDetailPage() {
  const { id }            = useParams()
  const navigate          = useNavigate()
  const accessToken       = useAuthStore((s) => s.accessToken)
  const [quantity, setQuantity] = useState(1)
  const [addedMsg, setAddedMsg] = useState(null)

  const { data: product, isLoading, isError } = useProductDetail(id)
  const addToCart = useAddToCart()

  const handleMinus = () => setQuantity((q) => Math.max(1, q - 1))
  const handlePlus  = () => setQuantity((q) => q + 1)
  const handleQtyChange = (e) => {
    const val = parseInt(e.target.value)
    setQuantity(Number.isFinite(val) && val >= 1 ? val : 1)
  }

  const handleAddToCart = () => {
    if (!accessToken) {
      navigate(`/login?returnUrl=/products/${id}`)
      return
    }
    addToCart.mutate(
      { productId: Number(id), quantity },
      {
        onSuccess: () => {
          setAddedMsg('Đã thêm vào giỏ hàng!')
          setTimeout(() => setAddedMsg(null), 3000)
        },
        onError: () => setAddedMsg('Có lỗi xảy ra. Vui lòng thử lại.'),
      }
    )
  }

  if (isLoading) {
    return (
      <div className="productDetail-page">
        <div className="main-wrap py-5 text-center">
          <div className="spinner-border text-secondary" role="status" />
        </div>
      </div>
    )
  }

  if (isError || !product) {
    return (
      <div className="productDetail-page">
        <div className="main-wrap py-5 text-center text-muted">
          Không tìm thấy sản phẩm.
        </div>
      </div>
    )
  }

  const inStock = product.quantityInStock > 0
  const images  = product.images?.length > 0
    ? product.images
    : product.primaryImage ? [product.primaryImage] : []

  return (
    <div className="productDetail-page">
      <div className="main-wrap">

        {/* Breadcrumb */}
        <div className="nav-header-wrap">
          <div className="nav-header ps-3 ps-md-4">
            <ol>
              <li>
                <Link to="/"><span>Trang chủ</span></Link>
                <meta itemProp="position" content="1" />
              </li>
              {product.idCategory && (
                <li>
                  <Link to={`/collections/${product.idCategory}`}>
                    <span>{product.categoryName}</span>
                  </Link>
                  <meta itemProp="position" content="2" />
                </li>
              )}
              <li className="active">
                <span><span>{product.productName}</span></span>
                <meta itemProp="position" content="3" />
              </li>
            </ol>
          </div>
        </div>

        {/* Product section */}
        <div className="product-detail-wrap">
          <div className="product-detail-2-wrap">

            {/* Mobile slider — full width, above info block */}
            <MobileSlider images={images} productName={product.productName} />

            <div className="product-detail-main">

              {/* Desktop gallery — hidden on mobile */}
              <div className="product-content-img">
                <DesktopGallery images={images} productName={product.productName} />
              </div>

              {/* Product info — 10/80/10 on mobile via product-info-mobile wrapper */}
              <div className="product-content-desc">
                <div className="product-info-mobile">

                  <div className="product-title">
                    <h1>{product.productName}</h1>
                    <span id="pro-sku">SKU: {product.productSku ?? `PROD-${product.productId}`}</span>
                  </div>

                  {addedMsg && (
                    <div
                      style={{
                        color: addedMsg.includes('lỗi') ? '#721c24' : '#155724',
                        backgroundColor: addedMsg.includes('lỗi') ? '#f8d7da' : '#d4edda',
                        padding: '10px',
                        margin: '10px 0',
                        borderRadius: '5px',
                        fontSize: '14px',
                      }}
                    >
                      <i className={`fa-solid ${addedMsg.includes('lỗi') ? 'fa-triangle-exclamation' : 'fa-check'}`} />
                      {' '}{addedMsg}
                    </div>
                  )}

                  <div className="product-price">
                    <span className="pro-price">{product.productPrice?.toLocaleString('vi-VN')}₫</span>
                  </div>

                  <div className="selector-actions">
                    <div className="quantity-area">
                      <input className="qty-btn" type="button" value="-" onClick={handleMinus} />
                      <input
                        type="number"
                        id="quantity"
                        className="quantity-selector"
                        value={quantity}
                        min="1"
                        onChange={handleQtyChange}
                        onBlur={handleQtyChange}
                      />
                      <input className="qty-btn" type="button" value="+" onClick={handlePlus} />
                    </div>

                    <div className="wrap-addcart">
                      {inStock ? (
                        <button
                          type="button"
                          className="addToCartProduct"
                          onClick={handleAddToCart}
                          disabled={addToCart.isPending}
                        >
                          {addToCart.isPending ? 'Đang thêm...' : 'Thêm vào giỏ'}
                        </button>
                      ) : (
                        <button
                          type="button"
                          className="addToCartProduct"
                          style={{ backgroundColor: 'red', cursor: 'not-allowed' }}
                          disabled
                        >
                          HẾT HÀNG
                        </button>
                      )}
                    </div>
                  </div>

                  {product.productDescription && (
                    <div className="product-description">
                      <div className="title"><h2>Mô tả</h2></div>
                      <div className="description-content">
                        <div
                          className="description-productdetail"
                          dangerouslySetInnerHTML={{
                            __html: product.productDescription
                              .replace(/\r/g, '')
                              .replace(/\n/g, '<br>'),
                          }}
                        />
                      </div>
                    </div>
                  )}

                </div>{/* end product-info-mobile */}
              </div>
            </div>

            {/* Related products */}
            <RelatedProducts productId={product.productId} categoryId={product.idCategory} />
          </div>
        </div>

      </div>
    </div>
  )
}
