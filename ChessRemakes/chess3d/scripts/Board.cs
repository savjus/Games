using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using Chess3D.scripts;

public partial class Board : MeshInstance3D
{
	public Dictionary<Vector3,MeshInstance3D> Tiles = new Dictionary<Vector3,MeshInstance3D>();
	public Dictionary<Vector3,Pieces> PiecesMap = new Dictionary<Vector3,Pieces>();
	
	
	private PackedScene KnightPiece = GD.Load<PackedScene>("res://scenes/Knight.tscn");
	private PackedScene RookPiece = GD.Load<PackedScene>("res://scenes/Rook.tscn");
	private PackedScene BishopPiece = GD.Load<PackedScene>("res://scenes/Bishop.tscn");
	private PackedScene QueenPiece = GD.Load<PackedScene>("res://scenes/Queen.tscn");
	private PackedScene KingPiece = GD.Load<PackedScene>("res://scenes/King.tscn");
	private PackedScene PawnPiece = GD.Load<PackedScene>("res://scenes/Pawn.tscn");
	
	private PackedScene BoardTile = GD.Load<PackedScene>("res://scenes/tile.tscn");
	
	private Resource PieceBlack = GD.Load<StandardMaterial3D>("res://art/MAts/PieceBlack.tres");
	private Resource PieceWhite = GD.Load<StandardMaterial3D>("res://art/MAts/PieceWhite.tres");
	private Camera3D camera;
	public Vector3 lastPressedPiecePosition;
	public Pieces lastPressedPiece;
	public bool isWhiteTurn = true;
	public bool isMoving;
	public PromotionScreen _promotionScreen;
	public Label _VictoryLabel;
	public Label _checkLabel;
	public bool IsChecked;
	public Pieces CheckedPiece;
	
	
	
	public override void _Ready()
	{
		camera = GetNode<Camera3D>($"../Camera3D");
		_promotionScreen = GetNode<PromotionScreen>($"../Camera3D/Hud/Promotion");
		_checkLabel = GetNode<Label>($"../Camera3D/Hud/Check/Label");
		_VictoryLabel = GetNode<Label>($"../Camera3D/Hud/victory/Label");
		MakeBoard();
		MakePieces();
		
	}

	public void PromotePawn(char promoteTo)
	{
	
		PiecesMap[lastPressedPiecePosition].Mesh.QueueFree(); // Remove the old pawn
		MeshInstance3D newPiece = null;

		switch (promoteTo)
		{
			case 'Q':
				newPiece = (MeshInstance3D)QueenPiece.Instantiate();
				break;
			case 'R':
				newPiece = (MeshInstance3D)RookPiece.Instantiate();
				break;
			case 'N':
				newPiece = (MeshInstance3D)KnightPiece.Instantiate();
				break;
			case 'B':
				newPiece = (MeshInstance3D)BishopPiece.Instantiate();
				break;
		}

		if (newPiece != null)
		{
			AddChild(newPiece);
			newPiece.GlobalPosition = lastPressedPiecePosition;
			PiecesMap[lastPressedPiecePosition] = new Pieces(lastPressedPiecePosition, promoteTo, PiecesMap[lastPressedPiecePosition].IsWhite, newPiece);
			if (PiecesMap[lastPressedPiecePosition].IsWhite)
			{
				var a = newPiece.GetChild(0).GetChild(0) as MeshInstance3D;
				a.SetMaterialOverride((StandardMaterial3D)PieceWhite);

			}
			else
			{
				var a = newPiece.GetChild(0).GetChild(0) as MeshInstance3D;
				a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
				
			}
		}
	}
	public Pieces GetPiece(Vector3 position)
	{
		if (PiecesMap.ContainsKey(position))
		{
			return PiecesMap[position];
		}

		return null;
	}
	public void AnimatePieceMovement(Node3D piece, Vector3 endPosition)
	{
		float duration = 0.5f; // Duration of the animation in seconds
		endPosition = endPosition + new Vector3(0, 0.4f, 0); // Add the height of the piece to the end position
		
		Tween tween = CreateTween(); // Create and add the Tween as a child of the Board (current node)
		tween.TweenProperty(piece, "global_position", endPosition, duration);
		tween.Play();
	}
	public void AnimatePieceRemove(Node3D piece, Vector3 endPosition)
	{
		float duration = 6f; // Duration of the animation in seconds
		endPosition = endPosition + new Vector3(0, 0.4f, 0); // Add the height of the piece to the end position
		
		Tween tween = CreateTween(); // Create and add the Tween as a child of the Board (current node)
		tween.TweenProperty(piece, "global_position", endPosition, duration);
		tween.Play();
		
	}

	public void MoveCameraBlack()
	{
		camera.Position = new Vector3(-5, 5, 0);
		camera.RotationDegrees = new Vector3(-45, -90,0);
	}
	public void MoveCameraWhite()
	{
		camera.Position = new Vector3(4, 5, -1);
		camera.RotationDegrees = new Vector3(-45, 90, 0);
	}
	
