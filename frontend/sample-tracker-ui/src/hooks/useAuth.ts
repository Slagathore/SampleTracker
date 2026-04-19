import { create } from 'zustand';

interface AuthStore {
  token: string | null;
  displayName: string | null;
  role: string | null;
  login: (token: string, displayName: string, role: string) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
}

export const useAuth = create<AuthStore>((set, get) => ({
  token:       localStorage.getItem('token'),
  displayName: localStorage.getItem('displayName'),
  role:        localStorage.getItem('role'),

  login: (token, displayName, role) => {
    localStorage.setItem('token', token);
    localStorage.setItem('displayName', displayName);
    localStorage.setItem('role', role);
    set({ token, displayName, role });
  },

  logout: () => {
    localStorage.clear();
    set({ token: null, displayName: null, role: null });
  },

  isAuthenticated: () => !!get().token,
}));
