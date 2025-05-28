using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Control
{
	private PackedScene Cell = (PackedScene)ResourceLoader.Load("res://cell.tscn");

	[Export(PropertyHint.Enum, "Human,AI")]
	private string playWith = "Human";

	private List<Cell> cells = new List<Cell>();
	public int Turn { get; set; } = 0;
	public bool IsGameEnd { get; set; } = false;

	public override void _Ready()
	{
		for (int cellCount = 0; cellCount < 9; cellCount++)
		{
			var cellInstance = Cell.Instantiate() as Cell;
			cellInstance.main = this; // Set the main property
			GetNode("Cells").AddChild(cellInstance);
			cells.Add(cellInstance);
			cellInstance.Connect("CellUpdated", new Callable(this, nameof(OnCellUpdated)));
		}
	}	

	private void OnCellUpdated(Cell cell)
	{
	var matchResults = CheckMatch();
	if (matchResults != null)
	{
		IsGameEnd = true;
		StartWinAnimation(matchResults);
	}
	else if (playWith == "AI" && Turn == 1)
	{
		var emptyCells = new List<Cell>();
		foreach (var c in cells)
		{
			if (c.CellValue == "")
			{
				emptyCells.Add(c);
			}
		}
			if (emptyCells.Count > 0)
			{
				var random = new Random();
				var aiCell = emptyCells[random.Next(emptyCells.Count)];
				aiCell.DrawCell();
			}
		}
	}
	private List<object> CheckMatch()
	{
		for (int h = 0; h < 3; h++)
		{
			if (cells[0 + 3 * h].CellValue == "X" && cells[1 + 3 * h].CellValue == "X" && cells[2 + 3 * h].CellValue == "X")
				return new List<object> { "X", 0 + 3 * h, 1 + 3 * h, 2 + 3 * h };
		}
		for (int v = 0; v < 3; v++)
		{
			if (cells[0 + v].CellValue == "X" && cells[3 + v].CellValue == "X" && cells[6 + v].CellValue == "X")
				return new List<object> { "X", v, 3 + v, 6 + v };
		}
		if (cells[0].CellValue == "X" && cells[4].CellValue == "X" && cells[8].CellValue == "X")
			return new List<object> { "X", 0, 4, 8 };
		if (cells[2].CellValue == "X" && cells[4].CellValue == "X" && cells[6].CellValue == "X")
			return new List<object> { "X", 2, 4, 6 };

		for (int h = 0; h < 3; h++)
		{
			if (cells[0 + 3 * h].CellValue == "O" && cells[1 + 3 * h].CellValue == "O" && cells[2 + 3 * h].CellValue == "O")
				return new List<object> { "O", 0 + 3 * h, 1 + 3 * h, 2 + 3 * h };
		}
		for (int v = 0; v < 3; v++)
		{
			if (cells[0 + v].CellValue == "O" && cells[3 + v].CellValue == "O" && cells[6 + v].CellValue == "O")
				return new List<object> { "O", v, 3 + v, 6 + v };
		}
		if (cells[0].CellValue == "O" && cells[4].CellValue == "O" && cells[8].CellValue == "O")
			return new List<object> { "O", 0, 4, 8 };
		if (cells[2].CellValue == "O" && cells[4].CellValue == "O" && cells[6].CellValue == "O")
			return new List<object> { "O", 2, 4, 6 };

		bool full = true;
		foreach (var c in cells)
		{
			if (c.CellValue == "")
			{
				full = false;
				break;
			}
		}

		if (full) return new List<object> { "draw" };
		return null;
	}

	private void StartWinAnimation(List<object> matchResult)
{
	Color color;

	if ((string)matchResult[0] == "X")
	{
		color = Colors.Blue;
	}
	else if ((string)matchResult[0] == "O")
	{
		color = Colors.Red;
	}
	else
	{
		return;
	}

	for (int i = 1; i < matchResult.Count; i++)
	{
		cells[(int)matchResult[i]].Glow(color);
	}
}
	public void OnRestartPressed(){
		GetTree().ReloadCurrentScene();
	} 
}
