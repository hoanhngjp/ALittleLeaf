import { useState, useRef } from 'react'
import toast from 'react-hot-toast'
import {
  useAdminBanners,
  useCreateBanner,
  useUpdateBanner,
  useDeleteBanner,
} from '../../hooks/useBanners'

// ── Helpers ───────────────────────────────────────────────────────────────────

const ACCEPTED_TYPES = ['image/jpeg', 'image/png', 'image/webp']
const MAX_MB         = 5

function validateFile(file) {
  if (!file) return 'Vui lòng chọn file ảnh.'
  if (!ACCEPTED_TYPES.includes(file.type)) return 'Chỉ chấp nhận: JPG, PNG, WebP.'
  if (file.size > MAX_MB * 1024 * 1024) return `Dung lượng tối đa là ${MAX_MB} MB.`
  return null
}

// ── Upload Form ───────────────────────────────────────────────────────────────

function UploadForm() {
  const createBanner = useCreateBanner()
  const fileRef      = useRef(null)

  const [preview,      setPreview]      = useState(null)
  const [targetUrl,    setTargetUrl]    = useState('')
  const [displayOrder, setDisplayOrder] = useState(0)
  const [fileError,    setFileError]    = useState(null)

  function handleFileChange(e) {
    const file = e.target.files?.[0] ?? null
    const err  = validateFile(file)
    setFileError(err)
    setPreview(err || !file ? null : URL.createObjectURL(file))
  }

  async function handleSubmit(e) {
    e.preventDefault()
    const file = fileRef.current?.files?.[0] ?? null
    const err  = validateFile(file)
    if (err) { setFileError(err); return }

    const fd = new FormData()
    fd.append('imageFile',    file)
    fd.append('targetUrl',    targetUrl)
    fd.append('displayOrder', displayOrder)

    await toast.promise(createBanner.mutateAsync(fd), {
      loading: 'Đang tải ảnh lên...',
      success: 'Banner đã được thêm!',
      error:   (err) => err?.response?.data?.error ?? 'Thêm banner thất bại.',
    })

    // Reset form
    e.target.reset()
    setPreview(null)
    setTargetUrl('')
    setDisplayOrder(0)
    setFileError(null)
  }

  return (
    <div className="card shadow-sm mb-4">
      <div className="card-header bg-success text-white">
        <h5 className="mb-0"><i className="fas fa-plus-circle me-2" />Thêm Banner Mới</h5>
      </div>
      <div className="card-body">
        <form onSubmit={handleSubmit}>
          <div className="row g-3 align-items-start">

            {/* File picker */}
            <div className="col-md-5">
              <label className="form-label fw-semibold">Ảnh Banner <span className="text-danger">*</span></label>
              <input
                ref={fileRef}
                type="file"
                accept="image/jpeg,image/png,image/webp"
                className={`form-control${fileError ? ' is-invalid' : ''}`}
                onChange={handleFileChange}
              />
              {fileError && <div className="invalid-feedback">{fileError}</div>}
              <small className="text-muted">JPG / PNG / WebP, tối đa 5 MB</small>

              {preview && (
                <img
                  src={preview}
                  alt="Preview"
                  className="mt-2 rounded border"
                  style={{ width: '100%', maxHeight: 140, objectFit: 'cover' }}
                />
              )}
            </div>

            {/* Target URL */}
            <div className="col-md-4">
              <label className="form-label fw-semibold">Link khi click (tuỳ chọn)</label>
              <input
                type="url"
                className="form-control"
                placeholder="https://..."
                value={targetUrl}
                onChange={(e) => setTargetUrl(e.target.value)}
              />
            </div>

            {/* Display order */}
            <div className="col-md-2">
              <label className="form-label fw-semibold">Thứ tự</label>
              <input
                type="number"
                className="form-control"
                min={0}
                value={displayOrder}
                onChange={(e) => setDisplayOrder(Number(e.target.value))}
              />
            </div>

            {/* Submit */}
            <div className="col-md-1 d-flex align-items-end">
              <button
                type="submit"
                className="btn btn-success w-100"
                disabled={createBanner.isPending}
              >
                {createBanner.isPending
                  ? <span className="spinner-border spinner-border-sm" />
                  : <i className="fas fa-upload" />}
              </button>
            </div>

          </div>
        </form>
      </div>
    </div>
  )
}

// ── Banner Row ────────────────────────────────────────────────────────────────

