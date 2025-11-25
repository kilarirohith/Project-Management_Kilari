// src/app/models/project.model.ts

export type ProjectStatus = 'Running' | 'Completed' | 'Delayed' | 'OnHold' | 'Pending';

export interface ProjectDTO {
  id: number;
  projectName: string;
  projectCode: string;
  projectType?: string;

  clientId: number;
  clientName?: string;

  clientLocation?: string;
  unit?: string;
  milestone?: string;

  planStartDate?: string;
  planEndDate?: string;
  actualStartDate?: string;
  actualEndDate?: string;

  status: string;
}

export interface DashboardProject {
  id: number;          // used for edit/delete
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
  status: ProjectStatus;
}

// Optional helpers (for milestone/project master lists)
export interface ProjectType {
  id: number;
  projectName: string;
}

export interface Milestone {
  id: number;
  name: string;
}
