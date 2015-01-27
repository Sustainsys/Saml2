using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    internal interface IExceptionGenerator
    {
        Exception SignatureMissing();
        Exception NoReferences();
        Exception MultipleReferences();
        Exception IncorrectReference();
        Exception DisallowedTransform(String transformAlgorithm);
        Exception SignatureValidationFail();
    }
}
