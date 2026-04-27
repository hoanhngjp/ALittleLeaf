import { useState, useEffect } from 'react'
import { useNavigate, useLocation, Link } from 'react-router-dom'
import { useAddresses } from '../hooks/useOrders'
import { useProvinces, useDistricts, useWards } from '../hooks/useShipping'
import CheckoutLayout from '../components/checkout/CheckoutLayout'

export default function CheckoutPage() {
  const navigate = useNavigate()
  const location = useLocation()

  // Note passed from CartPage via <Link state={{ note }}>
  const noteFromCart = location.state?.note ?? ''

  // ── Saved addresses ────────────────────────────────────────────────────────
  const { data: addresses = [] } = useAddresses()

  // ── Form state ─────────────────────────────────────────────────────────────
  const [selectedAdrsId, setSelectedAdrsId] = useState('add')
  const [fullName,    setFullName]    = useState('')
  const [phone,       setPhone]       = useState('')
  const [address,     setAddress]     = useState('')
  const [provinceId,  setProvinceId]  = useState(null)
  const [districtId,  setDistrictId]  = useState(null)
  const [wardCode,    setWardCode]    = useState('')
  const [errors,      setErrors]      = useState({})

  // ── GHN master data ────────────────────────────────────────────────────────
  const { data: provinces = [] } = useProvinces()
  const { data: districts = [] } = useDistricts(provinceId)
  const { data: wards     = [] } = useWards(districtId)

  // Pre-fill default saved address on load
  useEffect(() => {
    if (addresses.length > 0) {
      const def = addresses.find((a) => a.adrsIsDefault) ?? addresses[0]
      setSelectedAdrsId(String(def.adrsId))
      setFullName(def.adrsFullname)
      setPhone(def.adrsPhone)
      setAddress(def.adrsAddress)
      setProvinceId(def.provinceId ?? null)
      setDistrictId(def.districtId ?? null)
      setWardCode(def.wardCode ?? '')
    }
  }, [addresses])

  // ── Derived: is the form currently valid? (used by CheckoutLayout) ─────────
  const isNewAddress = selectedAdrsId === 'add'
  const isFormValid =
    fullName.trim().length > 0 &&
    phone.trim().length > 0 &&
    address.trim().length > 0 &&
    (!isNewAddress || (!!provinceId && !!districtId && !!wardCode))

  // ── Handle saved address selection ────────────────────────────────────────
  const handleSelectAddress = (e) => {
    const val = e.target.value
    setSelectedAdrsId(val)
    if (val === 'add') {
      setFullName(''); setPhone(''); setAddress('')
      setProvinceId(null); setDistrictId(null); setWardCode('')
    } else {
      const found = addresses.find((a) => String(a.adrsId) === val)
      if (found) {
        setFullName(found.adrsFullname)
        setPhone(found.adrsPhone)
        setAddress(found.adrsAddress)
        setProvinceId(found.provinceId ?? null)
        setDistrictId(found.districtId ?? null)
        setWardCode(found.wardCode ?? '')
      }
    }
    setErrors({})
  }

  // ── Validation ─────────────────────────────────────────────────────────────
  const validate = () => {
    const errs = {}
    if (!fullName.trim()) errs.fullName = 'Vui lòng không bỏ trống Họ tên'
    if (!phone.trim())    errs.phone    = 'Vui lòng không bỏ trống Số điện thoại'
    if (!address.trim())  errs.address  = 'Vui lòng không bỏ trống số nhà, tên đường'
    if (isNewAddress && !provinceId)   errs.province = 'Vui lòng chọn Tỉnh/Thành phố'
    if (isNewAddress && !districtId)   errs.district = 'Vui lòng chọn Quận/Huyện'
    if (isNewAddress && !wardCode)     errs.ward     = 'Vui lòng chọn Phường/Xã'
    return errs
  }

  // ── Submit: validate then navigate to payment step ────────────────────────
  const handleSubmit = (e) => {
    e.preventDefault()
    const errs = validate()
    if (Object.keys(errs).length > 0) {
      setErrors(errs)
      return
    }
    navigate('/checkout/payment', {
      state: {
        note:           noteFromCart,
        addressId:      selectedAdrsId !== 'add' ? Number(selectedAdrsId) : null,
        newFullName:    fullName,
        newPhone:       phone,
        newAddress:     address,
        newProvinceId:  selectedAdrsId === 'add' ? provinceId : null,
        newDistrictId:  selectedAdrsId === 'add' ? districtId : null,
        newWardCode:    selectedAdrsId === 'add' ? wardCode   : null,
        // Always forwarded for fee calculation — districtId/wardCode are populated
        // from the saved address form state even when addressId is set.
        feeDistrictId:  districtId,
        feeWardCode:    wardCode,
      },
    })
  }

  return (
    <CheckoutLayout step={1}>
      <div className="step">
        <div className="step-actions">
          <div id="section-shipping-rate" className="section">

            <div className="section-header">
              <h2 className="section-title">Thông tin giao hàng</h2>
            </div>

            <div className="section-content section-customer-information">

              <div className="fieldset">

                {/* Saved address selector — only when the user has saved addresses */}
                {addresses.length > 0 && (
                  <div className="field">
                    <div className="field-input-wrapper field-input-wrapper-select">
                      <select
                        id="stored_addresses"
                        className="field-input"
                        value={selectedAdrsId}
                        onChange={handleSelectAddress}
                      >
                        <option value="add">Thêm địa chỉ mới ...</option>
                        <option value="" disabled>Địa chỉ đã lưu trữ</option>
                        {addresses.map((a) => (
                          <option key={a.adrsId} value={String(a.adrsId)}>
                            {a.adrsAddress}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                )}

                {/* Shipping info form */}
                <form id="checkout_complete" onSubmit={handleSubmit} noValidate>

                  {/* Full name */}
                  <div className="field field-required field-show-floating-label">
                    <div className="field-input-wrapper">
                      <label htmlFor="billing_address_full_name" className="field-label">
                        Họ và Tên
                      </label>
                      <input
                        type="text"
                        placeholder="Họ và tên"
                        className="field-input"
                        id="billing_address_full_name"
                        name="billing_address_full_name"
                        value={fullName}
                        onChange={(e) => { setFullName(e.target.value); setErrors((p) => ({ ...p, fullName: '' })) }}
                        style={errors.fullName ? { borderColor: 'red' } : {}}
                      />
                      {errors.fullName && (
                        <p className="error-show" style={{ display: 'block', color: 'red', marginTop: '4px' }}>
                          {errors.fullName}
                        </p>
                      )}
                    </div>
                  </div>

                  {/* Phone */}
                  <div className="field field-required field-show-floating-label">
                    <div className="field-input-wrapper">
                      <label htmlFor="billing_address_phone" className="field-label">
                        Số điện thoại
                      </label>
                      <input
                        type="tel"
                        placeholder="Số điện thoại"
                        className="field-input"
                        id="billing_address_phone"
                        name="billing_address_phone"
                        maxLength={15}
                        value={phone}
                        onChange={(e) => { setPhone(e.target.value); setErrors((p) => ({ ...p, phone: '' })) }}
                        style={errors.phone ? { borderColor: 'red' } : {}}
                      />
                      {errors.phone && (
                        <p className="error-show" style={{ display: 'block', color: 'red', marginTop: '4px' }}>
                          {errors.phone}
                        </p>
                      )}
                    </div>
                  </div>

                  {/* Province */}
                  <div className="field field-required field-show-floating-label">
                    <div className="field-input-wrapper">
                      <label className="field-label">Tỉnh/Thành phố</label>
                      <select
                        className="field-input"
                        value={provinceId ?? ''}
                        onChange={(e) => {
                          const id = e.target.value ? Number(e.target.value) : null
                          setProvinceId(id); setDistrictId(null); setWardCode('')
                          setErrors((p) => ({ ...p, province: '' }))
                        }}
                        style={errors.province ? { borderColor: 'red' } : {}}
                      >
                        <option value="">-- Chọn Tỉnh/Thành phố --</option>
                        {provinces.map((p) => (
                          <option key={p.provinceId} value={p.provinceId}>{p.provinceName}</option>
                        ))}
                      </select>
                      {errors.province && (
                        <p className="error-show" style={{ display: 'block', color: 'red', marginTop: '4px' }}>
                          {errors.province}
                        </p>
                      )}
                    </div>
                  </div>

                  {/* District */}
                  <div className="field field-required field-show-floating-label">
                    <div className="field-input-wrapper">
                      <label className="field-label">Quận/Huyện</label>
                      <select
                        className="field-input"
                        value={districtId ?? ''}
                        disabled={!provinceId}
                        onChange={(e) => {
                          const id = e.target.value ? Number(e.target.value) : null
                          setDistrictId(id); setWardCode('')
                          setErrors((p) => ({ ...p, district: '' }))
                        }}
                        style={errors.district ? { borderColor: 'red' } : {}}
                      >
                        <option value="">-- Chọn Quận/Huyện --</option>
                        {districts.map((d) => (
                          <option key={d.districtId} value={d.districtId}>{d.districtName}</option>
                        ))}
                      </select>
                      {errors.district && (
                        <p className="error-show" style={{ display: 'block', color: 'red', marginTop: '4px' }}>
                          {errors.district}
                        </p>
                      )}
                    </div>
                  </div>

                  {/* Ward */}
                  <div className="field field-required field-show-floating-label">
                    <div className="field-input-wrapper">
                      <label className="field-label">Phường/Xã</label>
                      <select
                        className="field-input"
                        value={wardCode}
                        disabled={!districtId}
                        onChange={(e) => {
                          setWardCode(e.target.value)
                          setErrors((p) => ({ ...p, ward: '' }))
                        }}
                        style={errors.ward ? { borderColor: 'red' } : {}}
                      >
                        <option value="">-- Chọn Phường/Xã --</option>
                        {wards.map((w) => (
                          <option key={w.wardCode} value={w.wardCode}>{w.wardName}</option>
                        ))}
                      </select>
                      {errors.ward && (
                        <p className="error-show" style={{ display: 'block', color: 'red', marginTop: '4px' }}>
                          {errors.ward}
                        </p>
                      )}
                    </div>
                  </div>

                  {/* Street / house number */}
                  <div className="field field-required field-show-floating-label">
                    <div className="field-input-wrapper">
                      <label htmlFor="billing_address_address" className="field-label">
                        Số nhà, tên đường
                      </label>
                      <input
                        type="text"
                        placeholder="Số nhà, tên đường"
                        className="field-input"
                        id="billing_address_address"
                        name="billing_address_address"
                        value={address}
                        onChange={(e) => { setAddress(e.target.value); setErrors((p) => ({ ...p, address: '' })) }}
                        style={errors.address ? { borderColor: 'red' } : {}}
                      />
                      {errors.address && (
                        <p className="error-show" style={{ display: 'block', color: 'red', marginTop: '4px' }}>
                          {errors.address}
                        </p>
                      )}
                    </div>
                  </div>

                  <div className="step-footer" id="step-footer-checkout">
                    <button type="submit" className="step-footer-continue-btn btn">
                      <span className="btn-content">Tiếp tục đến phương thức thanh toán</span>
                    </button>
                    <Link to="/cart" className="step-footer-previous-link">Giỏ hàng</Link>
                  </div>

                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </CheckoutLayout>
  )
}
