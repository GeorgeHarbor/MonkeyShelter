import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ColumnDef {
  field: string;
  header: string;
}

@Component({
  selector: 'scrollable-table',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './scrollable-table.component.html',
  styleUrls: ['./scrollable-table.component.scss'],
})
export class ScrollableTableComponent {
  /** columns definitions */
  @Input() columns: ColumnDef[] = [];
  /** data rows */
  @Input() data: any[] = [];
  /** whether to show the Edit/Delete buttons */
  @Input() showActions = false;

  /** emits the row when edit is clicked */
  @Output() edit = new EventEmitter<any>();
  /** emits the row when delete is clicked */
  @Output() delete = new EventEmitter<any>();

  /** build the row of column keys + optional "actions" column */
  displayedColumns(): string[] {
    const cols = this.columns.map(c => c.field);
    return this.showActions ? [...cols, 'actions'] : cols;
  }

}

