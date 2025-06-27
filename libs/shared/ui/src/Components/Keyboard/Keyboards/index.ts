import English from './English';
import German from './German';

export const keyboards = {
  en: English,
  de: German,
};

export type Layouts = keyof typeof keyboards;
export const LayoutOptions = Object.keys(keyboards);
