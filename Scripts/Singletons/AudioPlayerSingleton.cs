using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

/// <summary>
/// Plays spatial and non-spatial audio in a queue. Singleton pattern instead of a Godot autoload to match the
/// existing approach in the project.
/// </summary>
public partial class AudioPlayerSingleton : Node
{
	public static AudioPlayerSingleton Instance { get; private set; }

	[ExportGroup("References")]
	[Export] private AudioStreamPlayer _musicPlayer;
	[Export] private Node _sfxPlayerContainer;

	private Queue<AudioStreamPlayer> _streamQueueNonSpatial;
	private Queue<AudioStreamPlayer3D> _streamQueueSpatial;

	[ExportGroup("Assets")]
	[Export] public AudioStream _initialMusic;

	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}

		Instance = this;
	}

	public override void _ExitTree()
	{
		if (Instance == this)
			Instance = null;
	}

	public override void _Ready()
	{
		_streamQueueNonSpatial =  new Queue<AudioStreamPlayer>();
		_streamQueueSpatial = new Queue<AudioStreamPlayer3D>();
		
		Array<Node>? children = _sfxPlayerContainer.GetChildren();
		if (children != null)
		{
			foreach (Node child in children)
			{
				if (child is AudioStreamPlayer nonSpatial)
				{
					_streamQueueNonSpatial.Enqueue(nonSpatial);
				}
				else if (child is AudioStreamPlayer3D spatial)
				{
					_streamQueueSpatial.Enqueue(spatial);
				}
			}
		}

		if (_initialMusic != null)
		{
			PlayMusic(_initialMusic);
		}
	}

	public async void PlayMusic(AudioStream stream, float crossFade = 0.0f)
	{
		try
		{
			if (_musicPlayer.Playing)
			{
				if (crossFade > 0.0f)
				{
					PropertyTweener? fadeOutTween =
						CreateTween().TweenProperty(_musicPlayer, "VolumeLinear", 0.0f, crossFade);
					await ToSignal(fadeOutTween, "finished");
				}

				_musicPlayer.Stop();
			}

			_musicPlayer.Stream = stream;
			_musicPlayer.Play();
			if (crossFade > 0.0f)
			{
				_musicPlayer.VolumeLinear = 0.0f;
				PropertyTweener? fadeInTween =
					CreateTween().TweenProperty(_musicPlayer, "VolumeLinear", 1.0f, crossFade);
				await ToSignal(fadeInTween, "finished");
			}
			else
			{
				_musicPlayer.VolumeLinear = 1.0f;
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex.ToString());
		}
	}

	public void PlaySfx(AudioStream stream)
	{
		if (_streamQueueNonSpatial.TryDequeue(out AudioStreamPlayer nonSpatial))
		{
			if (nonSpatial.Playing)
			{
				GD.PushWarning("SFX queue exhausted! Play fewer sound effects, or add more stream players. (Non-spatial)");
			}

			nonSpatial.Stream = stream;
			nonSpatial.Play();
			_streamQueueNonSpatial.Enqueue(nonSpatial);
		}
	}

	public void PlaySfx(AudioStream stream, Vector3 position)
	{
		if (_streamQueueSpatial.TryDequeue(out AudioStreamPlayer3D spatial))
		{
			if (spatial.Playing)
			{
				GD.PushWarning("SFX queue exhausted! Play fewer sound effects, or add more stream players. (Spatial)");
			}

			spatial.Stream = stream;
			spatial.Position = position;
			spatial.Play();
			_streamQueueSpatial.Enqueue(spatial);
		}
	}
}
