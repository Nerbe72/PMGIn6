using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AudioCtrl
{
    public float[] pitchRanges;
    public float[] playTimes;
    public float playTimer;
    public float interval;
    public float intervalTimer;
    public int cordCount;
    public int rangeCount;

    public AudioCtrl()
    {
        playTimer = 0f;
        interval = 0f;
        intervalTimer = 0f;
        cordCount = 0;
        rangeCount = 0;
    }

    public void CalculateAudio(float _measure, int _minFreq, int _maxFreq, float _low, float _high)
    {
        float playTotal = 0f;
        float lastPitch = Random.Range(_low, _high);
        int switchPitchCount = Random.Range(3, _maxFreq);
        int switchPitch = 0;
        int pitchDir = Random.Range(0, 2);

        cordCount = Random.Range(_minFreq, _maxFreq);
        playTimes = new float[cordCount];
        pitchRanges = new float[cordCount];

        for (int i = 0; i < cordCount; i++)
        {
            playTimes[i] = Random.Range(_minFreq / cordCount, _measure / cordCount);
            playTotal += playTimes[i];

            if (pitchDir == 0)
            {
                lastPitch = pitchRanges[i] = Random.Range(_low, lastPitch);
            } else if (pitchDir == 1)
            {
                lastPitch = pitchRanges[i] = Random.Range(lastPitch, _high);
            }

            switchPitch++;

            if (switchPitch == switchPitchCount)
            {
                if (pitchDir == 0)
                    pitchDir = 1;
                else
                    pitchDir = 0;
            }
        }

        playTimer = playTimes[0];
        interval = (_measure - playTotal) / cordCount;
        intervalTimer = interval;
    }

    public void PlaySoundLine(AudioSource _source)
    {
        if (rangeCount >= cordCount)
        {
            rangeCount = 0;
        }

        if (playTimer > 0)
        {
            playTimer -= Time.deltaTime;

            if (!_source.isPlaying)
            {
                _source.pitch = pitchRanges[rangeCount];
                _source.Play();
                rangeCount++;
            }
        }
        else if (playTimer <= 0)
        {
            _source.Stop();

            if (intervalTimer > 0)
            {
                intervalTimer -= Time.deltaTime;
            }
            else if (intervalTimer <= 0)
            {
                playTimer = playTimes[rangeCount];
                intervalTimer = interval;
            }
        }
    }
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource highSource;
    public AudioSource midSource;
    public AudioSource lowSource;

    public float highPitchRange = 0f;
    public float lowPitchRange = 0f;

    public float measure = 0f;

    public float[] basePlayTime;
    public float basePlayeTimer = 0f;
    public float baseInterval = 0f;
    public float baseIntervalTimer = 0f;
    public int baseCords;
    public int basePitchRangeCount = 0;

    public AudioCtrl baseAudio;
    public AudioCtrl midAudio;
    public AudioCtrl highAudio;

    private float[] basePitchRanges;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        lowPitchRange = 0.25f;
        highPitchRange = 1.75f;
        baseAudio = new AudioCtrl();
        midAudio = new AudioCtrl();
        highAudio = new AudioCtrl();

        FormAudio(false);
    }

    private void Update()
    {
        baseAudio.PlaySoundLine(lowSource);
        midAudio.PlaySoundLine(midSource);
        highAudio.PlaySoundLine(highSource);
    }

    public void FormAudio(bool _tension)
    {
        if (_tension)
            measure = Random.Range(1f, 3f);
        else
            measure = Random.Range(10f, 20f);

        baseAudio.CalculateAudio(measure, 3, 7, lowPitchRange, highPitchRange);
        midAudio.CalculateAudio(measure, 2, 6, lowPitchRange, highPitchRange);
        highAudio.CalculateAudio(measure, 5, 10, lowPitchRange, highPitchRange);
    }

    //private void Init()
    //{
    //    measure = Random.Range(3f, 20f);
    //    float playTotal = 0f;

    //    baseCords = Random.Range(3, 7);
    //    basePlayTime = new float[baseCords];
    //    basePitchRanges = new float[baseCords];
        
    //    for(int i = 0; i < baseCords; i++)
    //    {
    //        basePlayTime[i] = Random.Range(3f / baseCords, measure / baseCords);
    //        playTotal += basePlayTime[i];
    //        basePitchRanges[i] = Random.Range(lowPitchRange, highPitchRange);
    //    }
    //    basePlayeTimer = basePlayTime[0];
    //    baseInterval = (measure - playTotal) / baseCords;
    //    baseIntervalTimer = baseInterval;
    //}

    //private void PlaySoundLine(AudioSource _source, float[] _playtime, ref float _playtimer, float _intervaltime, ref float _intervaltimer, int _cords, float[] _pitchranges, ref int _pitchrangecount)
    //{
    //    if (_pitchrangecount >= _cords)
    //    {
    //        _pitchrangecount = 0;
    //    }

    //    if (_playtimer > 0)
    //    {
    //        _playtimer -= Time.deltaTime;

    //        if (!_source.isPlaying)
    //        {
    //            _source.pitch = _pitchranges[_pitchrangecount];
    //            _source.Play();
    //            _pitchrangecount++;
    //        }
    //    } else if (_playtimer <= 0)
    //    {
    //        _source.Stop();

    //        if (_intervaltimer > 0)
    //        {
    //            _intervaltimer -= Time.deltaTime;
    //        } else if ( _intervaltimer <= 0)
    //        {
    //            _playtimer = _playtime[_pitchrangecount];
    //            _intervaltimer = _intervaltime;
    //        }
    //    }
    //}
}
