import { type Database } from './types';
import { Pool } from 'pg';
import { Kysely, PostgresDialect, sql } from 'kysely';
import { initialCreateMigration } from './migrations/initial-create';
import { imageTableMigration } from './migrations/image-table';

export type MigrationProcess = {
  name: string;
  up: (db: Kysely<any>) => Promise<void>;
  down: (db: Kysely<any>) => Promise<void>;
};

const getDatabase = (connection: string) =>
  new Kysely<Database>({
    dialect: new PostgresDialect({
      pool: new Pool({
        connectionString: connection,
        ssl: {
          rejectUnauthorized: false,
        },
      }),
    }),
  });

const migrations: MigrationProcess[] = [
  initialCreateMigration,
  imageTableMigration,
];

export const migratedDb = async (
  connection: string,
): Promise<Kysely<Database>> => {
  let latestMigration: { name: string } | undefined;
  const db = getDatabase(connection);
  try {
    latestMigration = (
      await db.selectFrom('migration').selectAll().execute()
    ).findLast((_) => true);
  } catch {
    await db.schema
      .createTable('migration')
      .addColumn('name', 'varchar', (col) => col.primaryKey())
      .addColumn('created_at', 'varchar', (col) =>
        col.defaultTo(sql`now()`).notNull(),
      )
      .execute();
  }
  for (const m of migrations.filter(
    (_, i) => i > migrations.findIndex((m) => m.name === latestMigration?.name),
  )) {
    await m.up(db);
    await db
      .insertInto('migration')
      .values({ name: m.name, created_at: new Date() })
      .execute();
  }
  return db;
};
