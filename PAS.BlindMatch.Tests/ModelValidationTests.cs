using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PAS.BlindMatch.Models;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class ModelValidationTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, results, true);
            return results;
        }

        [Fact]
        public void ProjectModel_AbstractTooShort_ShouldFailValidation()
        {
            // Arrange
            var project = new Project
            {
                Title = "Valid Title",
                Abstract = "Too short", // Needs to be 50 characters
                TechnicalStack = "C#",
                ResearchAreaId = 1
            };

            // Act
            var errors = ValidateModel(project);

            // Assert
            Assert.Contains(errors, e => e.ErrorMessage.Contains("at least 50 characters"));
        }

        [Fact]
        public void ProjectModel_InvalidTechStack_ShouldFailRegex()
        {
            // Arrange
            var project = new Project
            {
                Title = "Valid Title",
                Abstract = "This abstract is definitely long enough to pass the fifty character minimum requirement for the application.",
                TechnicalStack = "<script>alert('hack')</script>", // Invalid characters
                ResearchAreaId = 1
            };

            // Act
            var errors = ValidateModel(project);

            // Assert
            Assert.Contains(errors, e => e.ErrorMessage.Contains("contains invalid characters"));
        }
    }
}