import { useQuery } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useDashboardStats({ startDate, endDate } = {}) {
  const params = {}
  if (startDate) params.startDate = startDate
  if (endDate)   params.endDate   = endDate

  return useQuery({
    queryKey: ['admin-dashboard', startDate ?? null, endDate ?? null],
    queryFn:  () => apiClient.get('/api/admin/dashboard', { params }).then((r) => r.data),
    staleTime: 60_000,
  })
}
