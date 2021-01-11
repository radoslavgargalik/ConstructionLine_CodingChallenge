using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    public class SearchEngineTestsBase
    {
        protected static SearchEngine ConstructSut(out List<Shirt> shirts, int numberOfShirts = 1000)
        {
            var random = new Random(DateTime.Now.Ticks.GetHashCode());
            shirts = Enumerable
                .Range(0, numberOfShirts)
                .Select(i => ConstructRandomShirt(i, random))
                .ToList();
            
            return new SearchEngine(shirts);
        }

        private static Shirt ConstructRandomShirt(int index, Random random)
        {
            var color = Color.All[random.Next(0, Color.All.Count)];
            var size = Size.All[random.Next(0, Size.All.Count)];
                    
            return new Shirt(Guid.NewGuid(), $"{index} - {color.Name} - {size.Name}", size, color);
        }

        protected static void VerifyResultsCorrectness(List<Shirt> shirts, SearchOptions options,
            SearchResults result)
        {
            result.Should().NotBeNull();
            result.Shirts.Should().NotBeNull();
            options.Colors.Should().NotBeNull();
            options.Sizes.Should().NotBeNull();

            AssertResults(shirts, options, result.Shirts);
            AssertSizeCounts(result.Shirts, options, result.SizeCounts);
            AssertColorCounts(result.Shirts, options, result.ColorCounts);
        }

        private static void AssertResults(List<Shirt> allShirts, SearchOptions options, List<Shirt> resultShirts)
        {
            resultShirts.Should().NotBeNull();

            var resultShirtsSet = resultShirts.ToHashSet();
            var searchColors = options.Colors.ToHashSet();
            var searchSizes = options.Sizes.ToHashSet();

            foreach (var shirt in allShirts)
            {
                if (DoesShirtMatchColorAndSize(shirt, searchColors, searchSizes))
                {
                    resultShirtsSet.Should().Contain(shirt,
                        $"'{shirt.Name}' with Size '{shirt.Size.Name}' and Color '{shirt.Color.Name}' not found in results, " +
                         $"when selected sizes where '{string.Join(",", options.Sizes.Select(s => s.Name))}' " +
                         $"and colors '{string.Join(",", options.Colors.Select(c => c.Name))}'");
                }
            }
            foreach (var resultShirt in resultShirts)
            {
                DoesShirtMatchColorAndSize(resultShirt, searchColors, searchSizes).Should().BeTrue(
                    $"'{resultShirt.Name}' with Size '{resultShirt.Size.Name}' and Color '{resultShirt.Color.Name}' found in results but it shouldn't be there, " +
                    $"when selected sizes where '{string.Join(",", options.Sizes.Select(s => s.Name))}' " +
                    $"and colors '{string.Join(",", options.Colors.Select(c => c.Name))}'");
            }
        }

        private static void AssertSizeCounts(List<Shirt> shirts, SearchOptions searchOptions, List<SizeCount> sizeCounts)
        {
            sizeCounts.Should().NotBeNull();
            sizeCounts.Should().HaveCount(Size.All.Count);

            foreach (var size in Size.All)
            {
                var sizeCount = sizeCounts.SingleOrDefault(s => s.Size.Id == size.Id);
                Assert.That(sizeCount, Is.Not.Null, $"Size count for '{size.Name}' not found in results");

                var expectedSizeCount = shirts
                    .Count(s => s.Size.Id == size.Id
                                && (!searchOptions.Colors.Any() || searchOptions.Colors.Select(c => c.Id).Contains(s.Color.Id)));

                Assert.That(sizeCount.Count, Is.EqualTo(expectedSizeCount), 
                    $"Size count for '{sizeCount.Size.Name}' showing '{sizeCount.Count}' should be '{expectedSizeCount}'");
            }
        }

        private static void AssertColorCounts(List<Shirt> shirts, SearchOptions searchOptions, List<ColorCount> colorCounts)
        {
            colorCounts.Should().NotBeNull();
            colorCounts.Should().HaveCount(Color.All.Count);
            
            foreach (var color in Color.All)
            {
                var colorCount = colorCounts.SingleOrDefault(s => s.Color.Id == color.Id);
                Assert.That(colorCount, Is.Not.Null, $"Color count for '{color.Name}' not found in results");

                var expectedColorCount = shirts
                    .Count(c => c.Color.Id == color.Id  
                                && (!searchOptions.Sizes.Any() || searchOptions.Sizes.Select(s => s.Id).Contains(c.Size.Id)));

                Assert.That(colorCount.Count, Is.EqualTo(expectedColorCount),
                    $"Color count for '{colorCount.Color.Name}' showing '{colorCount.Count}' should be '{expectedColorCount}'");
            }
        }

        protected static List<T> ToList<T>(params T[] items) => items.ToList();

        private static bool DoesShirtMatchColorAndSize(Shirt shirt, ICollection<Color> searchColors,
            ICollection<Size> searchSizes) =>
            (!searchColors.Any() || searchColors.Contains(shirt.Color)) &&
            (!searchSizes.Any() || searchSizes.Contains(shirt.Size));
    }
}