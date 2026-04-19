import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import { Sidebar } from './components/layout/Sidebar';
import Dashboard from './pages/Dashboard';
import Samples from './pages/Samples';
import Login from './pages/Login';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated() ? <>{children}</> : <Navigate to="/login" replace />;
}

function AppLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex">
      <Sidebar />
      <main className="ml-56 flex-1 min-h-screen">{children}</main>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={
          <ProtectedRoute><AppLayout><Dashboard /></AppLayout></ProtectedRoute>
        } />
        <Route path="/samples" element={
          <ProtectedRoute><AppLayout><Samples /></AppLayout></ProtectedRoute>
        } />
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </BrowserRouter>
  );
}
