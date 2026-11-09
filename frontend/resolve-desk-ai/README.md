# ResolveDesk AI frontend

Angular 21 with a prerendered public presentation and a client-rendered support
ticket demo. Requires Node.js 24 and npm 11.

## Development

```bash
npm ci
npm start
```

Open `http://localhost:4200/` for the presentation or `/dashboard` for the demo.
Start the ASP.NET Core backend with the HTTPS profile for the `/api` and `/health`
proxy configured in `proxy.conf.json`.

## Build and test

```bash
npm test -- --watch=false
npm run build:backend
```

`npm run build` generates `dist/resolve-desk-ai/browser`. `build:backend` also
replaces the generated backend `wwwroot`, copies licenses and removes scripts
from the prerendered error page so it can be returned at any missing URL.
Both output directories are excluded from Git.

## Rendering and SEO

- `app.routes.ts`: `/` loads the presentation; `/dashboard` and `/tickets/*` load
  the demo; unmatched routes display an error. Ticket identifiers must be GUIDs.
- `app.routes.server.ts`: prerenders only `/` and `/not-found`. Demo pages never
  fetch ticket data during the build.
- `PageTitleStrategy`: updates title, description, robots and canonical on every
  navigation. Only `/` is indexable and has a canonical link.
- `src/index.html`: non-indexable base document for client-rendered demo pages.
  Prerendering replaces its robots metadata for the public home.
- `public/robots.txt` and `public/sitemap.xml`: public crawl configuration. Demo
  URLs remain crawlable so search engines can read their `noindex` directives.

Production uses ASP.NET Core, without a Node SSR process.
`Hosting/WebsiteEndpoints.cs` serves the home, allows the demo shell only on known
routes, sends `X-Robots-Tag` on demo/API/error responses and returns HTTP 404 for
missing pages or tickets. Test HTTP statuses on the unified backend site rather
than the Angular development server.

See the [project README](../../README.md) and
[deployment guide](../../deployment/README.md) for validation and publication.
