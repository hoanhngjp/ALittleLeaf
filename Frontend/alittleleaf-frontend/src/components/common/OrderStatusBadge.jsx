import { ORDER_STATUS_LABEL, ORDER_STATUS_COLOR } from '../../constants/orderConstants'

export default function OrderStatusBadge({ status }) {
  const cls   = ORDER_STATUS_COLOR[status]  ?? 'bg-secondary'
  const label = ORDER_STATUS_LABEL[status]  ?? status
  return <span className={`badge ${cls}`}>{label}</span>
}
