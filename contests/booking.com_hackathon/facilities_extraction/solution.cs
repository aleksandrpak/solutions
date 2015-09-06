using System;
using System.Collections.Generic;

class Solution {
    static void Main(String[] args) {
        var n = Convert.ToInt32(Console.ReadLine());
        var facilities = new SortedSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < n; ++i) {
            facilities.Add(Console.ReadLine().Trim());
        }
        
        var desc = Console.ReadLine().ToLower();
        foreach (var facility in facilities) {
            if (desc.Contains(facility.ToLower())) {
                Console.WriteLine(facility);
            }
        }
    }
}