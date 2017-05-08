using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjourBT.Domain.Concrete
{
   public class VacationAlreadyExistException : InvalidOperationException
    {
        public VacationAlreadyExistException()
        {
 
        }

        public VacationAlreadyExistException(string message)
        {
 
        }

        public VacationAlreadyExistException(string message, Exception innerException)
        {
 
        }
    }
}
