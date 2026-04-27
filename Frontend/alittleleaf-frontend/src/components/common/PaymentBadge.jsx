import {
  PAYMENT_STATUS_LABEL,
  PAYMENT_STATUS_COLOR,
} from '../../constants/orderConstants'

export default function PaymentBadge({ status }) {
  const cls   = PAYMENT_STATUS_COLOR[status]  ?? 'bg-secondary'
  const label = PAYMENT_STATUS_LABEL[status]  ?? status
  return <span className={`badge ${cls}`}>{label}</span>
}
