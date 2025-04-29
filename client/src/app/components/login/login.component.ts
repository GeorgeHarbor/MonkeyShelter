import { Component, OnInit, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { authEnvironment } from '../../environments/environment';
import { ShelterService } from '../../services/shelter.service';
import { MatSelectModule } from '@angular/material/select';
import { AuthService } from '../../services/auth.service'
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ErrorListSnackComponent } from '../utils/error-list-snack-component';
import { AuthResponse } from '../../models/dtos';
import { Router, RouterModule } from '@angular/router';

interface IRegisterForm {
  userName: string | ValidationErrors;
  email: string | ValidationErrors;
  password: string | ValidationErrors;
  confirmPassword: string | ValidationErrors;
  shelterId: string | ValidationErrors;
  acceptTerms: boolean | ValidationErrors;
}
interface ILoginForm {
  userName: string | ValidationErrors;
  password: string | ValidationErrors;
  rememberMe: false[];
}


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatTabsModule,
    MatCheckboxModule,
    MatSelectModule,
    MatSnackBarModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  registerForm: FormGroup;
  hideLoginPassword = true;
  hideRegisterPassword = true;
  hideConfirmPassword = true;
  selectedTabIndex = 0;
  shelters: any[] = [];

  constructor(
    private fb: FormBuilder,
    private shelterService: ShelterService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    this.loginForm = this.fb.group<ILoginForm>({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      rememberMe: [false]
    });

    this.registerForm = this.fb.group<IRegisterForm>({
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
      shelterId: ['', [Validators.required]],
      acceptTerms: [false, [Validators.requiredTrue]]
    }, { validators: this.checkPasswords });
  }

  ngOnInit() {
    this.shelterService.getShelters().subscribe({
      next: (data) => {
        this.shelters = data;
      },
      error: (err) => {
        console.error('Error fethching shelters:', err);
      }
    })
  }

  checkPasswords: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const group = control as FormGroup;
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { notMatching: true };
  }

  onLoginSubmit() {
    if (this.loginForm.valid) {
      const { userName, password } = this.loginForm.value;
      this.authService.login({ username: userName, password: password })
        .subscribe({
          next:
            (res: AuthResponse) => {
              localStorage.setItem('auth_token', res.token)
              localStorage.setItem('user', JSON.stringify({ id: res.id, username: res.userName, email: res.email }))
              this.router.navigate(['/dashboard']);
            },
          error:
            (err: any) => console.error(err)
        },
        )

    }
  }

  onRegisterSubmit() {
    if (!this.registerForm.valid) return;

    const userData = this.registerForm.value;

    this.authService.register({ username: userData.userName, password: userData.password, email: userData.email, shelterId: userData.shelterId })
      .subscribe({
        next: (response) => {
          if (response.status === 201) {
            this.selectedTabIndex = 0;
            this.snackBar.open('Registered!', 'Close', { duration: 3000 });
          }
        },
        error: (response: HttpErrorResponse) => {
          if (response.status === 400) {
            const errorObj = response.error.errors as Record<string, string[]>;;
            const errors: string[] = Object
              .entries(errorObj)
              .flatMap(
                ([, messages]: [string, string[]]) =>
                  messages
              );

            this.snackBar.openFromComponent(ErrorListSnackComponent, {
              data: errors,
              duration: 3000,
            })
          }
        }
      });
  }
}
