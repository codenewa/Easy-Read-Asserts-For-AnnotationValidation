using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestEnhancements;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var phone = new Phone();



            phone
                .When(p => p.CellPhone)
                .HasValue("1234567890")
                .InvokeValidation()
                .ShouldPassBecause("10 characters");

            phone
                .When(p => p.CellPhone)
                .HasValue("asdf")
                .InvokeValidation()
                .ShouldFailBecause("phone cannot have alphabetical characters.");



            phone.When(p => p.SomeDate)
                .HasValue(null)
                .InvokeValidation()
                .ShouldFailBecause("Required");
        }
    }
}
