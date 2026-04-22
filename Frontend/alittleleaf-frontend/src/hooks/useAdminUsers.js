import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useAdminUsers({
  page = 1,
  pageSize = 10,
  keyword,
  isActive,
  userRole,
  userSex,
  sortBy,
  sortOrder,
} = {}) {
  const params = {
    page,
    pageSize,
    ...(keyword  !== undefined && keyword  !== '' ? { keyword }                         : {}),
    ...(isActive !== undefined && isActive !== '' ? { isActive }                         : {}),
    ...(userRole !== undefined && userRole !== '' ? { userRole }                         : {}),
    ...(userSex  !== undefined && userSex  !== '' ? { userSex }                          : {}),
    ...(sortBy   ? { sortBy, isDescending: sortOrder !== 'asc' }                         : {}),
  }

  return useQuery({
    queryKey: ['admin-users', params],
    queryFn:  () => apiClient.get('/api/admin/users', { params }).then((r) => r.data),
    staleTime: 30_000,
  })
}

export function useAdminUser(id) {
  return useQuery({
    queryKey: ['admin-user', id],
    queryFn:  () => apiClient.get(`/api/admin/users/${id}`).then((r) => r.data),
    enabled:  !!id,
    staleTime: 30_000,
  })
}

export function useUpdateAdminUser(id) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (dto) => apiClient.put(`/api/admin/users/${id}`, dto).then((r) => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] })
      queryClient.invalidateQueries({ queryKey: ['admin-user', id] })
    },
  })
}

export function useToggleUserActive(id) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (isActive) =>
      apiClient.put(`/api/admin/users/${id}`, { userIsActive: isActive }).then((r) => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] })
      queryClient.invalidateQueries({ queryKey: ['admin-user', id] })
    },
  })
}

export function useCreateAdminUser() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (dto) => apiClient.post('/api/admin/users', dto).then((r) => r.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] })
    },
  })
}
