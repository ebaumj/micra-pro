import { type Insertable, type Selectable, type Updateable } from 'kysely';

export interface Database {
  espressoRecipe: EspressoRecipeTable;
  v60Recipe: V60RecipeTable;
  user: UserTable;
  migration: MigrationTable;
  image: ImageTable;
}

export interface MigrationTable {
  name: string;
  created_at: Date;
}

export type Migration = Selectable<MigrationTable>;
export type NewMigration = Insertable<MigrationTable>;

export interface UserTable {
  id: string;
  username: string;
  email: string;
  password: string;
  clientId: string;
  secret2fa: string;
  enabled2fa: boolean;
}

export type User = Selectable<UserTable>;
export type NewUser = Insertable<UserTable>;
export type UpdateUser = Updateable<UserTable>;

export interface RecipeBaseTable {
  id: string;
  userId: string;
  roastery: string;
  beanName: string;
}

export interface EspressoRecipeTable extends RecipeBaseTable {
  grindSetting: number;
  coffeeQuantity: number;
  inCupQuantity: number;
  brewTemperature: number;
  targetExtractionTimeInSeconds: number;
}

export type EspressoRecipe = Selectable<EspressoRecipeTable>;
export type NewEspressoRecipe = Insertable<EspressoRecipeTable>;
export type EspressoRecipeUpdate = Updateable<EspressoRecipeTable>;

export interface V60RecipeTable extends RecipeBaseTable {
  grindSetting: number;
  coffeeQuantity: number;
  inCupQuantity: number;
  brewTemperature: number;
}

export type V60Recipe = Selectable<V60RecipeTable>;
export type NewV60Recipe = Insertable<V60RecipeTable>;
export type V60RecipeUpdate = Updateable<V60RecipeTable>;

export interface ImageTable {
  version: string;
  link?: string;
  created_at: Date;
}

export type Image = Selectable<ImageTable>;
export type NewImage = Insertable<ImageTable>;
export type UpdateImage = Updateable<ImageTable>;
