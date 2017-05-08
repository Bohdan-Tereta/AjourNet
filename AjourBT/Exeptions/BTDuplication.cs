using System;

namespace AjourBT.Exeptions
{
    public class BTDuplication:Exception
    {
        public BTDuplication()
        {
 
        }

        public BTDuplication(string message):base(message)
        {

        }

        public BTDuplication(string message, Exception innerException)
            : base(message, innerException)
        {
 
        }
    }
}