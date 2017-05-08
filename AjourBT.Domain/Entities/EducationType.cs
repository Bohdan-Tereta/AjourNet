using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public enum EducationType
    {
        None,
        BasicSecondary,
        CompleteSecondary,
        Vocational,
        Undergraduate,
        BasicHigher,
        CompleteHigher
    }

    public static class EducationTypeExtensions
    {
        public static string Description(this EducationType me)
        {
            switch (me)
            {
                case EducationType.BasicSecondary:
                    return "базова загальна середня";
                case EducationType.CompleteSecondary:
                    return "повна загальна середня";
                case EducationType.Vocational:
                    return "професійно-технічна";
                case EducationType.Undergraduate:
                    return "неповна вища";
                case EducationType.BasicHigher:
                    return "базова вища";
                case EducationType.CompleteHigher:
                    return "повна вища";
                default:
                    return ""; 
            }
        }
    }
}
