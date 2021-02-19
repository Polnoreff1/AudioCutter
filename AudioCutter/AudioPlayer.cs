/*
    Дмитрий Головин
    https://www.youtube.com/channel/UC3BlvFTVSTbyxsl1w0xcQdQ
    22 ноября 2019
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace DAudio
{
    public enum Status
    {
        Undefined,
        Stopped,
        Paused,
        Playing,
        Ended,
        Transitioning,
        Ready
    }

    public sealed class Audio
    {
        /// <summary>
        /// Название аудио
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Длительность аудио в секундах
        /// </summary>
        public double Duration { get; private set; }
        public double Duration1 { get; set; }
        /// <summary>
        /// Длительность аудио в формате TimeSpan
        /// </summary>
        public TimeSpan DurationTime => TimeSpan.FromSeconds(Duration);
        /// <summary>
        /// Путь к аудио файлу
        /// </summary>
        public string SourceUrl { get; private set; }
        /// <summary>
        /// Объект IWMPMedia
        /// </summary>
        public IWMPMedia Media { get; private set; }

        public Audio(IWMPMedia media)
        {
            Media = media;
            Name = Path.GetFileNameWithoutExtension(Media.sourceURL);
            Duration = Media.duration;
            SourceUrl = Media.sourceURL;
        }
    }

    public sealed class AudioPlayer
    {
        public readonly WindowsMediaPlayer wmp;
        public readonly Timer timer;
        public readonly List<Audio> playlist;
        public int currentIndex;

        #region Props
        /// <summary>
        /// Текущее аудио
        /// </summary>
        public Audio CurrentAudio => playlist[currentIndex];
        /// <summary>
        /// Плейлист
        /// </summary>
        public string[] Playlist => playlist.Select((a) => a.Name).ToArray();
        /// <summary>
        /// Статус воспроизведения
        /// </summary>
        public Status PlayingStatus
        {
            get
            {
                switch (wmp.playState)
                {
                    case WMPPlayState.wmppsStopped:
                        return Status.Stopped;
                    case WMPPlayState.wmppsPaused:
                        return Status.Paused;
                    case WMPPlayState.wmppsPlaying:
                        return Status.Playing;
                    case WMPPlayState.wmppsMediaEnded:
                        return Status.Ended;
                    case WMPPlayState.wmppsTransitioning:
                        return Status.Transitioning;
                    case WMPPlayState.wmppsReady:
                        return Status.Ready;
                    default:
                        return Status.Undefined;
                }
            }
        }
        /// <summary>
        /// Прогресс воспроизведения
        /// </summary>
        public double Position
        {
            get { return wmp.controls.currentPosition; }
            set { wmp.controls.currentPosition = value; }
        }
        /// <summary>
        /// Прогресс воспроизведения в формате TimeSpan
        /// </summary>
        public TimeSpan PositionTime => TimeSpan.FromSeconds((int)wmp.controls.currentPosition);
        /// <summary>
        /// Громкость воспроизведения
        /// </summary>
        public int Volume
        {
            get { return wmp.settings.volume; }
            set { wmp.settings.volume = value; }
        }
        /// <summary>
        /// Автоматическое переключение на следующее аудио
        /// </summary>
        public bool AutoNext { get; set; } = true;
        /// <summary>
        /// Автоматический повтор выбранного аудио
        /// </summary>
        public bool AutoRestart { get; set; }
        #endregion

        public AudioPlayer()
        {
            wmp = new WindowsMediaPlayer();
            wmp.PlayStateChange += (e) =>
            {
                if (PlayingStatus == Status.Undefined)
                    return;
                PlayingStatusChanged?.Invoke(this, PlayingStatus);
            };
            timer = new Timer() { Interval = 17 };
            timer.Tick += (s, e) =>
            {
                ProgressChanged?.Invoke(this, Position);

                if (PlayingStatus == Status.Stopped || PlayingStatus == Status.Ended)
                    ((Timer)s).Stop();

                if ((PlayingStatus == Status.Stopped || PlayingStatus == Status.Ended) && AutoRestart)
                {
                    SelectAudio(currentIndex);
                    return;
                }

                if ((PlayingStatus == Status.Stopped || PlayingStatus == Status.Ended) && AutoNext)
                    SelectAudio(++currentIndex);
            };
            playlist = new List<Audio>();
        }

        /// <summary>
        /// Метод выбора аудио в плейлисте
        /// </summary>
        /// <param name="index">Индекс аудио в плейлисте</param>
        public void SelectAudio(int index)
        {
            currentIndex = index;

            if (currentIndex >= playlist.Count)
                currentIndex = 0;

            if (currentIndex < 0)
                currentIndex = playlist.Count - 1;

            wmp.currentMedia = CurrentAudio.Media;

            ProgressChanged?.Invoke(this, Position);
            AudioSelected?.Invoke(this, CurrentAudio);
            timer.Start();
        }

        /// <summary>
        /// Метод добавления аудио в плейлист из файла
        /// </summary>
        /// <param name="filepath">Путь к аудиофайлу</param>
        public void LoadAudio(string filepath) => playlist.Add(new Audio(wmp.newMedia(filepath)));
        /// <summary>
        /// Метод добавления нескольких аудио в плейлист
        /// </summary>
        /// <param name="filepaths">Массив путей к аудио файлам</param>
        public void LoadAudio(params string[] filepaths)
        {
            foreach (var file in filepaths)
                LoadAudio(file);
        }

        #region Controls
        /// <summary>
        /// Воспроизвести аудио
        /// </summary>
        public void Play()
        {
            if (Playlist.Length == 0)
                return;

            timer.Start();
            wmp.controls.play();
        }
        /// <summary>
        /// Приостановить аудио
        /// </summary>
        public void Pause()
        {
            timer.Stop();
            wmp.controls.pause();
        }
        /// <summary>
        /// Остановить аудио
        /// </summary>
        public void Stop()
        {
            timer.Stop();
            wmp.controls.stop();
        }
        #endregion

        #region Events
        /// <summary>
        /// Событие изменения статуса воспроизведения
        /// </summary>
        public event Action<object, Status> PlayingStatusChanged;
        /// <summary>
        /// Событие изменения прогресса воспроизведения
        /// </summary>
        public event Action<object, double> ProgressChanged;
        /// <summary>
        /// Событие выбора аудио из плейлиста
        /// </summary>
        public event Action<object, Audio> AudioSelected;
        #endregion
    }
}