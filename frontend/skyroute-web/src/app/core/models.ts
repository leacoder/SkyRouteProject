export type CabinClass = 'Economy' | 'Business' | 'First';
export type DocumentType = 'Passport' | 'NationalId';

export interface Airport {
  code: string;
  city: string;
  country: string;
}

export interface SearchCriteria {
  originCode: string;
  destinationCode: string;
  departureDate: string; // yyyy-MM-dd
  passengers: number;
  cabin: CabinClass;
}

export interface FlightResult {
  id: string;
  provider: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  durationMinutes: number;
  cabin: string;
  perPassenger: number;
  total: number;
  passengerCount: number;
}

export interface SearchResponse {
  isInternational: boolean;
  requiredDocumentType: DocumentType;
  currency: string;
  results: FlightResult[];
}

export interface PassengerDetails {
  fullName: string;
  email: string;
  documentNumber: string;
}

export interface BookingRequest {
  flightId: string;
  originCode: string;
  destinationCode: string;
  departureDate: string;
  passengers: number;
  cabin: CabinClass;
  declaredTotal?: number;
  passenger: PassengerDetails;
}

export interface BookingResponse {
  referenceCode: string;
  provider: string;
  flightNumber: string;
  documentType: DocumentType;
  perPassenger: number;
  total: number;
  passengerCount: number;
  currency: string;
}
