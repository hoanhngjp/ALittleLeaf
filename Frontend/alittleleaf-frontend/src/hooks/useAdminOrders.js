import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useAdminOrders({
  page = 1,
  pageSize = 20,
  keyword,
  shippingStatus,
  paymentStatus,
  startDate,
  endDate,
  sortBy,
  sortOrder,
} = {}) {
  const params = {
    page,
    pageSize,
    ...(keyword        ? { keyword }                             : {}),
    ...(shippingStatus ? { shippingStatus }                      : {}),
    ...(paymentStatus  ? { paymentStatus }                       : {}),
    // Send ISO date strings (yyyy-MM-dd) so .NET DateOnly.TryParseExact("yyyy-MM-dd") binds correctly
    ...(startDate ? { startDate: startDate.slice(0, 10) } : {}),
    ...(endDate   ? { endDate:   endDate.slice(0, 10)   } : {}),
    ...(sortBy         ? { sortBy }                              : {}),
    ...(sortBy         ? { isDescending: sortOrder !== 'asc' }   : {}),
  }

  return useQuery({
    queryKey:  ['admin-orders', params],
    queryFn:   () => apiClient.get('/api/admin/orders', { params }).then((r) => r.data),
    staleTime: 30_000,
  })
}

export function useAdminOrder(id) {
  return useQuery({
    queryKey:  ['admin-order', id],
    queryFn:   () => apiClient.get(`/api/admin/orders/${id}`).then((r) => r.data),
    enabled:   !!id,
    staleTime: 30_000,
  })
}

export function useUpdateOrderStatus(id) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (dto) =>
      apiClient.patch(`/api/admin/orders/${id}/status`, dto).then((r) => r.data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-orders'] })
      qc.invalidateQueries({ queryKey: ['admin-order', id] })
    },
  })
}
