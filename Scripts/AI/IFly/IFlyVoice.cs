using UnityEngine;

namespace HuiHut.IFlyVoice
{
    public class IFlyVoice
    {
        // Your IFly AppID
        const string AppID = "5a5c6bfb";

        //---------------------------------------
        const string SpeechConstant_PARAMS = "params";
        const string SpeechConstant_ENGINE_TYPE = "engine_type";
        const string SpeechConstant_TYPE_CLOUD = "cloud";
        const string SpeechConstant_VOICE_NAME = "voice_name";
        const string SpeechConstant_SPEED = "speed";
        const string SpeechConstant_PITCH = "pitch";
        const string SpeechConstant_VOLUME = "volume";
        const string SpeechConstant_STREAM_TYPE = "stream_type";
        const string SpeechConstant_KEY_REQUEST_FOCUS = "request_audio_focus";
        const string SpeechConstant_AUDIO_FORMAT = "audio_format";
        const string SpeechConstant_TTS_AUDIO_PATH = "tts_audio_path";
        const string SpeechConstant_RESULT_TYPE = "result_type";
        const string SpeechConstant_LANGUAGE = "language";
        const string SpeechConstant_ACCENT = "accent";
        const string SpeechConstant_VAD_BOS = "vad_bos";
        const string SpeechConstant_VAD_EOS = "vad_eos";
        const string SpeechConstant_ASR_PTT = "asr_ptt";
        const string SpeechConstant_ASR_AUDIO_PATH = "asr_audio_path";
        //---------------------------------------

        //AndroidJavaClass
        private static AndroidJavaClass UnityPlayer;
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaClass SpeechSynthesizer;
        private static AndroidJavaClass SpeechRecognizer;

        //AndroidJavaObject
        private static AndroidJavaObject mTts;
        private static AndroidJavaObject mIat;

        private static XfInitListener mInitListener;
        private static XfSynthesizerListener mTtsListener;
        public static XfRecognizerListener mRecognizerListener;

        //to judge if the program has execute initFlyVoice before speak or recognize
        private static bool inited = false;

        private static void initIFlyVoice()
        {
#if UNITY_ANDROID
            //Initialize AndroidJavaClass(Please do not delete the commended codes for that those code are for test and check)
            UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            string param = "appid=" + AppID + ",engine_mode=msc";

            AndroidJavaClass SpeechUtility = new AndroidJavaClass("com.iflytek.cloud.SpeechUtility");

            SpeechUtility.CallStatic<AndroidJavaObject>("createUtility",
                    currentActivity.Call<AndroidJavaObject>("getApplicationContext"),
                    new AndroidJavaObject("java.lang.String", param)
            );

            //Init Listeners
            mInitListener = new XfInitListener();
            mTtsListener = new XfSynthesizerListener();
            mRecognizerListener = new XfRecognizerListener();

            //Init mTts and mIat
            if (mInitListener != null)
            {
                SpeechSynthesizer = new AndroidJavaClass("com.iflytek.cloud.SpeechSynthesizer");
                SpeechRecognizer = new AndroidJavaClass("com.iflytek.cloud.SpeechRecognizer");

                mTts = SpeechSynthesizer.CallStatic<AndroidJavaObject>("createSynthesizer", currentActivity, mInitListener);
                mIat = SpeechRecognizer.CallStatic<AndroidJavaObject>("createRecognizer", currentActivity, mInitListener);
            }
            inited = true;
#endif
        }

        public static void startSpeaking(string text, string voicer = "xiaoyan")
        {
            if (!inited)
            {
                initIFlyVoice();
            }
            setTtsParam(voicer);
            int code = mTts.Call<int>("startSpeaking", text.toJavaString(), mTtsListener);
            if (code != 0)
            {
                Debug.LogError("SpeakFailed,ErrorCode" + code);
            }
        }

        public static void startRecognize(string language = "mandarin")
        {
            if (!inited)
            {
                initIFlyVoice();
            }
            setIatParam(language);//����ʶ�����������
            int ret = mIat.Call<int>("startListening", mRecognizerListener);
            if (ret != 0)
            {
                Debug.LogError("��дʧ��,�����룺" + ret);
            }
            else
            {
                //"Please start talking.".showAsToast(currentActivity);
            }
        }

