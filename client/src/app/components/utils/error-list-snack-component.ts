import { CommonModule } from "@angular/common";
import { Component, Inject } from "@angular/core";
import { MAT_SNACK_BAR_DATA } from "@angular/material/snack-bar";

@Component({
  selector: 'app-error-list-snack',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ul class="snack-errors">
      <li *ngFor="let e of data">{{ e }}</li>
    </ul>
  `,
})
export class ErrorListSnackComponent {
  constructor(
    @Inject(MAT_SNACK_BAR_DATA) public data: string[]
  ) { }
}
