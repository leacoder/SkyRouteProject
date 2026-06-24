import { Component, computed, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FlightApiService } from '../../core/flight-api.service';
import { BookingState } from '../../core/booking-state.service';
import { BookingRequest, BookingResponse } from '../../core/models';
import { documentLabel, documentPlaceholder, documentValidators } from '../../core/document-rules';

@Component({
  selector: 'sky-booking-page',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './booking-page.html',
  styleUrl: './booking-page.css',
})
export class BookingPage {
  private readonly api = inject(FlightApiService);
  private readonly bookingState = inject(BookingState);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  readonly context = this.bookingState.context;
  readonly submitting = signal(false);
  readonly confirmation = signal<BookingResponse | null>(null);
  readonly error = signal<string | null>(null);

  // The required document type comes from the route the user picked (the backend is authoritative).
  readonly documentType = computed(() => this.context()?.requiredDocumentType ?? 'NationalId');
  readonly documentLabel = computed(() => documentLabel(this.documentType()));
  readonly documentPlaceholder = computed(() => documentPlaceholder(this.documentType()));
  readonly documentHint = computed(() =>
    this.documentType() === 'Passport'
      ? 'Passport must be a letter followed by 7 digits (e.g. A1234567).'
      : 'National ID must be 8 digits.',
  );

  readonly form = this.fb.nonNullable.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    documentNumber: ['', documentValidators(this.documentType())],
  });

  constructor() {
    // Dynamic validation: swap the document validators whenever the route's required document changes.
    effect(() => {
      const control = this.form.controls.documentNumber;
      control.setValidators(documentValidators(this.documentType()));
      control.updateValueAndValidity();
    });
  }

  confirm(): void {
    const context = this.context();
    if (!context) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    const request: BookingRequest = {
      flightId: context.flight.id,
      originCode: context.criteria.originCode,
      destinationCode: context.criteria.destinationCode,
      departureDate: context.criteria.departureDate,
      passengers: context.criteria.passengers,
      cabin: context.criteria.cabin,
      declaredTotal: context.flight.total, // display-only; the server re-prices and cross-checks this
      passenger: this.form.getRawValue(),
    };

    this.api.book(request).subscribe({
      next: (response) => {
        this.confirmation.set(response);
        this.submitting.set(false);
      },
      error: (err) => {
        this.error.set(this.describeError(err));
        this.submitting.set(false);
      },
    });
  }

  backToSearch(): void {
    this.bookingState.clear();
    this.router.navigate(['/']);
  }

  private describeError(err: unknown): string {
    const problem = (err as { error?: { title?: string; errors?: Record<string, string[]> } })?.error;
    const documentErrors = problem?.errors?.['documentNumber'];
    if (documentErrors?.length) return documentErrors[0];
    return problem?.title ?? 'Could not confirm the booking.';
  }
}
