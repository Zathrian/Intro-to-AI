using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class City
    {
        public double x;
        public double y;
        public double ID;
      //  public Dictionary<City, double> dMap = new Dictionary<City, double>();

        public City(List<string> data)
        {
            // Console.Write("We build this city on rock and roll!");
            this.ID = int.Parse(data[0]);
            this.x = double.Parse(data[1]);
            this.y = double.Parse(data[2]);
        }
 
      //  public void addCityToMap(City neighbor, double dist)
     //   {
      //      dMap.Add(neighbor, dist);
    //    }

        
    }
}
