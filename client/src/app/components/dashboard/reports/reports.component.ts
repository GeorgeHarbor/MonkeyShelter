import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ScrollableTableComponent } from '../../utils/scrollable-table/scrollable-table.component';
import { environment } from '../../../environments/environment';
import { ReportsService } from '../../../services/reports.service';
import { Report } from '../../../models/Report';
import { formatDate } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatIconButton } from '@angular/material/button';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [MatCardModule, MatIconModule, ScrollableTableComponent, MatFormFieldModule, MatDatepickerModule, FormsModule, MatNativeDateModule, MatInputModule, MatFormFieldModule, MatIconButton],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  columns = [
    { field: 'species', header: 'Species' },
    { field: 'count', header: 'Count' },
  ];
  fromDate?: Date;
  toDate?: Date;

  speciesReports: Report[] = []
  dateRangeReports: Report[] = []
  apiUrl = environment.apiUrl

  constructor(
    private reportsService: ReportsService
  ) { }

  ngOnInit(): void {
    this.reportsService.getAll().subscribe({
      next: result => {
        this.speciesReports = result;  // now truly an array
        console.log(result)
      },
      error: err => console.error(err)
    });
  }
  loadRange() {
    if (!this.fromDate || !this.toDate) {
      return; // or show validation
    }

    const from = formatDate(this.fromDate, 'yyyy-MM-dd', 'en-GB');
    const to = formatDate(this.toDate, 'yyyy-MM-dd', 'en-GB');

    this.reportsService.getInRange(from, to).subscribe({
      next: r => this.dateRangeReports = r,
      error: e => console.error(e)
    });
  }
}
