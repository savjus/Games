using Godot;
using System;


// TODO:
// en pessant
// promotion screen
// win screen
// score?
public partial class Square : Button
{
	[Signal]
	public delegate void SquareUpdateEventHandler(Square square);

	public Board board { get; set; }
	public Panel squarePanel;
	private string SquareValue = "";
	private int row;
	private int col;

	public override void _Ready()
	{
		squarePanel = GetNode<Panel>("Panel");
		MakePieces();
		ChangeColor();
	}
	private void _on_toggled()
	{
		GD.Print($"Button pressed at position: ({row}, {col}) its value is: {SquareValue}");
		GD.Print(board.selectedPiece);
		if (board.IsSquareHighlighted(row, col))
		{
			MovePieceTo(row, col);
			board.ClearHighlights();
			board.selectedPiece.SquareValue = "";
		}
		else
		{
			board.ClearHighlights(); // Clear previous highlights

			if (SquareValue != "")
			{
				board.selectedPiece = this;
				if (SquareValue == "P" || SquareValue == "p")
				{
					HighlightPawnMoves();
				}
				else if (SquareValue == "N" || SquareValue == "n")
				{
					HighlightKnightMoves();
				}
				else if (SquareValue == "B" || SquareValue == "b")
				{
					HighlightBishopMoves();
				}
				else if (SquareValue == "R" || SquareValue == "r")
				{
					HighlightRookMoves();
				}
				else if (SquareValue == "K" || SquareValue == "k")
				{
					HighlightKingMoves();
				}
				else if (SquareValue == "Q" || SquareValue == "q")
				{
					HighlightQueenMoves();
				}
			}
		}
		EmitSignal(nameof(SquareUpdate), this);
	}

	private void MovePieceTo(int targetRow, int targetCol)
	{
		var targetSquare = board.GetSquare(targetRow, targetCol);

		// Check if the target square contains an opponent's piece
		if (!string.IsNullOrEmpty(targetSquare.SquareValue) && char.IsUpper(targetSquare.SquareValue[0]) != char.IsUpper(board.selectedPiece.SquareValue[0]))
		{
			// Capture the opponent's piece
			targetSquare.SquareValue = board.selectedPiece.SquareValue;
			targetSquare.Text = board.selectedPiece.SquareValue;
		}
		else
		{
			// Move the piece to the target square
			targetSquare.SquareValue = board.selectedPiece.SquareValue;
			targetSquare.Text = board.selectedPiece.SquareValue;
		}

		// Handle castling
		if (board.selectedPiece.SquareValue == "K" || board.selectedPiece.SquareValue == "k")
		{
			GD.Print("king");
			if (board.selectedPiece.col == col + 2) // queen-side castling 
			{
				var rookSquare = board.GetSquare(row, col - 2);
				var newRookSquare = board.GetSquare(row, col + 1);
				GD.Print($"newRookSquare: {newRookSquare.SquareValue}");
				GD.Print($"RookSquare: {rookSquare.SquareValue}");
				
				newRookSquare.SquareValue = rookSquare.SquareValue;
				newRookSquare.Text = rookSquare.SquareValue;
				rookSquare.SquareValue = "";
				rookSquare.Text = "";
				rookSquare.ChangeColor();
			}
			else if (board.selectedPiece.col == col - 2) // King-side castling 
			{
				var rookSquare = board.GetSquare(row, col +1);
				var newRookSquare = board.GetSquare(row, col - 1);
				GD.Print($"newRookSquare: {newRookSquare.SquareValue}");
				GD.Print($"RookSquare: {rookSquare.SquareValue}");
				newRookSquare.SquareValue = rookSquare.SquareValue;
				newRookSquare.Text = rookSquare.SquareValue;
				rookSquare.SquareValue = "";
				rookSquare.Text = "";
				rookSquare.ChangeColor();
			}
		}
		// handle pawn promotion
		if ((SquareValue == "P" && targetRow == 7) || (SquareValue == "p" && targetRow == 0))
		{
			
			// open the promotion screen
			GD.Print("promotion");
			OpenPromotionScene(targetRow, targetCol);
		}
		// Clear the original square
		board.selectedPiece.SquareValue = "";
		board.selectedPiece.Text = "";
		board.selectedPiece.ChangeColor(); // Reset the color of the original square
	}
	
	
private void OpenPromotionScene(int targetRow, int targetCol)
{
	// Load the promotion scene
	var promotionScene = (PackedScene)ResourceLoader.Load("res://scenes/promotion_screen.tscn"); // Replace with your scene path
	var promotionInstance = promotionScene.Instantiate();

	// Add it to the scene tree
	GetTree().Root.AddChild(promotionInstance); // ADD THE CORRECT SIGNAL HERE

	// // Assign button actions
	// var queenButton = promotionInstance.GetNode<Button>("Q");
	// queenButton.Pressed += () => PromotePawn(targetRow, targetCol, "Q");
	//
	// var rookButton = promotionInstance.GetNode<Button>("R");
	// rookButton.Pressed += () => PromotePawn(targetRow, targetCol, "R");
	//
	// var bishopButton = promotionInstance.GetNode<Button>("B");
	// bishopButton.Pressed += () => PromotePawn(targetRow, targetCol, "B");
	//
	// var knightButton = promotionInstance.GetNode<Button>("N");
	// knightButton.Pressed += () => PromotePawn(targetRow, targetCol, "N");
	//
}
 // move this to the promotion screen script---------------------------------------
private void PromotePawn(int targetRow, int targetCol, string promotionPiece)
{
	var targetSquare = board.GetSquare(targetRow, targetCol);
	targetSquare.SquareValue = promotionPiece;
	targetSquare.Text = promotionPiece;

	// Close the promotion menu
	var promotionMenu = GetTree().Root.GetNode("Promotion screen"); 
	promotionMenu.QueueFree();
}




