using System;
using System.Collections.Generic;
using System.Linq;

class Solution {
    class Hotel {
        public uint Id;
        public uint Price;
        public string[] Facilities;
    }
    
    static void Main(String[] args) {
        var n = Convert.ToUInt32(Console.ReadLine());
        var hotels = new Hotel[n];
        
        for (var i = 0; i < n; ++i) {
            var line = Console.ReadLine().Split(new[] {' '});
            hotels[i] = new Hotel {
                Id = Convert.ToUInt32(line.First()),
                Price = Convert.ToUInt32(line.Skip(1).First()),
                Facilities = line.Skip(2).ToArray(),
            };            
        }
        
        var m = Convert.ToUInt32(Console.ReadLine());
        for (var i = 0; i < m; ++i) {
            var line = Console.ReadLine().Split(new[] {' '});
            var price = Convert.ToUInt32(line.First());
            var facilities = line.Skip(1).ToArray();
            
            var result = hotels
                .Where(h => h.Price <= price)
                .Where(h => {
                    foreach (var facility in facilities) {
                        if (!h.Facilities.Contains(facility)) {
                            return false;
                        }
                    }
                    return true;
                })
                .OrderByDescending(h => h.Facilities.Length)
                .ThenBy(h => h.Price)
                .ThenBy(h => h.Id);
            
            Console.WriteLine(string.Join(" ", result.Select(h => h.Id.ToString())));
        }        
    }
}