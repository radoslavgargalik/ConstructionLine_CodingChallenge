using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;
        }

        public SearchResults Search(SearchOptions options)
        {
            if (options?.Colors == null || options.Sizes == null)
                throw new ArgumentException("SearchOptions and its collections must not be null", nameof(options));

            var colorsToSearch = options.Colors.ToHashSet();
            var sizesToSearch = options.Sizes.ToHashSet();

            var matchedShirts = _shirts
                .Where(s =>
                    (!colorsToSearch.Any() || colorsToSearch.Contains(s.Color)) &&
                    (!sizesToSearch.Any() || sizesToSearch.Contains(s.Size)))
                .ToList();

            var colorCounts = matchedShirts
                .GroupBy(s => s.Color)
                .Select(g => new ColorCount {Color = g.Key, Count = g.Count()});
            var missingColorCounts = Color.All
                .Where(c => !colorCounts.Any(cc => cc.Color == c))
                .Select(c => new ColorCount {Color = c, Count = 0});

            var sizeCounts = matchedShirts
                .GroupBy(s => s.Size)
                .Select(g => new SizeCount {Size = g.Key, Count = g.Count()});
            var missingSizeCounts = Size.All
                .Where(s => !sizeCounts.Any(sc => sc.Size == s))
                .Select(s => new SizeCount {Size = s, Count = 0});
            
            return new SearchResults
            {
                Shirts = matchedShirts,
                ColorCounts = colorCounts.Union(missingColorCounts).ToList(),
                SizeCounts = sizeCounts.Union(missingSizeCounts).ToList()
            };
        }
    }
}