	private void HighlightKnightMoves()
	{
		int[,] moves = { { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 }, { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 } };
		for (int i = 0; i < moves.GetLength(0); i++)
		{
			HighlightSquare(row + moves[i, 0], col + moves[i, 1]);
		}
	}

	private void HighlightBishopMoves()
	{
		HighlightDiagonalMoves();
	}

	private void HighlightRookMoves()
	{
		HighlightStraightMoves();
	}

	private void HighlightKingMoves()
	{
		int[,] moves = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
		for (int i = 0; i < moves.GetLength(0); i++)
		{
			HighlightSquare(row + moves[i, 0], col + moves[i, 1]);
		}

		// Check for castling moves
		if (CanCastleKingSide())
		{
			HighlightSquare(row, col + 2);
		}
		if (CanCastleQueenSide())
		{
			HighlightSquare(row, col - 2);
		}
	}

	private void HighlightQueenMoves()
	{
		HighlightStraightMoves();
		HighlightDiagonalMoves();
	}
	private void HighlightPawnMoves()
	{
		int direction = SquareValue == "P" ? 1 : -1;
		int startRow = SquareValue == "P" ? 1 : 6;
		int Row = row + direction;
		
		if(board.GetSquare(Row, col).SquareValue == "")
			HighlightSquare(Row, col);

		if (row == startRow && IsSquareEmpty(Row, col) && IsSquareEmpty(Row + direction,col))
		{
			HighlightSquare(Row + direction, col);
		}
		
		
		if (IsValidPosition(Row, col - 1) && !IsSquareEmpty(Row, col - 1) && char.IsUpper(board.GetSquare(Row, col - 1).SquareValue[0]) != char.IsUpper(this.SquareValue[0]))
		{
			HighlightSquare(Row, col - 1);
		}
		if (IsValidPosition(Row, col + 1) && !IsSquareEmpty(Row, col + 1) && char.IsUpper(board.GetSquare(Row, col + 1).SquareValue[0]) != char.IsUpper(this.SquareValue[0]))
		{
			HighlightSquare(Row, col + 1);
		}
	}
	private void HighlightStraightMoves()
	{
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row + i, col)) break;
		}
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row - i, col)) break;
		}
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row, col + i)) break;
		}
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row, col - i)) break;
		}
	}

	private void HighlightDiagonalMoves()
	{
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row + i, col + i)) break;
		}
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row + i, col - i)) break;
		}
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row - i, col + i)) break;
		}
		for (int i = 1; i < 8; i++)
		{
			if (!HighlightSquareAndCheck(row - i, col - i)) break;
		}
	}

	private bool HighlightSquareAndCheck(int targetRow, int targetCol)
	{
		if (IsValidPosition(targetRow, targetCol))
		{
			var targetSquare = board.GetSquare(targetRow, targetCol);
			if (targetSquare.SquareValue == "")
			{
				targetSquare.squarePanel.SelfModulate = new Color(Colors.Yellow, 1);
				return true;
			}
			else if (char.IsUpper(targetSquare.SquareValue[0]) != char.IsUpper(this.SquareValue[0]))
			{
				targetSquare.squarePanel.SelfModulate = new Color(Colors.Yellow, 1);
			}
			return false;
		}
		return false;
	}

	private bool IsValidPosition(int row, int col)
	{
		return row >= 0 && row < 8 && col >= 0 && col < 8;
	}
	
	private bool IsSquareEmpty(int row, int col)
	{
		return board.GetSquare(row, col).SquareValue == "";
	}

	private void HighlightSquare(int targetRow, int targetCol)
	{
		if (IsValidPosition(targetRow, targetCol))
		{
			var targetSquare = board.GetSquare(targetRow, targetCol);
			if (targetSquare.SquareValue == "" || char.IsUpper(targetSquare.SquareValue[0]) != char.IsUpper(this.SquareValue[0]))
			{
				targetSquare.squarePanel.SelfModulate = new Color(Colors.Yellow, 1);
			}
		}
	}
	private bool CanCastleKingSide()
	{
		// Check if the king and the rook have not moved and the squares between them are empty
		if (SquareValue == "K" && row == 0 && col == 4 && IsSquareEmpty(0, 5) && IsSquareEmpty(0, 6) && board.GetSquare(0, 7).SquareValue == "R")
		{
			return true;
		}
		if (SquareValue == "k" && row == 7 && col == 4 && IsSquareEmpty(7, 5) && IsSquareEmpty(7, 6) && board.GetSquare(7, 7).SquareValue == "r")
		{
			return true;
		}
		return false;
	}

	private bool CanCastleQueenSide()
	{
		// Check if the king and the rook have not moved and the squares between them are empty
		if (SquareValue == "K" && row == 0 && col == 4 && IsSquareEmpty(0, 3) && IsSquareEmpty(0, 2) && IsSquareEmpty(0, 1) && board.GetSquare(0, 0).SquareValue == "R")
		{
			return true;
		}
		if (SquareValue == "k" && row == 7 && col == 4 && IsSquareEmpty(7, 3) && IsSquareEmpty(7, 2) && IsSquareEmpty(7, 1) && board.GetSquare(7, 0).SquareValue == "r")
		{
			return true;
		}
		return false;
	}
	
