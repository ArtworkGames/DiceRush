using Cysharp.Threading.Tasks;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.Sound.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.Sound
{
	public class MusicInfo
	{
		public string ClipName;
	}

	public class SoundManager : MonoBehaviour, IService
	{
		private class SoundFXParams
		{
			public string clipName;
			public float delay = 0.0f;
			public bool loop = false;
		}

		private const string MUSIC_ENABLED_KEY = "Music";
		private const string SOUND_ENABLED_KEY = "Sound";
		private const string ON_VALUE = "On";
		private const string OFF_VALUE = "Off";

		public bool MusicEnabled => _musicEnabled;
		public bool SoundFXEnabled => _soundFXEnabled;
		public int currentMusicIndex => _currentMusicIndex;
		public MusicInfo currentMusicInfo => _musicInfos[_currentMusicIndex];

		private bool _musicEnabled = true;
		private bool _soundFXEnabled = true;

		private float _musicVolume = 0.4f;
		private float _soundFXVolume = 0.7f;

		private MusicInfo[] _musicInfos = new MusicInfo[]
		{
			new MusicInfo() { ClipName = "music_1_64" },
			new MusicInfo() { ClipName = "music_1_64" }
		};
		private int _currentMusicIndex = 0;

		private AudioClip _musicClip;
		private AudioSource _musicAudioSource;
		private bool isMusicLoaded;

		private Dictionary<string, AudioClip> _soundClips = new Dictionary<string, AudioClip>();
		private List<AudioSource> _soundAudioSources = new List<AudioSource>();
		private List<AudioSource> _pausedAudioSources = new List<AudioSource>();
		private Dictionary<string, float> _soundTimes = new Dictionary<string, float>();

		private bool isDestroyed;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			ServiceLocator.Register(this);

			SignalBus.Subscribe<ChangeMusicStateSignal>(OnChangeMusicState);
			SignalBus.Subscribe<ChangeSoundFXStateSignal>(OnChangeSoundFXState);
			SignalBus.Subscribe<PlaySoundFXSignal>(OnPlaySoundFX);
			SignalBus.Subscribe<StopSoundFXSignal>(OnStopSoundFX);

			if (PlayerPrefs.HasKey(MUSIC_ENABLED_KEY))
			{
				_musicEnabled = PlayerPrefs.GetString(MUSIC_ENABLED_KEY) == ON_VALUE;
			}

			if (PlayerPrefs.HasKey(SOUND_ENABLED_KEY))
			{
				_soundFXEnabled = PlayerPrefs.GetString(SOUND_ENABLED_KEY) == ON_VALUE;
			}

			_musicAudioSource = gameObject.AddComponent<AudioSource>();
			_musicAudioSource.playOnAwake = false;
		}

		private void Start()
		{
			PlayMusic();
		}

		private void OnDestroy()
		{
			isDestroyed = true;

			SignalBus.Unsubscribe<ChangeMusicStateSignal>(OnChangeMusicState);
			SignalBus.Unsubscribe<ChangeSoundFXStateSignal>(OnChangeSoundFXState);
			SignalBus.Unsubscribe<PlaySoundFXSignal>(OnPlaySoundFX);
			SignalBus.Unsubscribe<StopSoundFXSignal>(OnStopSoundFX);

			ServiceLocator.Unregister<SoundManager>();
		}

		#region Music
		private void OnChangeMusicState(ChangeMusicStateSignal signal)
		{
			SetMusicState(signal.State);
		}

		public void SetMusicState(bool state)
		{
			if (_musicEnabled == state) return;

			_musicEnabled = state;
			PlayerPrefs.SetString(MUSIC_ENABLED_KEY, _musicEnabled ? ON_VALUE : OFF_VALUE);

			if (_musicEnabled)
			{
				PlayMusic();
			}
			else
			{
				StopMusic();
			}

			SignalBus.Publish(new MusicStateChangedSignal(_musicEnabled));
		}

		public void SetTemporaryMusicState(bool state)
		{
			if (_musicEnabled == state) return;
			_musicEnabled = state;

			if (_musicEnabled)
			{
				PlayMusic();
			}
			else
			{
				StopMusic();
			}
		}

		public void PlayPrevMusic()
		{
			int index = _currentMusicIndex;
			index--;
			if (index == -1) index = _musicInfos.Length - 1;

			PlayMusic(index);
		}

		public void PlayNextMusic()
		{
			int index = _currentMusicIndex;
			index++;
			if (index == _musicInfos.Length) index = 0;

			PlayMusic(index);
		}

		public void PlayMusic()
		{
			//return;
			PlayMusic(_currentMusicIndex);
		}

		public void PlayMusic(int index)
		{
			//if (_currentMusicIndex == index) return;

			if ((_currentMusicIndex != index) && (_musicClip != null))
			{
				_musicAudioSource.Stop();
				_musicAudioSource.clip = null;
				_musicClip = null;
			}

			_currentMusicIndex = index;
			//if (OnMusicChange != null) OnMusicChange();

			if (!_musicEnabled) return;

			if (_musicClip == null)
			{
				_ = LoadMusicAsync();
			}
			else
			{
				_musicAudioSource.UnPause();
			}
		}

		private async UniTask LoadMusicAsync()
		{
			isMusicLoaded = false;

			if (!_musicEnabled || isDestroyed) return;
			string currentMusicName = currentMusicInfo.ClipName;

			ResourceRequest musicRequest = Resources.LoadAsync<AudioClip>(GetMusicPath(currentMusicInfo.ClipName));
			await UniTask.WaitUntil(() => musicRequest.isDone);

			if (!_musicEnabled || isDestroyed || (currentMusicName != currentMusicInfo.ClipName)) return;

			_musicClip = (AudioClip)musicRequest.asset;
			_musicAudioSource.clip = _musicClip;
			_musicAudioSource.volume = _musicVolume;
			_musicAudioSource.Play();

			isMusicLoaded = true;
		}

		private string GetMusicPath(string clipName)
		{
			return $"Music/{clipName}";
		}

		public void StopMusic(bool clear = false)
		{
			if (clear)
			{
				_musicAudioSource.Stop();
				_musicAudioSource.clip = null;
				_musicClip = null;
			}
			else
			{
				_musicAudioSource.Pause();
			}
		}

		private void FixedUpdate()
		{
			if (_musicEnabled && isMusicLoaded && !_musicAudioSource.isPlaying)
			{
				PlayNextMusic();
			}
		}
		#endregion

		#region SoundFX
		private void OnChangeSoundFXState(ChangeSoundFXStateSignal signal)
		{
			SetSoundFXState(signal.State);
		}

		public void SetSoundFXState(bool state)
		{
			if (_soundFXEnabled == state) return;

			_soundFXEnabled = state;
			PlayerPrefs.SetString(SOUND_ENABLED_KEY, _soundFXEnabled ? ON_VALUE : OFF_VALUE);

			SignalBus.Publish(new SoundFXStateChangedSignal(_soundFXEnabled));
		}

		public void SetTemporarySoundFXState(bool state)
		{
			if (_soundFXEnabled == state) return;
			_soundFXEnabled = state;
		}

		private void OnPlaySoundFX(PlaySoundFXSignal signal)
		{
			PlaySoundFX(signal.ClipName, signal.Loop, signal.Delay);
		}

		private void OnStopSoundFX(StopSoundFXSignal signal)
		{
			StopSoundFX(signal.ClipName);
		}

		public void PlaySoundFX(string clipName, bool loop = false, float delay = 0.0f)
		{
			if (!_soundFXEnabled) return;

			if (_soundTimes.ContainsKey(clipName))
			{
				float lastTime = _soundTimes[clipName];
				if ((Time.unscaledTime - lastTime) <= 0.05f)
				{
					return;
				}
			}
			_soundTimes[clipName] = Time.unscaledTime;

			SoundFXParams @params = new SoundFXParams();
			@params.clipName = clipName;
			@params.delay = delay;
			@params.loop = loop;

			if (_soundClips.ContainsKey(clipName))
			{
				_ = PlaySoundFXAsync(@params);
			}
			else
			{
				_ = LoadSoundFXAsync(@params);
			}
		}

		private async UniTask LoadSoundFXAsync(SoundFXParams @params)
		{
			ResourceRequest soundRequest = Resources.LoadAsync<AudioClip>(GetSoundFXPath(@params.clipName));
			await UniTask.WaitUntil(() => soundRequest.isDone);

			if (isDestroyed) return;

			AudioClip soundClip = (AudioClip)soundRequest.asset;
			if (!_soundClips.ContainsKey(@params.clipName))
			{
				_soundClips.Add(@params.clipName, soundClip);
			}
			_ = PlaySoundFXAsync(@params);
		}

		private string GetSoundFXPath(string clipName)
		{
			return $"Sound/{clipName}";
		}

		private async UniTask PlaySoundFXAsync(SoundFXParams @params)
		{
			if (!_soundFXEnabled || isDestroyed) return;
			if (@params.delay > 0.0f)
            {
				await UniTask.WaitForSeconds(@params.delay);
            }
			if (!_soundFXEnabled || isDestroyed) return;

			AudioSource source = GetFreeSoundAudioSource();
			source.clip = _soundClips[@params.clipName];
			source.loop = @params.loop;
			source.Play();
		}

		private AudioSource GetFreeSoundAudioSource()
		{
			AudioSource freeSource = null;
			for (int i = 0; i < _soundAudioSources.Count; i++)
			{
				AudioSource audioSource = _soundAudioSources[i];
				if ((!_pausedAudioSources.Contains(audioSource) && !audioSource.isPlaying) || (audioSource.clip == null))
				{
					freeSource = audioSource;
					break;
				}
			}

			if (freeSource == null)
			{
				freeSource = gameObject.AddComponent<AudioSource>();
				freeSource.playOnAwake = false;
				_soundAudioSources.Add(freeSource);
			}
			freeSource.loop = false;
			freeSource.volume = _soundFXVolume;
			return freeSource;
		}

		public void StopSoundFX(string clipName)
		{
			AudioSource audioSource = GetPausedAudioSourceByClipName(clipName);
			if (audioSource != null)
			{
				_pausedAudioSources.Remove(audioSource);
			}

			audioSource = GetSoundAudioSourceByClipName(clipName);
			if (audioSource != null)
			{
				audioSource.Stop();
			}
		}

		private AudioSource GetSoundAudioSourceByClipName(string clipName)
		{
			for (int i = 0; i < _soundAudioSources.Count; i++)
			{
				AudioSource audioSource = _soundAudioSources[i];
				if ((audioSource != null) && !_pausedAudioSources.Contains(audioSource) &&
					audioSource.isPlaying && (audioSource.clip.name == clipName))
				{
					return audioSource;
				}
			}
			return null;
		}

		private AudioSource GetPausedAudioSourceByClipName(string clipName)
		{
			for (int i = 0; i < _pausedAudioSources.Count; i++)
			{
				AudioSource audioSource = _pausedAudioSources[i];
				if ((audioSource != null) && (audioSource.clip.name == clipName))
				{
					return audioSource;
				}
			}
			return null;
		}

		public void PauseSoundFX(string clipName)
		{
			AudioSource audioSource = GetSoundAudioSourceByClipName(clipName);

			if ((audioSource != null) && !_pausedAudioSources.Contains(audioSource))
			{
				_pausedAudioSources.Add(audioSource);
				audioSource.Pause();
			}
		}

		public void UnpauseSoundFX(string clipName)
		{
			AudioSource audioSource = GetPausedAudioSourceByClipName(clipName);

			if (audioSource != null)
			{
				audioSource.UnPause();
				_pausedAudioSources.Remove(audioSource);
			}
		}
		#endregion
	}
}
