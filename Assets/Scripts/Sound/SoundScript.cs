using DG.Tweening;
using System;
using UnityEngine;

namespace WaterSort
{
    public class SoundScript : MonoBehaviour
    {
        public AUDIO_KEY nameClip = AUDIO_KEY.NONE;
        private AudioSource myAudioSource;
        private Tween tweenDeactive;
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        public void AddAudioSource(bool isLoop)
        {
            myAudioSource = this.gameObject.AddComponent<AudioSource>();
            myAudioSource.loop = isLoop;
        }

        public void PlaySound(AUDIO_KEY nameClip, AudioClip clip)
        {
            this.nameClip = nameClip;
            this.gameObject.name = nameClip.ToString();
            this.gameObject.SetActive(true);
            myAudioSource.clip = clip;
            myAudioSource.Play();
            tweenDeactive = DOVirtual.DelayedCall(clip.length, () =>
           {
               this.gameObject.SetActive(false);
           });
        }

        internal void PlayMusic(AUDIO_KEY nameClip, AudioClip audioClip)
        {
            if (this.nameClip == nameClip) return;
            this.nameClip = nameClip;
            this.gameObject.name = nameClip.ToString();

            myAudioSource.clip = audioClip;
            myAudioSource.Play();
            myAudioSource.volume = 0;
            myAudioSource.DOFade(.7f, 0.3f);
        }

        internal void SetMute(bool isMute)
        {
            myAudioSource.mute = isMute;
        }

        internal void Resume(bool isMute)
        {
            SetMute(isMute);
            myAudioSource.DOFade(.7f, 0.3f);
        }

        internal void Stop(bool isMute)
        {
            SetMute(isMute);
            myAudioSource.DOFade(0, 0.3f);
        }

        public void SetVolume(float volume)
        {
            myAudioSource.volume = volume;
        }

        public void Stop()
        {
            myAudioSource.Stop();
            if (tweenDeactive != null) tweenDeactive.Kill();
            this.gameObject.SetActive(false);
        }
    }
}