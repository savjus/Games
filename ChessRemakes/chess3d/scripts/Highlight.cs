using Godot;
using System;
using Chess3D.scripts;

// after moving -> highlight, -> check if the king is checked  and unhighlight 

public partial class Highlight : Area3D
{
	[Signal]
	public delegate void MoveModelsEventHandler(Node piece);

	private Board board;
	private MeshInstance3D mesh;
	private Resource highlight = GD.Load("res://art/MAts/Highlight.tres");
	private Resource highlightLight = GD.Load("res://art/MAts/Highlightlight.tres");
	
	private bool kingUnmoved = true;

	public override void _Ready()
	{
		board = (Board)GetParent().GetParent();
		mesh = (MeshInstance3D)GetParent();
		
		
		Connect("mouse_entered", new Callable(this, nameof(OnMouseHover)));
		Connect("mouse_exited", new Callable(this, nameof(OnMouseHoverExit)));
		Connect("input_event", new Callable(this, nameof(OnInputEvent)));
	}

	private void OnInputEvent(Node camera3D, InputEvent @event, Vector3 clickPosition, Vector3 clickNormal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			HandleTileSelection();
		}
	}

	private void HandleTileSelection()
	{
		var piece = board.PiecesMap[this.GlobalPosition];

		if (!IsHighlighted())
		{
			if (piece != null && piece.IsWhite == board.isWhiteTurn)
			{
				SelectPiece(piece);
			}
			else
			{
				GD.Print("Invalid selection or not your turn.");
			}
		}
		else
		{
			AudioStreamPlayer3D moveSoundPlayer = GetParent().GetParent().GetParent().GetNode<AudioStreamPlayer3D>("MoveSound");
			GD.Print("sound");
			moveSoundPlayer.Play();
			board._checkLabel.Visible = false;
			MoveSelectedPiece();
			// for checking if the piece is cheking the enemy king
			GD.Print(board.GetPiece(board.lastPressedPiecePosition));
			HighlightValidMoves(board.GetPiece(board.lastPressedPiecePosition));
			ResetHighlight();
		}
	}

	private void SelectPiece(Pieces piece)
	{
		board.isMoving = true;
		ResetHighlight();
		board.lastPressedPiece = piece;
		GD.Print(board.lastPressedPiece);
		board.lastPressedPiecePosition = this.GlobalPosition;

		HighlightValidMoves(piece);
	}

	private void HighlightValidMoves(Pieces piece)
	{
		switch (piece.Type)
		{
			case 'P':
				HighlightPawnMoves(this.GlobalPosition);
				break;
			case 'R':
				HighlightRookMoves(this.GlobalPosition);
				break;
			case 'N':
				HighlightKnightMoves(this.GlobalPosition);
				break;
			case 'B':
				HighlightBishopMoves(this.GlobalPosition);
				break;
			case 'Q':
				HighlightQueenMoves(this.GlobalPosition);
				break;
			case 'K':
				HighlightKingMoves(this.GlobalPosition);
				break;
		}
	}

	private void MoveSelectedPiece()
	{
		TakePiece();
		MovePieceToTile(this.GlobalPosition);

		board.isWhiteTurn = !board.isWhiteTurn;
		GD.Print($"It's now {(board.isWhiteTurn ? "White" : "Black")}'s turn.");

		if (board.isWhiteTurn)
			board.MoveCameraWhite();
		else
			board.MoveCameraBlack();

		board.isMoving = false;
		ResetHighlight();
	}

	private void TakePiece()
	{
		var targetPosition = this.GlobalPosition;
		var targetPiece = board.PiecesMap[targetPosition];

		if (targetPiece.Type != ' ' && targetPiece.IsWhite != board.lastPressedPiece.IsWhite)
		{
			if (targetPiece.Type == 'K')
			{
				board._VictoryLabel.Visible = true;
				if (board.CheckedPiece.IsWhite)
				{
				GD.Print("White king was taken");
				board._VictoryLabel.Text = "Black Wins!";
				}
				else
				{
				GD.Print("Black king was taken");
				board._VictoryLabel.Text = "White Wins!";
				}
			}
			board.AnimatePieceRemove(targetPiece.Mesh, new Vector3(0, 10, 0));
			board.PiecesMap[targetPosition] = new Pieces(targetPosition, ' ', false, null);
			AudioStreamPlayer3D takeSoundPlayer = GetParent().GetParent().GetParent().GetNode<AudioStreamPlayer3D>("TakeSound");
			takeSoundPlayer.Play();
		}
	}

	private void MovePieceToTile(Vector3 newPosition)
	{
		HandleSpecialMoves(newPosition);

		board.PiecesMap[board.lastPressedPiecePosition] = new Pieces(board.lastPressedPiecePosition, ' ', false, null);
		board.lastPressedPiecePosition = newPosition;
		board.PiecesMap[newPosition] = board.lastPressedPiece;
		board.AnimatePieceMovement(board.lastPressedPiece.Mesh, newPosition);

		board.lastPressedPiece = null;
		ResetHighlight();
	}

	private void HandleSpecialMoves(Vector3 newPosition)
	{
		if (board.lastPressedPiece.Type == 'K')
		{
			kingUnmoved = false;
			HandleCastling(newPosition);
		}

		if (board.lastPressedPiece.Type == 'P' && (GlobalPosition.X == -4 || GlobalPosition.X == 3))
		{
			GD.Print("Promotion");
			board._promotionScreen.Visible = true;
		}
	}

	private void HandleCastling(Vector3 newPosition)
	{
		if (Math.Abs(newPosition.Z - board.lastPressedPiecePosition.Z) > 1)
		{
			GD.Print("Castling");
			PerformCastling(newPosition);
		}
	}

	private void PerformCastling(Vector3 kingNewPosition)
	{
		Vector3 rookOldPosition, rookNewPosition;

		if (kingNewPosition.Z > board.lastPressedPiecePosition.Z)
		{
			rookOldPosition = new Vector3(kingNewPosition.X, 0, kingNewPosition.Z + 2);
			rookNewPosition = new Vector3(kingNewPosition.X, 0, kingNewPosition.Z - 1);
		}
		else
		{
			rookOldPosition = new Vector3(kingNewPosition.X, 0, kingNewPosition.Z - 2);
			rookNewPosition = new Vector3(kingNewPosition.X, 0, kingNewPosition.Z + 1);
		}

		MoveRookForCastling(rookOldPosition, rookNewPosition);
	}

	private void MoveRookForCastling(Vector3 rookOldPosition, Vector3 rookNewPosition)
	{
		var rook = board.PiecesMap[rookOldPosition];
		board.PiecesMap[rookNewPosition] = rook;
		board.PiecesMap[rookOldPosition] = new Pieces(rookOldPosition, ' ', false, null);
		board.AnimatePieceMovement(rook.Mesh, rookNewPosition);
	}

	private void ResetHighlight()
	{
		foreach (var tile in board.Tiles.Values)
		{
			tile.SetMaterialOverride((BaseMaterial3D)GD.Load(
				((int)Math.Floor(tile.GlobalTransform.Origin.X) + (int)Math.Floor(tile.GlobalTransform.Origin.Z)) % 2 == 0
				? "res://art/MAts/BoardTileBlack.tres"
				: "res://art/MAts/BoardTileWhite.tres"
			));
		}
	}

	private void OnMouseHover()
	{
		if (!IsHighlighted())
		{
			GD.Print("Tile: " + GlobalPosition + board.PiecesMap[GlobalPosition].Type);
			mesh.SetMaterialOverride((BaseMaterial3D)highlightLight);
		}
	}

	private void OnMouseHoverExit()
	{
		if (!IsHighlighted())
		{
			mesh.SetMaterialOverride((BaseMaterial3D)GD.Load(
				((int)Math.Floor(mesh.GlobalTransform.Origin.X) + (int)Math.Floor(mesh.GlobalTransform.Origin.Z)) % 2 == 0
				? "res://art/MAts/BoardTileBlack.tres"
				: "res://art/MAts/BoardTileWhite.tres"
			));
		}
	}

	private bool IsHighlighted() => mesh.GetMaterialOverride() == (BaseMaterial3D)highlight;

	public void HighlightTile(Vector3 position) => board.Tiles[position].SetMaterialOverride((StandardMaterial3D)highlight);
	
	private void CheckForWin()
	{
		GD.Print("Check!");
		
		if (board.CheckedPiece.IsWhite)
		{
			GD.Print("White is in check.");
			board._checkLabel.Text = "White is in check!";
			board._checkLabel.Visible = true;
		}
		else
		{
			GD.Print("Blackis in check.");
			board._checkLabel.Text = "Black is in check!";
			board._checkLabel.Visible = true;
		}
	}
	// movement logic
		public void HighlightPawnMoves(Vector3 position)
	{
		var piece = board.PiecesMap[position];
		int direction = piece.IsWhite ? -1 : 1; // Forward direction for the pawn
		Vector3 forward = position + new Vector3(direction, 0, 0); // One square forward
		Vector3 doubleForward = position + new Vector3(2 * direction, 0, 0); // Two squares forward
		Vector3 captureLeft = position + new Vector3(direction, 0, -1); // Diagonal left
		Vector3 captureRight = position + new Vector3(direction, 0, 1); // Diagonal right

		// Forward move
		if (board.PiecesMap.ContainsKey(forward) && board.PiecesMap[forward].Type == ' ')
		{
			HighlightTile(forward);

			// Double move on the pawn's initial position
			if ((piece.IsWhite && position.X == 2) || (!piece.IsWhite && position.X == -3)) 
			{
				if (board.PiecesMap.ContainsKey(doubleForward) && board.PiecesMap[doubleForward].Type == ' ')
				{
					HighlightTile(doubleForward);
				}
			}
		}

		// Captures
		if (board.PiecesMap.ContainsKey(captureLeft) && board.PiecesMap[captureLeft].Type != ' ' && board.PiecesMap[captureLeft].IsWhite != piece.IsWhite)
		{
			HighlightTile(captureLeft);
			if (board.PiecesMap[captureLeft].Type == 'K')
			{
				board.IsChecked = true;
				board.CheckedPiece = board.PiecesMap[captureLeft];
				CheckForWin();
			}
		}
		if (board.PiecesMap.ContainsKey(captureRight) && board.PiecesMap[captureRight].Type != ' ' && board.PiecesMap[captureRight].IsWhite != piece.IsWhite)
		{
			HighlightTile(captureRight);
			if (board.PiecesMap[captureRight].Type == 'K')
			{
				board.IsChecked = true;
				board.CheckedPiece = board.PiecesMap[captureRight];

				CheckForWin();
			}
		}
	}

	public void HighlightKnightMoves(Vector3 position)
	{
		var piece = board.PiecesMap[position];
		Vector3[] knightMoves = new Vector3[]
		{
			new Vector3(2, 0, 1),
			new Vector3(2, 0, -1),
			new Vector3(-2, 0, 1),
			new Vector3(-2, 0, -1),
			new Vector3(1, 0, 2),
			new Vector3(1, 0, -2),
			new Vector3(-1, 0, 2),
			new Vector3(-1, 0, -2)
		};

		foreach (var move in knightMoves)
		{
			Vector3 newPosition = position + move;
			if (board.PiecesMap.ContainsKey(newPosition))
			{
				var targetPiece = board.PiecesMap[newPosition];
				if (targetPiece.Type == ' ' || targetPiece.IsWhite != piece.IsWhite)
				{
					HighlightTile(newPosition);
					if (board.PiecesMap[newPosition].Type == 'K')
					{
						board.IsChecked = true;
						board.CheckedPiece = board.PiecesMap[newPosition];
						CheckForWin();
					}
				}
			}
		}
	}
	public void HighlightQueenMoves(Vector3 position)
	{
		// Move in all straight directions (like a rook)
		MoveStraight(position, 1, false);  // Move forward
		MoveStraight(position, -1, false); // Move backward
		MoveStraight(position, 1, true);   // Move right
		MoveStraight(position, -1, true);  // Move left

		// Move in all diagonal directions (like a bishop)
		MoveDiagonal(position, 1, 1);      // Move diagonally up-right
		MoveDiagonal(position, 1, -1);     // Move diagonally up-left
		MoveDiagonal(position, -1, 1);     // Move diagonally down-right
		MoveDiagonal(position, -1, -1);    // Move diagonally down-left
	}
	public void HighlightKingMoves(Vector3 position)
	{
		var piece = board.PiecesMap[position];
		Vector3[] kingMoves = new Vector3[]
		{
			new Vector3(1, 0, 0),   // Move forward
			new Vector3(-1, 0, 0),  // Move backward
			new Vector3(0, 0, 1),   // Move right
			new Vector3(0, 0, -1),  // Move left
			new Vector3(1, 0, 1),   // Move diagonally up-right
			new Vector3(1, 0, -1),  // Move diagonally up-left
			new Vector3(-1, 0, 1),  // Move diagonally down-right
			new Vector3(-1, 0, -1)  // Move diagonally down-left
		};

		foreach (var move in kingMoves)
		{
			Vector3 newPosition = position + move;
			if (board.PiecesMap.ContainsKey(newPosition))
			{
				var targetPiece = board.PiecesMap[newPosition];
				if (targetPiece.Type == ' ' || targetPiece.IsWhite != piece.IsWhite)
				{
					HighlightTile(newPosition);
					
				}
			}
		}

		if (kingUnmoved)
		{
			if (piece.IsWhite == true)
			{
				if (board.PiecesMap[new Vector3(3, 0, -2)].Type == ' ' &&
					board.PiecesMap[new Vector3(3, 0, -3)].Type == ' ' &&
					board.PiecesMap[new Vector3(3, 0, -4)].Type == 'R')
				{
					HighlightTile(new Vector3(3, 0, -3));
				}

				if (board.PiecesMap[new Vector3(3, 0, 0)].Type == ' ' &&
					board.PiecesMap[new Vector3(3, 0, 1)].Type == ' ' &&
					board.PiecesMap[new Vector3(3, 0, 2)].Type == ' ' &&
					board.PiecesMap[new Vector3(3, 0, 3)].Type == 'R')
				{
					HighlightTile(new Vector3(3, 0, 0));
					HighlightTile(new Vector3(3, 0, 1));
				}
			}
			else
			{
				if (board.PiecesMap[new Vector3(-4, 0, -2)].Type == ' ' &&
					board.PiecesMap[new Vector3(-4, 0, -3)].Type == ' ' &&
					board.PiecesMap[new Vector3(-4, 0, -4)].Type == 'R')
				{
					HighlightTile(new Vector3(-4, 0, -3));
				}

				if (board.PiecesMap[new Vector3(-4, 0, 0)].Type == ' ' &&
					board.PiecesMap[new Vector3(-4, 0, 1)].Type == ' ' &&
					board.PiecesMap[new Vector3(-4, 0, 2)].Type == ' ' &&
					board.PiecesMap[new Vector3(-4, 0, 3)].Type == 'R')
				{
					HighlightTile(new Vector3(-4, 0, 0));
					HighlightTile(new Vector3(-4, 0, 1));
				}
			}
		}
	}
	public void HighlightRookMoves(Vector3 position)
	{
		// Move forward
		MoveStraight(position, 1,false);

		// Move backward
		MoveStraight(position, -1,false);

		// Move right
		MoveStraight(position, 1,true);

		// Move left
		MoveStraight(position, -1,true);
	}
	
	public void HighlightBishopMoves(Vector3 position)
	{
		// Move diagonally up-right
		MoveDiagonal(position, 1, 1);

		// Move diagonally up-left
		MoveDiagonal(position, 1, -1);

		// Move diagonally down-right
		MoveDiagonal(position, -1, 1);

		// Move diagonally down-left
		MoveDiagonal(position, -1, -1);
	}
	private void MoveDiagonal(Vector3 position, int directionX, int directionZ)
	{
		var piece = board.PiecesMap[position];
		int i = 1;
		while (board.PiecesMap.ContainsKey(position + new Vector3(i * directionX, 0, i * directionZ)) && board.PiecesMap[position + new Vector3(i * directionX, 0, i * directionZ)].Type == ' ')
		{
			HighlightTile(position + new Vector3(i * directionX, 0, i * directionZ));
			if (board.PiecesMap[position + new Vector3(i * directionX, 0, i * directionZ)].Type == 'K')
			{
				board.IsChecked = true;
				board.CheckedPiece = board.PiecesMap[position + new Vector3(i * directionX, 0, i * directionZ)];
				CheckForWin();
			}
			i++;
		}
		if (board.PiecesMap.ContainsKey(position + new Vector3(i * directionX, 0, i * directionZ)) && board.PiecesMap[position + new Vector3(i * directionX, 0, i * directionZ)].IsWhite != piece.IsWhite)
		{
			HighlightTile(position + new Vector3(i * directionX, 0, i * directionZ));
			if (board.PiecesMap[position + new Vector3(i * directionX, 0, i * directionZ)].Type == 'K')
			{
				board.IsChecked = true;
				board.CheckedPiece = board.PiecesMap[position + new Vector3(i * directionX, 0, i * directionZ)];
				CheckForWin();
			}
		}
	}
	private void MoveStraight(Vector3 position, int direction, bool isHorizontal)
	{
		var piece = board.PiecesMap[position];
		int i = 1;
		if (isHorizontal)
		{
			while (board.PiecesMap.ContainsKey(position + new Vector3(0, 0, i * direction)) && board.PiecesMap[position + new Vector3(0, 0, i * direction)].Type == ' ')
			{
				HighlightTile(position + new Vector3(0, 0, i * direction));
				if (board.PiecesMap[position + new Vector3(0, 0, i * direction)].Type == 'K')
				{
					board.IsChecked = true;
					board.CheckedPiece = board.PiecesMap[position + new Vector3(0, 0, i * direction)];
					CheckForWin();
				}
				i++;
			}
			if (board.PiecesMap.ContainsKey(position + new Vector3(0, 0, i * direction)) && board.PiecesMap[position + new Vector3(0, 0, i * direction)].IsWhite != piece.IsWhite)
			{
				HighlightTile(position + new Vector3(0, 0, i * direction));
				if (board.PiecesMap[position + new Vector3(0, 0, i * direction)].Type == 'K')
				{
					board.IsChecked = true;
					board.CheckedPiece = board.PiecesMap[position + new Vector3(0, 0, i * direction)];
					CheckForWin();
				}
			}
		}
		else
		{
			while (board.PiecesMap.ContainsKey(position + new Vector3(i * direction, 0, 0)) && board.PiecesMap[position + new Vector3(i * direction, 0, 0)].Type == ' ')
			{
				HighlightTile(position + new Vector3(i * direction, 0, 0));
				if (board.PiecesMap[position + new Vector3(i * direction, 0, 0)].Type == 'K')
				{
					board.IsChecked = true;
					board.CheckedPiece = board.PiecesMap[position + new Vector3(i * direction, 0, 0)];
					CheckForWin();
				}
				i++;
			}
			if (board.PiecesMap.ContainsKey(position + new Vector3(i * direction, 0, 0)) && board.PiecesMap[position + new Vector3(i * direction, 0, 0)].IsWhite != piece.IsWhite)
			{
				HighlightTile(position + new Vector3(i * direction, 0, 0));
				if (board.PiecesMap[position + new Vector3(i * direction, 0, 0)].Type == 'K')
				{
					board.IsChecked = true;
					board.CheckedPiece = board.PiecesMap[position + new Vector3(i * direction, 0, 0)];
					CheckForWin();
				}
			}
		}
	}
}
