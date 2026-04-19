import { useEffect, useState } from 'react';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts';
import { samplesApi } from '../api/client';

const BAR_COLORS: Record<string, string> = {
  Received:   '#94a3b8',
  InPrep:     '#fbbf24',
  InAnalysis: '#60a5fa',
  QCReview:   '#c084fc',
  Complete:   '#34d399',
  Rejected:   '#f87171',
};

export default function Dashboard() {
  const [stats, setStats] = useState<{ status: string; count: number }[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    samplesApi.getStats()
      .then(res => setStats(res.data))
      .finally(() => setLoading(false));
  }, []);

  const total      = stats.reduce((sum, s) => sum + s.count, 0);
  const complete   = stats.find(s => s.status === 'Complete')?.count ?? 0;
  const inProgress = stats
    .filter(s => ['InPrep', 'InAnalysis', 'QCReview'].includes(s.status))
    .reduce((sum, s) => sum + s.count, 0);

  return (
    <div className="p-8 animate-fade-in">
      <div className="mb-8">
        <h1 className="text-2xl font-semibold text-white">Dashboard</h1>
        <p className="text-slate-500 text-sm mt-1">Sample status overview</p>
      </div>

      <div className="grid grid-cols-3 gap-4 mb-8">
        {[
          { label: 'Total Samples', value: total,      color: 'text-white'       },
          { label: 'In Progress',   value: inProgress, color: 'text-blue-400'    },
          { label: 'Complete',      value: complete,   color: 'text-emerald-400' },
        ].map(card => (
          <div key={card.label} className="bg-surface-800 border border-surface-500 rounded-xl p-5">
            <p className="text-slate-500 text-xs mb-2">{card.label}</p>
            <p className={`text-3xl font-display font-medium ${card.color}`}>
              {loading ? '—' : card.value}
            </p>
          </div>
        ))}
      </div>

      <div className="bg-surface-800 border border-surface-500 rounded-xl p-6">
        <h2 className="text-sm font-medium text-slate-300 mb-6">Samples by Status</h2>
        {loading ? (
          <div className="h-48 flex items-center justify-center text-slate-600 text-sm">
            Loading...
          </div>
        ) : (
          <ResponsiveContainer width="100%" height={220}>
            <BarChart data={stats} barSize={32}>
              <XAxis dataKey="status" tick={{ fill: '#64748b', fontSize: 12 }} axisLine={false} tickLine={false} />
              <YAxis tick={{ fill: '#64748b', fontSize: 12 }} axisLine={false} tickLine={false} allowDecimals={false} />
              <Tooltip
                contentStyle={{
                  backgroundColor: '#0f1a14',
                  border: '1px solid #2a3d2b',
                  borderRadius: '8px',
                  color: '#e2e8e4',
                  fontSize: '12px',
                }}
                cursor={{ fill: 'rgba(255,255,255,0.03)' }}
              />
              <Bar dataKey="count" radius={[4, 4, 0, 0]}>
                {stats.map(entry => (
                  <Cell key={entry.status} fill={BAR_COLORS[entry.status] ?? '#64748b'} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  );
}
