Contributing
===========

Kentor.AuthServices is maintained by and have mostly been developed by Kentor in Stockholm,
Sweden. The library's source code is hosted on [github](https://github.com/KentorIT/authservices).

##Issue tracking
Github issues are used to keep track of issues and releases. For requests of functionality or
to report bugs, please open an issue in the github repo.

There is also a [Kanban board](https://huboard.com/KentorIT/authservices/#/) powered by HuBoard that
is used to keep track of the state of issues.

##Versioning
Kentor Authentication services uses semantic versioning as defined on http://semver.org/.

    Given a version number MAJOR.MINOR.PATCH, increment the:

    MAJOR version when you make incompatible API changes,
    MINOR version when you add functionality in a backwards-compatible manner, and
    PATCH version when you make backwards-compatible bug fixes.

Additionally *even* PATCH numbers are releases that corresponds to a tag in the 
repository. *Odd* PATCH numbers are development versions. This means that the 
current code in the repository will always have an *odd* PATCH number to denote that 
it is a development version.

##Coding Conventions
The coding conventions are to keep to quite classic .NET style of coding, with the following
styles:
* Always use `{}` for `if` statements, even though there is only one line.
* Code analysis is enabled and all code should compile without compiler warnings or
code analysis errors. Code analysis rules warnings that are not relevant are supressed in
the source. Rules should only be disabled on a global level if it really is appropriate to
disable the rule for the entire code base. Unknown words are added to `CustomDictionary.xml`
instead of suppressing individual warnings.
* Private members in classes are named with camelCasing, no underscores or similar.
* Any single method is so short that it fits on one screen (on a typical laptop monitor, 
not a 30-feet development monster-monitor in vertical orientation).

##Unit tests
The core Kentor.AuthServices library has been developed using TDD (Test Drive Development). All
functionality is covered by tests, and it will remain that way. Pull requests will only be
merged if they contain tests covering the added functionality. Parts of the code that aren't
practically possible to test because of tight integration with the web server (see e.g. 
`CommandResult.ApplyPrincipal`) are excluded from this rule and should be marked with a
`[ExcludeFromCodeCoverage]` attribute. The code coverage report for Kentor.AuthServices
is at 100.00% coverage and should remain so.

##Licensing
The library is licensed under LGPL and by submitting code it is accepted that the submitted
code will be released under the same license. Third party code may only be added to the library
if the author of the pull request holds the copyright to the code, or the code is previously
licensed under a license compatible with LGPL.