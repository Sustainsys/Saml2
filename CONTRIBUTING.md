Contributing
===========

Kentor.AuthServices is maintained by and have mostly been developed by Kentor in Stockholm,
Sweden. The library's source code is hosted on [github](https://github.com/KentorIT/authservices).

When doing work on protocol features, it is recommended to consult the
[official SAML specifications](https://wiki.oasis-open.org/security/FrontPage#SAMLV2.0Standard).

## Issue tracking
Github issues are used to keep track of issues and releases. For requests of functionality or
to report bugs, please open an issue in the github repo.

It is advised to open an issue describing the plans before starting any major coding work.
Discussing before writing code significantly reduces the risk of getting a pull request
denied.

## Versioning
Kentor Authentication services uses semantic versioning as defined on http://semver.org/.

    Given a version number MAJOR.MINOR.PATCH, increment the:

    MAJOR version when you make incompatible API changes,
    MINOR version when you add functionality in a backwards-compatible manner, and
    PATCH version when you make backwards-compatible bug fixes.

Additionally *even* PATCH numbers are releases that corresponds to a tag in the 
repository. *Odd* PATCH numbers are development versions. This means that the 
current code in the repository will always have an *odd* PATCH number to denote that 
it is a development version.

## Coding Conventions
The coding conventions follow the classic .NET style of coding, with the following
styles:
* Always use `{}` for `if` statements, even when there is only one line.
* Code analysis is enabled and all code should compile without compiler warnings or
code analysis errors. Code analysis warnings that are not relevant are supressed in
the source. Rules should only be disabled on a global level if it really is appropriate to
disable the rule for the entire code base. Unknown words are added to `CustomDictionary.xml`
instead of suppressing individual warnings.
* Private members in classes are named with camelCasing, no underscores or similar.
* Member variables are not prefixed with `this.` unless required to resolve ambiguity (such
as in a constructor having parameters with the same name as the members).
* Any single method is short enough to fit on one screen (on a typical laptop monitor, 
not a 30-inch development monster-monitor in vertical orientation).
* The code is formatted to (mostly) fit in 80 columns.

## Unit Tests
The core Kentor.AuthServices library has been developed using TDD (Test Driven Development). All
functionality is covered by tests, and it will remain that way. Pull requests will only be
merged if they contain tests covering the added functionality. Parts of the code that aren't
practically possible to test because of tight integration with the web server (see e.g. 
`CommandResult.ApplyPrincipal`) are excluded from this rule and should be marked with a
`[ExcludeFromCodeCoverage]` attribute. The code coverage report is at 100.00% coverage and 
should remain so.

## Integration Tests
There are also some integration tests that run through the sample applications and make
sure that they work. To run the integration tests:
* Make sure that you have Google Chrome installed.
* Open the main solution.
* Set multiple startup projects: Kentor.AuthServices.StubIdp, SampleApplication, SampleMvcApplication 
and SampleOwinApplication.
* Debug the solution.
* Open Kentor.AuthServices.IntegrationTests in a separate Visual Studio Instance.
* Run all tests in the IntegrationTests solution.

## Continous integration / build server
Kentor.AuthServices contains configuration for [AppVeyor CI](|https://ci.appveyor.com/).

You may set up a free build of all branches in your GitHub fork by signing up to AppVeyor 
(preferably with your GitHub account) and then creating a new project for your GitHub fork.

## Branching
To make a clean pull request, it is important to follow some git best practices. Nancy
has an [excellent guide](https://github.com/NancyFx/Nancy/wiki/Git-Workflow) that outlines
the steps required.

## Contributors File
There is a [contributors list](../CONTRIBUTORS.txt) with a list of all contributors to the library. If
you want to be listed there, please add yourself as part of the pull request.

## Licensing
The library is licensed under LGPL and by submitting code it is accepted that the submitted
code will be released under the same license. Third party code may only be added to the library
if the author of the pull request holds the copyright to the code, or the code is previously
licensed under a license compatible with LGPL.
