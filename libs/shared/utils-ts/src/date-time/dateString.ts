const twoDigits = (value: number) => (value < 9 ? `0${value}` : `${value}`);

export const dateString = (date: Date): string =>
  `${twoDigits(date.getDate())}.${twoDigits(date.getMonth() + 1)}.${date.getFullYear()}`;
