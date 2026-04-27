import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import ProtectedRoute from './components/ProtectedRoute'
import MainLayout      from './components/layout/MainLayout'
import LoginPage       from './pages/LoginPage'
import RegisterPage    from './pages/RegisterPage'
import HomePage        from './pages/HomePage'
import CollectionsPage    from './pages/CollectionsPage'
import ProductDetailPage  from './pages/ProductDetailPage'
import SearchResultsPage  from './pages/SearchResultsPage'
import CartPage              from './pages/CartPage'
import CheckoutPage         from './pages/CheckoutPage'
import CheckoutPaymentPage  from './pages/CheckoutPaymentPage'
import ProfilePage          from './pages/ProfilePage'
import AddressPage          from './pages/AddressPage'
import OrderDetailPage      from './pages/OrderDetailPage'
import PaymentResultPage    from './pages/PaymentResultPage'
import OrderSuccessPage     from './pages/OrderSuccessPage'
import AdminRoute              from './components/AdminRoute'
import AdminLayout             from './layouts/AdminLayout'
import AdminDashboardPage      from './pages/admin/AdminDashboardPage'
import AdminProductsPage       from './pages/admin/AdminProductsPage'
import AdminProductFormPage    from './pages/admin/AdminProductFormPage'
import AdminOrdersPage         from './pages/admin/AdminOrdersPage'
import AdminOrderDetailPage    from './pages/admin/AdminOrderDetailPage'
import AdminUsersPage          from './pages/admin/AdminUsersPage'
import AdminUserFormPage       from './pages/admin/AdminUserFormPage'
import GhnSimulatorPage        from './pages/admin/GhnSimulatorPage'

const queryClient = new QueryClient()

// ── Placeholder pages (replaced in Phases 11 – 12) ───────────────────────────
const Placeholder = ({ name }) => (
  <div className="container py-5">
    <h2>{name}</h2>
    <p className="text-muted">Coming soon.</p>
  </div>
)

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          {/* Public routes — wrapped in MainLayout (Header + Footer + Sidebar) */}
          <Route element={<MainLayout />}>
            <Route path="/"                  element={<HomePage />} />
            <Route path="/collections"        element={<CollectionsPage />} />
            <Route path="/collections/:slug" element={<CollectionsPage />} />
            <Route path="/products/:id"      element={<ProductDetailPage />} />
            <Route path="/search"            element={<SearchResultsPage />} />
          </Route>

          {/* Auth pages — standalone (own Header/Footer, no MainLayout) */}
          <Route path="/login"    element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* VNPay redirects here after payment — must be public (no JWT in redirect) */}
          <Route path="/payment-result" element={<PaymentResultPage />} />
          {/* COD order placed successfully */}
          <Route path="/order-success"  element={<OrderSuccessPage />} />

          {/* Checkout — protected, own CheckoutLayout (no MainLayout) */}
          <Route element={<ProtectedRoute />}>
            <Route path="/checkout"         element={<CheckoutPage />} />
            <Route path="/checkout/payment" element={<CheckoutPaymentPage />} />
          </Route>

          {/* Protected customer routes — inside MainLayout */}
          <Route element={<ProtectedRoute />}>
            <Route element={<MainLayout />}>
              <Route path="/cart"             element={<CartPage />} />
              <Route path="/orders"           element={<Placeholder name="Orders" />} />
              <Route path="/orders/:id"       element={<Placeholder name="Order Detail" />} />
              <Route path="/profile"                  element={<ProfilePage />} />
              <Route path="/profile/addresses"      element={<AddressPage />} />
              <Route path="/profile/orders/:id"     element={<OrderDetailPage />} />
            </Route>
          </Route>

          {/* Admin routes — AdminRoute checks role=admin, AdminLayout provides the shell */}
          <Route element={<AdminRoute />}>
            <Route element={<AdminLayout />}>
              <Route path="/admin"                    element={<AdminDashboardPage />} />
              <Route path="/admin/products"           element={<AdminProductsPage />} />
              <Route path="/admin/products/new"       element={<AdminProductFormPage />} />
              <Route path="/admin/products/:id"       element={<AdminProductFormPage />} />
              <Route path="/admin/orders"             element={<AdminOrdersPage />} />
              <Route path="/admin/orders/:id"         element={<AdminOrderDetailPage />} />
              <Route path="/admin/users"              element={<AdminUsersPage />} />
              <Route path="/admin/users/:id"          element={<AdminUserFormPage />} />
              <Route path="/admin/ghn-simulator"      element={<GhnSimulatorPage />} />
            </Route>
          </Route>

          {/* Catch-all */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}
