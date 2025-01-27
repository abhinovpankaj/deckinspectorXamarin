﻿using AVFoundation;
using Foundation;
using Mobile.Code.iOS;
using Speech;
using System;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(SpeechToTextImplementation))]
namespace Mobile.Code.iOS
{
    public class SpeechToTextImplementation : ISpeechToText
    {
        private AVAudioEngine _audioEngine = new AVAudioEngine();
        private SFSpeechRecognizer _speechRecognizer = new SFSpeechRecognizer();
        private SFSpeechAudioBufferRecognitionRequest _recognitionRequest;
        private SFSpeechRecognitionTask _recognitionTask;
        private string _recognizedString;
        private bool _isAuthorized;
        private NSTimer _timer;

        public SpeechToTextImplementation()
        {
            AskForSpeechPermission();
        }
        public void StartSpeechToText()
        {

            if (_audioEngine.Running)
            {
                StopRecordingAndRecognition();

            }
            StartRecordingAndRecognizing();
        }

        public void StopSpeechToText()
        {
            StopRecordingAndRecognition();
        }

        private void AskForSpeechPermission()
        {
            SFSpeechRecognizer.RequestAuthorization((SFSpeechRecognizerAuthorizationStatus status) =>
            {
                switch (status)
                {
                    case SFSpeechRecognizerAuthorizationStatus.Authorized:
                        _isAuthorized = true;
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Denied:
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.NotDetermined:
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Restricted:
                        break;
                }
            });
        }

        private void DidFinishTalk()
        {
            MessagingCenter.Send<ISpeechToText>(this, "Final");
            MessagingCenter.Send<ISpeechToText, string>(this, "STT", _recognizedString);
            if (_timer != null)
            {
                _timer.Invalidate();
                _timer = null;
            }


            if (_audioEngine.Running)
            {
                StopRecordingAndRecognition();
            }

        }

        private void StartRecordingAndRecognizing()
        {
            _recognizedString = string.Empty;
            //_timer = NSTimer.CreateRepeatingScheduledTimer(5, delegate
            //{
            //    DidFinishTalk();
            //});


            _recognitionTask?.Cancel();
            _recognitionTask = null;

            var audioSession = AVAudioSession.SharedInstance();
            NSError nsError;
            nsError = audioSession.SetCategory(AVAudioSessionCategory.Record);
            audioSession.SetMode(AVAudioSession.ModeMeasurement, out nsError);
            nsError = audioSession.SetActive(true, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);
            audioSession.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out nsError);
            _recognitionRequest = new SFSpeechAudioBufferRecognitionRequest();

            var inputNode = _audioEngine.InputNode;
            if (inputNode == null)
            {
                throw new Exception();
            }

            var recordingFormat = inputNode.GetBusOutputFormat(0);
            inputNode.InstallTapOnBus(0, 1024, recordingFormat, (buffer, when) =>
            {
                _recognitionRequest?.Append(buffer);
            });

            _audioEngine.Prepare();
            _audioEngine.StartAndReturnError(out nsError);

            _recognitionTask = _speechRecognizer.GetRecognitionTask(_recognitionRequest, (result, error) =>
            {
                var isFinal = false;
                if (result != null)
                {
                    _recognizedString = result.BestTranscription.FormattedString;


                    _timer.Invalidate();
                    _timer = null;
                    _timer = NSTimer.CreateRepeatingScheduledTimer(5, delegate
                    {
                        DidFinishTalk();

                    });
                    //isFinal = result.Final;


                }
                if (error != null || isFinal)
                {
                    //MessagingCenter.Send<ISpeechToText>(this, "Final");
                    //MessagingCenter.Send<ISpeechToText, string>(this, "STT", _recognizedString);
                    StopRecordingAndRecognition(audioSession);
                }
            });




        }

        private void StopRecordingAndRecognition(AVAudioSession aVAudioSession = null)
        {
            if (_audioEngine.Running)
            {
                _audioEngine.Stop();
                _audioEngine.InputNode.RemoveTapOnBus(0);
                _recognitionTask?.Cancel();
                _recognitionRequest.EndAudio();
                _recognitionRequest = null;
                _recognitionTask = null;
            }


        }


    }
}