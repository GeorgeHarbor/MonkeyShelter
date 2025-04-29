// safe-date.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'safeDate',
  standalone: true,
  pure: true
})
export class SafeDatePipe implements PipeTransform {
  constructor(private datePipe: DatePipe) { }

  transform(value: any, format: string = 'dd/MM/yyyy'): any {
    if (value instanceof Date) {
      return this.datePipe.transform(value, format);
    }
    // const timestamp = Date.parse(value);
    // if (!isNaN(timestamp)) {
    //   return this.datePipe.transform(new Date(timestamp), format);
    // }
    return value;
  }
}