function BannerRow({ banner }) {
  const updateBanner = useUpdateBanner()
  const deleteBanner = useDeleteBanner()

  const [order,     setOrder]     = useState(banner.displayOrder)
  const [targetUrl, setTargetUrl] = useState(banner.targetUrl ?? '')

  function handleToggleActive() {
    toast.promise(
      updateBanner.mutateAsync({ id: banner.bannerId, isActive: !banner.isActive }),
      {
        loading: 'Đang cập nhật...',
        success: `Banner đã ${!banner.isActive ? 'bật' : 'tắt'}.`,
        error:   'Cập nhật thất bại.',
      }
    )
  }

  function handleOrderBlur() {
    if (order === banner.displayOrder) return
    toast.promise(
      updateBanner.mutateAsync({ id: banner.bannerId, displayOrder: order }),
      {
        loading: 'Đang lưu...',
        success: 'Thứ tự đã cập nhật.',
        error:   'Cập nhật thất bại.',
      }
    )
  }

  function handleTargetUrlBlur() {
    if (targetUrl === (banner.targetUrl ?? '')) return
    toast.promise(
      updateBanner.mutateAsync({ id: banner.bannerId, targetUrl: targetUrl || null }),
      {
        loading: 'Đang lưu...',
        success: 'Link đã cập nhật.',
        error:   'Cập nhật thất bại.',
      }
    )
  }

  function handleDelete() {
    if (!window.confirm('Xoá banner này? Ảnh cũng sẽ bị xoá khỏi Cloudinary.')) return
    toast.promise(
      deleteBanner.mutateAsync(banner.bannerId),
      {
        loading: 'Đang xoá...',
        success: 'Banner đã xoá.',
        error:   'Xoá thất bại.',
      }
    )
  }

  const isBusy = updateBanner.isPending || deleteBanner.isPending

  return (
    <tr style={{ opacity: isBusy ? 0.6 : 1 }}>
      {/* Thumbnail */}
      <td style={{ width: 120 }}>
        <img
          src={banner.imageUrl}
          alt={`Banner ${banner.bannerId}`}
          className="rounded border"
          style={{ width: 110, height: 60, objectFit: 'cover' }}
        />
      </td>

      {/* Display order */}
      <td style={{ width: 90 }}>
        <input
          type="number"
          className="form-control form-control-sm"
          min={0}
          value={order}
          onChange={(e) => setOrder(Number(e.target.value))}
          onBlur={handleOrderBlur}
          disabled={isBusy}
        />
      </td>

      {/* Target URL */}
      <td>
        <input
          type="url"
          className="form-control form-control-sm"
          placeholder="Không có link"
          value={targetUrl}
          onChange={(e) => setTargetUrl(e.target.value)}
          onBlur={handleTargetUrlBlur}
          disabled={isBusy}
        />
      </td>

      {/* Active toggle */}
      <td className="text-center" style={{ width: 90 }}>
        <div className="form-check form-switch d-flex justify-content-center">
          <input
            className="form-check-input"
            type="checkbox"
            role="switch"
            checked={banner.isActive}
            onChange={handleToggleActive}
            disabled={isBusy}
          />
        </div>
      </td>

      {/* Delete */}
      <td className="text-center" style={{ width: 80 }}>
        <button
          className="btn btn-sm btn-outline-danger"
          onClick={handleDelete}
          disabled={isBusy}
          title="Xoá banner"
        >
          <i className="fas fa-trash" />
        </button>
      </td>
    </tr>
  )
}

// ── Page ──────────────────────────────────────────────────────────────────────

export default function AdminBannersPage() {
  const { data: banners, isLoading, isError } = useAdminBanners()

  return (
    <div className="container-fluid py-3">
      <div className="d-flex align-items-center mb-3">
        <i className="fas fa-images fa-lg me-2 text-success" />
        <h4 className="mb-0">Quản lý Banner</h4>
      </div>

      <UploadForm />

      <div className="card shadow-sm">
        <div className="card-header">
          <h5 className="mb-0">Danh sách Banner</h5>
        </div>
        <div className="card-body p-0">
          {isLoading && (
            <div className="text-center py-5">
              <div className="spinner-border text-success" />
            </div>
          )}

          {isError && (
            <div className="alert alert-danger m-3">Không thể tải danh sách banner.</div>
          )}

          {!isLoading && !isError && (
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0">
                <thead className="table-light">
                  <tr>
                    <th>Ảnh</th>
                    <th>Thứ tự</th>
                    <th>Link khi click</th>
                    <th className="text-center">Hiển thị</th>
                    <th className="text-center">Xoá</th>
                  </tr>
                </thead>
                <tbody>
                  {banners?.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="text-center text-muted py-4">
                        Chưa có banner nào. Hãy thêm banner đầu tiên!
                      </td>
                    </tr>
                  ) : (
                    banners?.map((b) => <BannerRow key={b.bannerId} banner={b} />)
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
