import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Input() usersHome: any;
  @Output() cancelRegister = new EventEmitter();

  // model: any = {};
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];


  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(8)
      ]],
      confirmPassword: ['', [
        Validators.required,
        this.matchValue('password')
      ]]
    })
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });
  }

  // initializeForm() {
  //   this.registerForm = new FormGroup({
  //     username: new FormControl('',Validators.required),
  //     password: new FormControl('',[Validators.required, 
  //       Validators.minLength(4),Validators.maxLength(8)]),
  //     confirmPassword: new FormControl('',[Validators.required, this.matchValue('password')])
  //   })
  //   this.registerForm.controls.password.valueChanges.subscribe(() => {
  //     this.registerForm.controls.confirmPassword.updateValueAndValidity();
  //   });
  // }

  // matchValue(matchTo: string): ValidatorFn {
  //     return(control: AbstractControl) => {
  //       return control?.value === control?.parent?.controls[matchTo].value
  //         ? null : { isMatching: true}
  //     }
  // }

  matchValue(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === '' ? null : control?.value === control?.parent?.controls[matchTo].value
        ? null : { isMatching: true }
    }
  }


  register() {
    console.log(this.registerForm.value);
    // console.log(this.model);
    this.accountService.register(this.registerForm.value).subscribe(
      response => {
        //console.log(response);
        //this.cancel();
        this.router.navigateByUrl('/members');
      },
      error => {
        console.log(error);
        this.validationErrors = error;


      }
    );
  }

  cancel() {
    console.log('cancelled')
    this.cancelRegister.emit(false);
  }

}
