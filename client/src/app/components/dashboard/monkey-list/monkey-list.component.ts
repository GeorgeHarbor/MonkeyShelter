import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ScrollableTableComponent } from '../../utils/scrollable-table/scrollable-table.component';
import { MonkeyService } from '../../../services/monkey.service';
import { ShelterService } from '../../../services/shelter.service';
import { SpeciesService } from '../../../services/species.service';
import { Monkey } from '../../../models/Monkey';
import { Species } from '../../../models/Species';
import { Shelter } from '../../../models/Shelter';
import { forkJoin } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatIconButton } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { InputDialogComponent, InputDialogData } from '../../utils/input-dialog/input-dialog.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-monkey-list',
  standalone: true,
  imports: [MatCardModule, ScrollableTableComponent, MatIconModule, MatIconButton, MatDialogModule],
  templateUrl: './monkey-list.component.html',
  styleUrl: './monkey-list.component.scss'
})
export class MonkeyListComponent implements OnInit {
  monkeys: Monkey[] = []
  species: Species[] = []
  shelters: Shelter[] = []

  columns = [
    { field: 'name', header: 'Name' },
    { field: 'species', header: 'Species' },
    { field: 'shelterName', header: 'Shelter' },
    { field: 'weight', header: 'Weight (kg)' },
    { field: 'arrivalDate', header: 'Arrival Date' },
  ];
  columnsShelter = [
    { field: 'name', header: 'Name' },
    { field: 'location', header: 'Location' },
  ];
  columnsSpecies = [
    { field: 'name', header: 'Name' },
    { field: 'description', header: 'Description' },
  ];
  constructor(
    private monkeyService: MonkeyService,
    private shelterService: ShelterService,
    private speciesService: SpeciesService,
    private dialog: MatDialog,
    private http: HttpClient
  ) { }

  ngOnInit() {
    forkJoin({
      monkeys: this.monkeyService.getAll(),
      shelters: this.shelterService.getAll(),
      species: this.speciesService.getAll(),
    }).subscribe({
      next: ({ monkeys, shelters, species }) => {
        this.monkeys = monkeys.reverse();
        this.shelters = shelters.reverse();
        this.species = species.reverse();
        localStorage.setItem("shelters", JSON.stringify(shelters))
      },
      error: (err) => {
        console.error(err)
      }
    })
  }


  onEdit = (row: Monkey) => {

    const data: InputDialogData = {
      title: 'Enter new weight',
      fields: [
        { name: 'weight', label: 'Weight', type: 'number', required: true },
      ]
    };

    this.dialog.open<InputDialogComponent, InputDialogData, any>(InputDialogComponent, { data })
      .afterClosed()
      .subscribe(result => {
        if (result) {
          console.log('User input:', result);
          this.http.put(`${environment.apiUrl}/monkeys`, { monkeyId: row.id, newWeight: result.weight })
            .subscribe({
              next: () => {
                this.monkeys = this.monkeys.map(m =>
                  m.id === row.id
                    ? { ...m, weight: result.weight }
                    : m)
              },
              error: err => {
                console.error('Edit failed', err);
              }
            })
        }
      });

  };

  onDelete = (row: Monkey) => {
    const params = {
      monkeyId: row.id.toString(),
      departureDate: new Date().toISOString(),
      weightAtDeparture: row.weight.toString()
    };

    this.http.delete(`${environment.apiUrl}/monkeys`, { body: params })
      .subscribe({
        next: () => {
          this.monkeys = this.monkeys.filter(m => m.id !== row.id);
        },
        error: err => {
          console.error('Delete failed', err);
        }
      })


  };

  addMonkeyArrival = () => {
    const data: InputDialogData = {
      title: 'Enter Details',
      fields: [
        { name: 'name', label: 'Name', required: true },
        {
          name: 'shelter', label: 'Shelters', type: 'select',
          options: this.shelters.map(s => ({ value: s.id, viewValue: s.name })),
          required: true
        },
        {
          name: 'species', label: 'Species', type: 'select',
          options: this.species.map(s => ({ value: s.id, viewValue: s.name })),
          required: true
        },
        { name: 'weight', label: 'Weight', type: 'number', required: true },
      ]
    };
    this.dialog.open<InputDialogComponent, InputDialogData, any>(InputDialogComponent, { data })
      .afterClosed()
      .subscribe(result => {
        if (result) {
          this.http.post(`${environment.apiUrl}/monkeys`, { speciesId: result.species, shelterId: result.shelter, name: result.name, weight: result.weight })
            .subscribe({
              next: () => {
                var addedMonkey = result as Monkey;
                addedMonkey.species = this.species.find(x => x.id === addedMonkey.species)?.name!
                this.monkeys = [addedMonkey, ...this.monkeys]
              },
              error: err => {
                console.error('Edit failed', err);
              }
            })
        }
      });

  }
}
