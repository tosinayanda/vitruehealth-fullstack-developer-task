
export interface Employee {
  id:         string;
  name:       string;
  department: string;
  riskLevel:  RiskLevel;

  suggestions?: Suggestion[];
}

export enum RiskLevel {
  High = "high",
  Low = "low",
  Medium = "medium",
}

export interface Suggestion {
  id:             string | null;
  employeeId:     string;
  name:   string;
  department?:     string;
  type:           string;
  description:    string;
  status:         string;
  priority:       string;
  source:         Source;
  dateCreated:    Date;
  dateUpdated:    Date;
  notes:          string;
  dateCompleted?: Date;
  createdBy?:     string;
}

export enum Source {
  Admin = "admin",
  Vida = "vida",
}

export enum SuggestionStatus {
    Pending = "Pending",
    In_Progress = "In_Progress",
    Overdue = "Overdue",
    Completed = "Completed",
    Dismissed = "Dismissed",
}

export enum SuggestionPriority {
    Low = "Low",
    Medium = "Medium",
    High = "High",
}

export enum SuggestionType {
    Equipment = "Equipment",
    Exercise = "Exercise",
    Behavioural = "Behavioural",
    Lifestyle = "Lifestyle",
}