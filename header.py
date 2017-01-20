class Node:
	def __init__(self, x_coord, y_coord, cell_type):
			self.x = x_coord
			self.y = y_coord
			self.cell_type = cell_type

class Edge:
	def __init__(self, to_node, from_node):
		self.to_node = to_node
		self.from_node = from_node

		#self.edges[from_node].append(to_node)
		#self.edges[to_node].append(from_node)

class Graph:
	def __init__(self):
		self.nodes = set()
		self.edges = set()
		self.distances = {}

	def add_node(self, new_node):
		self.nodes.add(new_node)

	def add_edge(self, new_edge):
		self.edges.add(new_edge)

	def dijsktra(graph, initial):
		visited = {initial: 0}
		path = {}

		nodes = set(graph.nodes)

		while nodes:
			min_node = None
			for node in nodes:
				if node in visited:
					if min_node is None:
						min_node = node
					elif visited[node] < visited[min_node]:
						min_node = node
			if min_node is None:
				break

			nodes.remove(min_node)
			current_weight = visited[min_node]

			for edge in graph.edges[min_node]:
				weight = current_weight + graph.distances[(min_node, edge)]
				if edge not in visited or weight < visited[edge]:
					visited[edge] = weight
					path[edge] = min_node

		return visited, path
