import { useState } from 'react'
import AccountSidebar from '../components/account/AccountSidebar'
import {
  useAddresses,
  useCreateAddress,
  useUpdateAddress,
  useDeleteAddress,
} from '../hooks/useOrders'
import { useProvinces, useDistricts, useWards } from '../hooks/useShipping'

// ── Empty form factory ──────────────────────────────────────────────────────────
const emptyForm = () => ({
  adrsFullname:  '',
  adrsAddress:   '',
  adrsPhone:     '',
  adrsIsDefault: false,
  provinceId:    null,
  districtId:    null,
  wardCode:      '',
})

// ── Single address row ──────────────────────────────────────────────────────────
function AddressRow({ address, onEdit, onDelete }) {
  return (
    <div className="address-table-wrap">
      <div id="address-table">
        <div className="row-addr">
          <div className="address-title-wrap">
            <div className="address-title">
              <h3><strong>{address.adrsFullname}</strong></h3>
              <p className="adrress_actions">
                <span className="action_link action_edit">
                  <a href="#" onClick={(e) => { e.preventDefault(); onEdit(address) }}>
                    <i className="fa fa-pencil-square-o" aria-hidden="true" />
                  </a>
                </span>
                <span className="action_link action_delete">
                  <a href="#" onClick={(e) => { e.preventDefault(); onDelete(address.adrsId) }}>
                    <i className="fa fa-times" aria-hidden="true" />
                  </a>
                </span>
              </p>
            </div>
          </div>
        </div>

        {/* View mode */}
        <div className="address_table">
          <div className="view_address">
            <div className="customer-infor-wrap">
              <div className="customer_name row-addr">
                <p><strong>{address.adrsFullname}</strong></p>
              </div>
              <div className="customer_name_infor" />
            </div>
            <div className="customer-infor-wrap">
              <div className="customer_address row-addr">
                <p><b>Địa chỉ</b></p>
              </div>
              <div className="customer_address_infor">
                <p>{address.adrsAddress}</p>
              </div>
            </div>
            <div className="customer-infor-wrap">
              <div className="customer_phone row-addr">
                <p><b>Số điện thoại</b></p>
              </div>
              <div className="customer_phone_infor">
                <p>{address.adrsPhone}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

// ── Address form (shared for add & edit) ───────────────────────────────────────
function AddressForm({ initialValues = emptyForm(), onSubmit, onCancel, submitLabel, isPending }) {
  const [form, setForm] = useState(initialValues)

  const { data: provinces = [] } = useProvinces()
  const { data: districts = [] } = useDistricts(form.provinceId)
  const { data: wards     = [] } = useWards(form.districtId)

  const set = (field) => (e) => {
    const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value
    setForm((prev) => ({ ...prev, [field]: value }))
  }

  const handleProvinceChange = (e) => {
    const id = e.target.value ? Number(e.target.value) : null
    setForm((prev) => ({ ...prev, provinceId: id, districtId: null, wardCode: '' }))
  }

  const handleDistrictChange = (e) => {
    const id = e.target.value ? Number(e.target.value) : null
    setForm((prev) => ({ ...prev, districtId: id, wardCode: '' }))
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    onSubmit(form)
  }

  return (
    <div className="customer_address edit_address">
      <form onSubmit={handleSubmit}>
        <div className="addr-input-group">
          <span className="addr-input-addon">
            <i className="fa fa-user" aria-hidden="true" />
          </span>
          <input
            className="addr-form-control addr-textbox"
            type="text"
            placeholder="Họ Tên"
            value={form.adrsFullname}
            onChange={set('adrsFullname')}
            required
          />
        </div>

        {/* Province */}
        <div className="addr-input-group">
          <span className="addr-input-addon">
            <i className="fa fa-map-marker" aria-hidden="true" />
          </span>
          <select
            className="addr-form-control addr-textbox"
            value={form.provinceId ?? ''}
            onChange={handleProvinceChange}
            required
          >
            <option value="">-- Chọn Tỉnh/Thành phố --</option>
            {provinces.map((p) => (
              <option key={p.provinceId} value={p.provinceId}>{p.provinceName}</option>
            ))}
          </select>
        </div>

        {/* District */}
        <div className="addr-input-group">
          <span className="addr-input-addon">
            <i className="fa fa-map-marker" aria-hidden="true" />
          </span>
          <select
            className="addr-form-control addr-textbox"
            value={form.districtId ?? ''}
            onChange={handleDistrictChange}
            disabled={!form.provinceId}
            required
          >
            <option value="">-- Chọn Quận/Huyện --</option>
            {districts.map((d) => (
              <option key={d.districtId} value={d.districtId}>{d.districtName}</option>
            ))}
          </select>
        </div>

        {/* Ward */}
        <div className="addr-input-group">
          <span className="addr-input-addon">
            <i className="fa fa-map-marker" aria-hidden="true" />
          </span>
          <select
            className="addr-form-control addr-textbox"
            value={form.wardCode}
            onChange={set('wardCode')}
            disabled={!form.districtId}
            required
          >
            <option value="">-- Chọn Phường/Xã --</option>
            {wards.map((w) => (
              <option key={w.wardCode} value={w.wardCode}>{w.wardName}</option>
            ))}
          </select>
        </div>

        {/* Street / house number */}
        <div className="addr-input-group">
          <span className="addr-input-addon">
            <i className="fa fa-home" aria-hidden="true" />
          </span>
          <input
            className="addr-form-control addr-textbox"
            type="text"
            placeholder="Số nhà, tên đường"
            value={form.adrsAddress}
            onChange={set('adrsAddress')}
            required
          />
        </div>

        <div className="addr-input-group">
          <span className="addr-input-addon">
            <i className="fa fa-phone" aria-hidden="true" />
          </span>
          <input
            className="addr-form-control addr-textbox"
            type="text"
            placeholder="Số điện thoại"
            value={form.adrsPhone}
            onChange={set('adrsPhone')}
            required
          />
        </div>
        <div className="addr-input-group addr-checkbox-group">
          <input
            type="checkbox"
            id={`default_${initialValues.adrsId ?? 'new'}`}
            checked={form.adrsIsDefault}
            onChange={set('adrsIsDefault')}
          />
          <label htmlFor={`default_${initialValues.adrsId ?? 'new'}`}>
            Đặt làm địa chỉ mặc định
          </label>
        </div>
        <div className="action_bottom">
          <button type="submit" className="addr-btn" disabled={isPending}>
            {isPending ? 'Đang lưu...' : submitLabel}
          </button>
          <span>
            {' '}hoặc{' '}
            <a href="#" onClick={(e) => { e.preventDefault(); onCancel() }}>Hủy</a>
          </span>
        </div>
      </form>
    </div>
  )
}

// ── Main page ──────────────────────────────────────────────────────────────────
export default function AddressPage() {
  const { data: addresses = [], isLoading } = useAddresses()
  const createAddress = useCreateAddress()
  const updateAddress = useUpdateAddress()
  const deleteAddress = useDeleteAddress()

  // which address is currently open for editing (by adrsId), null = none
  const [editingId, setEditingId] = useState(null)
  // whether the "add new" form is visible
  const [isAdding,  setIsAdding]  = useState(false)

  // ── Handlers ────────────────────────────────────────────────────────────────
  const handleEdit = (address) => {
    setIsAdding(false)
    setEditingId(editingId === address.adrsId ? null : address.adrsId)
  }

  const handleDelete = (id) => {
    if (window.confirm('Bạn có chắc chắn muốn xóa địa chỉ này không?')) {
      deleteAddress.mutate(id)
    }
  }

  const handleCreate = (form) => {
    createAddress.mutate(form, {
      onSuccess: () => setIsAdding(false),
    })
  }

  const handleUpdate = (id, form) => {
    updateAddress.mutate({ id, dto: form }, {
      onSuccess: () => setEditingId(null),
    })
  }

  return (
    <div className="layout-address">
      <div className="address-header">
        <h1>Thông tin địa chỉ</h1>
      </div>
      <div className="address-content-wrap">
        <div className="row-account">

          <AccountSidebar />

          <div className="content-wrap">
            <div className="content-page">

              {isLoading && <p>Đang tải địa chỉ...</p>}

              {addresses.map((address) => (
                <div key={address.adrsId}>
                  {editingId === address.adrsId ? (
                    <div className="address-table-wrap">
                      <div id="address-table">
                        <div className="row-addr">
                          <div className="address-title-wrap">
                            <div className="address-title">
                              <h3><strong>{address.adrsFullname}</strong></h3>
                              <p className="adrress_actions">
                                <span className="action_link action_edit">
                                  <a href="#" onClick={(e) => { e.preventDefault(); setEditingId(null) }}>
                                    <i className="fa fa-pencil-square-o" aria-hidden="true" />
                                  </a>
                                </span>
                                <span className="action_link action_delete">
                                  <a href="#" onClick={(e) => { e.preventDefault(); handleDelete(address.adrsId) }}>
                                    <i className="fa fa-times" aria-hidden="true" />
                                  </a>
                                </span>
                              </p>
                            </div>
                          </div>
                        </div>
                        <div className="address_table">
                          <AddressForm
                            initialValues={{
                              adrsId:        address.adrsId,
                              adrsFullname:  address.adrsFullname,
                              adrsAddress:   address.adrsAddress,
                              adrsPhone:     address.adrsPhone,
                              adrsIsDefault: address.adrsIsDefault ?? false,
                              provinceId:    address.provinceId   ?? null,
                              districtId:    address.districtId   ?? null,
                              wardCode:      address.wardCode     ?? '',
                            }}
                            onSubmit={(form) => handleUpdate(address.adrsId, form)}
                            onCancel={() => setEditingId(null)}
                            submitLabel="CẬP NHẬT"
                            isPending={updateAddress.isPending}
                          />
                        </div>
                      </div>
                    </div>
                  ) : (
                    <AddressRow
                      address={address}
                      onEdit={handleEdit}
                      onDelete={handleDelete}
                    />
                  )}
                </div>
              ))}

            </div>

            {/* ── Add new address panel — inside content-wrap so it stays in the right column ── */}
            <div className="add-address-wrap">
              <a
                href="#"
                id="add-new-address"
                className="add-new-address"
                onClick={(e) => { e.preventDefault(); setIsAdding((p) => !p); setEditingId(null) }}
              >
                Nhập địa chỉ mới
              </a>

              {isAdding && (
                <div id="add_address" style={{ marginTop: '12px' }}>
                  <AddressForm
                    onSubmit={handleCreate}
                    onCancel={() => setIsAdding(false)}
                    submitLabel="THÊM MỚI"
                    isPending={createAddress.isPending}
                  />
                </div>
              )}
            </div>

          </div>

        </div>
      </div>
    </div>
  )
}