        private static void setTtsParam(string voicer)
        {
            if (mTts == null)
            {
                Debug.LogError("mTts=null");
                return;
            }
            //��ղ���
            mTts.Call<bool>("setParameter", SpeechConstant_PARAMS.toJavaString(), null);

            //���úϳ�
            //����ʹ���ƶ�����
            mTts.Call<bool>("setParameter", SpeechConstant_ENGINE_TYPE.toJavaString(), SpeechConstant_TYPE_CLOUD.toJavaString());

            //���÷�����
            mTts.Call<bool>("setParameter", SpeechConstant_VOICE_NAME.toJavaString(), voicer.toJavaString());
            //���úϳ�����
            mTts.Call<bool>("setParameter", SpeechConstant_SPEED.toJavaString(), "50".toJavaString());
            //���úϳ�����
            mTts.Call<bool>("setParameter", SpeechConstant_PITCH.toJavaString(), "50".toJavaString());
            //���úϳ�����
            mTts.Call<bool>("setParameter", SpeechConstant_VOLUME.toJavaString(), "50".toJavaString());
            //���ò�������Ƶ������
            mTts.Call<bool>("setParameter", SpeechConstant_STREAM_TYPE.toJavaString(), "3".toJavaString());

            // ���ò��źϳ���Ƶ������ֲ��ţ�Ĭ��Ϊtrue
            mTts.Call<bool>("setParameter", SpeechConstant_KEY_REQUEST_FOCUS.toJavaString(), "true".toJavaString());

            // ������Ƶ����·����������Ƶ��ʽ֧��pcm��wav������·��Ϊsd����ע��WRITE_EXTERNAL_STORAGEȨ��
            // ע��AUDIO_FORMAT���������Ҫ���°汾������Ч
            mTts.Call<bool>("setParameter", SpeechConstant_AUDIO_FORMAT.toJavaString(), "wav".toJavaString());

            AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment");
            AndroidJavaObject rootDir = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<AndroidJavaObject>("toString");
            rootDir = rootDir.Call<AndroidJavaObject>("concat", "/msc/tts.wav".toJavaString());

            mTts.Call<bool>("setParameter", SpeechConstant_TTS_AUDIO_PATH.toJavaString(), rootDir);
        }

        private static void setIatParam(string lag)
        {
            // ��ղ���
            mIat.Call<bool>("setParameter", SpeechConstant_PARAMS.toJavaString(), null);
            // ��������
            mIat.Call<bool>("setParameter", SpeechConstant_ENGINE_TYPE.toJavaString(), SpeechConstant_TYPE_CLOUD.toJavaString());
            // ���÷��ؽ����ʽ
            mIat.Call<bool>("setParameter", SpeechConstant_RESULT_TYPE.toJavaString(), "json".toJavaString());

            if (lag.Equals("en_us"))
            {
                // ��������
                mIat.Call<bool>("setParameter", SpeechConstant_LANGUAGE.toJavaString(), "en_us".toJavaString());
            }
            else
            {
                // ��������
                mIat.Call<bool>("setParameter", SpeechConstant_LANGUAGE.toJavaString(), "zh_cn".toJavaString());
                // ������������
                mIat.Call<bool>("setParameter", SpeechConstant_ACCENT.toJavaString(), lag.toJavaString());
            }

            // ��������ǰ�˵�:������ʱʱ�䣬���û��೤ʱ�䲻˵��������ʱ����
            mIat.Call<bool>("setParameter", SpeechConstant_VAD_BOS.toJavaString(), "4000".toJavaString());

            // ����������˵�:��˵㾲�����ʱ�䣬���û�ֹͣ˵���೤ʱ���ڼ���Ϊ�������룬 �Զ�ֹͣ¼��
            mIat.Call<bool>("setParameter", SpeechConstant_VAD_EOS.toJavaString(), "1000".toJavaString());

            // ���ñ�����,����Ϊ"0"���ؽ���ޱ��,����Ϊ"1"���ؽ���б��
            mIat.Call<bool>("setParameter", SpeechConstant_ASR_PTT.toJavaString(), "1".toJavaString());

            // ������Ƶ����·����������Ƶ��ʽ֧��pcm��wav������·��Ϊsd����ע��WRITE_EXTERNAL_STORAGEȨ��
            // ע��AUDIO_FORMAT���������Ҫ���°汾������Ч
            mIat.Call<bool>("setParameter", SpeechConstant_AUDIO_FORMAT.toJavaString(), "wav".toJavaString());

            AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment");
            AndroidJavaObject rootDir = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<AndroidJavaObject>("toString");
            rootDir = rootDir.Call<AndroidJavaObject>("concat", "/msc/iat.wav".toJavaString());
            mIat.Call<bool>("setParameter", SpeechConstant_ASR_AUDIO_PATH.toJavaString(), rootDir);
        }
    }
}