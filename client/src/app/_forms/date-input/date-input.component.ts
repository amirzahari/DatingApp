import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {

  @Input() id: string;
  @Input() label: string;
  @Input() maxDate: Date;
  bsConfig?: Partial<BsDatepickerConfig>;
  colorTheme = 'theme-green';

  constructor(@Self() public ngControl:NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: this.colorTheme,
      dateInputFormat: 'DD MMMM YYYY'
    };
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

}