//----------------------------------------------------------------------------
	public void SetPosition(int row, int col)
	{
		this.row = row;
		this.col = col;
	}
	public void ChangeColor()
	{
		if ((row + col) % 2 == 0)
			squarePanel.SelfModulate = new Color(Colors.SandyBrown, 1);
		else
			squarePanel.SelfModulate = new Color(Colors.SaddleBrown, 1);
	}

	public void MakePieces()
	{
		MakePawns();
		MakeKnights();
		MakeBishops();
		MakeRooks();
		MakeKings();
		MakeQueens();
	}

	public void MakePawns()
	{
		if (row == 1)
		{
			SquareValue = "P";
			Text = "P";
		}
		else if (row == 6)
		{
			SquareValue = "p";
			Text = "p";
		}
	}

	public void MakeKnights()
	{
		if (row == 0 && (col == 1 || col == 6))
		{
			SquareValue = "N";
			Text = "N";
		}
		else if (row == 7 && (col == 1 || col == 6))
		{
			SquareValue = "n";
			Text = "n";
		}
	}

	public void MakeBishops()
	{
		if (row == 0 && (col == 2 || col == 5))
		{
			SquareValue = "B";
			Text = "B";
		}
		else if (row == 7 && (col == 2 || col == 5))
		{
			SquareValue = "b";
			Text = "b";
		}
	}

	public void MakeRooks()
	{
		if (row == 0 && (col == 0 || col == 7))
		{
			SquareValue = "R";
			Text = "R";
		}
		else if (row == 7 && (col == 0 || col == 7))
		{
			SquareValue = "r";
			Text = "r";
		}
	}

	public void MakeKings()
	{
		if (row == 0 && col == 4)
		{
			SquareValue = "K";
			Text = "K";
		}
		else if (row == 7 && col == 4)
		{
			SquareValue = "k";
			Text = "k";
		}
	}

	public void MakeQueens()
	{
		if (row == 0 && col == 3)
		{
			SquareValue = "Q";
			Text = "Q";
		}
		else if (row == 7 && col == 3)
		{
			SquareValue = "q";
			Text = "q";
		}
	}
}
