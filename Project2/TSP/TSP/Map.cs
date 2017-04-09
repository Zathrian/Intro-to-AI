using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
	
	public class CityMap
	{
		private static CityMap map;
		public HashSet<Node> dMap;// = new Dictionary<Node, double>();
		
		private CityMap()
		{
			dMap = new HashSet<Node>();
		}
		public static CityMap Map
		{
			get
			{
				if (map == null)
				{
					map = new CityMap();
				}
				return map;
			}
		}








	}
}

