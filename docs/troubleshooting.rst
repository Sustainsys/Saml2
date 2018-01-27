Troubleshooting
===============
If you're having trouble - don't give up!  :)

The items below may point you in the right direction.

* Check the `issues archive <https://github.com/Sustainsys/Saml2/issues>`_.
* Check the `SAML2 specification <http://saml.xml.org/saml-specifications>`_, starting with the core section, or the newer `OASIS Saml Wiki <https://wiki.oasis-open.org/security/FrontPage>`_.
* Log your actual SAML2 conversation with `SAML Chrome Panel <https://chrome.google.com/webstore/detail/saml-chrome-panel/paijfdbeoenhembfhkhllainmocckace>`_ or `SAML Tracer for Firefox <https://addons.mozilla.org/en-US/firefox/addon/saml-tracer/>`_.
* Connect an ``ILoggerAdapter`` to your ``SPOptions.Logger``. If you are using the ``OWIN`` middleware this is done for you automatically and you can see the output in the OWIN/Katana logging.
* Last but not least, download the Saml2 source and check out what's really happening.

