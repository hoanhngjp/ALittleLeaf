import { create } from 'zustand'

export const useCartStore = create((set) => ({
  items: [],
  count: 0,

  setCart: (cart) =>
    set({
      items: cart.items ?? [],
      count: cart.items?.reduce((sum, i) => sum + i.quantity, 0) ?? 0,
    }),

  clearCart: () => set({ items: [], count: 0 }),
}))
