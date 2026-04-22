import { useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { useLogin } from '../hooks/useAuth'
import Header  from '../components/layout/Header'
import Footer  from '../components/layout/Footer'
import Sidebar from '../components/Sidebar'
// Note: MobileSearchBar is intentionally NOT imported here.
// Auth pages don't need it and it would cause a double clearance bug.

function validateEmail(email) {
  return /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email)
}

export default function LoginPage() {
  const [form,   setForm]   = useState({ email: '', password: '' })
  const [errors, setErrors] = useState({})
  const { mutate: login, isPending, error } = useLogin()

  const emailRef    = useRef(null)
  const passwordRef = useRef(null)

  const onChange = (e) => {
    const { name, value } = e.target
    setForm((f) => ({ ...f, [name]: value }))
    // Clear the field error as soon as the user starts typing
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }))
  }

  const validate = () => {
    const next = {}
    if (!form.email.trim()) {
      next.email = '*Vui lòng nhập Email của bạn'
    } else if (!validateEmail(form.email)) {
      next.email = '*Email không đúng định dạng'
    }
    if (!form.password.trim()) {
      next.password = '*Vui lòng nhập mật khẩu của bạn'
    }
    return next
  }

  const onSubmit = (e) => {
    e.preventDefault()
    const next = validate()
    if (Object.keys(next).length) {
      setErrors(next)
      // Focus the first invalid field
      if (next.email)    emailRef.current?.focus()
      else if (next.password) passwordRef.current?.focus()
      return
    }
    login(form)
  }

  const serverError = error?.response?.data?.message ?? (error ? 'Email hoặc mật khẩu không đúng.' : null)

  return (
    <div className="d-flex flex-column min-vh-100">
      <Header />
      <Sidebar />

      <div className="container-fluid auth-page-wrap" style={{ paddingTop: '70px' }}>
        <div className="row g-0" style={{ minHeight: 'calc(100vh - 70px)' }}>

          {/* Left — title (desktop only) */}
          <div className="col-lg-6 d-none d-lg-flex align-items-center justify-content-center border-end bg-white">
            <h1 className="auth-title">Đăng nhập</h1>
          </div>

          {/* Right — form: centered on desktop, top-aligned + padded on mobile */}
          <div className="col-12 col-lg-6 d-flex flex-column justify-content-lg-center align-items-center bg-white pb-5 mb-5">
            <div className="w-100 p-4 p-lg-5" style={{ maxWidth: '500px' }}>

              {/* Mobile title */}
              <h1 className="auth-title d-lg-none mt-4 mb-4 text-center" style={{ fontSize: '36px' }}>Đăng nhập</h1>

              {serverError && (
                <div className="alert alert-danger py-2 small mb-4">
                  {serverError}
                </div>
              )}

              <form onSubmit={onSubmit} noValidate>

                {/* Email */}
                <div className="mb-4">
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

                {/* Password */}
                <div className="mb-4">
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

                <div className="d-flex align-items-center gap-4 mt-4">
                  <button type="submit" disabled={isPending} className="btn brand-btn">
                    {isPending && (
                      <span className="spinner-border spinner-border-sm me-2" role="status" />
                    )}
                    ĐĂNG NHẬP
                  </button>

                  <div className="d-flex flex-column">
                    <a href="#" className="text-decoration-none text-dark fw-bold mb-1" style={{ fontSize: '14px' }}>
                      Quên mật khẩu?
                    </a>
                    <span className="text-muted" style={{ fontSize: '14px' }}>
                      Hoặc: <Link to="/register" className="text-decoration-none text-dark fw-bold">Đăng ký</Link>
                    </span>
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
