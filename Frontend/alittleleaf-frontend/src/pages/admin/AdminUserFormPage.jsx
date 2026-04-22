import { useState, useEffect } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'
import { useAdminUser, useUpdateAdminUser, useCreateAdminUser } from '../../hooks/useAdminUsers'

// ── Validation ────────────────────────────────────────────────────────────────
// Rules ported from legacy validLoginAndRegistry.js

const EMAIL_RE = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/

function validateFields({ fullname, email, password, isCreate }) {
  const errs = {}

  if (!fullname.trim())
    errs.fullname = 'Vui lòng nhập họ và tên.'

  if (isCreate) {
    if (!email.trim())
      errs.email = 'Vui lòng nhập email.'
    else if (!EMAIL_RE.test(email))
      errs.email = 'Vui lòng nhập đúng định dạng email.'

    if (!password)
      errs.password = 'Vui lòng nhập mật khẩu.'
    else if (password.length < 8)
      errs.password = 'Mật khẩu phải dài hơn 8 ký tự.'
    else if (!/[A-Z]/.test(password))
      errs.password = 'Mật khẩu phải có ít nhất 1 chữ hoa.'
    else if (!/[a-z]/.test(password))
      errs.password = 'Mật khẩu phải có ít nhất 1 chữ thường.'
    else if (!/\d/.test(password))
      errs.password = 'Mật khẩu phải có ít nhất 1 chữ số.'
    else if (!/[@$!%*?&]/.test(password))
      errs.password = 'Mật khẩu phải có ít nhất 1 ký tự đặc biệt (@$!%*?&).'
  }

  return errs
}

// ── AdminUserFormPage ─────────────────────────────────────────────────────────

