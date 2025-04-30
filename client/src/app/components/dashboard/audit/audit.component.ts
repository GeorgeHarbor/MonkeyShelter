
import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ScrollableTableComponent } from '../../utils/scrollable-table/scrollable-table.component';
import { auditEnvironment } from '../../../environments/environment';
import { AuditLog } from '../../../models/AuditLog';
import { HttpClient } from '@angular/common/http';
import { AuditLogView } from '../../../models/AuditLogView';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [MatCardModule, MatIconModule, ScrollableTableComponent],
  templateUrl: './audit.component.html',
  styleUrl: './audit.component.scss'

})
export class AuditComponent implements OnInit {
  columns = [
    { field: 'eventType', header: 'Event Type' },
    { field: 'payload', header: 'Payload' },
    { field: 'receivedAt', header: 'Received At' },
  ];

  apiUrl = auditEnvironment.apiUrl
  logs: AuditLogView[] = []

  constructor(
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.http.get(this.apiUrl)
      .subscribe({
        next: res => {
          var auditLogs = res as AuditLog[];
          this.logs = auditLogs.map(a => ({
            ...a,
            receivedAt: new Date(a.receivedAt).toLocaleDateString('en-GB')
          }));
        },
        error: err => console.error(err)
      })
  };

}
