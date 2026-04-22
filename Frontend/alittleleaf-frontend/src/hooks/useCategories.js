import { useQuery } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: () => apiClient.get('/api/categories').then((r) => r.data),
    staleTime: 5 * 60 * 1000, // 5 min — category tree rarely changes
  })
}
