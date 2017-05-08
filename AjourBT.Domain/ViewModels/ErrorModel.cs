using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class ErrorModel
    {
        public int statusCode { get; set; }
        public Exception Exception { get; set; }
        public string RequestedURL { get; set; }
    }
}