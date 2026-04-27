import { useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { useRegister } from '../hooks/useAuth'
import Header  from '../components/layout/Header'
import Footer  from '../components/layout/Footer'
import Sidebar from '../components/Sidebar'
// Note: MobileSearchBar is intentionally NOT imported here.

// ── Validation helpers (ported from validLoginAndRegistry.js) ──────────────

function validateEmail(email) {
  return /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email)
}

function validateBirthday(isoDate) {
  if (!isoDate) return false
  // Browser date input gives YYYY-MM-DD; convert to MM/DD/YYYY for pattern check
  const [year, month, day] = isoDate.split('-')
  const mmddyyyy = `${month}/${day}/${year}`
  if (!/^(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])\/\d{4}$/.test(mmddyyyy)) return false
  const date = new Date(Number(year), Number(month) - 1, Number(day))
  return (
    date.getFullYear() === Number(year) &&
    date.getMonth()    === Number(month) - 1 &&
    date.getDate()     === Number(day)
  )
}

function checkStrongPass(password) {
  if (password.length < 8)          return '*Mật khẩu phải dài hơn 8 ký tự'
  if (!/[A-Z]/.test(password))      return '*Mật khẩu phải có chữ ký tự viết hoa'
  if (!/[a-z]/.test(password))      return '*Mật khẩu phải có chữ ký tự viết thường'
  if (!/\d/.test(password))         return '*Mật khẩu phải có ít nhất 1 chữ số'
  if (!/[@$!%*?&]/.test(password))  return '*Mật khẩu phải có ít nhất 1 ký tự đặc biệt'
  return ''
}

// ── Initial state ──────────────────────────────────────────────────────────

const INITIAL = {
  fullName: '',
  sex:      true,   // true = Nam, false = Nữ (matches RegisterRequestDto)
  birthday: '',
  email:    '',
  password: '',
}

// ── Component ──────────────────────────────────────────────────────────────

