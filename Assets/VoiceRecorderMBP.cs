using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceRecorderMVP : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference recordToggleAction;

    [Header("Audio")]
    public AudioSource playbackSource;

    [Tooltip("Max recording length in seconds (MVP).")]
    public int maxRecordSeconds = 10;

    [Tooltip("Recording sample rate. 44100 is safe for MVP.")]
    public int frequency = 44100;

    private string micDevice;
    private bool isRecording = false;
    private AudioClip recordingClip;
    private int recordStartSample = 0;

    void Awake()
    {
        if (playbackSource == null)
            playbackSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (recordToggleAction != null)
        {
            recordToggleAction.action.performed += OnRecordTogglePerformed;
            recordToggleAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (recordToggleAction != null)
        {
            recordToggleAction.action.performed -= OnRecordTogglePerformed;
            recordToggleAction.action.Disable();
        }
    }

    void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone device found. Check OS microphone permissions / device settings.");
            return;
        }

        micDevice = Microphone.devices[0];
        Debug.Log($"[MVP] Using microphone: {micDevice}");
    }

    private void OnRecordTogglePerformed(InputAction.CallbackContext ctx)
    {
        if (!isRecording) StartRecording();
        else StopAndPlay();
    }

    private void StartRecording()
    {
        if (string.IsNullOrEmpty(micDevice))
        {
            Debug.LogError("Microphone device not ready.");
            return;
        }

        if (playbackSource != null && playbackSource.isPlaying)
            playbackSource.Stop();

        recordingClip = Microphone.Start(micDevice, false, maxRecordSeconds, frequency);
        isRecording = true;

        recordStartSample = 0;
        Debug.Log("[MVP] Recording started...");
        StartCoroutine(WaitForMicToStart());
    }

    private IEnumerator WaitForMicToStart()
    {
        int pos = 0;
        while (pos <= 0)
        {
            pos = Microphone.GetPosition(micDevice);
            yield return null;
        }
        recordStartSample = pos;
        Debug.Log($"[MVP] Mic started. Start sample position: {recordStartSample}");
    }

    private void StopAndPlay()
    {
        if (!isRecording) return;

        int endPos = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        isRecording = false;

        if (recordingClip == null)
        {
            Debug.LogError("[MVP] recordingClip is null.");
            return;
        }

        if (endPos <= 0)
        {
            Debug.LogWarning("[MVP] End position is 0. Try speaking a bit later after pressing record.");
            return;
        }

        AudioClip trimmed = TrimClip(recordingClip, endPos);

        Debug.Log($"[MVP] Recording stopped. Samples: {endPos}, Length: {trimmed.length:F2}s");

        playbackSource.clip = trimmed;
        playbackSource.Play();
        Debug.Log("[MVP] Playback started.");
    }

    private AudioClip TrimClip(AudioClip clip, int samplesRecorded)
    {
        float[] data = new float[samplesRecorded * clip.channels];
        clip.GetData(data, 0);

        AudioClip newClip = AudioClip.Create("Recorded_Trimmed", samplesRecorded, clip.channels, clip.frequency, false);
        newClip.SetData(data, 0);
        return newClip;
    }
}