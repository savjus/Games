using Godot;
using System;

public partial class Cell : Button
{
	[Signal]
	public delegate void CellUpdatedEventHandler(Cell cell);

	public Main main {get;set;}
	public string CellValue {get;set;} = "";

	private Panel background;
	private Panel border;



	public override void _Ready()
{
	background = GetNode<Panel>("background");
	border = GetNode<Panel>("border");
	Modulate = new Color(Modulate, 1);
}

	public void DrawX()
	{
		var tween = GetTree().CreateTween();
		Modulate = new Color(Modulate, 0);
			Text = "X";
		CellValue = "X";
		SelfModulate = new Color("#00ffff");
		tween.TweenProperty(this, "modulate:a", 1, 0.5f);
	}

	public void DrawO()
	{
		var tween = GetTree().CreateTween();
		Modulate = new Color(Modulate, 0); // Ensure the button is visible
		Text = "O";
		CellValue = "O";
		SelfModulate = new Color("#ff4200"); // Change only the text color
		tween.TweenProperty(this, "modulate:a", 1, 0.5f);
	}

	public void DrawCell()
	{
		if (main.IsGameEnd) return;
		if (CellValue == "")
		{
			switch (main.Turn)
			{
				case 0:
					main.Turn = 1;
					DrawX();
					break;
				case 1:
					main.Turn = 0;
					DrawO();
					break;
			}
		}

		MouseDefaultCursorShape = Control.CursorShape.Arrow;
		EmitSignal(nameof(CellUpdated), this);
	}

	public void Glow(Color color)
	{
		background.SetModulate(color);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(background, "modulate:a", 0.8, 0.5f); // Fade in to alpha 1 over 0.5 seconds
	}
}
