type RecipeBase = {
  id: string;
  userId: string;
  roastery: string;
  beanName: string;
};

export type EspressoRecipe = RecipeBase & {
  grindSetting: number;
  coffeeQuantity: number;
  inCupQuantity: number;
  brewTemperature: number;
  targetExtractionTimeInSeconds: number;
};

export type V60Recipe = RecipeBase & {
  grindSetting: number;
  coffeeQuantity: number;
  inCupQuantity: number;
  brewTemperature: number;
};
