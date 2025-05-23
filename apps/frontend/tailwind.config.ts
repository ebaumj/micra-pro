import { Config } from 'tailwindcss';
import tailwindConfig from '../../tailwind.config'; // eslint-disable-line @nx/enforce-module-boundaries
import { createGlobPatternsForDependencies } from '@nx/js/src/utils/generate-globs';

import { join } from 'path';

export default {
  presets: [tailwindConfig],
  content: [
    join(
      __dirname,
      '{src,pages,components,app}/**/*!(*.stories|*.spec).{ts,tsx,html}',
    ),
    ...createGlobPatternsForDependencies(
      __dirname,
      '/**/*!(*.stories|*.spec).{ts,tsx,html}',
    ),
  ],
} satisfies Config;
