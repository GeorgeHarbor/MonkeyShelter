import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ScrollableTableComponent } from '../../utils/scrollable-table/scrollable-table.component';
import { MatRow } from '@angular/material/table';
import { HttpClient } from '@angular/common/http';
import { MonkeyService } from '../../../services/monkey.service';
import { ShelterService } from '../../../services/shelter.service';
import { SpeciesService } from '../../../services/species.service';
import { Monkey } from '../../../models/Monkey';
import { Species } from '../../../models/Species';
import { Shelter } from '../../../models/Shelter';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-monkey-list',
  imports: [MatCardModule, ScrollableTableComponent],
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
    private http: HttpClient,
    private monkeyService: MonkeyService,
    private shelterService: ShelterService,
    private speciesService: SpeciesService
  ) { }

  ngOnInit() {
    forkJoin({
      monkeys: this.monkeyService.getAll(),
      shelters: this.shelterService.getAll(),
      species: this.speciesService.getAll(),
    }).subscribe({
      next: ({ monkeys, shelters, species }) => {
        this.monkeys = monkeys;
        this.shelters = shelters;
        this.species = species;

        console.log(this)
      },
      error: (err) => {
        console.error(err)
      }
    })
  }


  onEdit = (e: MatRow) => console.log(e);
  onDelete = (e: MatRow) => console.log(e);
}
