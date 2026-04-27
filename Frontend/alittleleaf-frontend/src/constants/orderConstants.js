// ── Internal order lifecycle ──────────────────────────────────────────────────

export const ORDER_STATUS_OPTIONS = [
  { value: 'PENDING',    label: 'Chờ xử lý'    },
  { value: 'CONFIRMED',  label: 'Đã xác nhận'  },
  { value: 'SHIPPING',   label: 'Đang vận chuyển' },
  { value: 'COMPLETED',  label: 'Hoàn thành'   },
  { value: 'CANCELLED',  label: 'Đã hủy'       },
]

export const ORDER_STATUS_LABEL = Object.fromEntries(
  ORDER_STATUS_OPTIONS.map((o) => [o.value, o.label])
)

export const ORDER_STATUS_COLOR = {
  PENDING:    'bg-secondary',
  CONFIRMED:  'bg-info text-dark',
  SHIPPING:   'bg-primary',
  COMPLETED:  'bg-success',
  CANCELLED:  'bg-danger',
}

// ── GHN carrier tracking status (exact strings from GHN API) ─────────────────

export const SHIPPING_STATUS_OPTIONS = [
  { value: 'not_fulfilled',           label: 'Chưa xử lý'                                        },
  { value: 'ready_to_pick',           label: 'Mới tạo đơn hàng'                                  },
  { value: 'picking',                 label: 'Nhân viên đang lấy hàng'                            },
  { value: 'cancel',                  label: 'Hủy đơn hàng'                                      },
  { value: 'money_collect_picking',   label: 'Đang thu tiền người gửi'                            },
  { value: 'picked',                  label: 'Nhân viên đã lấy hàng'                              },
  { value: 'storing',                 label: 'Hàng đang nằm ở kho'                                },
  { value: 'transporting',            label: 'Đang luân chuyển hàng'                              },
  { value: 'sorting',                 label: 'Đang phân loại hàng hóa'                            },
  { value: 'delivering',              label: 'Nhân viên đang giao cho người nhận'                 },
  { value: 'money_collect_delivering',label: 'Nhân viên đang thu tiền người nhận'                 },
  { value: 'delivered',               label: 'Nhân viên đã giao hàng thành công'                  },
  { value: 'delivery_fail',           label: 'Nhân viên giao hàng thất bại'                       },
  { value: 'waiting_to_return',       label: 'Đang đợi trả hàng về cho người gửi'                 },
  { value: 'return',                  label: 'Trả hàng'                                           },
  { value: 'return_transporting',     label: 'Đang luân chuyển hàng trả'                          },
  { value: 'return_sorting',          label: 'Đang phân loại hàng trả'                            },
  { value: 'returning',               label: 'Nhân viên đang đi trả hàng'                         },
  { value: 'return_fail',             label: 'Nhân viên trả hàng thất bại'                        },
  { value: 'returned',                label: 'Nhân viên trả hàng thành công'                      },
  { value: 'exception',               label: 'Đơn hàng ngoại lệ không nằm trong quy trình'        },
  { value: 'damage',                  label: 'Hàng bị hư hỏng'                                    },
  { value: 'lost',                    label: 'Hàng bị mất'                                        },
]

export const SHIPPING_STATUS_LABEL = Object.fromEntries(
  SHIPPING_STATUS_OPTIONS.map((o) => [o.value, o.label])
)

export const SHIPPING_STATUS_COLOR = {
  not_fulfilled:            'bg-secondary',
  ready_to_pick:            'bg-info text-dark',
  picking:                  'bg-info text-dark',
  money_collect_picking:    'bg-info text-dark',
  picked:                   'bg-info text-dark',
  storing:                  'bg-info text-dark',
  transporting:             'bg-primary',
  sorting:                  'bg-primary',
  delivering:               'bg-primary',
  money_collect_delivering: 'bg-primary',
  delivered:                'bg-success',
  delivery_fail:            'bg-danger',
  waiting_to_return:        'bg-warning text-dark',
  return:                   'bg-warning text-dark',
  return_transporting:      'bg-warning text-dark',
  return_sorting:           'bg-warning text-dark',
  returning:                'bg-warning text-dark',
  return_fail:              'bg-warning text-dark',
  returned:                 'bg-warning text-dark',
  cancel:                   'bg-danger',
  exception:                'bg-danger',
  damage:                   'bg-danger',
  lost:                     'bg-danger',
}

// ── Payment status ────────────────────────────────────────────────────────────

export const PAYMENT_STATUS_OPTIONS = [
  { value: 'pending',       label: 'Chưa thanh toán' },
  { value: 'pending_vnpay', label: 'Chờ VNPAY'       },
  { value: 'paid',          label: 'Đã thanh toán'   },
]

export const PAYMENT_STATUS_LABEL = Object.fromEntries(
  PAYMENT_STATUS_OPTIONS.map((o) => [o.value, o.label])
)

export const PAYMENT_STATUS_COLOR = {
  paid:          'bg-success',
  pending_vnpay: 'bg-warning text-dark',
  pending:       'bg-danger',
}
