using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UserManagement.Tests;

[TestClass]
public class BasicExample
{
    [TestMethod]
    public void TestMethod1()
    {
        string x = "John Doe";
        Assert.AreEqual("John Doe", x);  // Should Succeed

    }

    [TestMethod]
    public void TestMethod2()
    {
        int x = 12345;
        Assert.ThrowsException<AssertFailedException>(() =>  // just make sure that an exception was raised
        {
            Assert.AreEqual("xxxxx", x.ToString());  // On fail, raises an exception
        });
    }

    [TestMethod]
    public void TestMethod3()
    {
        string x = "Jack The Ripper";
        Assert.ThrowsException<AssertFailedException>(() =>  // just make sure that an exception was raised
        {
            Assert.AreEqual("John Doe", x.ToString());    // On fail, raises an exception
        });
    }
}
