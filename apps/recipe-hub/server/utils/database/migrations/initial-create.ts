import { Kysely, sql } from 'kysely';
import type { MigrationAction } from '../database';

async function up(db: Kysely<any>): Promise<void> {
  await db.schema
    .createTable('user')
    .addColumn('id', 'varchar', (col) => col.primaryKey())
    .addColumn('username', 'varchar', (col) => col.unique().notNull())
    .addColumn('email', 'varchar', (col) =>
      col.notNull().unique().defaultTo(''),
    )
    .addColumn('password', 'varchar', (col) => col.notNull())
    .addColumn('secret2fa', 'varchar', (col) => col.notNull())
    .addColumn('enabled2fa', 'boolean', (col) => col.notNull())
    .addColumn('clientId', 'varchar', (col) => col.notNull())
    .addColumn('created_at', 'varchar', (col) =>
      col.notNull().defaultTo(sql`now()`),
    )
    .execute();
  await db.schema
    .createTable('espressoRecipe')
    .addColumn('id', 'varchar', (col) => col.primaryKey())
    .addColumn('userId', 'varchar', (col) =>
      col.references('user.id').onDelete('cascade').notNull(),
    )
    .addColumn('roastery', 'varchar', (col) => col.notNull())
    .addColumn('beanName', 'varchar', (col) => col.notNull())
    .addColumn('grindSetting', 'double precision', (col) => col.notNull())
    .addColumn('coffeeQuantity', 'double precision', (col) => col.notNull())
    .addColumn('inCupQuantity', 'double precision', (col) => col.notNull())
    .addColumn('brewTemperature', 'double precision', (col) => col.notNull())
    .addColumn('targetExtractionTimeInSeconds', 'double precision', (col) =>
      col.notNull(),
    )
    .execute();
  await db.schema
    .createTable('v60Recipe')
    .addColumn('id', 'varchar', (col) => col.primaryKey())
    .addColumn('userId', 'varchar', (col) =>
      col.references('user.id').onDelete('cascade').notNull(),
    )
    .addColumn('roastery', 'varchar', (col) => col.notNull())
    .addColumn('beanName', 'varchar', (col) => col.notNull())
    .addColumn('grindSetting', 'double precision', (col) => col.notNull())
    .addColumn('coffeeQuantity', 'double precision', (col) => col.notNull())
    .addColumn('inCupQuantity', 'double precision', (col) => col.notNull())
    .addColumn('brewTemperature', 'double precision', (col) => col.notNull())
    .execute();
  await db.schema
    .createIndex('espressoRecipe_user_id_index')
    .on('espressoRecipe')
    .column('userId')
    .execute();
  await db.schema
    .createIndex('v60Recipe_user_id_index')
    .on('v60Recipe')
    .column('userId')
    .execute();
}

async function down(db: Kysely<any>): Promise<void> {
  await db.schema.dropTable('user').execute();
  await db.schema.dropTable('espressoRecipe').execute();
  await db.schema.dropTable('v60Recipe').execute();
}

export const initialCreateMigration: MigrationAction = {
  name: 'initial-create',
  up,
  down,
};
