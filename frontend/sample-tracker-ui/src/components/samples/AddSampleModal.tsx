import { useEffect, useState } from 'react';
import { X } from 'lucide-react';
import { samplesApi, methodsApi } from '../../api/client';
import type { Method } from '../../types';

interface Props {
  open: boolean;
  onClose: () => void;
  onCreated: () => void;
}

const MATRICES = ['Water', 'Soil', 'Effluent', 'Air', 'Sediment', 'Tissue'];

export function AddSampleModal({ open, onClose, onCreated }: Props) {
  const [methods, setMethods] = useState<Method[]>([]);
  const [sampleId, setSampleId]     = useState('');
  const [matrix, setMatrix]         = useState('');
  const [collectedAt, setCollectedAt] = useState('');
  const [methodId, setMethodId]     = useState('');
  const [notes, setNotes]           = useState('');
  const [loading, setLoading]       = useState(false);
  const [error, setError]           = useState('');

  useEffect(() => {
    if (open) methodsApi.getAll().then(r => setMethods(r.data));
  }, [open]);

  const reset = () => {
    setSampleId(''); setMatrix(''); setCollectedAt('');
    setMethodId(''); setNotes(''); setError('');
  };

  const handleClose = () => { reset(); onClose(); };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      await samplesApi.create({
        sampleId,
        matrix,
        collectedAt: new Date(collectedAt).toISOString(),
        methodId:   methodId ? parseInt(methodId) : null,
        analystId:  null,
        notes:      notes || null,
      } as any);
      reset();
      onCreated();
      onClose();
    } catch {
      setError('Failed to create sample. Check all fields and try again.');
    } finally {
      setLoading(false);
    }
  };

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/60" onClick={handleClose} />
      <div className="relative bg-surface-800 border border-surface-500 rounded-xl w-full max-w-md animate-slide-up">
        <div className="flex items-center justify-between px-6 py-4 border-b border-surface-500">
          <h2 className="text-white font-medium">Add Sample</h2>
          <button onClick={handleClose} className="text-slate-500 hover:text-white transition-colors">
            <X size={18} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="px-6 py-4 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-xs text-slate-400 mb-1.5">Sample ID *</label>
              <input
                value={sampleId}
                onChange={e => setSampleId(e.target.value)}
                required
                placeholder="TRA-2024-001"
                className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-brand-500 transition-colors"
              />
            </div>
            <div>
              <label className="block text-xs text-slate-400 mb-1.5">Matrix *</label>
              <input
                list="matrix-options"
                value={matrix}
                onChange={e => setMatrix(e.target.value)}
                required
                placeholder="Water"
                className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-brand-500 transition-colors"
              />
              <datalist id="matrix-options">
                {MATRICES.map(m => <option key={m} value={m} />)}
              </datalist>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-xs text-slate-400 mb-1.5">Collected Date *</label>
              <input
                type="date"
                value={collectedAt}
                onChange={e => setCollectedAt(e.target.value)}
                required
                className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2 text-sm text-white focus:outline-none focus:border-brand-500 transition-colors scheme-dark"
              />
            </div>
            <div>
              <label className="block text-xs text-slate-400 mb-1.5">Method</label>
              <select
                value={methodId}
                onChange={e => setMethodId(e.target.value)}
                className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2 text-sm text-white focus:outline-none focus:border-brand-500 transition-colors"
              >
                <option value="">— None —</option>
                {methods.map(m => (
                  <option key={m.id} value={m.id}>{m.code} — {m.name}</option>
                ))}
              </select>
            </div>
          </div>

          <div>
            <label className="block text-xs text-slate-400 mb-1.5">Notes</label>
            <textarea
              value={notes}
              onChange={e => setNotes(e.target.value)}
              rows={2}
              placeholder="Optional notes..."
              className="w-full bg-surface-900 border border-surface-500 rounded-lg px-3 py-2 text-sm text-white placeholder-slate-600 focus:outline-none focus:border-brand-500 transition-colors resize-none"
            />
          </div>

          {error && <p className="text-red-400 text-xs">{error}</p>}

          <div className="flex justify-end gap-2 pt-1">
            <button
              type="button"
              onClick={handleClose}
              className="px-4 py-2 text-sm text-slate-400 hover:text-white border border-surface-500 rounded-lg transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 text-sm bg-brand-500 hover:bg-brand-600 disabled:opacity-50 text-white rounded-lg transition-colors"
            >
              {loading ? 'Creating...' : 'Create Sample'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
