using Godot;
using System;

public partial class PlayerMovement : CharacterBody3D
{
	[Export] float speed;
	 private Vector3 _targetVelocity = Vector3.Zero;


	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = Vector3.Zero;

		if(Input.IsKeyPressed(Key.A)) direction.X+=1.0f;
		if(Input.IsKeyPressed(Key.D)) direction.X-=1.0f;
		if(Input.IsKeyPressed(Key.W)) direction.Z+=1.0f;
		if(Input.IsKeyPressed(Key.S)) direction.Z-=1.0f;

		_targetVelocity.X=direction.X * speed;
		_targetVelocity.Z=direction.Z * speed;

		Velocity = _targetVelocity;
		MoveAndSlide();
		
	}
}
