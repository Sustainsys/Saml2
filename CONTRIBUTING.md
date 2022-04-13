Sustainsys.Saml2 Contribution Guidelines
===============

Sustainsys.Saml2 is maintained by and have mostly been developed by Sustainsys in Stockholm, Sweden.

Being a security library, it is important that all code in the library follows high quality standards
and is easy to read and maintain. Automated tests are required for any features added.

 When doing work on protocol features, it is recommended to consult 
[the official SAML specifications](<https://wiki.oasis-open.org/security/FrontPage#SAMLV2.0Standard>).

Issue tracking
--------------
Github issues are used to keep track of issues and releases. For requests of functionality or to 
report bugs, please open an issue in the github repo. It is advised to open an issue describing the plans 
before starting any coding work. Discussing before writing code significantly reduces the risk of 
getting a pull request denied.

Versioning 
----------
Sustainsys uses semantic versioning as defined on http://semver.org/.
Given a version number MAJOR.MINOR.PATCH, increment the:

* MAJOR version when you make incompatible API changes,
* MINOR version when you add functionality in a backwards-compatible manner, and
* PATCH version when you make backwards-compatible bug fixes.

Coding Conventions
------------------
The coding conventions follow the classic .NET style of coding, with the following styles:

* Always use ``{}`` for if statements, even when there is only one line.
* Code analysis is enabled and all code should compile without compiler warnings or code analysis errors. 
  Code analysis warnings that are not relevant are supressed in the source. Rules should only be disabled on a 
  global level if it really is appropriate to disable the rule for the entire code base.
* Private members in classes are named with camelCasing, no underscores.
* Member variables are not prefixed with ``this``. unless required to resolve ambiguity (such as in a 
  constructor having parameters with the same name as the members).
* Any single method is short enough to fit on one screen (on a typical laptop monitor, 
  not a 30-inch development monster-monitor).
* The code is formatted to (mostly) fit in 80 columns.

Unit Tests
----------
The Sustainsys.Saml2 library has been developed using TDD (Test Driven Development). All functionality is covered 
by tests, and it will remain that way. Pull requests will only be merged if they contain tests covering the 
added functionality.

Branching
---------
To make a clean pull request, it is important to follow some git best practices. Nancy has an 
[excellent guide](https://github.com/NancyFx/Nancy/wiki/Git-Workflow) that outlines the steps required.

Licensing
---------
The library is licensed under MIT (for the `develop` branch) and by submitting code it is accepted that the
submitted code will be released under the same license. Third party code may only be added to the
library if the author of the pull request holds the copyright to the code, or the code is previously
licensed under a license compatible with MIT.