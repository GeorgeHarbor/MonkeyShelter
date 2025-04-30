export interface AuditLog {
  id: string;
  eventType: string;
  payload: string;
  receivedAt: Date;
}
