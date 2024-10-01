using minimal_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
      // Arrange
      var vehicle =  new Vehicle();

      // Act
      vehicle.Id = 1;
      vehicle.Name = "teste";
      vehicle.Brand = "teste";
      vehicle.Year = 2000;

      // Assert
      Assert.AreEqual(1, vehicle.Id);
      Assert.AreEqual("teste", vehicle.Name);
      Assert.AreEqual("teste", vehicle.Brand);
      Assert.AreEqual(2000, vehicle.Year);
    }
}