using System;
using System.Numerics;
using Godot;
using Vector3 = Godot.Vector3;

namespace Chess3D.scripts;

public class Pieces
{
	public Vector3 Position { get; set; }
	public char Type { get; set; }
	public Node3D Mesh { get; set; }
	public bool IsWhite { get; set; }
	private Board board;
	private Highlight highlight;
	public Pieces(Vector3 position, char type, bool isWhite,Node3D mesh)
	{
		Position = position;
		Type = type;
		IsWhite = isWhite;
		Mesh = mesh;
	}
   
	
	public override string ToString()
	{
		return $"{Position},{Type},{IsWhite},{Mesh}";
	}
	
	
}
