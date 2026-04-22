import { useQuery } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useProducts(params = {}) {
  // Strip keys whose value is null, undefined, or NaN so they never appear in the query string
  const cleanParams = Object.fromEntries(
    Object.entries(params).filter(([, v]) => v !== null && v !== undefined && !(typeof v === 'number' && isNaN(v)))
  )

  return useQuery({
    queryKey: ['products', cleanParams],
    queryFn: () =>
      apiClient.get('/api/products', { params: cleanParams }).then((r) => r.data),
    staleTime: 2 * 60 * 1000,
  })
}
