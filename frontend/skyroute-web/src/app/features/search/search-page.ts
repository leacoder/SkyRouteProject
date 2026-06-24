import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FlightApiService } from '../../core/flight-api.service';
import { BookingState } from '../../core/booking-state.service';
import { Airport, FlightResult, SearchCriteria, SearchResponse } from '../../core/models';
import { SearchForm } from './search-form';
import { ResultsTable, SortKey } from './results-table';

@Component({
  selector: 'sky-search-page',
  imports: [SearchForm, ResultsTable],
  templateUrl: './search-page.html',
  styleUrl: './search-page.css',
})
export class SearchPage {
  private readonly api = inject(FlightApiService);
  private readonly router = inject(Router);
  private readonly bookingState = inject(BookingState);

  readonly airports = signal<Airport[]>([]);
  readonly results = signal<FlightResult[]>([]); // raw response, never mutated
  readonly sortKey = signal<SortKey>('priceAsc');
  readonly loading = signal(false);
  readonly searched = signal(false);
  readonly error = signal<string | null>(null);

  private lastResponse: SearchResponse | null = null;
  private lastCriteria: SearchCriteria | null = null;

  // Pure derivation: sort a COPY of the raw results. Changing the sort key never triggers a re-fetch.
  readonly sortedResults = computed(() => {
    const comparators: Record<SortKey, (a: FlightResult, b: FlightResult) => number> = {
      priceAsc: (a, b) => a.total - b.total,
      priceDesc: (a, b) => b.total - a.total,
      durationAsc: (a, b) => a.durationMinutes - b.durationMinutes,
      departureAsc: (a, b) => a.departureTime.localeCompare(b.departureTime),
    };
    return [...this.results()].sort(comparators[this.sortKey()]);
  });

  readonly isEmpty = computed(() => this.searched() && !this.loading() && this.results().length === 0);

  constructor() {
    this.api.airports().subscribe({
      next: (airports) => this.airports.set(airports),
      error: () => this.error.set('Could not load airports. Is the backend running?'),
    });
  }

  onSearch(criteria: SearchCriteria): void {
    this.loading.set(true);
    this.error.set(null);
    this.lastCriteria = criteria;
    this.api.search(criteria).subscribe({
      next: (response) => {
        this.lastResponse = response;
        this.results.set(response.results);
        this.searched.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(this.describeError(err));
        this.results.set([]);
        this.searched.set(true);
        this.loading.set(false);
      },
    });
  }

  onSortChange(key: SortKey): void {
    this.sortKey.set(key);
  }

  onSelect(flight: FlightResult): void {
    const criteria = this.lastCriteria;
    const response = this.lastResponse;
    if (!criteria || !response) return;

    const origin = this.airports().find((airport) => airport.code === criteria.originCode);
    const destination = this.airports().find((airport) => airport.code === criteria.destinationCode);
    if (!origin || !destination) return;

    this.bookingState.set({
      flight,
      criteria,
      origin,
      destination,
      isInternational: response.isInternational,
      requiredDocumentType: response.requiredDocumentType,
    });
    this.router.navigate(['/booking']);
  }

  private describeError(err: unknown): string {
    const problem = (err as { error?: { title?: string } })?.error;
    return problem?.title ?? 'Something went wrong with the search.';
  }
}
