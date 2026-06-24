import { ValidatorFn, Validators } from '@angular/forms';
import { Airport, DocumentType } from './models';

// UX-only replica of the backend route rules. The backend remains the source of truth and re-validates
// every booking; this just gives the user instant feedback and the right field label.
export const PASSPORT_PATTERN = /^[A-Z][0-9]{7}$/; // e.g. A1234567
export const NATIONAL_ID_PATTERN = /^[0-9]{8}$/; // 8 digits

export function isInternational(origin?: Airport, destination?: Airport): boolean {
  if (!origin || !destination) return false;
  return origin.country !== destination.country;
}

export function requiredDocument(origin?: Airport, destination?: Airport): DocumentType {
  return isInternational(origin, destination) ? 'Passport' : 'NationalId';
}

export function documentLabel(type: DocumentType): string {
  return type === 'Passport' ? 'Passport Number' : 'National ID';
}

export function documentPlaceholder(type: DocumentType): string {
  return type === 'Passport' ? 'e.g. A1234567' : 'e.g. 12345678';
}

export function documentValidators(type: DocumentType): ValidatorFn[] {
  return type === 'Passport'
    ? [Validators.required, Validators.pattern(PASSPORT_PATTERN)]
    : [Validators.required, Validators.pattern(NATIONAL_ID_PATTERN)];
}
