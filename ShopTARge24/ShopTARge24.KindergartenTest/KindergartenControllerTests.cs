using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopTARge24.Controllers;
using ShopTARge24.Core.Domain;
using ShopTARge24.Core.ServiceInterface;
using ShopTARge24.Models.Kindergarten;
using ShopTARge24.Data;
using Xunit;

namespace ShopTARge24.KindergartenTest
{
    public class KindergartenControllerTests : TestBase
    {
        private KindergartenController CreateController()
        {
            var kindergartenService = new Mock<IKindergartenServices>().Object;
            var fileService = new Mock<IFileServices>().Object;

            return new KindergartenController(DbContext, kindergartenService, fileService);
        }

        [Fact]
        public void Index_Returns_View_With_List()
        {
            // Arrange – seed some data into InMemory DB
            DbContext.Kindergarten.AddRange(
                new Kindergarten
                {
                    Id = Guid.NewGuid(),
                    GroupName = "Orparid",
                    ChildrenCount = 10,
                    KindergartenName = "Sipsik",
                    TeacherName = "Ulvi"
                },
                new Kindergarten
                {
                    Id = Guid.NewGuid(),
                    GroupName = "Päikeserattad",
                    ChildrenCount = 15,
                    KindergartenName = "Sipsik",
                    TeacherName = "Mari"
                });
            DbContext.SaveChanges();

            var controller = CreateController();

            // Act
            var result = controller.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KindergartenIndexViewModel>>(view.Model);
            Assert.Equal(2, model.Count());
        }
    }
}
