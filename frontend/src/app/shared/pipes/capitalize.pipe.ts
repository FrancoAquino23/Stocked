import { Pipe, PipeTransform } from '@angular/core';

// Pone en mayúscula la primera letra de todos los ingredientes en Spoonacular
@Pipe({
  name: 'capitalize',
  standalone: true,
})
export class CapitalizePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) {
      return value;
    }
    return value.charAt(0).toUpperCase() + value.slice(1);
  }
}
