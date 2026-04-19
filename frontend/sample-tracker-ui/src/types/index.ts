export type SampleStatus =
  | 'Received'
  | 'InPrep'
  | 'InAnalysis'
  | 'QCReview'
  | 'Complete'
  | 'Rejected';

export interface Sample {
  id: number;
  sampleId: string;
  matrix: string;
  collectedAt: string;
  status: SampleStatus;
  methodCode: string | null;
  analystName: string | null;
  notes: string | null;
}

export interface Method {
  id: number;
  code: string;
  name: string;
  instrument: string;
}

export interface AuthState {
  token: string | null;
  displayName: string | null;
  role: string | null;
}
