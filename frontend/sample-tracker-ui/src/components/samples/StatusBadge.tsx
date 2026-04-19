import type { SampleStatus } from '../../types';

const STATUS_CONFIG: Record<SampleStatus, { label: string; classes: string; dot: string }> = {
  Received:   { label: 'Received',    classes: 'bg-slate-800 text-slate-300 border-slate-600',      dot: 'bg-slate-400' },
  InPrep:     { label: 'In Prep',     classes: 'bg-yellow-950 text-yellow-300 border-yellow-800',    dot: 'bg-yellow-400 animate-pulse' },
  InAnalysis: { label: 'In Analysis', classes: 'bg-blue-950 text-blue-300 border-blue-800',          dot: 'bg-blue-400 animate-pulse' },
  QCReview:   { label: 'QC Review',   classes: 'bg-purple-950 text-purple-300 border-purple-800',    dot: 'bg-purple-400 animate-pulse' },
  Complete:   { label: 'Complete',    classes: 'bg-emerald-950 text-emerald-300 border-emerald-700', dot: 'bg-emerald-400' },
  Rejected:   { label: 'Rejected',    classes: 'bg-red-950 text-red-300 border-red-800',             dot: 'bg-red-500' },
};

export function StatusBadge({ status }: { status: SampleStatus }) {
  const config = STATUS_CONFIG[status];
  return (
    <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md text-xs font-medium border ${config.classes}`}>
      <span className={`w-1.5 h-1.5 rounded-full ${config.dot}`} />
      {config.label}
    </span>
  );
}
