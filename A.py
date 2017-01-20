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

			if ((i - abs(prev_i)) == 0) & ((j - abs(prev_j)) == 1):
				new_edge = header.Edge(prev_node, new_node)
				g.add_edge(new_edge)

			if ((i - abs(prev_i)) == 1) & ((j - abs(prev_j)) == 0):
				new_edge = header.Edge(prev_node, new_node)
				g.add_edge(new_edge)

			#save prev for adjaceny generation
			prev_i = i
			prev_j = j
			prev_node = new_node	


def print_world():
	#for node in g.nodes:
	#	print("({}, {}, {})".format(int(node.x), int(node.y), str(node.cell_type)))
	

	#for edge in g.edges:
	#	print("({}, {}) to ({}, {})".format(int(edge.from_node.x), int(edge.from_node.y), int(edge.to_node.x), int(edge.to_node.y))) 
	
	print("Size: {}".format(g.nodes.__len__()))
	print("No. Edges: {}".format(g.edges.__len__()))


generate_world(rows, columns)
print_world()

