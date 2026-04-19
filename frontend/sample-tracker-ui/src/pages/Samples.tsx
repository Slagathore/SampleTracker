import { useEffect, useState } from 'react';
import { Plus, RefreshCw, Download, ChevronRight } from 'lucide-react';
import { samplesApi } from '../api/client';
import { StatusBadge } from '../components/samples/StatusBadge';
import { AddSampleModal } from '../components/samples/AddSampleModal';
import type { Sample, SampleStatus } from '../types';

const STATUS_ORDER: SampleStatus[] = [
  'Received', 'InPrep', 'InAnalysis', 'QCReview', 'Complete',
];

const FILTERS: { label: string; value: string }[] = [
  { label: 'All',         value: ''           },
  { label: 'Received',    value: 'Received'   },
  { label: 'In Prep',     value: 'InPrep'     },
  { label: 'In Analysis', value: 'InAnalysis' },
  { label: 'QC Review',   value: 'QCReview'   },
  { label: 'Complete',    value: 'Complete'   },
  { label: 'Rejected',    value: 'Rejected'   },
];

function nextStatus(current: SampleStatus): SampleStatus | null {
  const idx = STATUS_ORDER.indexOf(current);
  if (idx === -1 || idx === STATUS_ORDER.length - 1) return null;
  return STATUS_ORDER[idx + 1];
}

export default function Samples() {
  const [samples, setSamples]       = useState<Sample[]>([]);
  const [loading, setLoading]       = useState(true);
  const [filter, setFilter]         = useState('');
  const [modalOpen, setModalOpen]   = useState(false);
  const [advancing, setAdvancing]   = useState<number | null>(null);

  const load = (status = filter) => {
    setLoading(true);
    samplesApi.getAll(status || undefined)
      .then(res => setSamples(res.data))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [filter]);

  const handleAdvance = async (sample: Sample) => {
    const next = nextStatus(sample.status);
    if (!next) return;
    setAdvancing(sample.id);
    try {
      await samplesApi.updateStatus(sample.id, next);
      load();
    } finally {
      setAdvancing(null);
    }
  };

  const handleExport = async () => {
    const res = await samplesApi.exportCsv();
    const url = URL.createObjectURL(res.data);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'samples.csv';
    a.click();
    URL.revokeObjectURL(url);
  };

  return (
    <>
      <AddSampleModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreated={() => load()}
      />

      <div className="p-8 animate-fade-in">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-semibold text-white">Samples</h1>
            <p className="text-slate-500 text-sm mt-1">{samples.length} samples</p>
          </div>
          <div className="flex gap-2">
            <button
              onClick={handleExport}
              className="flex items-center gap-2 px-3 py-2 text-sm text-slate-400 hover:text-white border border-surface-500 hover:border-surface-400 rounded-lg transition-colors"
            >
              <Download size={14} />
              Export CSV
            </button>
            <button
              onClick={() => load()}
              className="flex items-center gap-2 px-3 py-2 text-sm text-slate-400 hover:text-white border border-surface-500 hover:border-surface-400 rounded-lg transition-colors"
            >
              <RefreshCw size={14} />
              Refresh
            </button>
            <button
              onClick={() => setModalOpen(true)}
              className="flex items-center gap-2 px-4 py-2 text-sm bg-brand-500 hover:bg-brand-600 text-white rounded-lg transition-colors"
            >
              <Plus size={14} />
              Add Sample
            </button>
          </div>
        </div>

        {/* Filter bar */}
        <div className="flex gap-1 mb-4 flex-wrap">
          {FILTERS.map(f => (
            <button
              key={f.value}
              onClick={() => setFilter(f.value)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                filter === f.value
                  ? 'bg-brand-900 text-brand-400 border border-brand-900'
                  : 'text-slate-500 hover:text-slate-300 border border-transparent hover:border-surface-500'
              }`}
            >
              {f.label}
            </button>
          ))}
        </div>

        <div className="bg-surface-800 border border-surface-500 rounded-xl overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-surface-500">
                {['Sample ID', 'Matrix', 'Method', 'Analyst', 'Collected', 'Status', ''].map(h => (
                  <th key={h} className="px-4 py-3 text-left text-xs text-slate-500 font-medium uppercase tracking-wide">
                    {h}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={7} className="text-center py-12 text-slate-600">Loading samples...</td>
                </tr>
              ) : samples.length === 0 ? (
                <tr>
                  <td colSpan={7} className="text-center py-12 text-slate-600">No samples yet.</td>
                </tr>
              ) : (
                samples.map(sample => {
                  const next = nextStatus(sample.status);
                  return (
                    <tr key={sample.id} className="border-b border-surface-700 hover:bg-surface-700/50 transition-colors">
                      <td className="px-4 py-3">
                        <span className="font-data text-brand-400">{sample.sampleId}</span>
                      </td>
                      <td className="px-4 py-3 text-slate-300">{sample.matrix}</td>
                      <td className="px-4 py-3">
                        {sample.methodCode
                          ? <span className="font-data text-slate-400">{sample.methodCode}</span>
                          : <span className="text-slate-600">—</span>}
                      </td>
                      <td className="px-4 py-3 text-slate-400">{sample.analystName ?? '—'}</td>
                      <td className="px-4 py-3 text-slate-500 font-data text-xs">
                        {new Date(sample.collectedAt).toLocaleDateString()}
                      </td>
                      <td className="px-4 py-3">
                        <StatusBadge status={sample.status} />
                      </td>
                      <td className="px-4 py-3">
                        {next && (
                          <button
                            onClick={() => handleAdvance(sample)}
                            disabled={advancing === sample.id}
                            title={`Advance to ${next}`}
                            className="flex items-center gap-1 px-2 py-1 text-xs text-slate-500 hover:text-brand-400 hover:bg-surface-600 rounded transition-colors disabled:opacity-40"
                          >
                            <ChevronRight size={14} />
                            {next}
                          </button>
                        )}
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
