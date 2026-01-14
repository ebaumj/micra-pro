import {
  type EspressoRecipe,
  type V60Recipe,
  type Update,
} from '@micra-pro/recipe-hub/data-definition';
import { migratedDb } from './database';
import { v4 as uuid } from 'uuid';
import type { Image, User } from './types';

type Entity = {
  id: string;
};

type Optional<T, K extends keyof T> = Omit<T, K> & Partial<Pick<T, K>>;

type Repository<T extends Entity> = {
  add: (value: Optional<T, 'id'>) => Promise<T>;
  update: (value: T) => Promise<T>;
  remove: (id: string) => Promise<string>;
  getAll: () => Promise<T[]>;
  getById: (id: string) => Promise<T>;
};

export const getUpdates = async (connection: string): Promise<Update[]> => {
  const db = await migratedDb(connection);
  const dbObject = await db.selectFrom('update').selectAll().execute();
  if (!dbObject) throw new Error();
  return dbObject;
};

export const getEspressoRecipeRepository: (
  connection: string,
) => Repository<EspressoRecipe> = (connection: string) => ({
  add: (value: Optional<EspressoRecipe, 'id'>) =>
    addEspressoRecipe(connection, value),
  update: (value: EspressoRecipe) => updateEspressoRecipe(connection, value),
  remove: (id: string) => removeEspressoRecipe(connection, id),
  getAll: () => getAllEspressoRecipes(connection),
  getById: (id: string) => getEspressoRecipe(connection, id),
});

export const getV60RecipeRepository: (
  connection: string,
) => Repository<V60Recipe> = (connection: string) => ({
  add: (value: Optional<V60Recipe, 'id'>) => addV60Recipe(connection, value),
  update: (value: V60Recipe) => updateV60Recipe(connection, value),
  remove: (id: string) => removeV60Recipe(connection, id),
  getAll: () => getAllV60Recipes(connection),
  getById: (id: string) => getV60Recipe(connection, id),
});

export const getUserRepository: (connection: string) => Repository<User> & {
  getByUsername: (username: string) => Promise<User>;
} = (connection: string) => ({
  add: (value: Omit<User, 'id'>) => addUser(connection, value),
  update: (value: User) => updateUser(connection, value),
  remove: (id: string) => removeUser(connection, id),
  getAll: () => getUsers(connection),
  getByUsername: (username: string) => getUserByUsername(connection, username),
  getById: (id: string) => getUserById(connection, id),
});

export const getImages = async (connection: string): Promise<Image[]> => {
  const db = await migratedDb(connection);
  const dbObject = await db.selectFrom('image').selectAll().execute();
  if (!dbObject) throw new Error();
  return dbObject;
};

const addEspressoRecipe = async (
  connection: string,
  value: Optional<EspressoRecipe, 'id'>,
): Promise<EspressoRecipe> => {
  const db = await migratedDb(connection);
  if (
    value.id &&
    (
      await db
        .selectFrom('espressoRecipe')
        .where('id', '=', value.id)
        .selectAll()
        .execute()
    ).length > 0
  )
    throw new Error();
  const id = value.id ?? uuid();
  const dbObject = await db
    .insertInto('espressoRecipe')
    .values({
      id,
      userId: value.userId,
      roastery: value.roastery,
      beanName: value.beanName,
      grindSetting: value.grindSetting,
      coffeeQuantity: value.coffeeQuantity,
      inCupQuantity: value.inCupQuantity,
      brewTemperature: value.brewTemperature,
      targetExtractionTimeInSeconds: value.targetExtractionTimeInSeconds,
    })
    .returningAll()
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return {
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
    targetExtractionTimeInSeconds: dbObject.targetExtractionTimeInSeconds,
  };
};

const updateEspressoRecipe = async (
  connection: string,
  value: EspressoRecipe,
): Promise<EspressoRecipe> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .updateTable('espressoRecipe')
    .set({
      id: value.id,
      userId: value.userId,
      roastery: value.roastery,
      beanName: value.beanName,
      grindSetting: value.grindSetting,
      coffeeQuantity: value.coffeeQuantity,
      inCupQuantity: value.inCupQuantity,
      brewTemperature: value.brewTemperature,
      targetExtractionTimeInSeconds: value.targetExtractionTimeInSeconds,
    })
    .where('id', '=', value.id)
    .returningAll()
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return {
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
    targetExtractionTimeInSeconds: dbObject.targetExtractionTimeInSeconds,
  };
};

const removeEspressoRecipe = async (
  connection: string,
  id: string,
): Promise<string> => {
  const db = await migratedDb(connection);
  const result = await db
    .deleteFrom('espressoRecipe')
    .where('id', '=', id)
    .returning('id')
    .executeTakeFirst();
  if (!result) throw new Error();
  return result.id;
};

