import { useState, useMemo } from 'react'
import { Link } from 'react-router-dom'
import apiClient from '../../lib/apiClient'
import { SHIPPING_STATUS_OPTIONS } from '../../constants/orderConstants'

// ── Auto-generate a realistic message for each GHN status ─────────────────────
const STATUS_MESSAGE = {
  ready_to_pick:            'Đơn hàng đã được tạo, chờ lấy hàng',
  picking:                  'Nhân viên đang trên đường lấy hàng',
  money_collect_picking:    'Nhân viên đang thu tiền tại kho người gửi',
  picked:                   'Nhân viên đã lấy hàng thành công',
  storing:                  'Hàng đang được lưu tại kho GHN',
  transporting:             'Hàng đang được luân chuyển giữa các kho',
  sorting:                  'Hàng đang được phân loại tại kho',
  delivering:               'Nhân viên đang giao hàng đến người nhận',
  money_collect_delivering: 'Nhân viên đang thu tiền COD tại địa chỉ người nhận',
  delivered:                'Giao hàng thành công → OrderStatus: COMPLETED',
  delivery_fail:            'Giao hàng thất bại, sẽ thử lại hoặc hoàn hàng',
  waiting_to_return:        'Chờ xác nhận hoàn hàng về người gửi',
  return:                   'Đang tiến hành hoàn trả hàng → OrderStatus: CANCELLED',
  return_transporting:      'Hàng đang được luân chuyển về kho gốc',
  return_sorting:           'Hàng đang được phân loại để hoàn trả',
  returning:                'Nhân viên đang trên đường trả hàng cho người gửi',
  return_fail:              'Trả hàng thất bại',
  returned:                 'Trả hàng thành công → OrderStatus: CANCELLED',
  cancel:                   'Đơn hàng bị hủy → OrderStatus: CANCELLED',
  exception:                'Đơn hàng gặp sự cố ngoại lệ, cần xử lý thủ công',
  damage:                   'Hàng bị hư hỏng trong quá trình vận chuyển → OrderStatus: CANCELLED',
  lost:                     'Hàng bị mất → OrderStatus: CANCELLED',
}

const TYPE_OPTIONS = [
  { value: 'switch_status', label: 'switch_status' },
  { value: 'pick_cargo',    label: 'pick_cargo'    },
  { value: 'shop_cancel',   label: 'shop_cancel'   },
  { value: 'return',        label: 'return'        },
]

// ── Helpers ───────────────────────────────────────────────────────────────────
function buildPayload(orderCode, shopId, type, status) {
  return {
    OrderCode:       orderCode,
    Status:          status,
    message_display: STATUS_MESSAGE[status] ?? status,
    Type:            type,
    Time:            new Date().toISOString(),
    CODAmount:       0,
    PaymentTypeID:   1,
    Fee:             { MainService: 30000 },
    ...(shopId ? { ShopID: Number(shopId) } : {}),
  }
}

