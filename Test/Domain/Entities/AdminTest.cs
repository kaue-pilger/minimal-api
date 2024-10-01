using minimal_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdminTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
      // Arrange
      var admin =  new Admin();

      // Act
      admin.Id = 1;
      admin.Email = "teste@teste.com";
      admin.Password = "teste";
      admin.Profile = "admin";

      // Assert
      Assert.AreEqual(1, admin.Id);
      Assert.AreEqual("teste@teste.com", admin.Email);
      Assert.AreEqual("teste", admin.Password);
      Assert.AreEqual("admin", admin.Profile);
    }
}