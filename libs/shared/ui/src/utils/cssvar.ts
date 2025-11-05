export const cssvar = (name: string) =>
  getComputedStyle(document.documentElement).getPropertyValue(name);
export const twColor = (name: string) => cssvar(`--color-${name}`);
