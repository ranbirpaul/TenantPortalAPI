using CommonTypeModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Service.Abstract;
using Service.Concrete;
using System;

namespace UnitTestTenant
{
	[TestClass]
	public class UnitTestRackService
	{
		private IRackRepository rackRepo;
		// Mock Rack repository creation
		Mock<IRackRepository> mock = new Mock<IRackRepository>();
		private IList<Rack> mockracks = new List<Rack>{
			new Rack { Id = "1", Name = "Rack1" },
			new Rack { Id  = "2", Name = "Rack2" },
			new Rack { Id  = "3", Name = "Rack3" },
			new Rack { Id  = "4", Name = "Rack4" },
			new Rack { Id  = "5", Name = "Rack5" }
		};

		[TestInitialize]
		public void Initialize()
		{
			// This Initialize method will run before all of the test methods run so we can work inside that method in order 
			// To mock repository object

			// 1. Setup GetAllRacks Method
			mock.Setup(mr => mr.GetAllRacks()).Returns(Task.FromResult(mockracks.AsEnumerable()));

			// 2. Setup GetRackByName method
			// Return rack
			mock.Setup(mr => mr.GetRackByName(
				It.IsAny<string>())).Returns((string i) => Task.FromResult(mockracks.Where(
				x => x.Name.ToLower() == i.ToLower()).SingleOrDefault()));

			// 3. Setup GetRackById method
			// Return rack
			mock.Setup(mr => mr.GetRackById(
				It.IsAny<string>())).Returns((string i) => Task.FromResult(mockracks.Where(
				x => x.Id.ToLower() == i.ToLower()).SingleOrDefault()));

			// 4. Setup AddRack method
			mock.Setup(mr => mr.AddRack(
				It.IsAny<Rack>())).Callback((Rack target) => {
					if (target.Id.Equals("0"))
					{
						target.Id = Convert.ToString(mockracks.Count() + 1);
						mockracks.Add(target);
					}
				}).Returns((Rack target) => Task.FromResult(Task.CompletedTask));

			// 5. Setup UpdateRack method
			mock.Setup(mr => mr.UpdateRack(
				It.IsAny<Rack>())).Callback((Rack target) => {
					var original = mockracks.Where(
							q => q.Id == target.Id).SingleOrDefault();

					if (original != null)
						original.Name = target.Name;
					
				}).Returns((Rack target) => Task.FromResult(
					mockracks.Where(q => q.Id == target.Id).SingleOrDefault() != null?true:false));

			// 6. Setup RemoveRackByName method
			mock.Setup(mr => mr.RemoveRackByName(
				It.IsAny<string>())).Callback((string name) => {
					var original = mockracks.Where(
							q => q.Name.ToLower() == name.ToLower()).SingleOrDefault();

					if (original != null)
						mockracks.Remove(original);
	
				}).Returns((string name) => Task.FromResult(
					mockracks.Where(q => q.Name.ToLower() == name.ToLower()).SingleOrDefault() == null ? true : false));

			// 7. Setup RemoveRackById method
			mock.Setup(mr => mr.RemoveRackById(
				It.IsAny<string>())).Callback((string Id) => {
					var original = mockracks.Where(
							q => q.Id.ToLower() == Id.ToLower()).SingleOrDefault();

					if (original != null)
						mockracks.Remove(original);

				}).Returns((string Id) => Task.FromResult(
					mockracks.Where(q => q.Id.ToLower() == Id.ToLower()).SingleOrDefault() == null ? true : false));

			// 8. Setup RemoveAllRacks method
			mock.Setup(mr => mr.RemoveAllRacks()).Callback(() => {
						mockracks.Clear();

				}).Returns(() => Task.FromResult(
					mockracks.Count == 0 ? true : false));

			// Finally Assign mock repository object instance to Interface instance
			rackRepo = mock.Object;
		}

		[TestMethod]
		public void is_get_all_racks_returns_model_type_of_ienumerable_rack()
		{
			// 1. Arrange
			// Create the Rack Service instance
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			var rackModel = rackservice.GetAllRacks();

			// 3. Assert
			Assert.IsInstanceOfType(rackModel, typeof(Task<IEnumerable<Rack>>));
		}

