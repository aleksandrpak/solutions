using System;
using System.Linq;

class Solution {
    struct Location {
        public int Id;
        public double X;
        public double Y;
    }

    static void Main(String[] args) {
        var n = Convert.ToInt32(Console.ReadLine());
        var a = new Location[n];

        for (var i = 0; i < n; ++i) {
            var line = Console.ReadLine().Split(new[] {' '});
            a[i] = new Location {
                Id = Convert.ToInt32(line[0]),
                   X = Convert.ToDouble(line[1]),
                   Y = Convert.ToDouble(line[2]),
            };
        }

        var m = Convert.ToInt32(Console.ReadLine());

        for (var i = 0; i < m; ++i) {
            var line = Console.ReadLine().Split(new[] {' '});

            var x = Convert.ToDouble(line[0]);
            var y = Convert.ToDouble(line[1]);

            double speed;
            switch (line[2]) {
                case "foot": speed = 5.0; break;
                case "bike": speed = 15.0; break;
                case "metro": speed = 20.0; break;
                default: throw new Exception("unknown transport");
            }

            var minutes = Convert.ToDouble(line[3]);

            var result = a
                .Select(p => new { p.Id, Distance = GetDistance(x, y, p.X, p.Y) })
                .Where(p => (p.Distance / speed) * 60.0 <= minutes)
                .OrderBy(p => p.Distance)
                .ThenBy(p => p.Id);

            Console.WriteLine(string.Join(" ", result.Select(p => p.Id.ToString())));
        }
    }

    static double GetDistance(double x1, double y1, double x2, double y2) {
        var radius = 6371.0;

        x1 = ConvertToRadians(x1);
        y1 = ConvertToRadians(y1);
        x2 = ConvertToRadians(x2);
        y2 = ConvertToRadians(y2);

        var distance = Math.Acos(Math.Sin(x1)*Math.Sin(x2)+Math.Cos(x1)*Math.Cos(x2)*Math.Cos(y2-y1))*radius;
        return Math.Round(distance, 2, MidpointRounding.AwayFromZero);
    }

    static double ConvertToRadians(double angle) {
        return (3.14159265359 / 180.0) * angle;
    }
}
