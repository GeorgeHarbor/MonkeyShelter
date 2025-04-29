import { CommonModule } from "@angular/common";
import { Component, Inject } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";

export interface DialogField {
  name: string;
  label: string;
  type?: 'text' | 'email' | 'password' | 'select' | 'number';
  required?: boolean;
  value?: any;
  options?: { value: any; viewValue: string }[];  // for dropdowns
}

export interface InputDialogData {
  title: string;
  fields: DialogField[];
}

@Component({
  selector: 'app-input-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule],
  templateUrl: './input-dialog.component.html',
  styleUrls: ['./input-dialog.component.scss']
})
export class InputDialogComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<InputDialogComponent, any>,
    @Inject(MAT_DIALOG_DATA) public data: InputDialogData
  ) {
    const controlsConfig: { [key: string]: any } = {};
    data.fields.forEach(field => {
      controlsConfig[field.name] = [field.value || '', field.required ? Validators.required : []];
    });
    this.form = this.fb.group(controlsConfig);
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}

