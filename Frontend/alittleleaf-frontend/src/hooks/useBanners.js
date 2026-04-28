import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

const QUERY_KEY = ['admin', 'banners']

// ── Public ────────────────────────────────────────────────────────────────────

export function usePublicBanners() {
  return useQuery({
    queryKey: ['banners'],
    queryFn:  () => apiClient.get('/api/banners').then((r) => r.data),
    staleTime: 5 * 60 * 1000, // 5 min — banners change infrequently
  })
}

// ── Admin ─────────────────────────────────────────────────────────────────────

export function useAdminBanners() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn:  () => apiClient.get('/api/admin/banners').then((r) => r.data),
  })
}

export function useCreateBanner() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (formData) =>
      apiClient.post('/api/admin/banners', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      }).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEY }),
  })
}

export function useUpdateBanner() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, ...body }) =>
      apiClient.put(`/api/admin/banners/${id}`, body).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEY }),
  })
}

export function useDeleteBanner() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id) =>
      apiClient.delete(`/api/admin/banners/${id}`).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEY }),
  })
}
