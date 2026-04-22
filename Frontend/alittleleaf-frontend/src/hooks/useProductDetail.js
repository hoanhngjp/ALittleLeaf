import { useQuery } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useProductDetail(id) {
  return useQuery({
    queryKey: ['product', id],
    queryFn: () => apiClient.get(`/api/products/${id}`).then((r) => r.data),
    enabled: !!id,
    staleTime: 2 * 60 * 1000,
  })
}

export function useRelatedProducts(id) {
  return useQuery({
    queryKey: ['products', 'related', id],
    queryFn: async () => {
      try {
        const r = await apiClient.get(`/api/products/${id}/related`)
        return r.data
      } catch (err) {
        // 404 means no related products — treat as empty list, not an error
        if (err?.response?.status === 404) return []
        throw err
      }
    },
    enabled: !!id,
    staleTime: 2 * 60 * 1000,
  })
}
