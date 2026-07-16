using Godot;
using System;
using System.Collections.Generic;

public partial class Guard : CharacterBody3D
{
	[ExportGroup("State Variables")]
	[Export] Path3D patrolPath;
	[Export] float CharacterSpeed;
	[Export] float SearchTime;
	[Export] float SearchTurnDuration;
	
	[ExportGroup("View Variables")]
	[Export] int NumberOfRaycasts = 6;
	[Export] float ViewAngle=45.0f;
	[Export] float ViewSize = 10.0f;

	RayCast3D template;
	List<RayCast3D> rays=new List<RayCast3D>();

	public override void _Ready()
	{

		StatesSetUp();

		template = GetNode("PlayerMesh/RayCast3D") as RayCast3D;
		template.TargetPosition= new Vector3(0.0f,0.0f,ViewSize);

		foreach (RayCast3D ray in rays)
		{
			if (IsInstanceValid(ray)) ray.QueueFree();
		}
		
		if(NumberOfRaycasts-1<=0) return;

		for(int i=0; i<NumberOfRaycasts-1;i++)
		{
			RayCast3D ray=template.Duplicate() as RayCast3D;
			ray.Enabled = false;
			template.AddSibling(ray);
			ray.Transform = template.Transform;
			rays.Add(ray);
		}

		template.Enabled = false;
		FanOut();	

	}
	
	void StatesSetUp()
	{
		Node patrollingState = GetNode<Node>("EnemyStateMachine/PatrollingState");
		Node searchingState = GetNode<Node>("EnemyStateMachine/SearchState");
		Node chasingState = GetNode<Node>("EnemyStateMachine/ChasingState");
		Node MachineState = GetNode<Node>("EnemyStateMachine");

		if (patrollingState != null ){

			(patrollingState as PatrollingState).SetPatrolPath(patrolPath);
			(patrollingState as PatrollingState).SetCharacterSpeed(CharacterSpeed);
			(patrollingState as PatrollingState).SetCharacter(this);
		}

		if(searchingState != null)
		{
		}

		if(chasingState != null)
		{
			(chasingState as ChasingState).SetCharacterSpeed(CharacterSpeed);
			(chasingState as ChasingState).SetCharacter(this);
		}

		(MachineState as StateMachine).StateMachineSetUp();
	}

	void FanOut()
	{
		float number = NumberOfRaycasts;

		if(number <=0) return;

		float half = ViewAngle*0.5f;
		float step = (number <=1.0f)? 0.0f : (ViewAngle/(number-1));

		for(int i=0; i<NumberOfRaycasts-1;i++)
		{
			float offset= -half+step*i;
			var ray = rays[i];
			ray.Rotation= new Vector3(0.0f,template.Rotation.Y+Mathf.DegToRad(offset),0.0f);
			ray.Enabled = true;
		}		
	}
}
