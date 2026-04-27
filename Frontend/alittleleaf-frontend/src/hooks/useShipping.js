import { useQuery } from '@tanstack/react-query'
import apiClient from '../lib/apiClient'

export function useShippingFee({ districtId, wardCode, weight = 500, insuranceValue = 0 } = {}) {
  return useQuery({
    queryKey: ['ghn', 'fee', districtId, wardCode, weight, insuranceValue],
    queryFn:  () =>
      apiClient
        .post('/api/shipping/fee', { districtId, wardCode, weight, insuranceValue })
        .then((r) => r.data.fee),
    enabled:   !!districtId && !!wardCode,
    staleTime: 5 * 60 * 1000,
    retry:     1,
  })
}

export const FALLBACK_SHIPPING_FEE = 35_000

export function useProvinces() {
  return useQuery({
    queryKey: ['ghn', 'provinces'],
    queryFn:  () => apiClient.get('/api/shipping/provinces').then((r) => r.data),
    staleTime: Infinity,
  })
}

export function useDistricts(provinceId) {
  return useQuery({
    queryKey: ['ghn', 'districts', provinceId],
    queryFn:  () => apiClient.get('/api/shipping/districts', { params: { provinceId } }).then((r) => r.data),
    enabled:  !!provinceId,
    staleTime: Infinity,
  })
}

export function useWards(districtId) {
  return useQuery({
    queryKey: ['ghn', 'wards', districtId],
    queryFn:  () => apiClient.get('/api/shipping/wards', { params: { districtId } }).then((r) => r.data),
    enabled:  !!districtId,
    staleTime: Infinity,
  })
}
