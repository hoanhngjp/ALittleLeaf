import axios from 'axios'
import { useAuthStore } from '../store/useAuthStore'

// In Docker (nginx), VITE_API_URL is unset so baseURL is '' and the browser
// sends /api/... to the same origin, which nginx proxies to the backend.
// In local dev, VITE_API_URL=http://localhost:8081 points directly at the API.
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '',
})

// Attach JWT token from auth store on every request
apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// On 401, clear auth state so the user is prompted to log in again
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      useAuthStore.getState().logout()
    }
    return Promise.reject(error)
  }
)

export default apiClient
