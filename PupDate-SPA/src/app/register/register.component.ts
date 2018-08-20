import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();

  bsConfiguration:  Partial<BsDatepickerConfig>;
  model: any = {};
  registerForm: FormGroup;

  constructor(private authService: AuthService, 
              private alertify: AlertifyService,
              private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.createRegisterForm();
    this.bsConfiguration = {
      containerClass: 'theme-red'
    }
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(10)]],
      confirmPassword: ['', Validators.required] 
    }, {validator: this.passwordMatchValidator});
  }
  

  passwordMatchValidator(control: FormGroup) {
    return control.get('password').value === control.get('confirmPassword').value ? null : {'mismatch' : true};
  }

  register() {
    // this.authService.register(this.model).subscribe(() => {
    //   this.alertify.success("registration Successfull");
    // }, error => {
    //   this.alertify.error(error);
    // });
    console.log(this.registerForm.value);
  }

  cancel() {
    this.cancelRegister.emit(false);
  }



}
