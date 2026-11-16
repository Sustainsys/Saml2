# Security Policy

| Version | Supported          | Action Required |
| ------- | ------------------ | ------ |
| Kentor.AuthServices (any version) | :x: | Migrate to latest Sustainsys.Saml2 2.x.y |
| Sustainsys.Saml2 latest 1.x.y   | :warning: | Migrate to latest Sustainsys.Saml2 2.x.y before end of March 2027 |
| Sustainsys.Saml2 latest 2.x.y   | :white_check_mark: |
| Sustainsys.Saml2 develop branch, planned major version 3. | :construction: | Wait for develpment to get ready. Or help out! |

Version 1.x has been on maintenance mode for years and the number of downloads is very low, so it is time to sunset it.

Starting in November 2026, Version 1.x packages will be marked as deprecated on NuGet to encourage upgrading. Security updates
will be available until end of March 2027.

The 2.x version/branch only receives security updates or critical functionality patches.

The main branch is for development of a new major version 3. It is work in progress and is not supported yet.

## Reporting a Vulnerability
Report any security issues through GitHub's [vulnerability reporting](https://github.com/Sustainsys/Saml2/security/advisories/new). Never open a normal issue in the repo unless instructed so by mail. Do not submit any pull request.
