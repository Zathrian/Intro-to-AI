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

	# Create 160*120 = 19200 white cells

	for i in range(0, rows):
		for j in range(0, columns):
			new_node = header.Node(i, j, white)
			g.add_node(new_node)
		

def establish_adjacencies():

	# For each cell check all cells and detect adjacency using coordinates
	# So far this has proved to be an inefficient algo taking upwards of 3 minutes or more to compute
	# all edges. It does however produce a number that is nearly correct. 

	# There are 160*120 = 19200 cells. For each cell check every cell for adjacency. Therefore there
	# will be 19200*19200 checks for adjancency = 368,640,000... thats a lot of checks. After running
	# there are 76,240 edges which is very close to my hand calculation of 76,592 adjacencies

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
	
	
	print("Size: {}".format(g.nodes.__len__()))
	print("No. Edges: {}".format(g.edges.__len__()))


generate_world(rows, columns)
establish_adjacencies()
print_world()

