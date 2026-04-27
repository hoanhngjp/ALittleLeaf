import {
  SHIPPING_STATUS_LABEL,
  SHIPPING_STATUS_COLOR,
} from '../../constants/orderConstants'

export default function ShippingBadge({ status }) {
  const cls   = SHIPPING_STATUS_COLOR[status]  ?? 'bg-secondary'
  const label = SHIPPING_STATUS_LABEL[status]  ?? status
  return <span className={`badge ${cls}`}>{label}</span>
}
