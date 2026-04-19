import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Beaker } from 'lucide-react';
import { authApi } from '../api/client';
import { useAuth } from '../hooks/useAuth';

export default function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleGuest = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await authApi.guest();
      login(res.data.token, res.data.displayName, res.data.role);
      navigate('/');
    } catch {
      setError('Guest login unavailable.');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      const res = await authApi.login(email, password);
      login(res.data.token, res.data.displayName, res.data.role);
      navigate('/');
    } catch {
      setError('Invalid email or password.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-surface-900 flex items-center justify-center p-4">
      <div className="w-full max-w-sm animate-fade-in">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-12 h-12 bg-surface-700 rounded-xl mb-4 border border-surface-500">
            <Beaker className="text-brand-400" size={24} />
          </div>
          <h1 className="font-display text-white text-xl tracking-wide">SampleTracker</h1>
          <p className="text-slate-500 text-sm mt-1">Sign in to your workspace</p>
        </div>

        <div className="bg-surface-800 border border-surface-500 rounded-xl p-6 space-y-4">
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-xs text-slate-400 mb-1.5">Email</label>
              <input
                type="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
                className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-brand-500 transition-colors"
                placeholder="analyst@lab.com"
              />
            </div>
            <div>
              <label className="block text-xs text-slate-400 mb-1.5">Password</label>
              <input
                type="password"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
                className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2.5 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-brand-500 transition-colors"
              />
            </div>
            {error && <p className="text-red-400 text-xs">{error}</p>}
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-brand-500 hover:bg-brand-600 disabled:opacity-50 text-white rounded-lg py-2.5 text-sm font-medium transition-colors"
            >
              {loading ? 'Signing in...' : 'Sign in'}
            </button>
          </form>

          <div className="relative my-1">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-surface-500" />
            </div>
            <div className="relative flex justify-center">
              <span className="bg-surface-800 px-3 text-xs text-slate-600">or</span>
            </div>
          </div>

          <button
            onClick={handleGuest}
            disabled={loading}
            className="w-full border border-surface-500 hover:border-surface-400 text-slate-400 hover:text-white rounded-lg py-2.5 text-sm transition-colors disabled:opacity-50"
          >
            Continue as Guest
          </button>
        </div>
      </div>
    </div>
  );
}
