import { Component, inject, input, output } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Airport, CabinClass, SearchCriteria } from '../../core/models';

/** Cross-field validator: origin and destination must differ. */
function distinctAirports(group: AbstractControl): ValidationErrors | null {
  const origin = group.get('originCode')?.value;
  const destination = group.get('destinationCode')?.value;
  return origin && destination && origin === destination ? { sameAirport: true } : null;
}

@Component({
  selector: 'sky-search-form',
  imports: [ReactiveFormsModule],
  templateUrl: './search-form.html',
  styleUrl: './search-form.css',
})
export class SearchForm {
  readonly airports = input.required<Airport[]>();
  readonly loading = input(false);
  readonly search = output<SearchCriteria>();

  private readonly fb = inject(FormBuilder);
  readonly cabins: CabinClass[] = ['Economy', 'Business', 'First'];

  // Pre-filled with a sensible international route so the demo is one click.
  readonly form = this.fb.nonNullable.group(
    {
      originCode: ['JFK', Validators.required],
      destinationCode: ['LHR', Validators.required],
      departureDate: ['2026-07-15', Validators.required],
      passengers: [2, [Validators.required, Validators.min(1), Validators.max(9)]],
      cabin: ['Economy' as CabinClass, Validators.required],
    },
    { validators: distinctAirports },
  );

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.search.emit(this.form.getRawValue());
  }
}