	// make board tiles
	private void MakeBoard()
	{
		for (int i = -4; i < 4; i++)
		{
			for (int j = -4; j < 4; j++)
			{
				var tile = (MeshInstance3D)BoardTile.Instantiate();
				AddChild(tile);
				tile.Position = new Vector3(i, 0, j);
				Tiles[new Vector3(i, 0, j)] = tile;
				GD.Print($"valid tile: " + tile.GlobalPosition);

				if ((i + j) % 2 == 0)
					tile.SetMaterialOverride((StandardMaterial3D)GD.Load("res://art/MAts/BoardTileBlack.tres"));
				else
					tile.SetMaterialOverride((StandardMaterial3D)GD.Load("res://art/MAts/BoardTileWhite.tres"));
			}
		}
	}
	
	
		
		// draws the initial pieces on the board
	private void MakePieces()
	{
		foreach (var children in GetChildren())
		{
			if (children is MeshInstance3D meshInstance3D)
			{
				var pos = meshInstance3D.GlobalTransform.Origin;

				if (pos.X == -3 || pos.X == 2)
				{
					// instantiate pawns
					var pawn = (MeshInstance3D)PawnPiece.Instantiate();
					AddChild(pawn);
					pawn.GlobalPosition = pos + new Vector3(0, 0.3f, 0);
					if (pos.X == -3)
					{
						PiecesMap[pos] = new Pieces(pos, 'P', false, pawn);
						var a = pawn.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
					}
					else
					{
						PiecesMap[pos] = new Pieces(pos, 'P', true, pawn);
						var a = pawn.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceWhite);
					}

					continue;
				}

				if (pos == new Vector3(-4, 0, -4) ||
					pos == new Vector3(-4, 0, 3) ||
					pos == new Vector3(3, 0, 3) ||
					pos == new Vector3(3, 0, -4))
				{
					// instantiate rook
					var rook = (Node3D)RookPiece.Instantiate();
					AddChild(rook);
					rook.GlobalPosition = pos + new Vector3(0, 0.5f, 0);
					if (pos == new Vector3(-4, 0, -4) || pos == new Vector3(-4, 0, 3))
					{
						PiecesMap[pos] = new Pieces(pos, 'R', false, rook);
						var a = rook.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
					}
					else
					{
						PiecesMap[pos] = new Pieces(pos, 'R', true, rook);
						var a = rook.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceWhite);
					}

					continue;
				}

				if (pos == new Vector3(3, 0, -3) ||
					pos == new Vector3(3, 0, 2) ||
					pos == new Vector3(-4, 0, -3) ||
					pos == new Vector3(-4, 0, 2))
				{
					// instantiate knight
					var knight = (Node3D)KnightPiece.Instantiate();
					AddChild(knight);
					knight.GlobalPosition = pos + new Vector3(0, 0.4f, 0);
					if (pos == new Vector3(-4, 0, -3) || pos == new Vector3(-4, 0, 2))
					{
						PiecesMap[pos] = new Pieces(pos, 'N', false, knight);
						var a = knight.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
					}
					else
					{
						PiecesMap[pos] = new Pieces(pos, 'N', true, knight);
						var a = knight.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceWhite);
					}

					continue;
				}

				if (pos == new Vector3(3, 0, -2) ||
					pos == new Vector3(3, 0, 1) ||
					pos == new Vector3(-4, 0, -2) ||
					pos == new Vector3(-4, 0, 1))
				{
					// instantiate bishop
					var bishop = (Node3D)BishopPiece.Instantiate();
					AddChild(bishop);
					bishop.GlobalPosition = pos + new Vector3(0, 0.5f, 0);
					if (pos == new Vector3(-4, 0, -2) || pos == new Vector3(-4, 0, 1))
					{
						PiecesMap[pos] = new Pieces(pos, 'B', false, bishop);
						var a = bishop.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
					}
					else
					{
						PiecesMap[pos] = new Pieces(pos, 'B', true, bishop);
						var a = bishop.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceWhite);
					}

					continue;
				}

				if (pos == new Vector3(3, 0, 0) ||
					pos == new Vector3(-4, 0, 0))
				{
					// instantiate queen
					var queen = (Node3D)QueenPiece.Instantiate();
					AddChild(queen);
					queen.GlobalPosition = pos + new Vector3(0, 0.5f, 0);
					if (pos == new Vector3(-4, 0, 0))
					{
						PiecesMap[pos] = new Pieces(pos, 'Q', false, queen);
						var a = queen.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
					}
					else
					{
						PiecesMap[pos] = new Pieces(pos, 'Q', true, queen);
						var a = queen.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceWhite);
					}

					continue;
				}

				if (pos == new Vector3(3, 0, -1) ||
					pos == new Vector3(-4, 0, -1))
				{
					// instantiate king
					var king = (Node3D)KingPiece.Instantiate();
					AddChild(king);
					king.GlobalPosition = pos + new Vector3(0, 0.5f, 0);
					if (pos == new Vector3(-4, 0, -1))
					{
						PiecesMap[pos] = new Pieces(pos, 'K', false, king);
						var a = king.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceBlack);
					}
					else
					{
						PiecesMap[pos] = new Pieces(pos, 'K', true, king);
						var a = king.GetChild(0).GetChild(0) as MeshInstance3D;
						a.SetMaterialOverride((StandardMaterial3D)PieceWhite);
					}

					continue;
				}

				PiecesMap[pos] = new Pieces(pos, ' ', false, meshInstance3D);
			}
		}
		// tiles start from -3 -3, to 4 4 
	}

}
