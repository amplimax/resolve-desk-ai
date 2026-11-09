# Pubblicazione e indicizzazione di ResolveDesk AI

## Hosting e GitHub

Il sito `https://resolvedeskai.anywaresolutions.eu/` è ospitato su SmarterASP.NET/IIS.
Il repository è [amplimax/resolve-desk-ai](https://github.com/amplimax/resolve-desk-ai),
branch `main`. GitHub conserva i sorgenti; **la pubblicazione avviene su
SmarterASP.NET tramite Visual Studio/Web Deploy o upload del pacchetto**.

In questa postazione esistono i profili Visual Studio locali
`amplimax-001-subsite7 - Web Deploy` e `amplimax-001-subsite7 - FTP`, nella cartella
`backend/ResolveDeskAI.Api/Properties/PublishProfiles/`. I file `.pubxml` e
`.pubxml.user` restano esclusi da Git. Su altre postazioni importare il profilo
dal pannello dell'hosting.

## Pubblicare con Visual Studio

1. Aggiornare `main` con `git pull --ff-only`.
2. Aprire `ResolveDeskAI.sln`. Servono .NET 9 SDK, Node.js 24 e npm 11.6.2 nel `PATH`.
3. Sul progetto `ResolveDeskAI.Api`, scegliere **Pubblica** e il profilo
   `amplimax-001-subsite7 - Web Deploy` già configurato per SmarterASP.NET.
4. Controllare destinazione, configurazione **Release**, ambiente Production e
   impostazioni di connessione/AI già utilizzate. Pubblicare l'intera app:
   cambiano sia il frontend sia le regole HTTP del backend.
5. La pubblicazione esegue automaticamente `npm ci` e `npm run build:backend`.
   Includere anche `web.config`: `httpErrors existingResponse="PassThrough"`
   preserva il corpo della pagina 404 restituita da ASP.NET Core.
6. Eseguire i controlli pubblici descritti sotto.

Questa modifica non richiede migrazioni del database.

## Pacchetto da terminale

Dalla radice del repository:

```powershell
dotnet publish backend/ResolveDeskAI.Api/ResolveDeskAI.Api.csproj --configuration Release --output artifacts/site
```

Il comando costruisce frontend e backend e prepara una cartella pronta per IIS.

Distribuire il pacchetto nella root dell'applicazione su SmarterASP.NET con Web
Deploy o con l'upload del provider, conservando impostazioni Production e segreti
già configurati. Il server richiede il runtime .NET 9; Node.js serve solo durante
la build. Eliminare i vecchi bundle con hash dalla destinazione usando l'opzione
del profilo per i file obsoleti e verificando l'anteprima di pubblicazione.

Connection string di produzione e chiavi AI non vanno aggiunte a Git. Per il
rollback ridistribuire un pacchetto precedente completo; lo schema dati non cambia.

## Verifica prima della pubblicazione

```powershell
npm ci --prefix frontend/resolve-desk-ai
npm test --prefix frontend/resolve-desk-ai -- --watch=false
npm run build:backend --prefix frontend/resolve-desk-ai
dotnet test ResolveDeskAI.sln --configuration Release
```

I test HTTP richiedono i file reali in `wwwroot` e usano un database isolato in
memoria, senza modificare dati di produzione o chiamare servizi AI esterni.
Dopo i controlli, pubblicare con Visual Studio o con il comando della sezione precedente.

## Verifica dopo la pubblicazione

```powershell
curl.exe -I https://resolvedeskai.anywaresolutions.eu/
curl.exe -I https://resolvedeskai.anywaresolutions.eu/dashboard
curl.exe -I https://resolvedeskai.anywaresolutions.eu/tickets
curl.exe -I https://resolvedeskai.anywaresolutions.eu/robots.txt
curl.exe -I https://resolvedeskai.anywaresolutions.eu/sitemap.xml
curl.exe -I https://resolvedeskai.anywaresolutions.eu/pagina-inesistente-verifica-seo
curl.exe -I https://resolvedeskai.anywaresolutions.eu/tickets/00000000-0000-0000-0000-000000000000
```

| Indirizzo | Risposta attesa |
|---|---|
| `/` | `200`, presentazione leggibile senza JavaScript, titolo, descrizione e canonical `https://resolvedeskai.anywaresolutions.eu/` |
| `/dashboard`, `/tickets`, `/tickets/new` | `200`, `X-Robots-Tag: noindex, follow` e meta robots `noindex` |
| Ticket esistente e modifica | `200`, `noindex` |
| Ticket inesistente, ID non valido, URL sconosciuto | `404`, pagina di errore statica, `noindex` |
| `/robots.txt` | `200`, crawling consentito e riferimento alla sitemap |
| `/sitemap.xml` | `200`, una sola URL: la home canonica |
| `/index.html` | `301` verso `/` |

Usare anche **Visualizza sorgente pagina** sulla home: titolo, descrizione,
canonical e testo devono essere presenti nella risposta originale. Verificare il
pulsante della demo e il ritorno alla home. IIS deve mantenere lo stato `404`
quando mostra la pagina di errore personalizzata.

## Google Search Console

1. Nella proprietà che comprende `resolvedeskai.anywaresolutions.eu`, inviare
   `https://resolvedeskai.anywaresolutions.eu/sitemap.xml` nella sezione **Sitemap**.
2. Eseguire **Controllo URL** su `https://resolvedeskai.anywaresolutions.eu/`,
   testare l'URL pubblicato e richiedere l'indicizzazione.
3. Controllare `/dashboard` e un URL ticket: Google deve poter leggere `noindex`.
   Nella sitemap devono comparire soltanto pagine pubbliche indicizzabili.
4. Monitorare il report di indicizzazione dopo la nuova scansione. L'esclusione
   delle vecchie pagine non è immediata al deploy: Google deve ricontrollarle.

Non aggiungere `Disallow` per dashboard o ticket: impedirebbe a Google di leggere
`noindex`. Vedere la [documentazione Google](https://developers.google.com/search/docs/crawling-indexing/block-indexing).
Il server restituisce veri `404` per evitare i
[soft 404 delle SPA](https://developers.google.com/search/docs/crawling-indexing/javascript/javascript-seo-basics).

## Manutenzione e pulizia

`wwwroot`, `dist`, `node_modules`, `bin`, `obj` e `artifacts` sono output generati,
esclusi da Git. La build sostituisce il solo `wwwroot` generato, eliminando i vecchi
bundle. Dopo una pulizia locale, `dotnet publish` o la pubblicazione da Visual
Studio reinstallano le dipendenze e rigenerano gli asset necessari. Per lavorare
con `npm start`, eseguire prima `npm ci`. Conservare migrazioni, configurazioni e
profili locali di pubblicazione.

Per nuove rotte demo aggiornare `app.routes.ts`, `Hosting/WebsiteEndpoints.cs` e
i test HTTP. Per nuove pagine pubbliche aggiornare anche prerendering, metadati,
canonical e sitemap.
