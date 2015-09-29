using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TestEnhancements
{

    public abstract class AModel
    {
    }

    public class Phone : AModel
    {
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Work Phone must be 10 numeric characters")]
        public virtual string WorkPhone { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Home Phone must be 10 numeric characters")]
        public virtual string HomePhone { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Cell Phone must be 10 numeric characters")]
        public virtual string CellPhone { get; set; }

        [Required]
        public DateTime? SomeDate { get; set; }
    }
}
