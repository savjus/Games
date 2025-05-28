using Godot;
using System;

public partial class PromotionScreen : Control
{
	private Board _board;
	public override void _Ready()
	{
		_board = GetNode<Board>($"../../../Board");
		Visible = false;
	}
	
	public void PressedKnight()
	{
		_board.PromotePawn('N');
		Visible = false;
	}
	public void PressedRook()
	{
		_board.PromotePawn('R');
		Visible = false;
	}
	public void PressedBishop()
	{
		_board.PromotePawn('B');
		Visible = false;
	}
	public void PressedQueen()
	{
		_board.PromotePawn('Q');
		Visible = false;
	}
}
