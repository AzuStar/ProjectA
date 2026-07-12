using Godot;
using System;

public partial class Guard : CharacterBody3D
{
	[ExportGroup("State Variables")]
	[Export] Node3D[] Route;
	[Export] float CharacterSpeed;
	[Export] float SearchTime;
	[Export] float SearchTurnDuration;


	public override void _Ready()
	{
		Node patrollingState = GetNode<Node>("EnemyStateMachine/PatrollingState");
		Node searchingState = GetNode<Node>("EnemyStateMachine/SearchState");
		Node chasingState = GetNode<Node>("EnemyStateMachine/ChasingState");
		Node MachineState = GetNode<Node>("EnemyStateMachine");

		if (patrollingState != null ){

			(patrollingState as PatrollingState).SetRoute(Route);
			(patrollingState as PatrollingState).SetCharacterSpeed(CharacterSpeed);
			(patrollingState as PatrollingState).SetCharacter(this);
		}

		if(searchingState != null)
		{
			(searchingState as SearchState).SetCharacter(this);
			(searchingState as SearchState).SetSearchTime(SearchTime);
			(searchingState as SearchState).SetTurnDurations(SearchTurnDuration);
		}

		if(chasingState != null)
		{
			(chasingState as ChasingState).SetCharacterSpeed(CharacterSpeed);
			(chasingState as ChasingState).SetCharacter(this);
		}

		(MachineState as StateMachine).StateMachineSetUp();

	}
}
