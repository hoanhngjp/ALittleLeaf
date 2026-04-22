import { useState, useEffect, useRef } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import { useCategories } from '../../hooks/useCategories'
import { useAdminProduct, useCreateProduct, useUpdateProduct } from '../../hooks/useAdminProducts'
import { CategoryOptions } from '../../utils/categoryOptions'

// ── helpers ───────────────────────────────────────────────────────────────────
const EMPTY_FORM = {
  productName:        '',
  idCategory:         '',
  productPrice:       '',
  quantityInStock:    '',
  productDescription: '',
  isOnSale:           true,
}

// ── image preview item ─────────────────────────────────────────────────────────
function ImagePreviewCard({ src, isPrimary, onSetPrimary, onRemove, label }) {
  return (
    <div
      style={{
        position: 'relative',
        width: 110,
        flexShrink: 0,
        borderRadius: 8,
        border: isPrimary ? '2px solid #3b82f6' : '2px solid #e5e7eb',
        overflow: 'hidden',
        background: '#f9fafb',
      }}
    >
      <img
        src={src}
        alt={label}
        style={{ width: '100%', height: 90, objectFit: 'cover', display: 'block' }}
      />

      {/* Primary star badge */}
      <button
        type="button"
        title={isPrimary ? 'Ảnh chính' : 'Đặt làm ảnh chính'}
        onClick={onSetPrimary}
        style={{
          position: 'absolute', top: 4, left: 4,
          background: isPrimary ? '#3b82f6' : 'rgba(0,0,0,.35)',
          border: 'none', borderRadius: '50%',
          width: 24, height: 24, padding: 0,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          cursor: 'pointer', color: '#fff', fontSize: 11,
        }}
      >
        <i className={isPrimary ? 'fas fa-star' : 'far fa-star'} />
      </button>

      {/* Remove button */}
      <button
        type="button"
        title="Xóa ảnh"
        onClick={onRemove}
        style={{
          position: 'absolute', top: 4, right: 4,
          background: 'rgba(0,0,0,.45)',
          border: 'none', borderRadius: '50%',
          width: 24, height: 24, padding: 0,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          cursor: 'pointer', color: '#fff', fontSize: 11,
        }}
      >
        <i className="fas fa-times" />
      </button>

      {/* Label strip */}
      {isPrimary && (
        <div style={{
          position: 'absolute', bottom: 0, left: 0, right: 0,
          background: '#3b82f6', color: '#fff',
          fontSize: 10, textAlign: 'center', padding: '2px 0',
        }}>
          Ảnh chính
        </div>
      )}
    </div>
  )
}

