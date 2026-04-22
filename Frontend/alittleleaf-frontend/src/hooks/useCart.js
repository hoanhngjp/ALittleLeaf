import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useEffect } from 'react'
import apiClient from '../lib/apiClient'
import { useCartStore } from '../store/useCartStore'
import { useAuthStore } from '../store/useAuthStore'

// ── Fetch cart ────────────────────────────────────────────────────────────────
export function useCartQuery() {
  const accessToken = useAuthStore((s) => s.accessToken)
  const setCart     = useCartStore((s) => s.setCart)

  const query = useQuery({
    queryKey: ['cart'],
    queryFn:  () => apiClient.get('/api/cart').then((r) => r.data),
    enabled:  !!accessToken,
    staleTime: 0,
  })

  // Keep Zustand count in sync whenever query data changes
  useEffect(() => {
    if (query.data) setCart(query.data)
  }, [query.data, setCart])

  return query
}

// ── Add item ──────────────────────────────────────────────────────────────────
export function useAddToCart() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ productId, quantity }) =>
      apiClient.post('/api/cart/items', { productId, quantity }).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['cart'] }),
  })
}

// ── Remove item ───────────────────────────────────────────────────────────────
export function useRemoveCartItem() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (productId) =>
      apiClient.delete(`/api/cart/items/${productId}`).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['cart'] }),
  })
}

// ── Update quantity ───────────────────────────────────────────────────────────
export function useUpdateCartItem() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ productId, quantity }) =>
      apiClient.put(`/api/cart/items/${productId}`, { quantity }).then((r) => r.data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['cart'] }),
  })
}