		[TestMethod]
		public void is_get_all_returns_ienumerable_with_rack_count_of_5()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			Task<IEnumerable<Rack>> task = rackservice.GetAllRacks();
			var rackmodel = task.Result;

			// 3. Assert
			// Compare actual count with the expected count
			Assert.AreEqual(5, rackmodel.Count());
		}

		[TestMethod]
		public void is_get_rack_by_id_returns_object_with_rack_id_as_4_and_name_as_Rack4()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			Task<Rack> task = rackservice.GetRackById("4");
			var rackmodel = task.Result;

			// 3. Assert
			Assert.IsInstanceOfType(rackmodel, typeof(Rack));
			// Compare actual id with the expected id
			Assert.AreEqual("4", rackmodel.Id);
			// Compare actual id with the expected id
			Assert.AreEqual("Rack4", rackmodel.Name);
		}

		[TestMethod]
		public void is_get_rack_by_name_returns_object_with_rack_id_as_4_and_name_as_Rack4()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			Task<Rack> task = rackservice.GetRackByName("Rack4");
			var rackmodel = task.Result;

			// 3. Assert
			// Compare actual id with the expected id
			Assert.AreEqual("4", rackmodel.Id);
			// Compare actual id with the expected id
			Assert.AreEqual("Rack4", rackmodel.Name);
		}

		[TestMethod]
		public void is_add_rack_add_new_rack_object_with_total_count_as_6()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);
			Rack newrack = new Rack {Id="0", Name="Rack6"};
		
			// 2. Act
			rackservice.AddRack(newrack);
			
			// 3. Assert
			// Compare actual total count with the expected count
			Assert.AreEqual(6, mockracks.Count);
			// Compare newly added actual rack id with the expected rack id
			Assert.AreEqual("6", mockracks[mockracks.Count-1].Id);
			// Compare newly added actual rack name with the expected rack name
			Assert.AreEqual("Rack6", mockracks[mockracks.Count-1].Name);
		}

		[TestMethod]
		public void is_update_rack_returns_value_as_true_with_existing_rack_name_changed_from_rack5_to_rack55_with_id_as_5()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);
			Rack updatedrack = new Rack { Id = "5", Name = "Rack55" };

			// 2. Act
			rackservice.UpdateRack(updatedrack);
			Task<Rack> task = rackservice.GetRackById("5");
			var rackmodel = task.Result;

			// 3. Assert
			// Compare actual total count with the expected count
			Assert.AreEqual(5, mockracks.Count);
			// Compare updated actual rack id with the expected rack id
			Assert.AreEqual("5", rackmodel.Id);
			// Compare newly updated actual rack name with the expected rack name
			Assert.AreEqual("Rack55", rackmodel.Name);
		}

		[TestMethod]
		public void is_remove_rack_by_name_returns_value_as_true_with_existing_rack_name_deleted_with_name_as_Rack5()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			Task<bool> task = rackservice.RemoveRackByName("rack5");

			// 3. Assert
			// Compare actual deleted result with the expected result
			Assert.AreEqual(true, task.Result);
			// Compare actual total count with the expected count
			Assert.AreEqual(4, mockracks.Count);
		}

		[TestMethod]
		public void is_remove_rack_by_id_returns_value_as_true_with_existing_rack_name_deleted_with_id_as_5()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			Task<bool> task = rackservice.RemoveRackById("5");

			// 3. Assert
			// Compare actual deleted result with the expected result
			Assert.AreEqual(true, task.Result);
			// Compare actual total count with the expected count
			Assert.AreEqual(4, mockracks.Count);
		}

		[TestMethod]
		public void is_remove_all_racks_returns_value_as_true()
		{
			// 1. Arrange
			// Create the instance of rack service
			IRackService rackservice = new RackService(rackRepo);

			// 2. Act
			Task<bool> task = rackservice.RemoveAllRacks();

			// 3. Assert
			// Compare actual deleted result with the expected result
			Assert.AreEqual(true, task.Result);
			// Compare actual total count with the expected count
			Assert.AreEqual(0, mockracks.Count);
		}
	}
}