export default function RegisterPage() {
  const [form,   setForm]   = useState(INITIAL)
  const [errors, setErrors] = useState({})
  const { mutate: register, isPending, error } = useRegister()

  const fullNameRef = useRef(null)
  const birthdayRef = useRef(null)
  const emailRef    = useRef(null)
  const passwordRef = useRef(null)

  const onChange = (e) => {
    const { name, value, type } = e.target
    setForm((f) => ({
      ...f,
      [name]: type === 'radio' ? value === 'true' : value,
    }))
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }))
  }

  const validate = () => {
    const next = {}

    if (!form.fullName.trim()) {
      next.fullName = '*Vui lòng nhập Họ tên của bạn'
    }
    if (!form.birthday) {
      next.birthday = '*Vui lòng nhập Ngày sinh của bạn'
    } else if (!validateBirthday(form.birthday)) {
      next.birthday = '*Ngày sinh không hợp lệ'
    }
    if (!form.email.trim()) {
      next.email = '*Vui lòng nhập Email của bạn'
    } else if (!validateEmail(form.email)) {
      next.email = '*Email không đúng định dạng'
    }
    const passError = checkStrongPass(form.password)
    if (passError) {
      next.password = passError
    }

    return next
  }

  const onSubmit = (e) => {
    e.preventDefault()
    const next = validate()
    if (Object.keys(next).length) {
      setErrors(next)
      // Focus the first invalid field in form order
      if      (next.fullName) fullNameRef.current?.focus()
      else if (next.birthday) birthdayRef.current?.focus()
      else if (next.email)    emailRef.current?.focus()
      else if (next.password) passwordRef.current?.focus()
      return
    }
    register(form)
  }

  const serverError = error?.response?.data?.message

  return (
    <div className="d-flex flex-column min-vh-100">
      <Header />
      <Sidebar />

      <div className="container-fluid auth-page-wrap" style={{ paddingTop: '70px' }}>
        <div className="row g-0" style={{ minHeight: 'calc(100vh - 70px)' }}>

          {/* Left — title (desktop only) */}
          <div className="col-lg-6 d-none d-lg-flex align-items-center justify-content-center border-end bg-white">
            <h1 className="auth-title">Tạo tài khoản</h1>
          </div>

          {/* Right — form: centered on desktop, top-aligned + padded on mobile */}
          <div className="col-12 col-lg-6 d-flex flex-column justify-content-lg-center align-items-center bg-white pb-5 mb-5">
            <div className="w-100 p-4 p-lg-5" style={{ maxWidth: '500px' }}>

              {/* Mobile title */}
              <h1 className="auth-title d-lg-none mt-4 mb-4 text-center" style={{ fontSize: '36px' }}>Tạo tài khoản</h1>

              {serverError && (
                <div className="alert alert-danger py-2 small mb-4">
                  {serverError}
                </div>
              )}

              <form onSubmit={onSubmit} noValidate>

                {/* 1. Họ và tên */}
                <div className="mb-4">
                  <label className="brand-label" htmlFor="fullname">Họ và tên*</label>
                  <input
                    ref={fullNameRef}
                    type="text"
                    name="fullName"
                    id="fullname"
                    value={form.fullName}
                    onChange={onChange}
                    placeholder="Họ và tên"
                    className={`form-control brand-input${errors.fullName ? ' error' : ''}`}
                  />
                  {errors.fullName && (
                    <div className="text-danger mt-1" style={{ fontSize: '13px' }}>{errors.fullName}</div>
                  )}
                </div>

                {/* 2. Giới tính */}
                <div className="mb-4">
                  <label className="brand-label">Giới tính*</label>
                  <div className="d-flex align-items-center gap-4">
                    {[
                      { value: 'true',  label: 'Nam' },
                      { value: 'false', label: 'Nữ'  },
                    ].map(({ value, label }) => (
                      <div key={value} className="form-check mb-0">
                        <input
                          className="form-check-input"
                          type="radio"
                          name="sex"
                          id={`sex-${value}`}
                          value={value}
                          checked={form.sex === (value === 'true')}
                          onChange={onChange}
                        />
                        <label className="form-check-label" htmlFor={`sex-${value}`}>{label}</label>
                      </div>
                    ))}
                  </div>
                </div>

                {/* 3. Ngày sinh */}
                <div className="mb-4">
                  <label className="brand-label" htmlFor="birthday">Ngày sinh*</label>
                  <input
                    ref={birthdayRef}
                    type="date"
                    name="birthday"
                    id="birthday"
                    value={form.birthday}
                    onChange={onChange}
                    className={`form-control brand-input${errors.birthday ? ' error' : ''}`}
                  />
                  {errors.birthday && (
                    <div className="text-danger mt-1" style={{ fontSize: '13px' }}>{errors.birthday}</div>
                  )}
                </div>

                {/* 4. Email */}
                <div className="mb-4">
                  <label className="brand-label" htmlFor="email">Email*</label>
                  <input
                    ref={emailRef}
                    type="email"
                    name="email"
                    id="email"
                    value={form.email}
                    onChange={onChange}
                    placeholder="Email"
                    className={`form-control brand-input${errors.email ? ' error' : ''}`}
                  />
                  {errors.email && (
                    <div className="text-danger mt-1" style={{ fontSize: '13px' }}>{errors.email}</div>
                  )}
                </div>

                {/* 5. Mật khẩu */}
                <div className="mb-4">
                  <label className="brand-label" htmlFor="password">Mật khẩu*</label>
                  <input
                    ref={passwordRef}
                    type="password"
                    name="password"
                    id="password"
                    value={form.password}
                    onChange={onChange}
                    placeholder="Mật khẩu"
                    className={`form-control brand-input${errors.password ? ' error' : ''}`}
                  />
                  {errors.password && (
                    <div className="text-danger mt-1" style={{ fontSize: '13px' }}>{errors.password}</div>
                  )}
                </div>

                <div className="mt-4">
                  <button type="submit" disabled={isPending} className="btn brand-btn mb-3">
                    {isPending && (
                      <span className="spinner-border spinner-border-sm me-2" role="status" />
                    )}
                    ĐĂNG KÝ
                  </button>
                  <div>
                    <Link
                      to="/login"
                      className="text-decoration-none text-dark fw-bold"
                      style={{ fontSize: '14px' }}
                    >
                      &larr; Quay lại đăng nhập
                    </Link>
                  </div>
                </div>

              </form>
            </div>
          </div>

        </div>
      </div>

      <Footer />
    </div>
  )
}
