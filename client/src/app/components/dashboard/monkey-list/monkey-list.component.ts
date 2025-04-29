import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ScrollableTableComponent } from '../../utils/scrollable-table/scrollable-table.component';
import { MatRow } from '@angular/material/table';
import { Monkey, MONKEY_DATA } from '../../../data/monkey-data';

@Component({
  selector: 'app-monkey-list',
  imports: [MatCardModule, ScrollableTableComponent],
  templateUrl: './monkey-list.component.html',
  styleUrl: './monkey-list.component.scss'
})
export class MonkeyListComponent {
  monkeys: Monkey[] = []
  columns = [
    { field: 'id', header: 'ID' },
    { field: 'name', header: 'Name' },
    { field: 'species', header: 'Species' },
    { field: 'age', header: 'Age' },
    { field: 'weightKg', header: 'Weight (kg)' },
    { field: 'arrivalDate', header: 'Arrival Date' },
  ];
  ngOnInit() {
    this.monkeys = MONKEY_DATA
  }


  onEdit = (e: MatRow) => console.log(e);
  onDelete = (e: MatRow) => console.log(e);
}
