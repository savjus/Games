extends Control

var Cell = preload("res://cell.tscn")



@export_enum("Human", "AI") var play_with: String = "Human"

var cells : Array = []
var turn: int = 0
var is_game_end : bool = false

func _ready():
	for cell_count in range(9):
		var cell = Cell.instantiate()
		cell.main = self
		$Cells.add_child(cell)
		cells.append(cell)
		cell.cell_updated.connect(_on_cell_updated)

func _on_cell_updated(cell):
	randomize()
	var match_results = check_match()
	print(match_results)
	if match_results:
		is_game_end = true
		start_win_animation(match_results)
	elif play_with == "AI" and turn ==1:
		var ai_cell = cells[randi() % cells.size()]
		if ai_cell.cell_value == "":
			ai_cell.draw_cell()
		else:
			_on_cell_updated(cell)

func _on_restart_pressed():
	get_tree().reload_current_scene()


func check_match():
	for h in range(3):
		if cells[0+3*h].cell_value == "X" and cells[1+3*h].cell_value == "X" and cells[2+3*h].cell_value == "X" :
			return ["X",0+3*h,1+3*h,2+3*h]
	for v in range(3):
		if cells[0+v].cell_value == "X" and cells[3+v].cell_value == "X" and cells[6+v].cell_value == "X" :
			return ["X",v,3+v,6+v]
	if cells[0].cell_value == "X" and cells[4].cell_value == "X" and cells[8].cell_value == "X":
		return ["X",0,4,8]
	elif cells[2].cell_value == "X" and cells[4].cell_value == "X" and cells[6].cell_value == "X":
		return ["X",2,4,6]
	
	for h in range(3):
		if cells[0+3*h].cell_value == "O" and cells[1+3*h].cell_value == "O" and cells[2+3*h].cell_value == "O" :
			return ["O",0+3*h,1+3*h,2+3*h]
	for v in range(3):
		if cells[0+v].cell_value == "O" and cells[3+v].cell_value == "O" and cells[6+v].cell_value == "O" :
			return ["O",v,3+v,6+v]
	if cells[0].cell_value == "O" and cells[4].cell_value == "O" and cells[8].cell_value == "O":
		return ["O",0,4,8]
	elif cells[2].cell_value == "O" and cells[4].cell_value == "O" and cells[6].cell_value == "O":
		return ["O",2,4,6]
	
	var full = true
	for cell in cells:
		if cell.cell_value =="":
			full = false
	
	if full: return ["draw"]

func start_win_animation(match_result : Array):
	var color : Color
	
	if match_result[0] == "X":
		color = Color.BLUE
	elif match_result[0] == "O":
		color = Color.RED
	
	for c in range(match_result.size()-1):
			cells[match_result[c+1]].glow(color)
		
	
