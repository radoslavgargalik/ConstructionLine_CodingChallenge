using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEnginePerformanceTests : SearchEngineTestsBase
    {
        private const int _numberOfShirts = 50000; 
        private List<Shirt> _shirts;
        private SearchEngine _sut;

        [SetUp]
        public void Setup()
        {
            _sut = ConstructSut(out _shirts, _numberOfShirts);
        }

        [Test]
        [TestCaseSource(nameof(PerformanceTestCases))]
        public void ItShouldPerformTheSearchBasedOnSearchOptionsWithin100ms(SearchOptions options, string description)
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = _sut.Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds");

            sw.ElapsedMilliseconds.Should().BeLessOrEqualTo(100, "Performance search test took more than 100ms");
            VerifyResultsCorrectness(_shirts, options, result);
        }

        private static IEnumerable<object[]> PerformanceTestCases =>
            Enumerable
                .Empty<object[]>()
                // single color
                .Union(from c in Color.All select new object[]{new SearchOptions{Colors = ToList(c)}, $"{c.Name} -"})
                // single size
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
                    new object[]{new SearchOptions{Colors = Color.All, Sizes = ToList(Size.Small)}, "all colors - small"},
                    new object[]{new SearchOptions{Colors = ToList(Color.White), Sizes = Size.All}, "white - all sizes"},
                    new object[]{new SearchOptions{Colors = ToList(Color.Black, Color.Blue), Sizes = ToList(Size.Small, Size.Large)}, "black/blue - small/large"},
                    new object[]{new SearchOptions{Colors = ToList(Color.Red, Color.Yellow), Sizes = ToList(Size.Medium, Size.Large)}, "red/yellow - medium/large"},
                    new object[]{new SearchOptions{Colors = Color.All, Sizes = Size.All}, "all colors - all sizes"}
                });
    }
}
