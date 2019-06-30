using UnityEngine;

namespace HuiHut.IFlyVoice
{
    public class XfSynthesizerListener : AndroidJavaProxy
    {

        //�������
        private int mPercentForBuffering = 0;
        //���Ž���
        private int mPercentForPlaying = 0;

        public XfSynthesizerListener() : base("com.iflytek.cloud.SynthesizerListener")
        {

        }

        public void onSpeakBegin()
        {
            //showTip("Start playing");
        }

        public void onSpeakPaused()
        {
            //showTip("Stop play");
        }

        public void onSpeakResumed()
        {
            //showTip("Continue to play");
        }

        public void onBufferProgress(int percent, int beginPos, int endPos,
                AndroidJavaObject info)
        {
            // �ϳɽ���
            mPercentForBuffering = percent;
            Debug.Log("�������Ϊ" + mPercentForBuffering + "%�����Ž���Ϊ" + mPercentForPlaying + "%");
        }

        public void onSpeakProgress(int percent, int beginPos, int endPos)
        {
            // ���Ž���
            mPercentForPlaying = percent;
            Debug.Log("�������Ϊ" + mPercentForBuffering + "%�����Ž���Ϊ" + mPercentForPlaying + "%");
        }

        public void onCompleted(AndroidJavaObject error)
        {
            if (null != error)
            {
                showTip("Play failure");
            }
            else
            {
                //showTip("Play completion");
            }
        }

        public void onEvent(int eventType, int arg1, int arg2, AndroidJavaObject BundleObj)
        {
            // ���´������ڻ�ȡ���ƶ˵ĻỰid����ҵ�����ʱ���Ựid�ṩ������֧����Ա�������ڲ�ѯ�Ự��־����λ����ԭ��
            // ��ʹ�ñ����������ỰidΪnull
            //        if (SpeechEvent.EVENT_SESSION_ID == eventType) {
            //                String sid = obj.getString(SpeechEvent.KEY_EVENT_SESSION_ID);
            //                Log.d(TAG, "session id =" + sid);
            //        }
        }

        void showTip(string text)
        {
            Debug.Log(text);
            text.showAsToast();
        }
    }
}