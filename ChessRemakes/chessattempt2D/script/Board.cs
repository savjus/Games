using Godot;
using System;
using System.Collections.Generic;

public partial class Board : Control
{
	private PackedScene Square = (PackedScene)ResourceLoader.Load("res://scenes/Square.tscn");
	[Export(PropertyHint.Enum, "Human,AI")]
	private string playWith = "Human";

	public int Turn { get; set; } = 0;
	public bool isGameOver { get; set; } = false;

	public Square selectedPiece{get;set;}

	public override void _Ready()
	{
		MakeBoard();
	}

	public void MakeBoard()
	{
		var squareContainer = GetNode("SquareContainer");
		for (int i = 0; i < 64; i++)
		{
			int row = i / 8;
			int col = i % 8;
			Square s = Square.Instantiate() as Square;
			s.board = this;
			s.SetPosition(row, col);
			squareContainer.AddChild(s);
			s.Connect("SquareUpdate", new Callable(this, nameof(OnSquareUpdate)));
		}
	}
	public Square GetSquare(int row, int col)
	{
		var squareContainer = GetNode("SquareContainer");
		int index = row * 8 + col;
		return squareContainer.GetChild<Square>(index);
	}
	public void OnSquareUpdate(Square square)
	{
		// Handle square update logic here
	}
	public void ClearHighlights()
	{
		var squareContainer = GetNode("SquareContainer");
		for (int i = 0; i < 64; i++)
		{
			var square = squareContainer.GetChild<Square>(i);
			square.ChangeColor();
		}
	}
	public bool IsSquareHighlighted(int row, int col)
	{
		var square = GetSquare(row, col);
		return square.squarePanel.SelfModulate == new Color(Colors.Yellow, 1);
	}
}
