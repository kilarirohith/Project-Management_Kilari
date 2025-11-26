// src/app/models/project.model.ts

export type ProjectStatus = 'Running' | 'Completed' | 'Delayed' | 'OnHold';

export interface ProjectDTO {
  id: number;
  projectCode: string;
  projectName: string;
  projectType: string;
  clientName: string;
  clientLocation?: string | null;
  unit?: string | null;
  milestone?: string | null;
  planStartDate?: string | null;
  planEndDate?: string | null;
  actualStartDate?: string | null;
  actualEndDate?: string | null;
  elapsedDays?: number | null;

  status: ProjectStatus; 
}

export interface CreateProjectPayload {
  projectCode: string;
  projectName: string;
  projectType: string;
  clientId: number;
  clientLocation?: string | null;
  unit?: string | null;
  milestone?: string | null;
  planStartDate?: string | null;
  planEndDate?: string | null;
  status: ProjectStatus;  // 👈
}

export interface UpdateProjectPayload extends CreateProjectPayload {
  actualStartDate?: string | null;
  actualEndDate?: string | null;
  elapsedDays?: number | null;
}

export interface DashboardProject {
  id: number;
  code: string;
  name: string;
  type: string;
  client: string;
  location: string;
  unit: string;
  milestone: string;
  planStart: string;
  planEnd: string;
  actualStart: string;
  actualEnd: string;
  elapsedDays: number | '-';
  status: ProjectStatus;
}

export interface ProjectType {
  id: number;
  projectName: string;
}

export interface Milestone {
  id: number;
  name: string;
}
