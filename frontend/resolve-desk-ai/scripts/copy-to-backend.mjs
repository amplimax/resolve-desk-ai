import {
  access,
  cp,
  mkdir,
  readFile,
  rm,
  writeFile,
} from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const frontendRoot = path.resolve(scriptDirectory, '..');

const buildRoot = path.join(
  frontendRoot,
  'dist',
  'resolve-desk-ai',
);

const browserSource = path.join(buildRoot, 'browser');
const licensesSource = path.join(
  buildRoot,
  '3rdpartylicenses.txt',
);

const backendWwwroot = path.resolve(
  frontendRoot,
  '..',
  '..',
  'backend',
  'ResolveDeskAI.Api',
  'wwwroot',
);

await access(path.join(browserSource, 'index.html'));

await rm(backendWwwroot, {
  recursive: true,
  force: true,
});

await mkdir(backendWwwroot, {
  recursive: true,
});

await cp(browserSource, backendWwwroot, {
  recursive: true,
});

const licenses = await readFile(licensesSource, 'utf8');

const normalizedLicenses = licenses
  .split(/\r?\n/)
  .map((line) => line.trimEnd())
  .join('\n');

await writeFile(
  path.join(backendWwwroot, '3rdpartylicenses.txt'),
  normalizedLicenses.endsWith('\n')
    ? normalizedLicenses
    : `${normalizedLicenses}\n`,
  'utf8',
);

console.log(`Angular application copied to ${backendWwwroot}`);
