// scrollable-table.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SafeDatePipe } from '../../../utils/safe-date.pipe';
import { FormsModule } from '@angular/forms';

export interface ColumnDef {
  field: string;
  header: string;
}

@Component({
  selector: 'scrollable-table',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, SafeDatePipe, FormsModule],
  providers: [DatePipe],
  templateUrl: './scrollable-table.component.html',
  styleUrls: ['./scrollable-table.component.scss']
})
export class ScrollableTableComponent {
  @Input() columns: ColumnDef[] = [];
  @Input() data: any[] = [];
  @Input() showActions = false;

  @Output() edit = new EventEmitter();
  @Output() delete = new EventEmitter();

  displayedColumns(): string[] {
    const cols = this.columns.map(c => c.field);
    return this.showActions ? [...cols, 'actions'] : cols;
  }
}
