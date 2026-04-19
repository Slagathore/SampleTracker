import { NavLink } from 'react-router-dom';
import { LayoutDashboard, FlaskConical, LogOut, Beaker } from 'lucide-react';
import { useAuth } from '../../hooks/useAuth';

const navItems = [
  { to: '/',        icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/samples', icon: FlaskConical,    label: 'Samples'   },
];

export function Sidebar() {
  const { displayName, logout } = useAuth();

  return (
    <aside className="w-56 h-screen bg-surface-800 border-r border-surface-500 flex flex-col fixed left-0 top-0">
      <div className="px-5 py-6 border-b border-surface-500">
        <div className="flex items-center gap-2">
          <Beaker className="text-brand-400" size={20} />
          <span className="font-display text-brand-400 text-sm tracking-widest uppercase">
            SampleTracker
          </span>
        </div>
      </div>

      <nav className="flex-1 px-3 py-4 space-y-1">
        {navItems.map(({ to, icon: Icon, label }) => (
          <NavLink
            key={to}
            to={to}
            end
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors ${
                isActive
                  ? 'bg-brand-900 text-brand-400 font-medium'
                  : 'text-slate-400 hover:text-slate-200 hover:bg-surface-700'
              }`
            }
          >
            <Icon size={16} />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="px-4 py-4 border-t border-surface-500">
        <p className="text-xs text-slate-500 mb-3 truncate">{displayName}</p>
        <button
          onClick={logout}
          className="flex items-center gap-2 text-xs text-slate-500 hover:text-red-400 transition-colors"
        >
          <LogOut size={14} />
          Sign out
        </button>
      </div>
    </aside>
  );
}
