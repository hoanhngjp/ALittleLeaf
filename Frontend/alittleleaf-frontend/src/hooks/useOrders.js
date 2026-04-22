import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

// ── Saved addresses ────────────────────────────────────────────────────────────
export function useAddresses() {
  return useQuery({
    queryKey: ['addresses'],
    queryFn:  () => apiClient.get('/api/addresses').then((r) => r.data),
    staleTime: 30_000,
  })
}

// ── Place order (COD or VNPAY) ─────────────────────────────────────────────────
// dto shape: { addressId?, newFullName?, newAddress?, newPhone?, paymentMethod, note? }
export function usePlaceOrder() {
  return useMutation({
    mutationFn: (dto) => apiClient.post('/api/orders', dto).then((r) => r.data),
  })
}

// ── Generate VNPay URL ─────────────────────────────────────────────────────────
// billId: number returned from POST /api/orders when paymentMethod = "VNPAY"
export function useCreateVnPayUrl() {
  return useMutation({
    mutationFn: (billId) =>
      apiClient.post('/api/payment/create-url', { billId }).then((r) => r.data),
  })
}

// ── Order history (client-side pagination via `page` + `pageSize`) ────────────
export function useOrders() {
  return useQuery({
    queryKey: ['orders'],
    queryFn:  () => apiClient.get('/api/orders').then((r) => r.data),
    staleTime: 30_000,
  })
}

// ── Address CRUD mutations ─────────────────────────────────────────────────────
export function useCreateAddress() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (dto) => apiClient.post('/api/addresses', dto).then((r) => r.data),
    onSuccess:  () => qc.invalidateQueries({ queryKey: ['addresses'] }),
  })
}

export function useUpdateAddress() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, dto }) => apiClient.put(`/api/addresses/${id}`, dto).then((r) => r.data),
    onSuccess:  () => qc.invalidateQueries({ queryKey: ['addresses'] }),
  })
}

export function useDeleteAddress() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id) => apiClient.delete(`/api/addresses/${id}`).then((r) => r.data),
    onSuccess:  () => qc.invalidateQueries({ queryKey: ['addresses'] }),
  })
}

// ── Retry VNPay payment for an unpaid order ───────────────────────────────────
export function useRetryPayment() {
  return useMutation({
    mutationFn: (billId) =>
      apiClient.post(`/api/payment/retry/${billId}`).then((r) => r.data),
  })
}

// ── Single order detail ────────────────────────────────────────────────────────
export function useOrderDetail(id) {
  return useQuery({
    queryKey: ['orders', id],
    queryFn:  () => apiClient.get(`/api/orders/${id}`).then((r) => r.data),
    enabled:  !!id,
  })
}
