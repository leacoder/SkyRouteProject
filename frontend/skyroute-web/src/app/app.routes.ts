import { Routes } from '@angular/router';
import { SearchPage } from './features/search/search-page';
import { BookingPage } from './features/booking/booking-page';

export const routes: Routes = [
  { path: '', component: SearchPage, title: 'SkyRoute — Search' },
  { path: 'booking', component: BookingPage, title: 'SkyRoute — Booking' },
  { path: '**', redirectTo: '' },
];
