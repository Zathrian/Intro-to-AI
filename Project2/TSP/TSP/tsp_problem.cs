using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    class tsp_problem
    {
        public Node firstState;
        public Node firstCity;
		//public Dictionary<City, double> dMap = new Dictionary<City, double>();
		CityMap map = CityMap.Map;
		public void initializeProblem(System.IO.StreamReader file, int size)
        {
            bool firstFlag = true;
            HashSet<Node> unvisited = new HashSet<Node>();

            // Iterate through file and add cities to the list
            for (int j = 0; j < size; j++)
            {
               Node city = new Node(parse_city(file.ReadLine()));
                if (firstFlag)
                {
                    firstCity = city;
                    firstFlag = false;
					this.firstState = city;
                }
                else
				{
					unvisited.Add(city);
				}
                map.dMap.Add(city);
            }

          //  this.firstState = new Node(firstCity, unvisited, new List<City>());
          //  createEdges(unvisited, firstCity);
        }
		void setNearestCity()
		{
			foreach(Node n in map.dMap)
			{
				double nearestDistance = n.getDistance(firstCity);
				Node closest = firstCity;
				foreach(Node k in map.dMap)
				{
					double dist = n.getDistance(k);
					if (nearestDistance > dist )
					{
						nearestDistance = dist;
						closest = k;
					}
					
				}
				n.nearestNode = closest;
			}
		}
        private List<string> parse_city(string line)
        {
            List<string> city_data = line.Split(',').ToList<string>();
            return city_data;
        }
/*
        private void createEdges(HashSet<City> unvisited, City firstCity)
        {
            //Console.WriteLine("In createEdges Function\n");
            List<City> allCities = new List<City>(unvisited);
            allCities.Add(firstCity);
            //Console.WriteLine("All Cities Size = " + allCities.Count + "Unvisited Siez = " + unvisited.Count);
            City c1, c2;
            double dist;

            for (int i = 0; i < allCities.Count; i++)
            {
                for(int j = 0; j < allCities.Count; j++)
                {
                    if (i == j)
                        continue;

                    //Console.WriteLine("i = " + i + " j = " + j);
                    c1 = allCities[i];
                    c2 = allCities[j];

                    if (c1.dMap.ContainsKey(c2) || c2.dMap.ContainsKey(c1))
                        continue;

                    dist = getDistanceCity(c1, c2);

                    c1.addCityToMap(c2, dist);
                    c2.addCityToMap(c1, dist);

                }

                //Console.WriteLine("City Conenections = " + firstCity.dMap.Count);
            }

            c1 = null;
            c2 = null;
            //Console.WriteLine("All Cities Count = " + allCities.Count);
            sortEdges(allCities);
        }

        public void sortEdges(List<City> allCities)
        {
            foreach(City city in allCities)
            {
                Dictionary<City, double> newMap = new Dictionary<City, double>();
                foreach(KeyValuePair<City, double> c in city.dMap.OrderBy(key => key.Value))
                {
                    newMap.Add(c.Key, c.Value);
                }
                city.dMap = newMap;
            }
        }
*/
        public double getDistanceCity(Node c1, Node c2)
        {
            double distance = (Math.Sqrt(Math.Pow((c1.x - c2.x), 2) + Math.Pow((c1.y - c2.y), 2)));
            return distance;
        }
		/*
        public double getDistance(Node node, Node neighbor)
        {
          //  return neighbor.current.dMap[node.current];
			float x = Math.pow(neighbor.getX() - currentNode.getX(), 2.0);
			float y = Math.pow(destination.getX() - currentNode.getX(), 2.0);
			return x + y;
		}
		*/
		/*
        public bool goalTest(Node node)
        {
            return ((node.current == firstCity) && (node.unvisited.Count == 0));
        }
		*/

        public List<Node> formNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();
           // HashSet<City> unvisited = node.unvisited;
         //   List<City> currentPath = node.path;
          //  List<City> newPath;
         //   HashSet<City> newUnvisited;

			foreach(Node city in map.dMap)
			{
				if(city != node)
				{
					neighbors.Add(city);
				}
			}
			return neighbors;
			



			/*
            //Console.WriteLine("Unvisited Nodes = " + unvisited.Count);
            if(unvisited.Count == 0)
            {
                newPath = new List<City>(currentPath);
                newPath.Add(node.current);
                newUnvisited = new HashSet<City>(unvisited);
                neighbors.Add(new Node(firstCity, newUnvisited, newPath));
                return neighbors;
            }

            foreach(City neighbor in unvisited)
            {
                newPath = new List<City>(currentPath);
                newPath.Add(node.current);

                newUnvisited = new HashSet<City>(unvisited);
                newUnvisited.Remove(neighbor);

                neighbors.Add(new Node(neighbor, newUnvisited, newPath));
                //Console.WriteLine("Neighbors count = " + neighbors.Count + "Unvisited Cuont = " + unvisited.Count);
            }

            unvisited = null;
            currentPath = null;
            newPath = null;
            newUnvisited = null;

            return neighbors;
			*/
        }

        public void calculateFCost(Node neighbor, double tempScore)
        {
            neighbor.FCost = neighbor.HCost + tempScore;
            //Console.WriteLine("FCOST = " + neighbor.FCost);
        }

        private void HCost(Node n)
        {
			// HSost = distance to source, MST to every other city, Distance to nearest city
			double dist1 = n.getDistance(firstCity);
			double dist2 = calculateMinSpanTree(n);
			double dist3 = n.getDistance(n.nearestNode);
			n.HCost = dist1 + dist2 + dist3;

			/*
			if (n.unvisited.Count == 0)
            {
                if (n.current == firstCity)
                    return 0.0f;
                return n.current.dMap[firstCity];
            }
            else if(n.unvisited.Count == 1)
            {
                return n.current.dMap[firstCity];
            }

            double dist1 = n.getNearestDistFromUnvisited(n.current);
            double dist2 = calculateMinSpanTree(n);
            double dist3 = n.getNearestDistFromUnvisited(firstCity);

            n.HCost = dist1 + dist2 + dist3;
            return n.HCost;
			*/
        }

        private double calculateMinSpanTree(Node n)
        {
            double sum = 0.0f; 
            foreach(Node city in map.dMap)
            {
				if(city != n)
				{
					sum += n.getDistance(city);
				}
               
            }
            return sum;
        }
    }

}
 