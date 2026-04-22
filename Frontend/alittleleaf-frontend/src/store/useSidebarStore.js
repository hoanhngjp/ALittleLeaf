import { create } from 'zustand'

/**
 * Global sidebar state.
 * mode: 'cart' | 'search' | 'menu'
 */
export const useSidebarStore = create((set) => ({
  isOpen: false,
  mode:   'cart',

  openCart:   () => set({ isOpen: true,  mode: 'cart'   }),
  openSearch: () => set({ isOpen: true,  mode: 'search' }),
  openMenu:   () => set({ isOpen: true,  mode: 'menu'   }),
  close:      () => set({ isOpen: false }),
}))
