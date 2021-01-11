using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        [Test]
        [TestCaseSource(nameof(InvalidSearchOptionTestCases))]
        public void ItShouldThrowArgumentExceptionIfInvalidSearchOptionsAreProvided(SearchOptions options, string description)
        {
            var sut = ConstructSut(out _, 0);

            sut.Invoking(x => x.Search(options)).Should().Throw<ArgumentException>();
        }

        [Test]
        public void ItShouldReturnNothingIfNoShirtsAreSetInSearchEngine()
        {
            var sut = ConstructSut(out var shirts, 0);
            var options = new SearchOptions {Colors = Color.All, Sizes = Size.All};

            var result = sut.Search(options);

            result.Shirts.Should().BeEmpty();
            VerifyResultsCorrectness(shirts, options, result);
        }

        [Test]
        [TestCaseSource(nameof(SearchOptionsMatchTestCases))]
        public void ItShouldReturnCorrectShirtsBasedOnSearchOptionsProvided(SearchOptions options, string description)
        {
            var sut = ConstructSut(out var shirts);

            var result = sut.Search(options);
            
            VerifyResultsCorrectness(shirts, options, result);
        }

        private static IEnumerable<object[]> InvalidSearchOptionTestCases =>
            new List<object[]>
            {
                new[] {null, "null search options"},
                new object[] {new SearchOptions {Sizes = null, Colors = null}, "null Sizes and Colors in search options"},
                new object[] {new SearchOptions {Sizes = null}, "null Sizes in search options"},
                new object[] {new SearchOptions {Colors = null}, "null Colors in search options"},
            };

        private static IEnumerable<object[]> SearchOptionsMatchTestCases =>
            Enumerable
                .Empty<object[]>()
                // only single color
                .Union(from c in Color.All select new object[]{new SearchOptions{Colors = ToList(c)}, $"{c.Name} -"})
                // only single size
                .Union(from s in Size.All select new object[]{new SearchOptions{Sizes = ToList(s)}, $"- {s.Name}"})
                // every combination of color and size
                .Union
                (
                    from c in Color.All
                    from s in Size.All
                    select new object[]{new SearchOptions{Colors = ToList(c), Sizes = ToList(s)}, $"{c.Name} - {s.Name}"}
                )
                // specific test cases
                .Union(new List<object[]>
                {
                    new object[]{new SearchOptions{Colors = ToList(Color.Red), Sizes = ToList(Size.Small)}, "red - small"},
                    new object[]{new SearchOptions{Colors = Color.All, Sizes = ToList(Size.Small)}, "all colors - small"},
                    new object[]{new SearchOptions{Colors = ToList(Color.White), Sizes = Size.All}, "white - all sizes"},
                    new object[]{new SearchOptions{Colors = ToList(Color.Black, Color.Blue), Sizes = ToList(Size.Small, Size.Large)}, "black/blue - small/large"},
                    new object[]{new SearchOptions{Colors = ToList(Color.Red, Color.Yellow), Sizes = ToList(Size.Medium, Size.Large)}, "red/yellow - medium/large"},
                    new object[]{new SearchOptions{Colors = Color.All, Sizes = Size.All}, "all colors - all sizes"}
                });
    }
}
