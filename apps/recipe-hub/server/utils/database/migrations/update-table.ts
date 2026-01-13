import { Kysely, sql } from 'kysely';
import type { MigrationProcess } from '../database';

async function up(db: Kysely<any>): Promise<void> {
  await db.schema
    .createTable('update')
    .addColumn('id', 'serial', (col) => col.primaryKey())
    .addColumn('version', 'varchar', (col) => col.notNull())
    .addColumn('link', 'varchar', (col) => col.notNull())
    .addColumn('signature', 'varchar', (col) => col.notNull())
    .addColumn('created_at', 'varchar', (col) =>
      col.notNull().defaultTo(sql`now()`),
    )
    .execute();
}

async function down(db: Kysely<any>): Promise<void> {
  await db.schema.dropTable('update').execute();
}

export const updateTableMigration: MigrationProcess = {
  name: 'update-table',
  up,
  down,
};
