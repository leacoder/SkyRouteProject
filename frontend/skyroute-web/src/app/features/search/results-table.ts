import { Component, input, output } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FlightResult } from '../../core/models';

export type SortKey = 'priceAsc' | 'priceDesc' | 'durationAsc' | 'departureAsc';

@Component({
  selector: 'sky-results-table',
  imports: [DatePipe, DecimalPipe],
  templateUrl: './results-table.html',
  styleUrl: './results-table.css',
})
export class ResultsTable {
  readonly rows = input.required<FlightResult[]>();
  readonly sortKey = input.required<SortKey>();
  readonly sortChange = output<SortKey>();
  readonly select = output<FlightResult>();

  readonly sortOptions: ReadonlyArray<{ key: SortKey; label: string }> = [
    { key: 'priceAsc', label: 'Price (low → high)' },
    { key: 'priceDesc', label: 'Price (high → low)' },
    { key: 'durationAsc', label: 'Duration (shortest)' },
    { key: 'departureAsc', label: 'Departure time' },
  ];

  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h ${mins.toString().padStart(2, '0')}m`;
  }

  onSort(value: string): void {
    this.sortChange.emit(value as SortKey);
  }
}
