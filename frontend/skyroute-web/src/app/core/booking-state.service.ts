import { Injectable, signal } from '@angular/core';
import { Airport, DocumentType, FlightResult, SearchCriteria } from './models';

/** Everything the booking screen needs about the flight the user picked on the search screen. */
export interface BookingContext {
  flight: FlightResult;
  criteria: SearchCriteria;
  origin: Airport;
  destination: Airport;
  isInternational: boolean;
  requiredDocumentType: DocumentType;
}

/** Hands the selected flight + route context from the search route to the booking route. */
@Injectable({ providedIn: 'root' })
export class BookingState {
  readonly context = signal<BookingContext | null>(null);

  set(context: BookingContext): void {
    this.context.set(context);
  }

  clear(): void {
    this.context.set(null);
  }
}
