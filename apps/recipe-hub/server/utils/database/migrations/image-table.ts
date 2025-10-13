import { Kysely, sql } from 'kysely';
import type { MigrationAction } from '../database';

async function up(db: Kysely<any>): Promise<void> {
  await db.schema
    .createTable('image')
    .addColumn('id', 'serial', (col) => col.primaryKey())
    .addColumn('version', 'varchar', (col) => col.notNull())
    .addColumn('link', 'varchar')
    .addColumn('created_at', 'varchar', (col) =>
      col.notNull().defaultTo(sql`now()`),
    )
    .execute();
}

async function down(db: Kysely<any>): Promise<void> {
  await db.schema.dropTable('image').execute();
}

export const imageTableMigration: MigrationAction = {
  name: 'image-table',
  up,
  down,
};