const getAllEspressoRecipes = async (
  connection: string,
): Promise<EspressoRecipe[]> => {
  const db = await migratedDb(connection);
  const dbObjects = await db.selectFrom('espressoRecipe').selectAll().execute();
  return dbObjects.map((dbObject) => ({
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
    targetExtractionTimeInSeconds: dbObject.targetExtractionTimeInSeconds,
  }));
};

const getEspressoRecipe = async (
  connection: string,
  id: string,
): Promise<EspressoRecipe> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .selectFrom('espressoRecipe')
    .selectAll()
    .where('id', '=', id)
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return {
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
    targetExtractionTimeInSeconds: dbObject.targetExtractionTimeInSeconds,
  };
};

const addV60Recipe = async (
  connection: string,
  value: Optional<V60Recipe, 'id'>,
): Promise<V60Recipe> => {
  const db = await migratedDb(connection);
  if (
    value.id &&
    (
      await db
        .selectFrom('v60Recipe')
        .where('id', '=', value.id)
        .selectAll()
        .execute()
    ).length > 0
  )
    throw new Error();
  const id = value.id ?? uuid();
  const dbObject = await db
    .insertInto('v60Recipe')
    .values({
      id,
      userId: value.userId,
      roastery: value.roastery,
      beanName: value.beanName,
      grindSetting: value.grindSetting,
      coffeeQuantity: value.coffeeQuantity,
      inCupQuantity: value.inCupQuantity,
      brewTemperature: value.brewTemperature,
    })
    .returningAll()
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return {
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
  };
};

const updateV60Recipe = async (
  connection: string,
  value: V60Recipe,
): Promise<V60Recipe> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .updateTable('v60Recipe')
    .set({
      id: value.id,
      userId: value.userId,
      roastery: value.roastery,
      beanName: value.beanName,
      grindSetting: value.grindSetting,
      coffeeQuantity: value.coffeeQuantity,
      inCupQuantity: value.inCupQuantity,
      brewTemperature: value.brewTemperature,
    })
    .where('id', '=', value.id)
    .returningAll()
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return {
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
  };
};

const removeV60Recipe = async (
  connection: string,
  id: string,
): Promise<string> => {
  const db = await migratedDb(connection);
  const result = await db
    .deleteFrom('v60Recipe')
    .where('id', '=', id)
    .returning('id')
    .executeTakeFirst();
  if (!result) throw new Error();
  return result.id;
};

const getAllV60Recipes = async (connection: string): Promise<V60Recipe[]> => {
  const db = await migratedDb(connection);
  const dbObjects = await db.selectFrom('v60Recipe').selectAll().execute();
  return dbObjects.map((dbObject) => ({
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
  }));
};

const getV60Recipe = async (
  connection: string,
  id: string,
): Promise<V60Recipe> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .selectFrom('v60Recipe')
    .selectAll()
    .where('id', '=', id)
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return {
    id: dbObject.id,
    userId: dbObject.userId,
    roastery: dbObject.roastery,
    beanName: dbObject.beanName,
    grindSetting: dbObject.grindSetting,
    coffeeQuantity: dbObject.coffeeQuantity,
    inCupQuantity: dbObject.inCupQuantity,
    brewTemperature: dbObject.brewTemperature,
  };
};

const addUser = async (
  connection: string,
  value: Omit<User, 'id'>,
): Promise<User> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .insertInto('user')
    .values({ id: uuid(), ...value })
    .returningAll()
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return dbObject;
};

const updateUser = async (connection: string, value: User): Promise<User> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .updateTable('user')
    .set(value)
    .where('id', '=', value.id)
    .returningAll()
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return dbObject;
};

const removeUser = async (connection: string, id: string): Promise<string> => {
  const db = await migratedDb(connection);
  const result = await db
    .deleteFrom('user')
    .where('id', '=', id)
    .returning('id')
    .executeTakeFirst();
  if (!result) throw new Error();
  return result.id;
};

const getUsers = async (connection: string): Promise<User[]> => {
  const db = await migratedDb(connection);
  const dbObject = await db.selectFrom('user').selectAll().execute();
  if (!dbObject) throw new Error();
  return dbObject;
};

const getUserByUsername = async (
  connection: string,
  username: string,
): Promise<User> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .selectFrom('user')
    .selectAll()
    .where((exp) =>
      exp('username', '=', username).or(exp('email', '=', username)),
    )
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return dbObject;
};

const getUserById = async (connection: string, id: string): Promise<User> => {
  const db = await migratedDb(connection);
  const dbObject = await db
    .selectFrom('user')
    .selectAll()
    .where('id', '=', id)
    .executeTakeFirst();
  if (!dbObject) throw new Error();
  return dbObject;
};