// ── main component ─────────────────────────────────────────────────────────────
export default function AdminProductFormPage() {
  const { id }   = useParams()
  const isEdit   = !!id
  const navigate = useNavigate()

  // ── remote data ────────────────────────────────────────────────────────────
  const { data: categoriesRaw = [], isLoading: catsLoading } = useCategories()
  const { data: product, isLoading: prodLoading }            = useAdminProduct(id)

  const createMutation = useCreateProduct()
  const updateMutation = useUpdateProduct()
  const isPending      = createMutation.isPending || updateMutation.isPending

  // ── form fields ────────────────────────────────────────────────────────────
  const [form, setForm] = useState(EMPTY_FORM)

  // Existing DB images: { imgId, imgName (URL), isPrimary }
  const [existingImages, setExistingImages] = useState([])
  // IDs of existing images marked for deletion
  const [deleteIds, setDeleteIds]           = useState([])

  // New local files: [{ file: File, previewUrl: string }]
  const [newFiles, setNewFiles]             = useState([])

  // Primary selection — { source: 'existing' | 'new', ref: imgId | newIndex }
  const [primary, setPrimary]               = useState(null)

  const fileRef = useRef(null)

  // Populate on edit
  useEffect(() => {
    if (isEdit && product) {
      setForm({
        productName:        product.productName        ?? '',
        idCategory:         product.idCategory         ?? '',
        productPrice:       product.productPrice       ?? '',
        quantityInStock:    product.quantityInStock    ?? '',
        productDescription: product.productDescription ?? '',
        isOnSale:           product.isOnSale           ?? true,
      })

      const imgs = product.images ?? []
      setExistingImages(imgs)

      const primaryImg = imgs.find((i) => i.isPrimary)
      if (primaryImg) {
        setPrimary({ source: 'existing', ref: primaryImg.imgId })
      }
    }
  }, [isEdit, product])

  // ── handlers ──────────────────────────────────────────────────────────────
  function handleChange(e) {
    const { name, value, type, checked } = e.target
    setForm((prev) => ({ ...prev, [name]: type === 'checkbox' ? checked : value }))
  }

  function handleFilesSelected(e) {
    const files = Array.from(e.target.files ?? [])
    if (!files.length) return

    const newEntries = files.map((file) => ({
      file,
      previewUrl: URL.createObjectURL(file),
    }))

    setNewFiles((prev) => {
      const merged = [...prev, ...newEntries]
      // If no primary set yet (create mode) auto-select first file
      if (!primary && merged.length > 0) {
        setPrimary({ source: 'new', ref: 0 })
      }
      return merged
    })

    // reset the input so the same file can be re-selected
    e.target.value = ''
  }

  function removeExistingImage(imgId) {
    setExistingImages((prev) => prev.filter((i) => i.imgId !== imgId))
    setDeleteIds((prev) => [...prev, imgId])
    // If primary was this image, clear it
    if (primary?.source === 'existing' && primary.ref === imgId) {
      setPrimary(null)
    }
  }

  function removeNewFile(index) {
    URL.revokeObjectURL(newFiles[index].previewUrl)
    setNewFiles((prev) => {
      const next = prev.filter((_, i) => i !== index)
      return next
    })
    // Adjust primary ref if it was a new file
    if (primary?.source === 'new') {
      if (primary.ref === index) setPrimary(null)
      else if (primary.ref > index) setPrimary({ source: 'new', ref: primary.ref - 1 })
    }
  }

  function isPrimaryItem(source, ref) {
    return primary?.source === source && primary?.ref === ref
  }

  // ── submit ─────────────────────────────────────────────────────────────────
  async function handleSubmit(e) {
    e.preventDefault()

    const fd = new FormData()
    fd.append('ProductName',        form.productName)
    fd.append('IdCategory',         form.idCategory)
    fd.append('ProductPrice',       form.productPrice)
    fd.append('QuantityInStock',    form.quantityInStock)
    fd.append('ProductDescription', form.productDescription)
    fd.append('IsOnSale',           String(form.isOnSale))

    if (isEdit) {
      // Delete IDs
      deleteIds.forEach((id) => fd.append('DeleteImageIds', id))

      // Promote an existing image to primary (no file upload needed)
      if (primary?.source === 'existing') {
        fd.append('ExistingPrimaryImageId', primary.ref)
      }

      // Split new files into NewPrimaryImage vs AdditionalImages.
      // NewPrimaryImage (new file) takes precedence over ExistingPrimaryImageId on the backend.
      newFiles.forEach((entry, idx) => {
        const fieldName =
          primary?.source === 'new' && primary.ref === idx
            ? 'NewPrimaryImage'
            : 'AdditionalImages'
        fd.append(fieldName, entry.file)
      })
    } else {
      // Create: first file matching primary goes as PrimaryImage, rest as AdditionalImages
      newFiles.forEach((entry, idx) => {
        const fieldName =
          primary?.source === 'new' && primary.ref === idx
            ? 'PrimaryImage'
            : 'AdditionalImages'
        fd.append(fieldName, entry.file)
      })
    }

    try {
      if (isEdit) {
        await updateMutation.mutateAsync({ id, formData: fd })
      } else {
        await createMutation.mutateAsync(fd)
      }
      navigate('/admin/products')
    } catch { /* error shown below */ }
  }

  // ── derived UI state ───────────────────────────────────────────────────────
  const pageLoading  = (isEdit && prodLoading) || catsLoading
  const serverError  =
    createMutation.error?.response?.data?.message ??
    updateMutation.error?.response?.data?.message ??
    ((createMutation.isError || updateMutation.isError) ? 'Có lỗi xảy ra. Vui lòng thử lại.' : null)

  const totalImages = existingImages.length + newFiles.length

  return (
    <>
      {/* ── Content header ──────────────────────────────────────────────── */}
      <div className="app-content-header">
        <div className="container-fluid">
          <div className="row">
            <div className="col-sm-6">
              <h3 className="mb-0">{isEdit ? 'Sửa sản phẩm' : 'Thêm sản phẩm'}</h3>
            </div>
            <div className="col-sm-6">
              <ol className="breadcrumb float-sm-end">
                <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
                <li className="breadcrumb-item"><Link to="/admin/products">Sản phẩm</Link></li>
                <li className="breadcrumb-item active">{isEdit ? 'Sửa thông tin' : 'Thêm mới'}</li>
              </ol>
            </div>
          </div>
        </div>
      </div>

      {/* ── Content body ────────────────────────────────────────────────── */}
      <div className="app-content">
        <div className="container-fluid">
          <div className="row justify-content-center">
            <div className="col-lg-8 col-12">

              <div className="card shadow-sm">
                <div className="card-header border-0">
                  <h3 className="card-title fw-semibold mb-0">
                    <i className={`fas ${isEdit ? 'fa-edit' : 'fa-plus'} me-2 text-primary`} />
                    Thông tin sản phẩm
                  </h3>
                </div>

                <div className="card-body">
                  {pageLoading ? (
                    <div className="d-flex justify-content-center py-5">
                      <div className="spinner-border text-primary" role="status" />
                    </div>
                  ) : (
                    <form onSubmit={handleSubmit} encType="multipart/form-data">

                      {serverError && (
                        <div className="alert alert-danger rounded mb-3">{serverError}</div>
                      )}

                      {/* ── Product name ──────────────────────────────── */}
                      <div className="mb-3">
                        <label className="form-label fw-semibold" htmlFor="productName">
                          Tên sản phẩm <span className="text-danger">*</span>
                        </label>
                        <input
                          id="productName" name="productName" type="text"
                          className="form-control"
                          value={form.productName} onChange={handleChange}
                          required placeholder="Nhập tên sản phẩm..."
                        />
                      </div>

                      {/* ── Category (optgroup tree) ───────────────────── */}
                      <div className="mb-3">
                        <label className="form-label fw-semibold" htmlFor="idCategory">
                          Danh mục <span className="text-danger">*</span>
                        </label>
                        <select
                          id="idCategory" name="idCategory"
                          className="form-select"
                          value={form.idCategory} onChange={handleChange}
                          required
                        >
                          <option value="">— Chọn danh mục —</option>
                          <CategoryOptions categories={categoriesRaw} />
                        </select>
                      </div>

                      {/* ── Price + Stock ─────────────────────────────── */}
                      <div className="row g-3 mb-3">
                        <div className="col-md-6">
                          <label className="form-label fw-semibold" htmlFor="productPrice">
                            Giá tiền (₫) <span className="text-danger">*</span>
                          </label>
                          <input
                            id="productPrice" name="productPrice" type="number"
                            min="0" step="1000" className="form-control"
                            value={form.productPrice} onChange={handleChange}
                            required placeholder="250000"
                          />
                        </div>
                        <div className="col-md-6">
                          <label className="form-label fw-semibold" htmlFor="quantityInStock">
                            Số lượng trong kho <span className="text-danger">*</span>
                          </label>
                          <input
                            id="quantityInStock" name="quantityInStock" type="number"
                            min="0" className="form-control"
                            value={form.quantityInStock} onChange={handleChange}
                            required
                          />
                        </div>
                      </div>

                      {/* ── Is on sale (edit only) ────────────────────── */}
                      {isEdit && (
                        <div className="mb-3">
                          <label className="form-label fw-semibold" htmlFor="isOnSale">
                            Trạng thái bán
                          </label>
                          <select
                            id="isOnSale" name="isOnSale"
                            className="form-select"
                            value={String(form.isOnSale)}
                            onChange={(e) =>
                              setForm((prev) => ({ ...prev, isOnSale: e.target.value === 'true' }))
                            }
                          >
                            <option value="true">Đang mở bán</option>
                            <option value="false">Ẩn sản phẩm</option>
                          </select>
                        </div>
                      )}

                      {/* ── Description ──────────────────────────────── */}
                      <div className="mb-3">
                        <label className="form-label fw-semibold" htmlFor="productDescription">
                          Mô tả sản phẩm
                        </label>
                        <textarea
                          id="productDescription" name="productDescription"
                          className="form-control" rows={6}
                          value={form.productDescription} onChange={handleChange}
                          placeholder="Nhập mô tả sản phẩm..."
                        />
                      </div>

                      {/* ── Images ───────────────────────────────────── */}
                      <div className="mb-4">
                        <label className="form-label fw-semibold">
                          Hình ảnh sản phẩm
                          {!isEdit && <span className="text-danger"> *</span>}
                        </label>

                        {/* Instruction */}
                        <div className="form-text mb-2">
                          <i className="fas fa-star text-primary me-1" style={{ fontSize: 11 }} />
                          Nhấn vào ngôi sao để đặt làm ảnh chính. Nhấn <strong>×</strong> để xóa ảnh.
                        </div>

                        {/* Preview grid */}
                        {totalImages > 0 && (
                          <div
                            className="d-flex flex-wrap gap-2 mb-3 p-2 rounded"
                            style={{ background: '#f8f9fa', border: '1px solid #e5e7eb' }}
                          >
                            {/* Existing DB images */}
                            {existingImages.map((img) => (
                              <ImagePreviewCard
                                key={`ex-${img.imgId}`}
                                src={img.imgName}
                                isPrimary={isPrimaryItem('existing', img.imgId)}
                                onSetPrimary={() => setPrimary({ source: 'existing', ref: img.imgId })}
                                onRemove={() => removeExistingImage(img.imgId)}
                                label={`img-${img.imgId}`}
                              />
                            ))}

                            {/* New local files */}
                            {newFiles.map((entry, idx) => (
                              <ImagePreviewCard
                                key={`new-${idx}`}
                                src={entry.previewUrl}
                                isPrimary={isPrimaryItem('new', idx)}
                                onSetPrimary={() => setPrimary({ source: 'new', ref: idx })}
                                onRemove={() => removeNewFile(idx)}
                                label={entry.file.name}
                              />
                            ))}
                          </div>
                        )}

                        {/* File picker */}
                        <input
                          ref={fileRef}
                          type="file"
                          accept="image/*"
                          multiple
                          className="form-control"
                          onChange={handleFilesSelected}
                          required={!isEdit && totalImages === 0}
                        />
                        <div className="form-text">
                          {isEdit
                            ? 'Chọn thêm ảnh mới hoặc xóa ảnh cũ ở trên. Ảnh chính hiện tại được giữ nguyên nếu không thay đổi.'
                            : 'Chọn một hoặc nhiều ảnh (JPG / PNG / WEBP). Đặt ảnh chính bằng biểu tượng ngôi sao.'}
                        </div>

                        {/* Validation hint when no primary selected */}
                        {totalImages > 0 && !primary && (
                          <div className="alert alert-warning py-2 mt-2 rounded" style={{ fontSize: 13 }}>
                            <i className="fas fa-exclamation-triangle me-1" />
                            Vui lòng chọn một ảnh làm ảnh chính (nhấn biểu tượng ngôi sao).
                          </div>
                        )}
                      </div>

                      {/* ── Actions ──────────────────────────────────── */}
                      <div className="d-flex gap-2">
                        <button
                          type="submit"
                          className="btn btn-primary"
                          disabled={isPending || (totalImages > 0 && !primary)}
                        >
                          {isPending ? (
                            <>
                              <span className="spinner-border spinner-border-sm me-2" role="status" />
                              {isEdit ? 'Đang lưu...' : 'Đang thêm...'}
                            </>
                          ) : (
                            <>
                              <i className={`fas ${isEdit ? 'fa-save' : 'fa-plus'} me-1`} />
                              {isEdit ? 'Lưu thay đổi' : 'Thêm sản phẩm'}
                            </>
                          )}
                        </button>
                        <Link to="/admin/products" className="btn btn-outline-secondary">
                          Hủy
                        </Link>
                      </div>

                    </form>
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