// ── Component ─────────────────────────────────────────────────────────────────
export default function GhnSimulatorPage() {
  const [orderCode, setOrderCode] = useState('')
  const [shopId,    setShopId]    = useState('')
  const [type,      setType]      = useState('switch_status')
  const [status,    setStatus]    = useState('delivering')
  const [loading,   setLoading]   = useState(false)
  const [result,    setResult]    = useState(null) // { ok: bool, message: string }

  const payload = useMemo(
    () => buildPayload(orderCode || 'YOUR_ORDER_CODE', shopId, type, status),
    [orderCode, shopId, type, status]
  )

  async function handleFire(e) {
    e.preventDefault()
    if (!orderCode.trim()) return

    setLoading(true)
    setResult(null)

    try {
      await apiClient.post('/api/shipping/webhook', payload)
      setResult({ ok: true, message: `✓ Webhook fired successfully for order "${orderCode}" with status "${status}".` })
    } catch (err) {
      const msg = err?.response?.data?.error
        ?? err?.response?.statusText
        ?? err.message
        ?? 'Unknown error'
      setResult({ ok: false, message: `✗ Request failed: ${msg}` })
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="app-content-header">
      <div className="container-fluid">

        {/* ── Page header ───────────────────────────────────────── */}
        <div className="row mb-3">
          <div className="col-sm-6">
            <h1 className="m-0">GHN Webhook Simulator</h1>
          </div>
          <div className="col-sm-6">
            <ol className="breadcrumb float-sm-end">
              <li className="breadcrumb-item"><Link to="/admin">Trang chủ</Link></li>
              <li className="breadcrumb-item active">GHN Simulator</li>
            </ol>
          </div>
        </div>

      </div>

      <div className="app-content">
        <div className="container-fluid">

          {/* ── Instructions ────────────────────────────────────── */}
          <div className="card mb-4">
            <div className="card-body py-3">
              <div className="d-flex flex-wrap align-items-center gap-2">
                {[
                  { n: '1', text: 'Mở trang Chi tiết đơn hàng và sao chép Mã vận đơn GHN' },
                  { n: '2', text: 'Dán mã vào ô bên dưới và chọn trạng thái muốn giả lập' },
                  { n: '3', text: 'Nhấn "BẮN WEBHOOK NGAY" để gửi sự kiện đến backend' },
                  { n: '4', text: 'Quay lại Chi tiết đơn hàng và kiểm tra kết quả' },
                ].map(({ n, text }) => (
                  <div key={n} className="d-flex align-items-start gap-2 me-4">
                    <span
                      className="badge bg-dark rounded-circle d-flex align-items-center justify-content-center flex-shrink-0"
                      style={{ width: 24, height: 24, fontSize: 12 }}
                    >
                      {n}
                    </span>
                    <small className="text-muted">{text}</small>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* ── Alert ───────────────────────────────────────────── */}
          {result && (
            <div
              className={`alert ${result.ok ? 'alert-success' : 'alert-danger'} alert-dismissible`}
              role="alert"
            >
              <i className={`bi ${result.ok ? 'bi-check-circle-fill' : 'bi-exclamation-triangle-fill'} me-2`} />
              {result.message}
              <button
                type="button"
                className="btn-close"
                onClick={() => setResult(null)}
                aria-label="Close"
              />
            </div>
          )}

          {/* ── Main row ────────────────────────────────────────── */}
          <div className="row">

            {/* ── LEFT: Form ────────────────────────────────────── */}
            <div className="col-lg-5 mb-4">
              <div className="card h-100">
                <div className="card-header">
                  <h5 className="mb-0">
                    <i className="bi bi-send-fill me-2 text-warning" />
                    Cấu hình sự kiện
                  </h5>
                </div>
                <div className="card-body">
                  <form onSubmit={handleFire}>

                    {/* Order code */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">
                        Mã vận đơn GHN <span className="text-danger">*</span>
                      </label>
                      <input
                        type="text"
                        className="form-control font-monospace"
                        placeholder="e.g. FFFNL9HH"
                        value={orderCode}
                        onChange={(e) => setOrderCode(e.target.value.trim())}
                        required
                      />
                      <div className="form-text">Lấy từ trang Chi tiết đơn hàng (Admin).</div>
                    </div>

                    {/* Shop ID */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">ShopID <span className="text-muted small">(tuỳ chọn)</span></label>
                      <input
                        type="number"
                        className="form-control"
                        placeholder="Để trống nếu không cần"
                        value={shopId}
                        onChange={(e) => setShopId(e.target.value)}
                      />
                    </div>

                    {/* Type */}
                    <div className="mb-3">
                      <label className="form-label fw-semibold">Loại sự kiện (Type)</label>
                      <select
                        className="form-select"
                        value={type}
                        onChange={(e) => setType(e.target.value)}
                      >
                        {TYPE_OPTIONS.map((o) => (
                          <option key={o.value} value={o.value}>{o.label}</option>
                        ))}
                      </select>
                    </div>

                    {/* Status */}
                    <div className="mb-4">
                      <label className="form-label fw-semibold">Trạng thái GHN (Status)</label>
                      <select
                        className="form-select"
                        value={status}
                        onChange={(e) => setStatus(e.target.value)}
                      >
                        {SHIPPING_STATUS_OPTIONS
                          .filter((o) => o.value !== 'not_fulfilled')
                          .map((o) => (
                            <option key={o.value} value={o.value}>{o.value} — {o.label}</option>
                          ))
                        }
                      </select>
                      <div className="form-text mt-1">
                        <i className="bi bi-info-circle me-1" />
                        {STATUS_MESSAGE[status] ?? ''}
                      </div>
                    </div>

                    <button
                      type="submit"
                      className="btn w-100 fw-bold"
                      style={{ background: '#e03e2d', color: '#fff', letterSpacing: 1 }}
                      disabled={loading || !orderCode.trim()}
                    >
                      {loading
                        ? <><span className="spinner-border spinner-border-sm me-2" />Đang gửi…</>
                        : <><i className="bi bi-lightning-charge-fill me-2" />BẮN WEBHOOK NGAY</>
                      }
                    </button>

                  </form>
                </div>
              </div>
            </div>

            {/* ── RIGHT: Terminal preview ────────────────────────── */}
            <div className="col-lg-7 mb-4">
              <div className="card h-100" style={{ background: '#1e1e1e', border: 'none' }}>
                {/* macOS-style title bar */}
                <div
                  className="card-header d-flex align-items-center gap-2"
                  style={{ background: '#323232', borderBottom: '1px solid #444', padding: '8px 14px' }}
                >
                  <span style={{ width: 12, height: 12, borderRadius: '50%', background: '#ff5f57', display: 'inline-block' }} />
                  <span style={{ width: 12, height: 12, borderRadius: '50%', background: '#ffbd2e', display: 'inline-block' }} />
                  <span style={{ width: 12, height: 12, borderRadius: '50%', background: '#28c840', display: 'inline-block' }} />
                  <span
                    className="ms-3"
                    style={{ color: '#aaa', fontSize: 12, fontFamily: 'monospace' }}
                  >
                    POST /api/shipping/webhook — payload preview
                  </span>
                </div>
                <div className="card-body p-0" style={{ overflow: 'auto' }}>
                  <pre
                    style={{
                      margin: 0,
                      padding: '20px',
                      background: '#1e1e1e',
                      color: '#d4d4d4',
                      fontFamily: '"Fira Code", "Cascadia Code", "Consolas", monospace',
                      fontSize: 13,
                      lineHeight: 1.6,
                      minHeight: '100%',
                    }}
                  >
                    <code>
                      {/* Prompt line */}
                      <span style={{ color: '#569cd6' }}>$</span>
                      <span style={{ color: '#ce9178' }}> curl -X POST</span>
                      {' '}
                      <span style={{ color: '#4ec9b0' }}>http://localhost:8081/api/shipping/webhook</span>
                      {'\n'}
                      <span style={{ color: '#ce9178' }}>  -H</span>
                      {' '}
                      <span style={{ color: '#ce9178' }}>"Content-Type: application/json"</span>
                      {'\n'}
                      <span style={{ color: '#ce9178' }}>  -d</span>
                      {" '"}
                      {'\n'}
                      {syntaxHighlight(JSON.stringify(payload, null, 2))}
                      {"'\n"}
                    </code>
                  </pre>
                </div>
              </div>
            </div>

          </div>
        </div>
      </div>
    </div>
  )
}

// ── Minimal JSON syntax highlighter ───────────────────────────────────────────
function syntaxHighlight(json) {
  const lines = json.split('\n')
  return lines.map((line, i) => {
    // Keys
    const keyMatch = line.match(/^(\s*)("[\w_]+")(:\s*)(.*)$/)
    if (keyMatch) {
      const [, indent, key, colon, val] = keyMatch
      return (
        <span key={i}>
          {indent}
          <span style={{ color: '#9cdcfe' }}>{key}</span>
          {colon}
          {colorValue(val)}
          {'\n'}
        </span>
      )
    }
    return <span key={i}>{line}{'\n'}</span>
  })
}

function colorValue(val) {
  if (val === undefined || val === '') return null
  const trimmed = val.replace(/,$/, '')
  const comma   = val.endsWith(',') ? ',' : ''
  if (trimmed === 'null')  return <><span style={{ color: '#569cd6' }}>null</span>{comma}</>
  if (trimmed === 'true' || trimmed === 'false')
    return <><span style={{ color: '#569cd6' }}>{trimmed}</span>{comma}</>
  if (/^-?\d/.test(trimmed))
    return <><span style={{ color: '#b5cea8' }}>{trimmed}</span>{comma}</>
  if (trimmed.startsWith('"'))
    return <><span style={{ color: '#ce9178' }}>{trimmed}</span>{comma}</>
  return <>{val}</>
}
