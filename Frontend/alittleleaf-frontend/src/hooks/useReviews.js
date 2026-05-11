import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useProductReviews(productId, page = 1, pageSize = 5) {
  return useQuery({
    queryKey: ['reviews', productId, page],
    queryFn: () =>
      apiClient
        .get('/api/reviews', { params: { productId, page, pageSize } })
        .then((r) => r.data),
    enabled: !!productId,
    staleTime: 2 * 60 * 1000,
  })
}

export function useProductRating(productId) {
  return useQuery({
    queryKey: ['reviews', 'rating', productId],
    queryFn: () =>
      apiClient.get('/api/reviews/rating', { params: { productId } }).then((r) => r.data),
    enabled: !!productId,
    staleTime: 2 * 60 * 1000,
  })
}

export function useMyReview(productId) {
  return useQuery({
    queryKey: ['reviews', 'my', productId],
    queryFn: () =>
      apiClient.get('/api/reviews/my', { params: { productId } }).then((r) => r.data),
    enabled: !!productId,
  })
}

export function useCreateReview() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (dto) => apiClient.post('/api/reviews', dto).then((r) => r.data),
    onSuccess: (_, variables) => {
      qc.invalidateQueries({ queryKey: ['reviews', variables.productId] })
      qc.invalidateQueries({ queryKey: ['reviews', 'rating', variables.productId] })
      qc.invalidateQueries({ queryKey: ['reviews', 'my', variables.productId] })
    },
  })
}

export function useAdminReviews({ productId, page = 1, pageSize = 20 } = {}) {
  return useQuery({
    queryKey: ['admin', 'reviews', { productId, page }],
    queryFn: () =>
      apiClient
        .get('/api/admin/reviews', { params: { productId: productId || undefined, page, pageSize } })
        .then((r) => r.data),
    staleTime: 30 * 1000,
  })
}

export function useAdminDeleteReview() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id) => apiClient.delete(`/api/admin/reviews/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin', 'reviews'] }),
  })
}