export default function AdminUserFormPage() {
  const { id }  = useParams()
  const isEdit  = !!id && id !== 'new'
  const navigate = useNavigate()

  const { data: user, isLoading, isError } = useAdminUser(isEdit ? Number(id) : null)
  const updateMutation = useUpdateAdminUser(isEdit ? Number(id) : null)
  const createMutation = useCreateAdminUser()

  const [fullname, setFullname] = useState('')
  const [email,    setEmail]    = useState('')
  const [password, setPassword] = useState('')
  const [sex,      setSex]      = useState('false')
  const [birthday, setBirthday] = useState('')
  const [isActive, setIsActive] = useState('true')
  const [role,     setRole]     = useState('customer')
  const [errors,   setErrors]   = useState({})

  useEffect(() => {
    if (isEdit && user) {
      setFullname(user.userFullname ?? '')
      setEmail(user.userEmail ?? '')
      setSex(user.userSex ? 'true' : 'false')
      setBirthday(user.userBirthday ? user.userBirthday.slice(0, 10) : '')
      setIsActive(user.userIsActive ? 'true' : 'false')
      setRole(user.userRole ?? 'customer')
    }
  }, [isEdit, user])

  // Clear individual error on change
  function clearError(field) {
    setErrors((prev) => {
      if (!prev[field]) return prev
      const next = { ...prev }
      delete next[field]
      return next
    })
  }

  // Blur-time validation for a single field
  function handleBlur(field) {
    const errs = validateFields({ fullname, email, password, isCreate: !isEdit })
    if (errs[field]) setErrors((prev) => ({ ...prev, [field]: errs[field] }))
  }

  function handleSubmit(e) {
    e.preventDefault()
    const errs = validateFields({ fullname, email, password, isCreate: !isEdit })
    if (Object.keys(errs).length > 0) {
      setErrors(errs)
      return
    }
    setErrors({})

    if (isEdit) {
      updateMutation.mutate(
        {
          userFullname: fullname,
          userSex:      sex === 'true',
          userBirthday: birthday || undefined,
          userIsActive: isActive === 'true',
          userRole:     role,
        },
        {
          onSuccess: () => {
            toast.success('Cập nhật người dùng thành công!')
            navigate('/admin/users')
          },
          onError: () => toast.error('Cập nhật thất bại. Vui lòng thử lại.'),
        }
      )
    } else {
      createMutation.mutate(
        {
          userEmail:    email,
          password,
          userFullname: fullname,
          userSex:      sex === 'true',
          userBirthday: birthday || undefined,
          userIsActive: isActive === 'true',
          userRole:     role,
        },
        {
          onSuccess: () => {
            toast.success('Tạo người dùng thành công!')
            navigate('/admin/users')
          },
          onError: (err) => {
            const msg = err?.response?.data?.error ?? 'Tạo người dùng thất bại.'
            toast.error(msg)
          },
        }
      )
    }
  }

  const isPending = isEdit ? updateMutation.isPending : createMutation.isPending

  if (isEdit && isLoading)
    return (
      <div className="app-content">
        <div className="container-fluid text-center py-5">
          <div className="spinner-border" />
        </div>
      </div>
    )

  if (isEdit && (isError || !user))
    return (
      <div className="app-content">
        <div className="container-fluid">
          <div className="alert alert-danger">
            Không tìm thấy người dùng #{id}.{' '}
            <Link to="/admin/users">Quay lại danh sách</Link>
          </div>
        </div>
      </div>
    )

  return (
    <div className="app-content-header">
      <div className="container-fluid">
        <div className="row mb-2">
          <div className="col-sm-6">
            <h1 className="m-0">
              {isEdit ? `Sửa người dùng #${id}` : 'Thêm người dùng'}
            </h1>
          </div>
          <div className="col-sm-6">
            <ol className="breadcrumb float-sm-end">
              <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
              <li className="breadcrumb-item"><Link to="/admin/users">Người dùng</Link></li>
              <li className="breadcrumb-item active">
                {isEdit ? `#${id}` : 'Thêm mới'}
              </li>
            </ol>
          </div>
        </div>
      </div>

      <div className="app-content">
        <div className="container-fluid">
          <div className="row justify-content-center">
            <div className="col-lg-6 col-md-8">
              <div className="card">
                <div className="card-header bg-primary text-white">
                  <h5 className="mb-0">
                    <i className={`bi ${isEdit ? 'bi-person-gear' : 'bi-person-plus-fill'} me-2`} />
                    {isEdit ? 'Thông tin người dùng' : 'Tạo tài khoản mới'}
                  </h5>
                </div>
                <div className="card-body">
                  <form onSubmit={handleSubmit} noValidate>

                    {/* Email */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">
                        Email {!isEdit && <span className="text-danger">*</span>}
                      </label>
                      <input
                        type="email"
                        className={`form-control${errors.email ? ' is-invalid' : ''}`}
                        value={email}
                        onChange={(e) => { setEmail(e.target.value); clearError('email') }}
                        onBlur={() => handleBlur('email')}
                        disabled={isEdit}
                        readOnly={isEdit}
                        required={!isEdit}
                        placeholder={isEdit ? '' : 'example@email.com'}
                      />
                      {errors.email
                        ? <div className="invalid-feedback">{errors.email}</div>
                        : isEdit && <small className="text-muted">Email không thể thay đổi.</small>
                      }
                    </div>

                    {/* Password — create mode only */}
                    {!isEdit && (
                      <div className="mb-3">
                        <label className="form-label fw-semibold">
                          Mật khẩu <span className="text-danger">*</span>
                        </label>
                        <input
                          type="password"
                          className={`form-control${errors.password ? ' is-invalid' : ''}`}
                          value={password}
                          onChange={(e) => { setPassword(e.target.value); clearError('password') }}
                          onBlur={() => handleBlur('password')}
                          required
                          placeholder="Tối thiểu 8 ký tự, gồm hoa, thường, số, ký tự đặc biệt"
                        />
                        {errors.password && (
                          <div className="invalid-feedback">{errors.password}</div>
                        )}
                      </div>
                    )}

                    {/* Fullname */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">
                        Họ và tên <span className="text-danger">*</span>
                      </label>
                      <input
                        type="text"
                        className={`form-control${errors.fullname ? ' is-invalid' : ''}`}
                        value={fullname}
                        onChange={(e) => { setFullname(e.target.value); clearError('fullname') }}
                        onBlur={() => handleBlur('fullname')}
                        required
                        placeholder="Nguyễn Văn A"
                      />
                      {errors.fullname && (
                        <div className="invalid-feedback">{errors.fullname}</div>
                      )}
                    </div>

                    {/* Sex */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">Giới tính</label>
                      <select
                        className="form-select"
                        value={sex}
                        onChange={(e) => setSex(e.target.value)}
                      >
                        <option value="false">Nam</option>
                        <option value="true">Nữ</option>
                      </select>
                    </div>

                    {/* Birthday */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">Ngày sinh</label>
                      <input
                        type="date"
                        className="form-control"
                        value={birthday}
                        onChange={(e) => setBirthday(e.target.value)}
                      />
                    </div>

                    {/* Role */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">Vai trò</label>
                      <select
                        className="form-select"
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                      >
                        <option value="customer">Khách hàng</option>
                        <option value="admin">Admin</option>
                      </select>
                    </div>

                    {/* Active status */}
                    <div className="mb-4">
                      <label className="form-label fw-semibold">Trạng thái tài khoản</label>
                      <select
                        className="form-select"
                        value={isActive}
                        onChange={(e) => setIsActive(e.target.value)}
                      >
                        <option value="true">Đang hoạt động</option>
                        <option value="false">Khóa tài khoản</option>
                      </select>
                    </div>

                    <div className="d-flex gap-2">
                      <button
                        type="submit"
                        className="btn btn-primary"
                        disabled={isPending}
                      >
                        {isPending
                          ? <><span className="spinner-border spinner-border-sm me-2" />Đang lưu…</>
                          : isEdit
                            ? <><i className="bi bi-check-circle me-2" />Cập nhật</>
                            : <><i className="bi bi-person-plus-fill me-2" />Tạo người dùng</>
                        }
                      </button>
                      <button
                        type="button"
                        className="btn btn-outline-secondary"
                        onClick={() => navigate('/admin/users')}
                      >
                        <i className="bi bi-arrow-left me-1" />Quay lại
                      </button>
                    </div>

                  </form>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
