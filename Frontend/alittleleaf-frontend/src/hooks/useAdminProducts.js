import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

// ── List (paginated + searchable + sortable + filterable) ────────────────────
// sortBy: 'productName' | 'productPrice' | 'quantityInStock' | 'createdAt' | 'updatedAt'
// sortOrder: 'asc' | 'desc'
export function useAdminProducts({ page = 1, pageSize = 10, search, sortBy, sortOrder, categoryId } = {}) {
  const params = {
    page,
    pageSize,
    ...(search                    ? { keyword:      search               } : {}),
    ...(sortBy                    ? { sortBy                             } : {}),
    ...(sortOrder                 ? { isDescending: sortOrder === 'desc' } : {}),
    ...(categoryId != null && categoryId !== '' ? { categoryId }         : {}),
  }

  return useQuery({
    queryKey: ['admin-products', params],
    queryFn:  () => apiClient.get('/api/admin/products', { params }).then((r) => r.data),
    staleTime: 30_000,
  })
}

// ── Single product (for edit form) ───────────────────────────────────────────
export function useAdminProduct(id) {
  return useQuery({
    queryKey: ['admin-product', id],
    queryFn:  () => apiClient.get(`/api/admin/products/${id}`).then((r) => r.data),
    enabled:  !!id,
    staleTime: 30_000,
  })
}

// ── Create ────────────────────────────────────────────────────────────────────
export function useCreateProduct() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (formData) =>
      apiClient.post('/api/admin/products', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      }).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-products'] }),
  })
}

// ── Update ────────────────────────────────────────────────────────────────────
export function useUpdateProduct() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, formData }) =>
      apiClient.put(`/api/admin/products/${id}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      }).then((r) => r.data),
    onSuccess: (_, { id }) => {
      qc.invalidateQueries({ queryKey: ['admin-products'] })
      qc.invalidateQueries({ queryKey: ['admin-product', id] })
    },
  })
}

// ── Delete ────────────────────────────────────────────────────────────────────
export function useDeleteProduct() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id) => apiClient.delete(`/api/admin/products/${id}`).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-products'] }),
  })
}
