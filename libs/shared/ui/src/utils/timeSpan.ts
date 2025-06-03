export interface Duration {
  milliseconds(): number;
  asMilliseconds(): number;

  seconds(): number;
  asSeconds(): number;

  minutes(): number;
  asMinutes(): number;

  hours(): number;
  asHours(): number;

  days(): number;
  asDays(): number;

  weeks(): number;
  asWeeks(): number;

  months(): number;
  asMonths(): number;

  years(): number;
  asYears(): number;

  toISOString(): string;
  toJSON(): string;

  isValid(): boolean;
}
