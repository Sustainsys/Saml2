---
name: Bug report
about: Report a (non-security) bug
labels: ["bug", "triage"]

---

# Security issues

If you have found a bug that you think might be security related, please *DO NOT OPEN A PUBLIC ISSUE*. Send an e-mail to security@sustainsys.com instead. Normally you should get a response by the next business day.

## Non Security Issues
Please note that only *critical* compatibility fixes, such as when major browsers change behaviour, are fixed in 1.X or 2.X versions.

### Information needed
1. What nuget packages are you using
2. What is the expected behaviour
3. What happens instead. In the case of an exception, this includes the exception typ, complete exception message (personal information may be redacted) and a stack trace.

### Additional info
Please include
* .Net Framework your application is compiled against (e.g. `net472`, `netcoreap2.1`)
* .Net Framework installed. This might be different than above. You can compile with net452, but have 4.7.2 installed.
* Version of Asp.Net MVC / Asp.NET Core used.
