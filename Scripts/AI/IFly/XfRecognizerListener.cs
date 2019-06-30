using UnityEngine;
using System;
using HuiHut.Facemoji;

namespace HuiHut.IFlyVoice
{
    public class XfRecognizerListener : AndroidJavaProxy
    {
        public string resultString = string.Empty;

        public XfRecognizerListener() : base("com.iflytek.cloud.RecognizerListener")
        {
        }

        public void onVolumeChanged(int volume, byte[] data)
        {
            string showText = "The current volume of speech is: " + volume;
            showText.showAsToast();
            Debug.Log("������Ƶ���ݣ�" + data.Length);
        }

        // һ��ʶ��Ự�Ľ�����ܻ��η��أ�����λص��˺�������ͨ������2�ж��Ƿ������һ�������isLast==true��
        // �����һ���������ʱ�����λỰ������¼��Ҳ��ֹͣ��
        public void onResult(AndroidJavaObject result, bool isLast)
        {
            string text = string.Empty;
            if (null != result)
            {
                try
                {
                    AndroidJavaObject res = result.Call<AndroidJavaObject>("getResultString");
                    byte[] resultByte = res.Call<byte[]>("getBytes");
                    text = System.Text.Encoding.Default.GetString(resultByte);
                }
                catch (Exception error)
                {
                    Debug.LogError(error.ToString());
                }
            }
            resultString = resultString + text;
            if (isLast)
            {
                //TODO ���Ľ��

                // ����Json����
                string userMessage = ParsingIFlyJson.Parsing(resultString);

                // ��������ʶ��Ļ���������
                FacemojiAI.SendToRobot(userMessage);

                Debug.Log(resultString);
            }
        }

        public void onEndOfSpeech()
        {
            // �˻ص���ʾ����⵽��������β�˵㣬�Ѿ�����ʶ����̣����ٽ�����������                
            //"End the talk.".showAsToast();
        }

        public void onBeginOfSpeech()
        {
            // �˻ص���ʾ��sdk�ڲ�¼�����Ѿ�׼�����ˣ��û����Կ�ʼ��������
            resultString = string.Empty;
        }

        public void onError(AndroidJavaObject error)
        {
            // ������룬���գ�https://shimo.im/sheet/w3yUy39uNKs0J7DT
            int errorCode = error.Call<int>("getErrorCode");
            // �����ı�
            string errorText = "onError Code��" + errorCode;

            if (errorCode == 10118)
            {
                //error code=10118 ������û��˵������鿴APP�Ƿ񱻰�ȫ���������¼������
                "please speak!".showAsToast();
            }
            else
            {
                // ������������������ı�
                errorText.showAsToast();
            }
        }

        public void onEvent(int eventType, int arg1, int arg2, AndroidJavaObject BundleObj)
        {
            ////���´������ڻ�ȡ���ƶ˵ĻỰid����ҵ�����ʱ���Ựid�ṩ��Ѷ���Ƶļ���֧����Ա�������ڲ�ѯ�Ự��־����λ����ԭ��
            //if (SpeechEvent.EVENT_SESSION_ID == eventType)
            //{
            //    String sid = obj.getString(SpeechEvent.KEY_EVENT_SESSION_ID);
            //    Log.d(TAG, "session id =" + sid);
            //}
        }
    }
}
