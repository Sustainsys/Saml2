using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// A Saml2 Subject, see core 2.4.1.
/// </summary>
public class Subject
{
    /// <summary>
    /// NameId
    /// </summary>
    public NameId? NameId { get; set; }

    /// <summary>
    /// SubjectConfirmation
    /// </summary>
    public SubjectConfirmation? SubjectConfirmation { get; set; }
}