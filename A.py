# A*
import header

print("Begin Graph Generation\n")

white = "unblocked"
light_gray = "partially blocked"
dark_gray = "blocked"
blue = "river"

columns = 160
rows = 120
g = header.Graph()

def generate_world(rows, columns):
	prev_i = 100
	prev_j = 100
	# Create blank cells
	for i in range(0, rows):
		for j in range(0, columns):
			new_node = header.Node(i, j, white)
			g.add_node(new_node)
			#print("{}, {}, {}".format(int(i), int(j), str(white)))

			#if (abs(i - prev_i) == 0) & (abs(j - prev_j) == 1):
			#	new_edge = header.Edge(prev_node, new_node)
			#	g.add_edge(new_edge)

			#if (abs(i - prev_i) == 1) & (abs(j - prev_j) == 0):
			#	new_edge = header.Edge(prev_node, new_node)
			#	g.add_edge(new_edge)

			#save prev for adjaceny generation
			#prev_i = i
			#prev_j = j
			#prev_node = new_node	

def establish_adjacencies():
	for node in g.nodes:
		curr_node = node
		for node in g.nodes:
			if (abs(curr_node.x - node.x) == 1) & (curr_node.y - node.y == 0):
				new_edge = header.Edge(curr_node, node)
				g.add_edge(new_edge)
				print("{}, {}".format(int(node.x), int(node.y)))

			if (abs(curr_node.y - node.y) == 1) & (curr_node.x - node.x == 0):
				new_edge = header.Edge(curr_node, node)
				g.add_edge(new_edge)


def print_world():
	#for node in g.nodes:
	#	print("({}, {}, {})".format(int(node.x), int(node.y), str(node.cell_type)))
	

	#for edge in g.edges:
	#	print("({}, {}) to ({}, {})".format(int(edge.from_node.x), int(edge.from_node.y), int(edge.to_node.x), int(edge.to_node.y))) 
	
	print("Size: {}".format(g.nodes.__len__()))
	print("No. Edges: {}".format(g.edges.__len__()))


generate_world(rows, columns)
establish_adjacencies()
print_world()

