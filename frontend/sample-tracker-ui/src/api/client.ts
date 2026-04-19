import axios from 'axios';
import type { Sample, Method } from '../types';

export const api = axios.create({
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(err);
  }
);

export const authApi = {
  login: (email: string, password: string) =>
    api.post<{ token: string; displayName: string; role: string }>(
      '/api/auth/login', { email, password }),
  register: (email: string, password: string, displayName: string) =>
    api.post<{ token: string; displayName: string; role: string }>(
      '/api/auth/register', { email, password, displayName }),
  guest: () =>
    api.post<{ token: string; displayName: string; role: string }>('/api/auth/guest'),
};

export const samplesApi = {
  getAll:       (status?: string) =>
    api.get<Sample[]>('/api/samples', { params: status ? { status } : undefined }),
  create:       (data: Omit<Sample, 'id' | 'status' | 'methodCode' | 'analystName'>) =>
    api.post<Sample>('/api/samples', data),
  updateStatus: (id: number, status: string) =>
    api.patch(`/api/samples/${id}/status`, JSON.stringify(status)),
  delete:       (id: number) =>
    api.delete(`/api/samples/${id}`),
  getStats:     () =>
    api.get<{ status: string; count: number }[]>('/api/samples/stats'),
  exportCsv:    () =>
    api.get('/api/samples/export', { responseType: 'blob' }),
};

export const methodsApi = {
  getAll: () => api.get<Method[]>('/api/methods'),
};
