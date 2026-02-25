import { migrateAppConfig } from '../utils/appconfig';
import { defineNitroPlugin } from 'nitropack/runtime/plugin';

export default defineNitroPlugin(migrateAppConfig);
